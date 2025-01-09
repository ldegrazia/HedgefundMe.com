using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class YahooService
    {
        private static string _login = "degrazl";
        private static string _password = "Al3xander1";
        public const string YahooDownloadPrefix = "http://finance.yahoo.com/d/quotes.csv?s=";
        //http://www.canbike.org/information-technology/yahoo-finance-url-download-to-a-csv-file.html
        //s symbol l1   Last Trade (Price Only) p2   Change in Percent  v Volume a2 Average Daily Volume c1 Change h Days high g days low k	52-week High j	52-week Low
        public const string YahooDownloadSuffix = "&f=sl1p2va2c1hgkj";
        public const string YahooChartPrefix = "http://chart.finance.yahoo.com/z?s=";
        public const string YahooChartSuffix = "&q=l&l=on&p=m50,m200&a=v&lang=en";
        public const string YahooFinancePrefix = "http://finance.yahoo.com/q/ta?s=";
        public const string YahooChartTarget = "http://finance.yahoo.com/q/ta?s=###&t=3m&l=on&z=l&q=l&p=m50%2Cm200&a=&c=";
        //http://finance.yahoo.com/q/ta?s=AAPL&t=3m&l=on&z=l&q=l&p=m50%2Cm200&a=&c=
        public const string ThreeMonths = "&t=3m";
        public const string FiveDays = "&t=5d";
        public const string OneDay = "&t=1d";
        public const string SixMonths = "&t=6m";
        public const string OneYear = "&t=1y";
        public const string Small = "&z=m";
        public const string Medium = "&z=m";
        public const string Large = "&z=l";
        HttpWebRequest _request = null;
        HttpWebResponse _response = null;
        public const string YahooFinanceSummaryPrefix = "http://finance.yahoo.com/q?s=";

        public static string GetYahooChartURL(string ticker)
        {
            return "http://ichart.finance.yahoo.com/t?s=" + ticker;
        }
        public static string GetYahooCharTarget(string ticker)
        {
            return Regex.Replace(YahooChartTarget, "###", ticker);
        }
        public static string YahooChartImageUrl(string ticker, TimeFrame tf)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(YahooChartPrefix);
            sb.Append(ticker);
            sb.Append(GetTimeFrame(tf));
            sb.Append(Small); //SIZE
            sb.Append(YahooChartSuffix);
            return sb.ToString();
        }
        public static string GetTimeFrame(TimeFrame tf)
        {
            switch (tf)
            {
                case TimeFrame.OneDay:
                    {
                        return OneDay;

                    }
                case TimeFrame.FiveDay:
                    {
                        return FiveDays;

                    }
                case TimeFrame.ThreeMonth:
                    {
                        return ThreeMonths;

                    }
                case TimeFrame.SixMonth:
                    {
                        return SixMonths;

                    }
                default:
                    {
                        return OneYear;

                    }
            }
        }
        /// <summary>
        /// Gets the cookie from loggin in to yahoo
        /// </summary>
        public CookieContainer GetCookie()
        {
            //create an authenticated web client request
            //hit yahoo
            //see the results
            CookieContainer _yahooContainer;

            string strPostData = String.Format("login={0}&passwd={1}", _login, _password);

            // Setup the http request.
            HttpWebRequest wrWebRequest = WebRequest.Create("https://login.yahoo.com/config/login") as HttpWebRequest;
            
            wrWebRequest.Method = "POST";
            wrWebRequest.ContentLength = strPostData.Length;
            wrWebRequest.ContentType = "application/x-www-form-urlencoded";
            _yahooContainer = new CookieContainer();
            wrWebRequest.CookieContainer = _yahooContainer;

            // Post to the login form.
            using (StreamWriter swRequestWriter = new StreamWriter(wrWebRequest.GetRequestStream()))
            {
                swRequestWriter.Write(strPostData);
                swRequestWriter.Close();
            }
            // Get the response.
            HttpWebResponse hwrWebResponse = (HttpWebResponse)wrWebRequest.GetResponse();

            if (hwrWebResponse.ResponseUri.AbsoluteUri.Contains("my.yahoo.com"))
            {
                // you authenticated properly
                System.Diagnostics.Debug.WriteLine("Authenticated...");
            }
            return _yahooContainer;
        }
        /// <summary>
        /// Fetches the data at least three times from yahoo after hitting the page
        /// </summary> 
        /// <param name="pe"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public async Task<List<MarketData>> GetPortfolioDataAsync(PortfolioEntry pe)
        {
            List<MarketData> theEntries = new List<MarketData>();
            var count = 0;
            do{//do this until the count is > 3 or we have all the prices                
                theEntries = await FetchDataFromYahooForAsync(pe);
                if(!theEntries.Any(t=>t.Price == 0.0))
                {
                    return theEntries;
                }
                Logger.WriteLine(MessageType.Warning,"Some prices are zero, this is fetch # " + count + " for " + pe.Name);
                count++;
                }
            while(count <4);//put in database
            Logger.WriteLine(MessageType.Warning, "Some prices maybe zero, this is fetch # " + count + " for " + pe.Name);
            return theEntries; //return what we have anyway after 3 tries
        }
        /// <summary>
        /// Fetches the data from yahoo for the portfolio
        /// </summary>
        /// <param name="pe"></param>
        /// <returns></returns>
        private async Task<List<MarketData>> FetchDataFromYahooForAsync(PortfolioEntry pe)
        {
             List<MarketData> theEntries = new List<MarketData>();
             try
            {
                var cookie = GetCookie();
                 HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pe.Url);
                 request.CookieContainer = cookie;
                 var resp = await request.GetResponseAsync(); 
                 using (Stream responseStream = resp.GetResponseStream())
                {

                    using (StreamReader htmlStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string line;
                        LoopController listLoop = new LoopController(true);
                        while (!listLoop.IsStopped)
                        {
                            line = htmlStream.ReadLine();
                            System.Diagnostics.Debug.WriteLine(line);
                                    if (line == null)
                                    {
                                        listLoop.Stop();
                                        continue;
                                    }
                                  
                                    if (line.Contains("<tr data-row-symbol="))
                                    {
                                         var rows = Regex.Split(line,"<tr");
                                        var rank = 1;
                                        foreach(var row in rows)
                                        {
                                            try
                                            { 
                                                if(string.IsNullOrEmpty(row.Trim()))
                                                {
                                                    continue;
                                                }
                                                var myEntry = GetMarketData(row);
                                                myEntry.CurrentRank = rank;
                                                if(pe.Name.ToLower().Contains("top"))
                                                {
                                                    myEntry.Side = Strings.Side.Long;
                                                } 
                                                else
                                                {
                                                    myEntry.Side = Strings.Side.Short;
                                                } 
                                                theEntries.Add(myEntry); 
                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.WriteLine(MessageType.Error, ex.Message + " for row " + row);
                                                
                                            } 
                                            rank++;
                                      }
                                        listLoop.Stop();
                                    }
                                } 
                    }//end using
                }//endusing
                return theEntries;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, ex.Message);
                throw new Exception(ex.Message);
            }
        
        }
        public static MarketData GetMarketData(string row)
        {
            MarketData entry = new MarketData();
            //split the line into trs
            entry.Ticker = HtmlHelper.GetValueBetween(row, "http://finance.yahoo.com/q?s=", "\">");
            entry.Ticker = entry.Ticker.TrimEnd();
            entry.Price = 0;
            double price;
            if (Double.TryParse(HtmlHelper.GetValueBetween(row, "tquote:value;' data-tmpl='change:yfin.datum'>", "</span>"), out price))
            {
                entry.Price = price;
            }
            entry.PriceChange = 0;
            double pricechange;
            if (Double.TryParse(HtmlHelper.GetValueBetween(row, "mkt:chg;' data-tmpl='change:yfin.datumTrend'>", "</span>"), out pricechange))
            {
                entry.PriceChange = pricechange;
            }
            entry.PriceChangePcnt = 0;
            double pricechangePct;
            if (Double.TryParse(HtmlHelper.GetValueBetween(row, ":mkt:pctchg;' data-tmpl='change:yfin.datumTrend'>", "%</span>"), out pricechangePct))
            {
                if(pricechangePct !=0)
                {
                    pricechangePct = pricechangePct / 100;
                }
                entry.PriceChangePcnt = pricechangePct;
            }
            entry.Volume = 0;
            double volume;
            if (Double.TryParse(HtmlHelper.GetValueBetween(row, "mkt:vol;' data-tmpl='change:yfin.datum'>", "</span>"), out volume))
            {
                entry.Volume = volume;
            }
            entry.AvgVol = 0;
            double avgvolume;
            if (Double.TryParse(HtmlHelper.GetValueBetween(row, "mkt.avgVol;' data-tmpl='change:yfin.datum'>", "</span>"), out avgvolume))
            {
                entry.AvgVol = avgvolume;
            }
            entry.VolumeChange = FinanceHelper.VolumeChange(entry.AvgVol, entry.Volume);
            return entry;
        }
      
        public async Task<List<MarketData>> GetStockDataAsync(List<MarketData> stocks)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string ticker = string.Empty;
                    foreach (var t in stocks)
                    {
                        ticker += t.Ticker + "+";
                    }
                    Uri address = new Uri(YahooDownloadPrefix + ticker + YahooDownloadSuffix);
                    var csvData = await web.DownloadStringTaskAsync(address); 
                    var pricedData = Parse(stocks, csvData);  
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Error downloading yahoo data:" + ex.Message);
            }
            return stocks;
        }

        public static List<MarketData> Parse(List<MarketData> existing, string csvData)
        {

            string[] rows = csvData.Replace("\r", "").Split('\n');
            //s symbol l1   Last Trade (Price Only) p2   Change in Percent  v Volume a2 Average Daily Volume   
           foreach(var row in rows)
            {
                try
                {
                   
                    if (string.IsNullOrEmpty(row)) continue;
                   
                    string newString = Regex.Replace(row,"N/A", "0.0");
                    newString = Regex.Replace(newString, "%", string.Empty);
                    string[] cols = newString.Replace("\"", string.Empty).Split(',');

                    //TopStock p = new TopStock();
                    if (!existing.Any(t => t.Ticker == cols[0]))
                    {
                        Logger.WriteLine(MessageType.Information, "No data for " + cols[0] + " from yahoo.");
                        continue; //we dont have it
                    }
                    var p = existing.Where(t => t.Ticker == cols[0]).First();
                    p.Ticker = cols[0];
                    p.Price = Convert.ToDouble(cols[1]);
                    p.PriceChangePcnt = Convert.ToDouble(cols[2]) / 100;
                    p.Volume = Convert.ToDouble(cols[3]);
                    p.AvgVol = Convert.ToDouble(cols[4]);
                    p.VolumeChange = FinanceHelper.VolumeChange(p.AvgVol, p.Volume);
                    p.PriceChange = Convert.ToDouble(cols[5]);
                    p.DayHigh = Convert.ToDouble(cols[6]);
                    p.DayLow = Convert.ToDouble(cols[7]);
                    p.YearHigh = Convert.ToDouble(cols[8]);
                    p.YearLow = Convert.ToDouble(cols[9]);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, "Error Parsing:\n" + row + "\nyahoo data:" + ex.Message);
                }
            } 
            return existing;
        }

     
        /// <summary>
        /// Using the list of data, we skip the amount ot skip and take the amount to take
        /// </summary>
        /// <param name="allData"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static IEnumerable<MarketData> GetBatch(List<MarketData> allData, int skip, int take)
        {
            if (allData.Count <= skip)
            {
                return allData;
            }
            return allData.Skip(skip).Take(take);
        }
        /// <summary>
        /// Gets the latest pricing data from yahoo for the latest date's set of tickers in batches
        /// </summary>
        public async Task<List<MarketData>> GetBatchedPricingData(List<MarketData> allData, int skip, int take)
        {
            List<MarketData> newData = new List<MarketData>();
            while (skip < allData.Count)
            {
                 
                    var thisBatch = GetBatch(allData, skip, take).ToList();
                    newData.AddRange(await GetStockDataAsync(thisBatch));
                    skip = skip + take; //0 + 100, 100 + 100, 200 + 100,300+ 100 
            } //end while           
            return newData;
        }
    }
}