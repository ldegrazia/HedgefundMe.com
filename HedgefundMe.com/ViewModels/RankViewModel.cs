using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HedgefundMe.com.Models;
using HedgefundMe.com.Services;
namespace HedgefundMe.com.ViewModels
{
    public class RankViewModel
    {
        public List<TopStock> Stocks { get; set; }
        public DateTime Date { get; set; }
        private ProjectEntities _context;
        private StockService service;
        public RankViewModel(string date, ProjectEntities context)
        {
            _context = context;
            //get all teh stocks on this date
            service = new StockService(_context);
            Date = DateTime.Parse(date);
            var results = service.GetStocksOn(Date);
            Stocks = new List<TopStock>();
            foreach(var t in results)
            {
                TopStock s = t.Clone() as TopStock;
                //s.Scans.Clear();
                Stocks.Add(s);
            }
        }
    }
}