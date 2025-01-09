using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using HedgefundMe.com.Models;
using System.Text;
namespace HedgefundMe.com.Services
{
    /// <summary>
    /// Does everything to query the top stock table
    /// </summary>
    public class StockService
    {
        private readonly ProjectEntities db;
        /// <summary>
        /// The latest date available
        /// </summary>
        public  DateTime StartOn
        {
            get { return GetLatestStockDateAvailable(); }
        }
        public   DateTime DayBefore
        {
            get { return GetStockDateAvailableDayBefore(StartOn); }
        }
        /// <summary>
        /// The first date in the database
        /// </summary>
        public DateTime FirstDate
        {
            get { return db.Stocks.Min(t => t.Date); }
        }
 
        public StockService(ProjectEntities context)
        {    
               db = context;
        }
        
        /// <summary>
        /// Returns the real date either for the day or the day before available in the database
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public DateTime GetStockDateAvailableOn(DateTime theDate)
        {
            List<TopStock> current = new List<TopStock>();
            int counter = 0;
            do
            {
                if (counter < -9)
                {
                    throw new Exception("No data found for earlier date.");
                }
                theDate = DateHelper.SanitizeDate(theDate);
                counter--;
                current = (from r in db.Stocks where r.Date == theDate select r).ToList();
            }
            while (current.Count == 0);
            return theDate;
        }
        /// <summary>
        /// Returns the latest  date int the database
        /// </summary>
        /// <returns></returns>
        public DateTime GetLatestStockDateAvailable(bool shorts = false)
        {
        if(shorts)
            {
                return (from d in db.Shorts select d.Date).Max();
            }
            return (from d in db.Stocks select d.Date).Max(); 
        }
         
        /// <summary>
        /// Gets the very frist date in the Stocks table
        /// </summary>
        /// <returns></returns>
        public DateTime GetEarliestStockDateAvailable()
        {
            return (from d in db.Stocks select d.Date).Min();
        }
        /// <summary>
        /// Gets the T-1 date that we have the rankings for from the given date
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public DateTime GetStockDateAvailableDayBefore(DateTime theDate)
        {
            List<TopStock> past = new List<TopStock>();
            DateTime previous = theDate;
            do
            {
                if (previous.Date.Year < theDate.Year - 3)
                {
                    return FirstDate;
                    //throw new Exception("No previous data found for earlier date.");
                }
                previous = DateHelper.SanitizeDate(previous.AddDays(-1)).Date;
                past = (from r in db.Stocks where r.Date == previous select r).ToList();
            }
            while (!past.Any());
            return previous;
        } 
        /// <summary>
        /// Gets the T-1 date and returns those stocks on that date
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public List<TopStock> GetStocksOnTheDayBefore(DateTime theDate)
        {
            List<TopStock> past = new List<TopStock>();
            DateTime previous = theDate;
            do
            {
                if (previous.Date.Year < theDate.Year - 3)
                {
                     throw new Exception("No previous data found for earlier date.");
                }
                previous = DateHelper.SanitizeDate(previous.AddDays(-1)).Date;
                past = (from r in db.Stocks where  r.Date == previous select r).ToList();
            }
            while (!past.Any());
            return past;
        }
        /// <summary>
        /// Gets the T-1 date and returns those stocks on that date
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public List<BottomStock> GetShortsOnTheDayBefore(DateTime theDate)
        {
            List<BottomStock> past = new List<BottomStock>();
            DateTime previous = theDate;
            do
            {
                if (previous.Date.Year < theDate.Year - 3)
                {
                    throw new Exception("No previous data found for earlier date.");
                }
                previous = DateHelper.SanitizeDate(previous.AddDays(-1)).Date;
                past = (from r in db.Shorts where r.Date == previous select r).ToList();
            }
            while (!past.Any());
            return past;
        }
        /// <summary>
        /// Figures out the real date and returns the list from that date
        /// </summary>
        /// <param name="theDateUncleaned"></param>
        /// <returns></returns>
        public List<BottomStock> GetShortStocksOn(DateTime theDateUncleaned)
        {
            List<BottomStock> current = new List<BottomStock>();
            int counter = 0;
            do
            {
                if (counter < -25)//300 days back
                {
                    if (!db.Shorts.Any(t => t.Date == theDateUncleaned))
                    {
                        //we dont have this date so return nothing.
                        return new List<BottomStock>();
                    }

                    throw new Exception("No data found for earlier date.");
                }
                theDateUncleaned = DateHelper.SanitizeDate(theDateUncleaned.AddDays(counter));
                counter--;
                current = (from r in db.Shorts where r.Date == theDateUncleaned select r).ToList();
            }
            while (!current.Any());
            return current;
        }
        /// <summary>
        /// Figures out the real date and returns the list from that date
        /// </summary>
        /// <param name="theDateUncleaned"></param>
        /// <returns></returns>
        public List<TopStock> GetStocksOn(DateTime theDateUncleaned)
        {
            List<TopStock> current = new List<TopStock>();
            int counter = 0;
            do
            {
                if (counter < -25)//300 days back
                {
                    if (!db.Stocks.Any(t => t.Date == theDateUncleaned))
                    {
                        //we dont have this date so return nothing.
                        return new List<TopStock>();
                    }

                    throw new Exception("No data found for earlier date.");
                }
                theDateUncleaned = DateHelper.SanitizeDate(theDateUncleaned.AddDays(counter));
                counter--;
                current = (from r in db.Stocks where r.Date == theDateUncleaned select r).ToList();
            }
            while (!current.Any());
            return current;
        }
        /// <summary>
        /// Figures out the real date and returns the list from that date
        /// </summary>
        /// <param name="theDateUncleaned"></param>
        /// <returns></returns>
        public List<TopStock> GetJustStocksOn(DateTime theDateUncleaned)
        {
            List<TopStock> current = new List<TopStock>();
            int counter = 0;
            do
            {
                if (counter < -25)//300 days back
                {
                    if (!db.Stocks.Any(t => t.Date <= theDateUncleaned))
                    {
                        //we dont have this date so return nothing.
                        return new List<TopStock>();
                    }

                    throw new Exception("No data found for earlier date.");
                }
                theDateUncleaned = DateHelper.SanitizeDate(theDateUncleaned.AddDays(counter));
                counter--;
                current = db.Stocks.Where(r => r.Date == theDateUncleaned).ToList();
            }
            while (!current.Any());
            return current;
        }
        public TopStock GetLatest(string ticker)
        {
            try
            {  
                var results = db.Stocks.Include("Scans")
                                    .Where(s => s.Ticker == ticker
                                     && s.Date == StartOn);
                if (!results.Any())
                {
                    return new TopStock();//the nothing scan
                }
                return results.First();
            }
            catch (Exception)
            {

                
            }
            return new TopStock(); //unknown 
        }
        /// <summary>
        /// Gets the latest short stock with this ticker
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public BottomStock GetLatestShort(string ticker)
        {
            try
            {
                var results = db.Shorts.Where(s => s.Ticker == ticker
                                     && s.Date == StartOn);
                if (!results.Any())
                {
                    return new BottomStock();//the nothing scan
                }
                return results.First();
            }
            catch (Exception)
            {


            }
            return new BottomStock(); //unknown 
        }
        /// <summary>
        /// Gets the latest data for today
        /// </summary>
        /// <returns></returns>
        public List<TopStock> GetLatest()
        {
            try
            {
                var results = db.Stocks.Include("Scans").Where(s=>s.Date == StartOn).ToList();
                if (!results.Any())
                {
                    return new List<TopStock>();//the nothing scan
                }
                return results;
            }
            catch (Exception)
            { 

            }
            return new List<TopStock>(); //unknown 
        }
        /// <summary>
        /// Gets the latest data for today
        /// </summary>
        /// <returns></returns>
        public List<BottomStock> GetLatestShorts()
        {
            try
            {
                var results = db.Shorts.Where(s => s.Date == StartOn).ToList();
                if (!results.Any())
                {
                    return new List<BottomStock>();//the nothing scan
                }
                return results;
            }
            catch (Exception)
            {

            }
            return new List<BottomStock>(); //unknown 
        }
        /// <summary>
        /// Gets the next rank up for today after the stock provided
        /// </summary>
        /// <returns></returns>
        public  TopStock GetNextLatest(string stock)
        {
            try
            {
                var results = db.Stocks.Include("Scans").Where(s => s.Date == StartOn).OrderByDescending(t => t.CurrentRank).ToList();
                if (!results.Any(t=>t.Ticker == stock)) //this doesnt exist
                {
                    return results.First();
                }
                var thisone = results.First(t => t.Ticker == stock).CurrentRank;
                if(!results.Any(t=>t.CurrentRank < thisone)) //there are none higher we are at 1
                {
                    return results.First();
                }
                return results.First(t=>t.CurrentRank < thisone);
            }
            catch (Exception)
            { 

            }
            return new  TopStock(); //unknown 
        }
        /// Gets the previous rank up for today after the stock provided
        /// </summary>
        /// <returns></returns>
        public TopStock GetPreviousLatest(string stock)
        {
            try
            {
                var results = db.Stocks.Include("Scans").Where(s => s.Date == StartOn).OrderBy(t => t.CurrentRank).ToList();
                if (!results.Any(t => t.Ticker == stock)) //this doesnt exist
                {
                    return results.First();
                }
                var thisone = results.First(t => t.Ticker == stock).CurrentRank;
                if (!results.Any(t => t.CurrentRank > thisone)) //there are none lower we are at 100
                {
                    return results.First();
                }
                return results.First(t => t.CurrentRank > thisone);
            }
            catch (Exception)
            {

            }
            return new TopStock(); //unknown 
        }
        /// <summary>
        /// Gets the data for the stock on the date
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public TopStock GetStockOn(string ticker, DateTime onDate)
        {
            try
            {
                var results = db.Stocks.Include("Scans")
                                   .Where(s => s.Ticker == ticker
                                    && s.Date == onDate);
                if (!results.Any())
                {
                    return new TopStock();//the nothing TopStock
                }
                return results.First();
            }
            catch (Exception)
            {

            }
             return new TopStock(); //unknown scan
        }
        public List<TopStock> GetStockValuesFrom(DateTime start, DateTime end)
        {
            List<TopStock> results = new List<TopStock>();
             try
            {  
                results = db.Stocks.Where(s => s.Date >= start && s.Date <= end).ToList();
                if (!results.Any())
                {
                    return new List<TopStock>();//the nothing TopStock
                }
                return results;
            }
            catch (Exception)
            {
                return new List<TopStock>(); //unknown TopStock
            }
        }
        /// <summary>
        /// Gets all the values for the stock since the date
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="startOn"></param>
        /// <returns></returns>
        public List<TopStock> GetAllStockValuesSince(string ticker, DateTime startOn)
        {
              try
            {
                var results = db.Stocks.Include("Scans").Where(s => s.Ticker == ticker
                                     && s.Date >= startOn).OrderByDescending(t => t.Date);
                if (!results.Any())
                {
                    return new List<TopStock>();
                } 
                return results.ToList();
            }
            catch (Exception)
            {

                return new List<TopStock>();//the nothing scan
            }
        }
        /// <summary>
        /// Gets all the values for the stock since the date
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="startOn"></param>
        /// <returns></returns>
        public List<BottomStock> GetAllShortStockValuesSince(string ticker, DateTime startOn)
        {
            try
            {
                var results = db.Shorts.Where(s => s.Ticker == ticker
                                     && s.Date >= startOn).OrderByDescending(t => t.Date);
                if (!results.Any())
                {
                    return new List<BottomStock>();
                }
                return results.ToList();
            }
            catch (Exception)
            {

                return new List<BottomStock>();//the nothing scan
            }
        }
        /// <summary>
        /// Gets all the values for all stocks since the date
        /// </summary>
        /// <param name="startOn"></param>
        /// <returns></returns>
        public List<TopStock> GetAllStockValuesSince(DateTime startOn)
        {
            var scanresults = new List<TopStock>();
            try
            {
                scanresults = db.Stocks.Where(s => s.Date >= startOn).ToList();
                if (!scanresults.Any())
                {
                    return new List<TopStock>();
                }
                return scanresults;
            }
            catch (Exception)
            {

                return scanresults;//the nothing scan
            }
        }
        /// <summary>
        /// Gets all the values for all stocks since the date
        /// </summary>
        /// <param name="startOn"></param>
        /// <returns></returns>
        public List<BottomStock> GetAllShortStockValuesSince(DateTime startOn)
        {
            var scanresults = new List<BottomStock>();
            try
            {
                scanresults = db.Shorts.Where(s => s.Date >= startOn).ToList();
                if (!scanresults.Any())
                {
                    return new List<BottomStock>();
                }
                return scanresults;
            }
            catch (Exception)
            {

                return scanresults;//the nothing scan
            }
        }
        /// <summary>
        /// Gets the number of each trend marker for good bad scans
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public MarketDataPoints GetTrendsFor(string scan, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-30);
            }
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }
            MarketDataPoints m = new MarketDataPoints { Name = string.Format("{0} from {1} to {2}", scan, startDate.Value.ToShortDateString(), endDate.Value.ToShortDateString()) };
             //for each scan and date
           //count( child.where(x=> x.parentID == inputParentID)
            //from the start to the end date
            DateTime start = startDate.Value;
            var sid = db.StockScans.Single(t => t.Name == scan).ID;
            var allValues = db.Stocks.Where(i => i.Date >= start && i.Date <= endDate.Value  && i.Scans.Any(t=> t.Name==scan));
            do
            {

                MarketDataPoint dp = new MarketDataPoint { Date = start.Date, Label = scan };
                dp.Value = allValues.Count(p => p.Scans.Any(y => y.ID == sid));//allValues.Where(testc=>testc.Date== start).Count(p => p.Scans.Any(o => o.Name == scan));
                m.Values.Add(dp); 
                start = start.AddDays(1);
            } 
            while (start <= endDate); 
            return m; 
        }
        /// <summary>
        /// Gets the latest NEW scan result of the first one we have in the database
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public TopStock GetFirstRecordAvailableFor(string ticker)
        {
            if(!db.Stocks.Any(f => f.Ticker == ticker))
            {
                return new TopStock();
            }
            DateTime firstRankDate;
            
                if (db.Stocks.Any(t=>t.Ticker == ticker && t.Direction == Strings.Directions.NEW))
                {
                     firstRankDate = (from t in db.Stocks
                                      where t.Ticker == ticker && t.Direction == Strings.Directions.NEW
                                 select t.Date).Max();
                }
                else
                {
                      firstRankDate = (from d in db.Stocks
                                 where d.Ticker == ticker
                                 select d.Date).Min();
                    
                }
                return db.Stocks.Include("Scans").Where(f => f.Ticker == ticker && f.Date == firstRankDate).First();         
        }
        public List<TopStock> RunScan(string scanType, IEnumerable<TopStock> onList)
        {
            switch (scanType)
            {

                case Strings.ScanNames.BigRankIncrease: //[+10 in Rank and +% Gain] or +8.00% in Price
                    {
                        return (from p in onList
                                where (p.RankChange >= 10 && p.Gain > 0.0) && p.Direction != Strings.Directions.NEW  
                                orderby p.CurrentRank ascending
                                select p).ToList();
                    }
                case Strings.ScanNames.BigPriceIncrease: //[+8.00% in Price and + rank change
                    {
                        return (from p in onList
                                where (p.RankChange >= 1 && p.Gain > 0.08) && p.Direction != Strings.Directions.NEW
                                orderby p.CurrentRank ascending
                                select p).ToList();
                    }
                case Strings.ScanNames.BigPriceDrops:
                    {
                        return (from p in onList
                                where (p.Gain < -0.09) && p.Direction != Strings.Directions.NEW
                                orderby p.CurrentRank ascending
                                select p).ToList();

                    }
                case Strings.ScanNames.BigRankDrops:
                    {
                        return (from p in onList
                                where (p.RankChange < -10) && p.Direction != Strings.Directions.NEW
                                orderby p.CurrentRank ascending
                                select p).ToList();

                    }
                case Strings.ScanNames.Top5PriceChanges:
                    {
                        return (from stocks in onList orderby stocks.Gain descending select stocks).Take(5).OrderBy(t => t.CurrentRank).ToList();

                    }
                case Strings.ScanNames.Top5RankChanges:
                    {
                        return (from stocks in onList orderby stocks.RankChange descending select stocks).Take(5).OrderBy(t => t.CurrentRank).ToList();

                    }
                case Strings.ScanNames.UpOnHighVolume:
                    {
                        return (from p in onList
                                where (p.VolumeChange > 0.015)
                                && p.Gain > 0.03
                                orderby p.CurrentRank ascending
                                select p).ToList();
                    }
                case Strings.ScanNames.DownOnHighVolume:
                    {
                        return (from p in onList
                                where (p.VolumeChange > 0.015)
                                && p.Gain < -0.03
                                orderby p.CurrentRank ascending
                                select p).ToList();
                    }
                
                case Strings.ScanNames.Adds:
                    {
                        return (from p in onList
                                where p.Direction == Strings.Directions.NEW
                                orderby p.CurrentRank ascending
                                select p).ToList();
                    }
                case Strings.ScanNames.Drops:
                    {
                        return GetDrops(onList);
                    }
                case Strings.ScanNames.NewPriceHighs:
                    {
                        var results = new List<TopStock>();
                        //find all the stocks where the max price date is equal to today
                        //and the stock is not new for today
                        var today = from t in onList where t.Direction != Strings.Directions.NEW && t.Gain > 0 select t.Ticker;
                        foreach (var ticker in today)
                        {
                            var maxPrice = db.Stocks.Where(r => r.Ticker == ticker).OrderByDescending(u => u.Price).FirstOrDefault();
                            if (maxPrice.Date == StartOn)//max price is today
                            {
                                try
                                { 
                                    results.Add(maxPrice.Clone() as TopStock);
                                }
                                catch (Exception)
                                {
                                    Logger.WriteLine(MessageType.Error, "Could not get new price high for " + ticker);
                                }
                            }
                        }
                        return results;
                    }
                case Strings.ScanNames.NewRankHighs:
                    {
                        var results = new List<TopStock>();
                        //find all the stocks where the max rank, which is really min rank, is equal to today
                        //and the stock is not new for today
                        var today = from t in onList where t.Direction != Strings.Directions.NEW && t.RankChange > 0 select t.Ticker;
                        foreach (var ticker in today)
                        {
                            var minRank = db.Stocks.Where(r => r.Ticker == ticker).OrderBy(u => u.CurrentRank).ThenByDescending(p => p.Date).FirstOrDefault();
                            if (minRank.Date == StartOn)//max price is today
                            {
                                try
                                {   
                                    results.Add(minRank);
                                }
                                catch (Exception)
                                {
                                    Logger.WriteLine(MessageType.Error, "Could not get new rank high scan result for " + ticker);
                                }
                            }
                        }
                        return results;
                    }
                case Strings.ScanNames.Buys:
                    {

                        return onList.Where(t => t.Opinion == Strings.Opinions.Buy).ToList();
                    }
                case Strings.ScanNames.Sells:
                    {

                        return onList.Where(t => t.Opinion == Strings.Opinions.Sell).ToList();
                    }
                case Strings.ScanNames.SellStops:
                    {
                        return onList.Where(t => t.Opinion == Strings.Opinions.SellStop).ToList();
                    }
                default:
                    {
                        return new List<TopStock>();
                    }
            }

        }
        
        public PerformanceScan GetScan(string scanName)
        {
            var p = db.StockScans.Where(r => r.Name == scanName);
            if (!p.Any())
            {
                PerformanceScan s = new PerformanceScan();
                s.Name = scanName;
                s.Description = Strings.ScanNames.DescribeScan(scanName);
                s.ScanSide = Strings.ScanNames.TypeOfScan(scanName);
                db.StockScans.Add(s);
                db.SaveChanges();
                return s;
            }
            return p.First();

        }
        public string RunAllScans(DateTime onDate)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine(string.Format("<h2>Running Scans for...{0}</h2>", onDate.ToShortDateString())); 
                var performances =  RunPerformanceScan(onDate);
                performances = RunIndividualScansOn(performances);
                sb.AppendFormat("<br/>Ran {0} scans.<br/>");
                foreach (TopStock ss in performances)
                {
                    db.Stocks.Add(ss);
                }
                db.SaveChanges();
                sb.AppendFormat("<br/>Saved {0} {1}", performances.Count());
                //get the count
                Logger.WriteLine(MessageType.Information, "Saved scans");
                sb.AppendLine("<br/>Saved all scans");
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, ex.Message);
                sb.AppendLine(string.Format("<br/>Error:{0}<br/>{1}", ex.Message, ex.InnerException.Message));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Runs the top 5, adds drops etc
        /// </summary>
        /// <param name="onstocks"></param>
        /// <returns></returns>
        public List<TopStock> RunIndividualScansOn(List<TopStock> onstocks)
        {
            var allscans = Strings.ScanNames.AllScanNames();
            foreach (string t in allscans)
            {
                var scan = GetScan(t);
                
                var scanresults = RunScan(t, onstocks);
                foreach (TopStock ss in scanresults)
                {
                    
                    try
                    {
                         //we need to add this scan to the stock
                        var p = onstocks.Where(g => g.Ticker == ss.Ticker).First();
                        p.Scans.Add(scan);
                    }
                    catch(Exception)
                    {
                        //we may not have this stock in the list like in the case of a drop
                    }
                } 
            }
            return onstocks;
        }
        public List<TopStock> GetDrops(IEnumerable<TopStock> onlist)
        {
            List<TopStock> dropped = new List<TopStock>();
          
            if(!onlist.Any())
            {
                return dropped;
            }
            var date = onlist.Select(t => t.Date).First();
             List<TopStock> past = GetStocksOnTheDayBefore(date);
            foreach (TopStock r in past)
            {
                if (!onlist.Any(t => t.Ticker == r.Ticker))
                {
                    TopStock droppedTicker = (TopStock)r.Clone();
                    if (!dropped.Any(p => p.Ticker == droppedTicker.Ticker))
                    {
                        dropped.Add(droppedTicker);
                    } 
                }
            }
            return dropped;
        }
        public List<BottomStock> GetDroppedShorts(IEnumerable<BottomStock> onlist)
        {
            List<BottomStock> dropped = new List<BottomStock>();

            if (!onlist.Any())
            {
                return dropped;
            }
            var date = onlist.Select(t => t.Date).First();
            List<BottomStock> past = GetShortsOnTheDayBefore(date);
            foreach (BottomStock r in past)
            {
                if (!onlist.Any(t => t.Ticker == r.Ticker))
                {
                    BottomStock droppedTicker = (BottomStock)r.Clone();
                    if (!dropped.Any(p => p.Ticker == droppedTicker.Ticker))
                    {
                        dropped.Add(droppedTicker);
                    }
                }
            }
            return dropped;
        }
        /// <summary>
        /// Gets the previous opinion if any before the given date
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        public TopStock GetPreviousOpinionFor(string ticker, DateTime before)
        {
             
            try
            {
                if(!db.Stocks.Any(t=>t.Ticker == ticker && t.Date < before && !string.IsNullOrEmpty(t.Opinion)))
                {
                    return new TopStock();//no opinion
                }
                var opinionDate = db.Stocks.Where(t => t.Ticker == ticker && t.Date < before && !string.IsNullOrEmpty(t.Opinion)).Max(t => t.Date);
                if (opinionDate == null)
                {
                    return new TopStock();//no opinion
                }
                //get the latest opinion before this
                return GetStockOn(ticker, opinionDate);
            }
            catch (Exception)
            {
                return new TopStock();//no opinion
            }
        } 
        /// <summary>
        /// loops over all stocks and adds them
        /// </summary>
        /// <param name="stocks"></param>
        public void AddStocks(List<TopStock> stocks)
        {
            foreach(var stock in stocks)
            {
                db.Stocks.Add(stock);
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Adds one stock
        /// </summary>
        /// <param name="stock"></param>
        public void AddStock(TopStock stock)
        {
                db.Stocks.Add(stock);
                db.SaveChanges();
        }
        public void UpdateStock(TopStock stock)
        { 
            db.Entry(stock).State = EntityState.Modified;  
            db.SaveChanges();
        }
        /// <summary>
        /// Updates 
        /// </summary>
        /// <param name="stocks"></param>
        public void UpdateStocks(List<TopStock> stocks)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            foreach( var stock in stocks)
            { 
                db.Entry(stock).State = EntityState.Modified; 
            }         
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Updates 
        /// </summary>
        /// <param name="stocks"></param>
        public void UpdateStocks(List<BottomStock> stocks)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            foreach (var stock in stocks)
            {
                db.Entry(stock).State = EntityState.Modified;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Returns XML for the latest trades only
        /// </summary>
        /// <returns></returns>
        public string GetLatestTradesAsXml()
        {
            return SerializerService.Serialize2(db.LatestTradeSignals.Where(t=>t.Date == StartOn).ToList());
        }
        /// <summary>
        /// Returns XML for the latest trades only
        /// </summary>
        /// <returns></returns>
        public string GetLatestShortsAsXml()
        {
            return SerializerService.Serialize3(db.LatestShortSignals.Where(t => t.Date == StartOn).ToList());
        }
        /// <summary>
        /// Gets the previous day's stocks and adds them to the dropped scan
        /// </summary>
        public void UpdateDrops()
        {
            //get the drops for today
            List<TopStock> current = db.Stocks.Where(t => t.Date == StartOn).ToList();

            if (!current.Any())
            {
                return;
            }
            var dropped = GetDrops(current); 
            //for each stock add the drop scan to the stock
            //save the scan to the stock 
            var droppedScan = db.StockScans.First(t => t.Name == Strings.ScanNames.Drops);
            foreach (var t in dropped)
            {
                if (!t.Scans.Any(r => r.ID == droppedScan.ID))
                {
                    t.Scans.Add(droppedScan);
                    db.Entry(t).State = EntityState.Modified;
                }
            }
            
        }
        /// <summary>
        /// Looks up the trades from the database
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<TodaysTrade> GetTradesOn(DateTime date)
        {
            return db.TodaysTrades.Where(t => t.Date == date).ToList();
        }
        /// <summary>
        /// Looks up the trades from the database
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<TodaysShortTrade> GetShortTradesOn(DateTime date)
        {
            return db.TodaysShortTrades.Where(t => t.Date == date).ToList();
        }
        /// <summary>
        /// Runs all the trades for the given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<TodaysTrade> RunTradesFor(DateTime date)
        {
            var trades = new List<TodaysTrade>();
            var onDate = GetStocksOn(date);
            var theDate = onDate.First().Date;
            var drops =  GetDrops(onDate);
            var scans = onDate.Where(t => !string.IsNullOrEmpty(t.Opinion)).OrderBy(t => t.Opinion).ToList(); 
            foreach (var r in scans)//find all the scan results for today that have an opinion
            {
                TodaysTrade t = new TodaysTrade(r); 
                t = OpinionService.UpdateSellPriceDetails(r, t);
                trades.Add(t);
            }
            //get drops each of these are sells
            foreach (var r in drops)
            { 
                TodaysTrade dropped = new TodaysTrade(r);
                dropped.Date = theDate;
                dropped = OpinionService.UpdateDropDetails(dropped);
                trades.Add(dropped);
            }
            return trades;
        }
        /// <summary>
        /// Runs the trade signals for the date, deletes any for the same day then adds the new trades
        /// </summary>
        /// <param name="theDate"></param>
        public void UpdateTradesOn(DateTime theDate)
        {
            //get the trades, then save them
            var theTrades = RunTradesFor(theDate);
            //no duplicates
            var oldTrades = db.TodaysTrades.Where(t => t.Date == theDate).ToList();
            oldTrades.ForEach(t => db.Entry(t).State = EntityState.Deleted);
            db.SaveChanges();
            theTrades.ForEach(y => db.TodaysTrades.Add(y));
            db.SaveChanges();
        }
        /// <summary>
        /// Runs the trade signals for the date, deletes any for the same day then adds the new trades
        /// </summary>
        /// <param name="theDate"></param>
        public void UpdateShortTradesOn(DateTime theDate)
        {
            //get the trades, then save them
            var theTrades = RunShortTradesFor(theDate);
            //no duplicates
            var oldTrades = db.TodaysShortTrades.Where(t => t.Date == theDate).ToList();
            oldTrades.ForEach(t => db.Entry(t).State = EntityState.Deleted);
            db.SaveChanges();
            theTrades.ForEach(y => db.TodaysShortTrades.Add(y));
            db.SaveChanges();
        }
        /// <summary>
        /// Gets the latest trades for today and updates them
        /// </summary>
        public void UpdateLatestTrades()
        {
            //get all the trades for today 
            var Trades = GetTradesOn(StartOn);
            var oldTrades = db.LatestTradeSignals.Where(t => t.Date == StartOn).ToList();
            var latestTrades = new List<LatestTradeSignal>();
            //find all the trades not in the LatestTrade Signals,those are new 
            foreach (var justin in Trades)
            {
                if (!oldTrades.Any(t => t.Ticker == justin.Ticker)) //we dont have this
                {
                    latestTrades.Add(new LatestTradeSignal(justin));   //we need to add this trade
                }
            }  
            foreach(var t in oldTrades)
            {
                db.Entry(t).State = EntityState.Deleted; //delete all old from the database
            }
            db.SaveChanges();
            foreach(var trade in latestTrades)
            {
                db.LatestTradeSignals.Add(trade);  //add the new ones
            } 
            db.SaveChanges(); //done  
        }
        #region shorts
        /// <summary>
        /// Gets the latest trades for today and updates them
        /// </summary>
        public void UpdateLatestShortTrades()
        {
            //get all the trades for today 
            var Trades = GetShortTradesOn(StartOn); 
            var oldTrades = db.LatestShortSignals.Where(t => t.Date == StartOn).ToList();
            var latestTrades = new List<LatestShortSignal>();
            //find all the trades not in the LatestTrade Signals,those are new 
            foreach (var justin in Trades)
            {
                if (!oldTrades.Any(t => t.Ticker == justin.Ticker)) //we dont have this
                {
                    latestTrades.Add(new LatestShortSignal(justin));   //we need to add this trade
                }
            }
            foreach (var t in oldTrades)
            {
                db.Entry(t).State = EntityState.Deleted; //delete all old from the database
            }
            db.SaveChanges();
            foreach (var trade in latestTrades)
            {
                db.LatestShortSignals.Add(trade);  //add the new ones
            }
            db.SaveChanges(); //done  
        }
        /// <summary>
        /// Runs a performance scan for all tickers for the date, including opinions.
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<BottomStock> RunShortScan(DateTime onDate)
        {
            List<BottomStock> current = db.Shorts.Where(t => t.Date == onDate).ToList();
            List<BottomStock> past = GetShortStocksOn(onDate.AddDays(-1));
            if (!past.Any())
            {
                return current;
            }
            return RunShortScan(current, past);
        }
        /// <summary>
        /// Runs a performance scan for all tickers for the latest date, including opinions.
        /// </summary>
        /// <returns></returns>
        public List<BottomStock> RunShortScan(List<BottomStock> current, List<BottomStock> past)
        {
            OpinionService oservice = new OpinionService(db);
            List<BottomStock> perf = new List<BottomStock>();
            foreach (BottomStock performance in current)
            {
                try
                {
                    var previousranks = (from t in past where t.Ticker.TrimEnd() == performance.Ticker.TrimEnd() select t);
                    #region newstock
                    if (previousranks.Count() == 0)//its new
                    {
                        performance.PreviousRank = 0;
                        performance.RankChange = 0;
                        performance.Direction = Strings.Directions.NEW;
                        performance.Color = FinanceHelper.HeatMapColor(0);
                        performance.StartDate = DateTime.Now.Date;
                        performance.StartPrice = performance.Price;
                        performance.DaysInList = 0;
                        performance.GainTillNow = 0.0;
                    }
                    #endregion
                    #region existing stock
                    else
                    {
                        BottomStock previousrank = previousranks.First();
                        performance.PreviousRank = previousrank.CurrentRank;
                        performance.RankChange = (previousrank.CurrentRank - performance.CurrentRank);
                        performance.VolumeChange = FinanceHelper.VolumeChange(performance.AvgVol, performance.Volume);
                        performance.Color = FinanceHelper.HeatMapColor(performance.RankChange);
                        performance.Direction = FinanceHelper.Direction(performance.RankChange);
                        performance.Gain = ((performance.Price - previousrank.Price) / previousrank.Price);
                        var firstScanResult = GetFirstRecordAvailableFor(performance.Ticker);
                        performance.StartDate = firstScanResult.Date;
                        performance.StartPrice = firstScanResult.Price;
                        performance.DaysInList = previousrank.DaysInList + 1; 
                        performance.GainTillNow = (performance.Price - performance.StartPrice) / performance.StartPrice; 
                        if( Double.IsInfinity(performance.GainTillNow))
                        {
                            performance.GainTillNow = 0.0;
                        }
                        performance.CurrentSide = previousrank.CurrentSide;
                        performance.StopPrice = previousrank.StopPrice;
                    }
                    #endregion
                   
                    oservice.GetOpinion(performance); 
                    perf.Add(performance);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, "Error running short performance scan for " + performance.Ticker + " :" + ex.Message);
                }
            }
            return perf.OrderBy(t => t.CurrentRank).ToList();
        }
        /// <summary>
        /// Runs all the trades for the given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<TodaysShortTrade> RunShortTradesFor(DateTime date)
        {
            var trades = new List<TodaysShortTrade>();
            var onDate = GetShortStocksOn(date);
            var theDate = onDate.First().Date;
            var drops = GetDroppedShorts(onDate);
            var scans = onDate.Where(t => !string.IsNullOrEmpty(t.Opinion)).OrderBy(t => t.Opinion).ToList();
            foreach (var r in scans)//find all the scan results for today that have an opinion
            {
                TodaysShortTrade t = new TodaysShortTrade(r);
                t = OpinionService.UpdateSellPriceDetails(r, t);
                trades.Add(t);
            }
            //get drops each of these are sells
            foreach (var r in drops)
            {
                TodaysShortTrade dropped = new TodaysShortTrade(r);
                dropped.Date = theDate;
                dropped = OpinionService.UpdateShortDropDetails(dropped);
                trades.Add(dropped);
            }
            return trades;
        }
        #endregion
        /// <summary>
        /// Runs a performance scan for all tickers for the date, including opinions.
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<TopStock> RunPerformanceScan(DateTime onDate)
        { 
            List<TopStock> current = db.Stocks.Where(t => t.Date == onDate).ToList();
            List<TopStock> past = GetStocksOn(onDate.AddDays(-1));
            if(!past.Any())
            {
                return current;
            }
            return RunPerformanceScan(current, past);
        }
        /// <summary>
        /// Runs a performance scan for all tickers for the latest date, including opinions.
        /// </summary>
        /// <returns></returns>
        public List<TopStock> RunPerformanceScan(List<TopStock> current, List<TopStock> past)
        {
            OpinionService oservice = new OpinionService(db);
            List<TopStock> perf = new List<TopStock>();
            foreach (TopStock performance in current)
            { 
                try
                {
                    var previousranks = (from t in past where t.Ticker.TrimEnd() == performance.Ticker.TrimEnd() select t);
                    #region newstock
                    if (previousranks.Count() == 0)//its new
                    {
                        performance.PreviousRank = 0;
                        performance.RankChange = 0;
                        performance.Direction = Strings.Directions.NEW; 
                        performance.Color = FinanceHelper.HeatMapColor(0); 
                        performance.StartDate = DateTime.Now.Date;
                        performance.StartPrice = performance.Price;
                        performance.DaysInList = 0;
                        performance.GainTillNow = 0.0; 
                    }
                    #endregion
                    #region existing stock
                    else
                    {
                        TopStock previousrank = previousranks.First();
                        performance.PreviousRank = previousrank.CurrentRank;
                        performance.RankChange = (previousrank.CurrentRank- performance.CurrentRank);
                        performance.VolumeChange = FinanceHelper.VolumeChange(performance.AvgVol, performance.Volume);
                        performance.Color = FinanceHelper.HeatMapColor(performance.RankChange);
                        performance.Direction = FinanceHelper.Direction(performance.RankChange);
                        performance.Gain = ((performance.Price - previousrank.Price) / previousrank.Price);
                        var firstScanResult = GetFirstRecordAvailableFor(performance.Ticker);
                        performance.StartDate = firstScanResult.Date;
                        performance.StartPrice = firstScanResult.Price;
                        performance.DaysInList = previousrank.DaysInList + 1;
                        performance.GainTillNow = (performance.Price - performance.StartPrice) / performance.StartPrice;
                        performance.CurrentSide = previousrank.CurrentSide;
                        performance.StopPrice = previousrank.StopPrice;
                    }
                    #endregion
                    oservice.GetOpinion(performance); 
                    performance.Scans.Clear();
                    perf.Add(performance);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, "Error running performance scan for " + performance.Ticker + " :" + ex.Message);
                }
            }
            return perf.OrderBy(t => t.CurrentRank).ToList();
        }
        /// <summary>
        /// Deletes latest data
        /// </summary>
        /// <param name="scanName"></param>
        /// <returns></returns>
        
        public void DeleteLatestData()
        {
                db.Configuration.AutoDetectChangesEnabled = false;
                var allRanks = from d in db.Stocks where d.Date == StartOn select d;
                allRanks.ToList().ForEach(y => db.Entry(y).State = EntityState.Deleted);
                var allShorts = from d in db.Shorts where d.Date == StartOn select d;
                allShorts.ToList().ForEach(y => db.Entry(y).State = EntityState.Deleted);
                var allTrades = from t in db.LatestTradeSignals where t.Date == StartOn select t;
                allTrades.ToList().ForEach(y => db.Entry(y).State = EntityState.Deleted);
                var todaysTrades = from t in db.TodaysTrades where t.Date == StartOn select t;
                todaysTrades.ToList().ForEach(y => db.Entry(y).State = EntityState.Deleted);
                db.SaveChanges();
                db.ChangeTracker.DetectChanges();
        }
      
         /// <summary>
        /// Deletes todays existing  data if we have it, then Reloads All Data
        /// </summary>
        /// <returns></returns>
        public string LoadData(DateTime forToday)
        {
            
            //get the latest rank date and delete if the data is for today
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<br/>");
            sb.AppendLine(EmailService.ReloadDataURL);
            sb.AppendLine(string.Format("<hr><h3>Fetching new data for {0}.</h3>", forToday.ToShortDateString()));
            List<TopStock> stocks;
            try
            {
                sb.AppendLine("Fetching stocks from Barchart...<br/>");
                BarChartService bc = new BarChartService(); 
                stocks = bc.GetStocks();
                sb.AppendLine(string.Format("Fetched {0} stocks from Barchart.<br/>", stocks.Count)); 
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
                stocks = yh.GetStockData(stocks);
                sb.AppendLine(string.Format("Fetched {0} stock data from Yahoo.<br/>", stocks.Count));
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
                 DateTime fetchedDate = stocks.First().Date;
                 if (GetLatestStockDateAvailable() == fetchedDate)
                 {
                     sb.AppendLine(string.Format("Deleting data for {0}.<br/>", fetchedDate.ToShortDateString()));
                     DeleteLatestData(); //now delete the latest data
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
                 db.Configuration.AutoDetectChangesEnabled = false;
                foreach (var stock in stocks)
                {
                    db.Stocks.Add(stock);
                }
                db.SaveChanges();
                db.ChangeTracker.DetectChanges();
                sb.AppendLine("Saved new data.<br/>");
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
                sb.AppendLine("Running Scans...<br/>");
                db.ChangeTracker.DetectChanges();
                var performances = RunPerformanceScan(StartOn); 
                var allscans = Strings.ScanNames.AllScanNames();
                foreach (string t in allscans)
                { 
                    var scan = GetScan(t);
                    var scanresults = RunScan(t, performances);
                    foreach (TopStock ss in scanresults)
                    {
                         //we need to add this scan to the stock but we dont have the drops
                        if(performances.Any(g => g.Ticker == ss.Ticker))
                        { 
                          var p = performances.Where(g => g.Ticker == ss.Ticker).First();
                          p.Scans.Add(scan);
                        } 
                    } 
                }
                UpdateStocks(performances);
                UpdateLatestTrades();
                SiteConfigurationService s = new SiteConfigurationService(db);
                s.UpdateLastDataFetch(DateTime.Now);
                db.ChangeTracker.DetectChanges();
                Logger.WriteLine(MessageType.Information, "Successfully saved new data.");
                sb.AppendLine("Saved scans. Fetch Complete.<br/><hr/>");

                EmailService.SendEmail("Successfully saved new data.", sb.ToString());
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);

                EmailService.SendEmail("Error running scan data", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                sb.AppendLine("Error running scan data:" + ex.Message); 
            } 
            return sb.ToString();
        }
        public async Task<string> LoadDataAsync(DateTime forToday)
        {

            //get the latest rank date and delete if the data is for today
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<br/>");
            sb.AppendLine(EmailService.ReloadDataURL);
            sb.AppendLine(string.Format("<hr><h3>Fetching new data for {0}.</h3>", forToday.ToShortDateString()));
            List<TopStock> stocks;
            List<TopStock> shortStocks;
            List<BottomStock> shorts = new List<BottomStock>();
            try
            {
                sb.AppendLine("Fetching stocks from Barchart...<br/>");
                BarChartService bc = new BarChartService();
                stocks = await bc.GetStocksAsync();
                shortStocks = await bc.GetStocksAsync(true);
              
                sb.AppendLine(string.Format("Fetched {0} stocks from Barchart.<br/>", stocks.Count + shortStocks.Count));
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
                stocks = await yh.GetStockDataAsync(stocks);
                shortStocks = await yh.GetStockDataAsync(shortStocks);
                sb.AppendLine(string.Format("Fetched {0} stock data from Yahoo.<br/>", stocks.Count + shortStocks.Count));
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
                DateTime fetchedDate = stocks.First().Date;
                if (GetLatestStockDateAvailable() == fetchedDate)
                {
                    sb.AppendLine(string.Format("Deleting data for {0}.<br/>", fetchedDate.ToShortDateString()));
                    DeleteLatestData(); //now delete the latest data
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
                shortStocks.ForEach(g => shorts.Add(new BottomStock(g))); //convert to BottomStocks here...
                sb.AppendLine("Saving new data.<br/>");
                foreach (var stock in stocks)
                {
                    db.Stocks.Add(stock);
                }
                db.SaveChanges();
                sb.AppendLine("Saved long data.<br/>");
                foreach (var stock in shorts)
                {
                    db.Shorts.Add(stock);
                }
                db.SaveChanges();
                sb.AppendLine("Saved short data.<br/>");
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
                sb.AppendLine("Running Scans...<br/>");
                var performances = RunPerformanceScan(StartOn);
                var allscans = Strings.ScanNames.AllScanNames().Where(r=> r != Strings.ScanNames.Drops).ToList();
                foreach (string t in allscans)
                {
                    var scan = GetScan(t);
                    var scanresults = RunScan(t, performances);
                    foreach (TopStock ss in scanresults)
                    {
                        //we need to add this scan to the stock but we dont have the drops
                        if (performances.Any(g => g.Ticker == ss.Ticker))
                        {
                            var p = performances.Where(g => g.Ticker == ss.Ticker).First();
                            p.Scans.Add(scan);
                        }
                    }
                }               
                UpdateStocks(performances);
                UpdateTradesOn(StartOn);
                UpdateLatestTrades();
                //now do the same for shorts....
                var shortStatus = RunShortScan(StartOn);
                UpdateStocks(shortStatus);//lets update the database with these
                UpdateShortTradesOn(StartOn);
                UpdateLatestShortTrades();
                SiteConfigurationService s = new SiteConfigurationService(db);
                s.UpdateLastDataFetch(DateTime.Now);
                Logger.WriteLine(MessageType.Information, "Successfully saved new data.");
                sb.AppendLine("Saved scans. Fetch Complete.<br/><hr/>");
                EmailService.SendTradeEmail(GetTradesOn(StartOn));
                EmailService.SendShortTradeEmail(GetShortTradesOn(StartOn));
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
                EmailService.SendEmail("Error running scan data", sb.ToString());
                Logger.WriteLine(MessageType.Error, ex.Message);
                sb.AppendLine("Error running scan data:" + ex.Message);
            }
            return sb.ToString();
        }
        public List<StockSearchResults> SearchStocks(StockSearchParameters parameters)
        {
            List<StockSearchResults> results = new List<StockSearchResults>(); 
            //GetAllScansSince
            DateTime days  = StartOn.AddDays(-parameters.DaysBack);
            var daysAgo = GetStockValuesFrom(days,StartOn).OrderByDescending(t=>t.Date);
            var today = GetStocksOn(days); 
            if (!daysAgo.Any() || !today.Any())
            {
                return results;
            }
            var realDateBack =  daysAgo.Min(y=>y.Date);//get the real days back
            parameters.DaysBack = DateHelper.GetDaysPast(DateTime.Now, realDateBack); 
            //lets see which stocks have a price change above the minpricechange
            /* need each distinct ticker in all preformance scans * */
            foreach(var stock in today)
            {
                StockSearchResults searchResults = new StockSearchResults { Ticker = stock.Ticker };
                //start at the first day back  
                searchResults.Since = realDateBack;
                try
                {
                    //find the scan results of this performance scan
                    var stockResults = daysAgo.Where(t=> t.Ticker == stock.Ticker && t.Date < StartOn).First();
                    searchResults.PriceChange += stockResults.Gain; //add up the price changes for each day
                    searchResults.RankChange += stockResults.RankChange;//add up the rank change for each day
                    if (searchResults.RankChange >= parameters.MinRankChange && (searchResults.PriceChange * 100) >= parameters.MinPriceChange)
                    {
                        results.Add(searchResults);//add it this passes so far
                    }
                }
                catch (Exception)
                {

                }
            }
            return results.OrderBy(i => i.PriceChange).ToList();
        }
        public void SendLongTradeEmail()
        {
            EmailService.SendTradeEmail(GetTradesOn(StartOn));
        }
        public void SendShortTradeEmail()
        {
            EmailService.SendShortTradeEmail(GetShortTradesOn(StartOn));
        }
        public static List<TopStock> DtoTopStockTable(DataTable ranks)
        {
            var list = new List<TopStock>();
            foreach (DataRow dr in ranks.Rows)
            {

                try
                {
                    TopStock r = new TopStock();
                    r.Ticker = dr["Ticker"].ToString().Trim();
                    r.CurrentRank = Convert.ToInt32(dr["CurrentRank"]);
                    r.PreviousRank = Convert.ToInt32(dr["PreviousRank"]);
                    r.RankChange = Convert.ToInt32(dr["RankChange"]);
                    r.Date = DateTime.Parse(dr["Date"].ToString());
                    r.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    r.DaysInList = Convert.ToInt32(dr["DaysInList"]);
                    r.Color = dr["Color"].ToString().Trim();
                    r.Opinion = dr["Opinion"].ToString().Trim();
                    r.Direction = dr["Direction"].ToString().Trim();
                    r.CurrentSide = dr["CurrentSide"].ToString().Trim();
                    try
                    {
                        if (dr["Price"] != DBNull.Value)
                        {
                            r.Price = Convert.ToDouble(dr["Price"]);
                        }
                        if (dr["StartPrice"] != DBNull.Value)
                        {
                            r.StartPrice = Convert.ToDouble(dr["StartPrice"]);
                        }
                        if (dr["AvgVol"] != DBNull.Value)
                        {
                            r.AvgVol = Convert.ToDouble(dr["AvgVol"]);
                        }
                        if (dr["Volume"] != DBNull.Value)
                        {
                            r.Volume = Convert.ToDouble(dr["Volume"]);
                        }
                        if (dr["GainTillNow"] != DBNull.Value)
                        {
                            r.GainTillNow = Convert.ToDouble(dr["GainTillNow"]);
                        }
                        if (dr["Gain"] != DBNull.Value)
                        {
                            r.Gain = Convert.ToDouble(dr["Gain"]);
                        }
                        
                    }
                    catch
                    {
                        //gulp
                    }

                    list.Add(r);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception for ticker " + dr["Ticker"].ToString() + ex.Message);
                }

            }
            return list;
        }
        public static List<TopStock> DtoTopStock(DataTable ranks)
        {
            var list = new List<TopStock>();
            foreach (DataRow dr in ranks.Rows)
            {

                try
                {
                    TopStock r = new TopStock();
                    r.Ticker = dr["Ticker"].ToString().Trim();
                     r.Date = DateTime.Parse(dr["Date"].ToString()); 
                     r.CurrentSide = dr["CurrentSide"].ToString().Trim();
                    try
                    {
                        if (dr["Price"] != DBNull.Value)
                        {
                            r.Price = Convert.ToDouble(dr["Price"]);
                        }
                       

                    }
                    catch
                    {
                        //gulp
                    }

                    list.Add(r);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception for ticker " + dr["Ticker"].ToString() + ex.Message);
                }

            }
            return list;
        }
        public static List<BottomStock> DtoBottomStock(DataTable ranks)
        {
            var list = new List<BottomStock>();
            foreach (DataRow dr in ranks.Rows)
            {

                try
                {
                    BottomStock r = new BottomStock();
                    r.Ticker = dr["Ticker"].ToString().Trim();
                    r.Date = DateTime.Parse(dr["Date"].ToString());
                    r.CurrentSide = dr["CurrentSide"].ToString().Trim();
                    try
                    {
                        if (dr["Price"] != DBNull.Value)
                        {
                            r.Price = Convert.ToDouble(dr["Price"]);
                        }


                    }
                    catch
                    {
                        //gulp
                    }

                    list.Add(r);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception for ticker " + dr["Ticker"].ToString() + ex.Message);
                }

            }
            return list;
        }
    }
 }
 