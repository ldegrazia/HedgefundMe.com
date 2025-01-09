using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class HomeViewModel
    {
        private ProjectEntities _context; 
        private StockService service;
        public DateTime LastestRanksAvailable { get; set; }
        public List<TopStock> MomoStocks { get; set; }
        public List<TopStock> Top5RankChanges { get; set; }
        public List<TopStock> Top5PriceChanges { get; set; }
        public List<TopStock> Rankings { get; set; }
        public List<TopStock> Adds { get; set; }
        public List<TopStock> Drops { get; set; }
        public List<TopStock> UpOnHighVolume { get; set; }
        public List<TopStock> DownOnHighVolume { get; set; }
        public List<TopStock> NewPriceHighs { get; set; }
        public List<TopStock> NewRankHighs { get; set; }
        public List<TopStock> NewBuys { get; set; }
        public List<TopStock> NewSells { get; set; }
        public List<TopStock> NewSellStops { get; set; }
        public List<TopStock> GoodScans { get; set; } 
         public HomeViewModel(  ProjectEntities context)
        {
            _context = context;
            service = new StockService(_context);
            //fetch the stuff we need for today
            //get today 
            SiteConfigurationService s = new SiteConfigurationService(_context);
            LastestRanksAvailable = s.GetLastDataFetch();  
            Rankings = service.GetLatest();
            Top5RankChanges = Rankings.Where(t => t.Scans.Any(r => r.Name == Strings.ScanNames.Top5RankChanges)).ToList();
            Top5PriceChanges = Rankings.Where(t => t.Scans.Any(r => r.Name == Strings.ScanNames.Top5PriceChanges)).ToList();
            UpOnHighVolume = Rankings.Where(t => t.Scans.Any(r => r.Name == Strings.ScanNames.UpOnHighVolume)).ToList();
            Adds = Rankings.Where(t => t.Direction == Strings.Directions.NEW).ToList();
            Drops = service.RunScan(Strings.ScanNames.Drops, Rankings).ToList();
            NewPriceHighs = Rankings.Where(t => t.Scans.Any(r => r.Name == Strings.ScanNames.NewPriceHighs)).ToList();
            NewRankHighs = Rankings.Where(t => t.Scans.Any(r => r.Name == Strings.ScanNames.NewRankHighs)).ToList();
            NewBuys = Rankings.Where(t => t.Opinion == Strings.Opinions.Buy).ToList();
            NewSells = Rankings.Where(t => t.Opinion == Strings.Opinions.Sell).ToList();
            NewSellStops = Rankings.Where(t => t.Opinion == Strings.Opinions.SellStop).ToList();
             //now lets get the good scans 
            GoodScans = Rankings.Where(t => t.Scans.Where(r=>r.ScanSide == Strings.ScanSides.Good).Count() >1 && t.Scans.Any(r => r.ScanSide == Strings.ScanSides.Good) && !t.Scans.Any(r => r.ScanSide == Strings.ScanSides.Bad)).OrderByDescending(r => r.Scans.Count()).ThenBy(i => i.CurrentRank).ToList();
            var fourDaysBack = service.GetJustStocksOn(DateTime.Now.AddDays(-5)).Where(p=>p.Opinion != Strings.Opinions.Sell);
            MomoStocks = new List<TopStock>();
            //get only thise that are in the current 
            foreach(var tt in fourDaysBack)
            {
                if(Rankings.Any(t=>t.Ticker == tt.Ticker))
                {
                    //get this ticker and then find all the ones that have increased in rank, increased in price, and high average volume
                    var ticker = Rankings.Where(t => t.Ticker == tt.Ticker).First();
                    if( tt.CurrentRank - ticker.CurrentRank > 1)
                    {
                        if(tt.Price < ticker.Price)
                        {
                            if(tt.AvgVol < ticker.AvgVol)
                            {
                                MomoStocks.Add(ticker);
                            }
                        }
                    }
                }
            } 

             
        }
        
    }
}