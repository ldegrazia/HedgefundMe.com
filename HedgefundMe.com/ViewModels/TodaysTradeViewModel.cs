using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class TodaysTradeViewModel
    {
        private ProjectEntities _context;
        public TodaysTradeParameters Parameters { get; set; }
        public DateTime SelectedDate { get; set; }
        private StockService service;
        public DateTime LastestRanksAvailable { get; set; }
        public List<TodaysTrade> Trades { get; set; }
        public IEnumerable<SelectListItem> Date
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (int i = 0; i > -15; i--)
                {
                    var date = DateTime.Now.AddDays(i);
                    if (DateHelper.IsWeekend(date))
                    {
                        continue;
                    }
                    items.Add(new SelectListItem { Text = date.ToShortDateString(), Value = date.ToShortDateString() });
                }
                return items;
            }
        }
        public TodaysTradeViewModel(TodaysTradeParameters parameters, ProjectEntities context)
            {
                Parameters = parameters;
                _context = context;
                SelectedDate = parameters.Date;
                Trades = new List<TodaysTrade>();
                service = new StockService(_context);
                SiteConfigurationService s = new SiteConfigurationService(_context);
                LastestRanksAvailable = s.GetLastDataFetch();  
            }
            public void GetTrades()
            {
                Trades = new List<TodaysTrade>();
                SelectedDate = Parameters.Date;
                
                BackTestService bs = new BackTestService(_context);//find all the pnlResults from the latest backtest
                var latestPnl = bs.GetLatestBestBets();
               
                Trades = service.GetTradesOn(SelectedDate);
                Trades.ForEach(t => UpdatePnl(latestPnl, t)); 
            }
            private static void UpdatePnl(List<BestBetResult> latestPnl, TodaysTrade stock)
            {
                if (latestPnl.Any(p => p.Ticker == stock.Ticker))
                {
                    var bestbetpnl = latestPnl.First(l => l.Ticker == stock.Ticker);
                    if (bestbetpnl.PcnlPercentage.HasValue)
                    {
                        stock.TwoWeekPnl = bestbetpnl.Pnl.Value;
                        stock.TwoWeekPnlPercent = bestbetpnl.PcnlPercentage.Value;
                    } 
                }
            }
    }
}