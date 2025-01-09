using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class BarChartService
    {
        public static string BarchartURL = "http://www.barchart.com/stocks/sectors/-TOP";
        public static string BarchartShorts = "http://www.barchart.com/stocks/sectors/-BOT";
        public static string BarchartLongs = "http://www.barchart.com/stocks/sectors/-TOP";
        /// <summary>
        /// Hits barchart.com and gets all the top100 stocks for today
        /// </summary>
        /// <returns></returns>
        
        public async Task<List<MarketData>> GetMarketDataAsync(bool shorts = false)
        {
            HttpWebRequest request = null;

            WebResponse resp;
            List<MarketData> theEntries = new List<MarketData>();
            try
            {
                if (!shorts)
                {
                    request = (HttpWebRequest)WebRequest.Create(new Uri(BarchartLongs, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(new Uri(BarchartShorts, UriKind.RelativeOrAbsolute));
                }

                resp = await request.GetResponseAsync();
                using (Stream responseStream = resp.GetResponseStream())
                {

                    using (StreamReader htmlStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string line;
                        LoopController listLoop = new LoopController(true);
                        while (!listLoop.IsStopped)
                        {
                            line = htmlStream.ReadLine();
                            if (line.Contains("class=\"datatable")) //"datatable"
                            {
                                int rank = 1;
                                LoopController stocksLoop = new LoopController(true);
                                while (!stocksLoop.IsStopped)
                                {
                                    line = htmlStream.ReadLine();
                                    if (line == null)
                                    {
                                        stocksLoop.Stop();
                                        continue;
                                    }
                                    if (line.Contains("</table>"))
                                    {
                                        listLoop.Stop();
                                        stocksLoop.Stop();
                                        continue;
                                    }
                                    if (line.Contains("<tr"))
                                    {
                                        MarketData myEntry;
                                        try
                                        {

                                            myEntry = GetMarketData(htmlStream, rank++);

                                            if (IsValid(myEntry))
                                            {
                                                myEntry.Side = shorts ? Strings.Side.Short : Strings.Side.Long;
                                                theEntries.Add(myEntry);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.WriteLine(MessageType.Error, ex.Message);
                                            throw new Exception("Could not fetch Entry", ex);
                                        }
                                    }
                                }
                            }
                        }//end listloop
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
         public static MarketData GetMarketData(StreamReader htmlStream, int rank)
        {
            MarketData entry = new MarketData();
            LoopController stockLoop = new LoopController(true);
            while (!stockLoop.IsStopped)
            {
                string line = htmlStream.ReadLine();

                if (line.Contains("/quotes/stocks"))
                {
                    entry.Ticker = HtmlHelper.GetValueBetween(line, "/quotes/stocks/", "\">");
                    entry.Ticker = entry.Ticker.TrimEnd();
                    entry.Price = GetPrice(htmlStream);
                    entry.CurrentRank = rank;
                    entry.Date = GetDate(htmlStream);
                    stockLoop.Stop();
                }
                if (line.Contains("</tr") || line == null)
                {
                    stockLoop.Stop();
                    continue;
                }
                if (line.Contains("quotes/etf/"))
                {
                    entry.Ticker = HtmlHelper.GetValueBetween(line, "quotes/etf/", "\">");
                    entry.Price = GetPrice(htmlStream);
                    entry.CurrentRank = rank;
                    entry.Date = GetDate(htmlStream);
                    stockLoop.Stop();
                }
            }
            if (entry == null)
            {
                Logger.WriteLine(MessageType.Error, "Could not GetMarketData from stream for rank");
                throw new Exception("Could not GetMarketData from stream for rank");
            }
            return entry;
        }
        
        public static bool IsValid(MarketData theEntry)
        {
            if (theEntry.Ticker.Contains("-"))
            {
                //Logger.WriteLine(MessageType.Error, "theEntry.Ticker.Contains - " + theEntry);
                return false;
            }
            if (theEntry.Ticker.Contains("."))
            {
                return false;
            }
            return true;
        }
        public static double GetPrice(StreamReader htmlStream)
        {
            LoopController stockLoop = new LoopController(true);
            while (!stockLoop.IsStopped)
            {
                string line = htmlStream.ReadLine();
                if (line.Contains("_last"))
                {
                    try
                    {
                        //  <td id="dt1_PCYC_last" align="right" class="qb_shad" nowrap="nowrap">18.74</td>
                        var p = HtmlHelper.GetValueBetween(line, "nowrap=\"nowrap\">", "</td>");
                        return Convert.ToDouble(p);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(MessageType.Error, ex.Message);
                        Logger.WriteLine(MessageType.Error, " -999");
                        return -999;
                    }
                }
                if (line.Contains("<img"))
                {
                    stockLoop.Stop();
                }
            }
            return -999;
        }
        private static DateTime GetDate(StreamReader htmlStream)
        {
            LoopController stockLoop = new LoopController(true);
            while (!stockLoop.IsStopped)
            {
                string line = htmlStream.ReadLine();
                if (line.Contains("_displaytime"))
                {
                    try
                    {
                        return DateTime.Parse(HtmlHelper.GetValueBetween(line, "nowrap\">", "<")).Date;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(MessageType.Error, ex.Message);
                        return DateTime.Now.Date;
                    }
                }
                if (line.Contains("<img"))
                {
                    stockLoop.Stop();
                }
            }
            return DateTime.Now;
        }
    }
}