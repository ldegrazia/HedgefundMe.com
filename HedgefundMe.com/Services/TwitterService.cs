using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using HedgefundMe.com.Models; 

namespace HedgefundMe.com.Services
{
    public class TwitterService
    {
        public static List<Tweet> GetTweets(string ticker)
        {

            using (var webClient = new WebClient())
            {
                //We should also add errorhandling, since the webclient (and parsing) could throw an exception. 
                var result = webClient.DownloadString("http://search.twitter.com/search.atom?q=" + HttpUtility.UrlEncode("%24" + ticker));
                var tweetDocument = XDocument.Parse(result);
                XNamespace xmlNamespace = "http://www.w3.org/2005/Atom";
                var tweets = from entry in tweetDocument.Descendants(xmlNamespace + "entry")
                             select new Tweet()
                             {
                                 Author = entry.Descendants(xmlNamespace + "name").First().Value,
                                 ID = entry.Element(xmlNamespace + "id").Value,
                                 Text = HttpUtility.UrlDecode(entry.Element(xmlNamespace + "title").Value),
                                 ImageUrl = entry.Elements(xmlNamespace + "link").Last().Attributes("href").First().Value,
                                 TimeAgo = DateHelper.GetTimeInWords(entry.Element(xmlNamespace + "published").Value)
                             };

                return tweets.ToList();
            }

        }
    }
}