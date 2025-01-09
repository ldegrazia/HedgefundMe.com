using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using HedgefundMe.com.Models;
using System.Text.RegularExpressions;
namespace HedgefundMe.com.Services
{
    public class RssReader
    {
        //private static string _feedUrl = "http://finance.yahoo.com/rss/headline?s=";
        private static string _feedUrl = "http://feeds.finance.yahoo.com/rss/2.0/headline?s=";
        private static string _feedsuffix = "&region=US&lang=en-US";
        public static IEnumerable<Rss> GetRssFeed(string ticker)
        {
            if(string.IsNullOrEmpty(ticker))
            {
                return new List<Rss>();
            }
            ticker = ticker.Trim().ToUpper();
            var url = _feedUrl + ticker + _feedsuffix;            
            try
            {
                XDocument feedXml = XDocument.Load(_feedUrl + ticker.Trim() + _feedsuffix);                
                if (feedXml.Root.Value.Contains("RSS feed not found"))
                {
                    Logger.WriteLine(MessageType.Information, "No feed found for " + ticker);
                    return new List<Rss>(); 
                }                 
                var feeds = from feed in feedXml.Descendants("item")
                            select new Rss
                            {
                                Title = HttpUtility.UrlDecode(feed.Element("title").Value),
                                Link = feed.Element("link").Value,
                                Description = Regex.Match(feed.Element("description").Value, @"^.{1,180}\b(?<!\s)").Value,
                                PublicationDate = DateHelper.GetTimeInWords(feed.Element("pubDate").Value)
                            };
                return feeds;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, "Error fetching news for " + ticker + ":" + ex.Message);
                return new List<Rss>();
            }
        }
    }
}