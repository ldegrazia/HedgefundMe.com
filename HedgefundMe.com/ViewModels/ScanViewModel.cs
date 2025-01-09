using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;

using System.ComponentModel.DataAnnotations;

namespace HedgefundMe.com.ViewModels
{
    public class ScanViewModel
    {
        public List<TopStock> Stocks { get; set; }
        public List<TopStock> MomoStocks { get; set; }
        public DateTime Date { get; set; }
        private ProjectEntities _context;
        private StockService service;
        public ScanViewModel(string date, ProjectEntities context)
        {
            _context = context; 
            service = new StockService(_context);
            Date = DateTime.Parse(date);     //get all the stocks on this date
            Stocks = service.GetStocksOn(Date).Where(t => t.Scans.Any(r => r.ScanSide == Strings.ScanSides.Good) && !t.Scans.Any(r => r.ScanSide == Strings.ScanSides.Bad)).OrderByDescending(r => r.Scans.Count()).ThenBy(i => i.CurrentRank).ToList();
            MomoStocks = new List<TopStock>();
            var fourDaysBack = service.GetJustStocksOn(DateTime.Now.AddDays(-5)).Where(p => p.Opinion != Strings.Opinions.Sell);
            var ranks = service.GetLatest();
            //get only thise that are in the current 
            foreach (var tt in fourDaysBack)
            {
                if (ranks.Any(t => t.Ticker == tt.Ticker))
                {
                    //get this ticker and then find all the ones that have increased in rank, increased in price, and high average volume
                    var ticker = ranks.Where(t => t.Ticker == tt.Ticker).First();
                    if (tt.CurrentRank - ticker.CurrentRank > 1)
                    {
                        if (tt.Price < ticker.Price)
                        {
                            if (tt.AvgVol < ticker.AvgVol)
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