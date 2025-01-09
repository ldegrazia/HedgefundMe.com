using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Data;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.IO;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class MarketDataService
    {
     
        private readonly ProjectEntities db;
        private readonly TradingSettingsService tsettings;
        public MarketDataService(ProjectEntities context)
        {
            db = context;
            tsettings = new TradingSettingsService(db);
        }
        /// <summary>
        /// The latest date available
        /// </summary>
        public DateTime StartOn
        {
            get { return GetLatestStockDateAvailable(); }
        }
        public DateTime GetLatestStockDateAvailable()
        {
            if(!db.MarketData.Any())
            {
                return DateTime.MinValue.Date;
            }
            return (from d in db.MarketData select d.Date).Max();
        }
        /// <summary>
        /// Returns XML for the latest universe
        /// </summary>
        /// <returns></returns>
        public string GetUniverseAsXml()
        {
            return SerializerService.SerializeUniverse(GetDataOn(StartOn));
        }
        /// <summary>
        /// Deletes latest data
        /// </summary>
        /// <param name="scanName"></param>
        /// <returns></returns>
        public void DeleteLatestMarketData()
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            var allData = from d in db.MarketData where d.Date == StartOn select d;
            allData.ToList().ForEach(y => db.Entry(y).State = EntityState.Deleted);
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Checks the volume cut offs and removes those tickers
        /// </summary>
        public void FilterData()
        {
            List<MarketData> filteredData = new List<MarketData>();
            //this finds all the longs with a lower volume cut off and then removes them
            var currentSettings = tsettings.GetCurrent();
            filteredData.AddRange(GetDataOn(StartOn, Strings.Side.Long).Where(t => t.AvgVol < currentSettings.MinimumLongVolume).ToList());//fix this move it to the signals service
            filteredData.AddRange(GetDataOn(StartOn, Strings.Side.Short).Where(t => t.AvgVol < currentSettings.MinimumShortVolume).ToList());
            db.Configuration.AutoDetectChangesEnabled = false;
            filteredData.ForEach(y => db.Entry(y).State = EntityState.Deleted);//lets remove them from the list
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        } 
        /// <summary>
        /// Hits barchart for the longs and shorts, then gets pricing data from yahoo.
        /// </summary>
        /// <param name="forToday"></param>
        /// <returns></returns>
        public async Task<string> LoadDataAsync(DateTime forToday)
        {
            //get the latest rank date and delete if the data is for today
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<br/>");
            sb.AppendLine(string.Format("<hr><h3>Fetching new data for {0}.</h3>", forToday.ToShortDateString()));
            sb.AppendLine(string.Format("Starting at {0}.</h3>", DateTime.Now.ToLongTimeString()));
            List<MarketData> marketDataLongs;
            List<MarketData> marketDataShorts;
            List<MarketData> allData = new List<MarketData>();
            try
            {
                sb.AppendLine("Fetching stocks from Barchart...<br/>");
                BarChartService bc = new BarChartService();
                marketDataLongs = await bc.GetMarketDataAsync();
                marketDataShorts = await bc.GetMarketDataAsync(true);

                sb.AppendLine(string.Format("Fetched {0} stocks from Barchart.<br/>", marketDataLongs.Count + marketDataShorts.Count));
            }
            catch (Exception ex)
            {
                //log and leave
                sb.AppendLine(ex.Message);
                EmailService.SendEmail("Error fetching Barchart data", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                return sb.ToString();
            }
            try
            {
                YahooService yh = new YahooService();
                sb.AppendLine("Fetching stocks from Yahoo..<br/>");
                marketDataLongs = await yh.GetStockDataAsync(marketDataLongs);
                marketDataShorts = await yh.GetStockDataAsync(marketDataShorts);
                sb.AppendLine(string.Format("Fetched {0} stock data from Yahoo.<br/>", marketDataLongs.Count + marketDataShorts.Count));
            }
            catch (Exception ex)
            {
                //log dont leave
                sb.AppendLine(ex.Message);
                EmailService.SendEmail("Error fetching Yahoo data:", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
            }
            try
            {
                DateTime fetchedDate = marketDataLongs.First().Date;
                if (GetLatestStockDateAvailable() == fetchedDate)
                {
                    sb.AppendLine(string.Format("Deleting data for {0}.<br/>", fetchedDate.ToShortDateString()));
                    DeleteLatestMarketData(); //now delete the latest data
                    sb.AppendLine(string.Format("Deleted data for {0}.<br/>", fetchedDate.ToShortDateString()));
                }
            }
            catch (Exception ex)
            {
                //log and leave, we can't have duplicates
                sb.AppendLine(ex.Message);

                EmailService.SendEmail("Error Deleting data.", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                return sb.ToString();
            }
            try
            {
                allData.AddRange(marketDataLongs);
                allData.AddRange(marketDataShorts);
                List<MarketData> final = new List<MarketData>();
                var DistinctItems = allData.GroupBy(x => x.Ticker).Select(y => y.First()); 
                foreach (var item in DistinctItems)
                {
                    //Add to other List
                    final.Add(item);
                }
                 
                sb.AppendLine("Saving new data.<br/>");
                foreach (var datapoint in final)
                {
                    db.MarketData.Add(datapoint);
                }
                db.SaveChanges();
                sb.AppendLine("Saved market data.<br/>");
            }
            catch (Exception ex)
            {
                //log and leave
                sb.AppendLine(ex.Message);
                sb.AppendLine("<br/>");
                EmailService.SendEmail("Error Saving new data.", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                return sb.ToString();
            }
            try
            {
                sb.AppendLine("Filtering new data.<br/>");
                FilterData();
                sb.AppendLine("Filtered market data.<br/>");
            }
            catch (Exception ex)
            {
                //log and leave
                sb.AppendLine(ex.Message);
                sb.AppendLine("<br/>");
                EmailService.SendEmail("Error Filtering new data.", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                return sb.ToString();
            }
            sb.AppendLine(string.Format("Completed at {0}.</h3>", DateTime.Now.ToLongTimeString()));
            return sb.ToString();
        }

        /// <summary>
        /// Hits pricing data from yahoo by logging in.
        /// </summary>
        /// <param name="forToday"></param>
        /// <returns></returns>
        public async Task<string> LoadDataAsync2(DateTime forToday)
        {
            //get the latest rank date and delete if the data is for today
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<br/>");
            sb.AppendLine(string.Format("<hr><h3>Fetching new data for {0}.</h3>", forToday.ToShortDateString()));
            sb.AppendLine(string.Format("Starting at {0}.<br/>", DateTime.Now.ToLongTimeString()));
           
            List<MarketData> allData = new List<MarketData>();
            var portfolios = db.Portfolios.ToList(); 
            foreach(var p in portfolios)
            { 
                try
                {
                    YahooService yh = new YahooService();
                    sb.AppendLine("Fetching stocks from " + p.Name + " ...<br/>"); 
                    allData.AddRange(await yh.GetPortfolioDataAsync(p));
                    allData.ForEach(t => t.Date = forToday);
                    sb.AppendLine(string.Format("Fetched {0} stock data from Yahoo.<br/>", allData.Count));
                }
                catch (Exception ex)
                {
                    //log dont leave
                    sb.AppendLine(ex.Message);
                    EmailService.SendEmail("Error fetching Yahoo data:", sb.ToString());
                    Logger.WriteLine(MessageType.Error, ex.Message);
                }
            }
            try
            {
                DateTime fetchedDate = allData.First().Date;
                if (GetLatestStockDateAvailable() == fetchedDate)
                {
                    sb.AppendLine(string.Format("Deleting data for {0}.<br/>", fetchedDate.ToShortDateString()));
                    DeleteLatestMarketData(); //now delete the latest data
                    sb.AppendLine(string.Format("Deleted data for {0}.<br/>", fetchedDate.ToShortDateString()));
                }
            }
            catch (Exception ex)
            {
                //log and leave, we can't have duplicates
                sb.AppendLine(ex.Message);

                EmailService.SendEmail("Error Deleting data.", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                return sb.ToString();
            }
            try
            {               
                sb.AppendLine("Saving new data.<br/>");
                foreach (var datapoint in allData)
                {
                    db.MarketData.Add(datapoint);
                }
                db.SaveChanges();
                UpdateSignalFetchSiteAudit();
                UpdatePricingFetchSiteAudit();
                sb.AppendLine("Saved market data.<br/>");
            }
            catch (Exception ex)
            {
                //log and leave
                sb.AppendLine(ex.Message);
                sb.AppendLine("<br/>");
                EmailService.SendEmail("Error Saving new data.", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                return sb.ToString();
            } 
            
            sb.AppendLine(string.Format("Completed at {0}.</h3>", DateTime.Now.ToLongTimeString()));
            return sb.ToString();
        }


        /// <summary>
        /// Figures out the real date and returns the list from that date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<MarketData> GetDataOn(DateTime date)
        {
            List<MarketData> current = new List<MarketData>();
            int counter = 0;
            do
            {
                if (counter < -25)//300 days back
                {
                    if (!db.MarketData.Any(t => t.Date == date))
                    {
                        //we dont have this date so return nothing.
                        return new List<MarketData>();
                    }
                    throw new Exception("No data found for earlier date.");
                }
                date = DateHelper.SanitizeDate(date.AddDays(counter));
                counter--;
                current = (from r in db.MarketData where r.Date == date select r).ToList();
            }
            while (!current.Any());
            return current;
        }
        /// <summary>
        /// Figures out the real date and returns the list from that date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<MarketData> GetDataOn(DateTime date, string side)
        {
            List<MarketData> current = new List<MarketData>();
            int counter = 0;
            do
            {
                if (counter < -25)//300 days back
                {
                    if (!db.MarketData.Any(t => t.Date == date && t.Side == side))
                    {
                        //we dont have this date so return nothing.
                        return new List<MarketData>();
                    }
                    throw new Exception("No data found for earlier date.");
                }
                date = DateHelper.SanitizeDate(date.AddDays(counter));
                counter--;
                current = (from r in db.MarketData where r.Date == date && r.Side == side select r).ToList();
            }
            while (!current.Any());
            return current;
        }
        /// <summary>
        /// Updates the market data provided
        /// </summary>
        /// <param name="marketData"></param>
        public void Update(List<MarketData> marketData)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            foreach (var datapoint in marketData)
            {
                db.Entry(datapoint).State = EntityState.Modified;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        
        /// <summary>
        /// Gets the latest pricing data from yahoo for the latest date's set of tickers
        /// </summary>
        public async Task<string> RefreshTodaysPricingData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<br/>");
            sb.AppendLine(string.Format("<hr><h3>Fetching new data for {0}.</h3>", StartOn.ToLongDateString()));
            //for each ticker, go to yahoo and update the pricing data
            var todaysData = GetDataOn(StartOn); //get all the data tickers we have for the latest date            
            int skip = 0;
            int take = 100;
            YahooService yh = new YahooService();
            sb.AppendLine("Fetching stocks from Yahoo..<br/>");           
            try
            {
                    todaysData = await yh.GetBatchedPricingData(todaysData,skip,take);//call yahoo, get the data and then update it do in batches of 100
                    Update(todaysData);
                    sb.AppendLine(string.Format("Fetched {0} stock data from Yahoo.<br/>", todaysData.Count)); 
            }
            catch (Exception ex)
            {
                //log dont leave
                sb.AppendLine(ex.Message); 
                Logger.WriteLine(MessageType.Error, ex.Message);
            } 
            return sb.ToString();
        }
        /// <summary>
        /// Gets the latest pricing data from yahoo portfolios for the latest date's set of tickers
        /// </summary>
        public async Task<string> RefreshTodaysPortfolioData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<br/>");
            sb.AppendLine(string.Format("<hr><h3>Fetching new data for {0}.</h3>", StartOn.ToLongDateString())); 
            YahooService yh = new YahooService();
            sb.AppendLine("Fetching stocks from Yahoo..<br/>");
            var portfolios = db.Portfolios.ToList();
            var todaysData = new List<MarketData>();
            foreach(var p in portfolios)
            {
                try
                {
                    todaysData.AddRange(await yh.GetPortfolioDataAsync(p));//call yahoo, get the data and then update it do in batches of 100
                     
                }
                catch (Exception ex)
                {
                    //log dont leave
                    sb.AppendLine(ex.Message);
                    Logger.WriteLine(MessageType.Error, ex.Message);
                }
            }
            var updatedData = GetDataOn(StartOn); //get all the data tickers we have for the latest date 
            foreach(var marketData in updatedData)
            {
                if(todaysData.Any(t=>t.Ticker == marketData.Ticker))//if we have the ticker, get it
                {
                    var updated = todaysData.Where(t => t.Ticker == marketData.Ticker).First();
                    marketData.Price = updated.Price;
                    marketData.PriceChange = updated.PriceChange;
                    marketData.PriceChangePcnt = updated.PriceChangePcnt;
                    marketData.Volume = updated.Volume;
                    marketData.AvgVol = updated.AvgVol;
                    marketData.VolumeChange = updated.VolumeChange;
                }
            }
            Update(updatedData);
            UpdatePricingFetchSiteAudit();
            sb.AppendLine("Updated today's data from Yahoo..<br/>");
            return sb.ToString();
        }
        /// <summary>
        /// Updates the signal data fetch for the current user
        /// </summary>
        public void UpdateSignalFetchSiteAudit()
        {
            var _userService = new UserService(HttpContext.Current.Session[Constants.SessionAppNameKey].ToString());
            try
            {
                User current = new User();
                SiteAuditRecord sr = new SiteAuditRecord();
                sr.LastSignalFetchDate = DateTime.Now;
                if(!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    current.UserName = "System";
                }
                else
                {
                    current = _userService.Get(HttpContext.Current.User.Identity.Name);
                } 
                sr.LastSignalFetchBy = current.UserName;
                db.SiteAuditRecords.Add(sr);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Could not update audit trail:" + ex.Message);
            }
          

        }
        /// <summary>
        /// Updates the pricing data fetch for the current user
        /// </summary>
        public void UpdatePricingFetchSiteAudit()
        {
            var _userService = new UserService(HttpContext.Current.Session[Constants.SessionAppNameKey].ToString());
            try
            {
                User current = new User();
                SiteAuditRecord sr = new SiteAuditRecord();
                sr.LastPricingFetchDate = DateTime.Now;
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    current.UserName = "System";
                }
                else
                {
                    current = _userService.Get(HttpContext.Current.User.Identity.Name);
                }
                sr.LastPricingFetchBy = current.UserName;
                db.SiteAuditRecords.Add(sr);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Could not update audit trail:" + ex.Message);
            }


        }
        /// <summary>
        /// Returns the latest Pricing fetch audit
        /// </summary>
        /// <returns></returns>
        public SiteAuditRecord GetLatestPricingFetch()
        {
            try
            {
                var dateLatest = db.SiteAuditRecords.Max(t => t.LastPricingFetchDate);
                return db.SiteAuditRecords.Single(t => t.LastPricingFetchDate == dateLatest);
            }
            catch(Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Couldn't get latest pricing audit:" + ex.Message);
            }
            return new SiteAuditRecord();
          
        }
        /// <summary>
        /// Returns the lastes Signal Fetch Audit
        /// </summary>
        /// <returns></returns>
        public SiteAuditRecord GetLatestSignalFetch()
        {
            try
            {
                var dateLatest = db.SiteAuditRecords.Max(t => t.LastSignalFetchDate);
                return db.SiteAuditRecords.Single(t => t.LastSignalFetchDate == dateLatest);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Couldn't get latest pricing audit:" + ex.Message);
            }
            return new SiteAuditRecord();

        }
    }
}