using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HedgefundMe.com.Models
{
    public class LineChartModel
    {
        DateTime _date =  DateTime.Now;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value; // string.Format("{0}/{1}/{2}", value.Day, value.Month, value.Year);
            }
        }
        public string Id { get; set; }

        public double Price { get; set; }
        public double? Volume { get; set; }
        public double? AvgVol { get; set; }
        public int Rank { get; set; }
        public string Opinion { get; set; }
        public string JsonDate
        {
            get
            {
                return string.Format("{0}/{1}/{2}", Date.Month, Date.Day, Date.Year);
            }
            set
            {
            }
        }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }
    public class RankOnDate
    {
        DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;  
            }
        }
        public int Rank { get; set; }
        public string JsonDate
        {
            get
            {
                return string.Format("{0}/{1}/{2}", Date.Month, Date.Day, Date.Year);
            }
            set
            {
            }
        }
    }
    public class StockRankingsModel
    {
        //represents the ranks of tickers for a series of dates
        public List<RankOnDate> Ranks { get; set; }
        public string Ticker { get; set; }
        public StockRankingsModel()
        {
            Ranks = new List<RankOnDate>();
        }
    }
    public class TrendModel
    {
        public double Value { get; set; }
        public string Label { get; set; }
    }
   
}