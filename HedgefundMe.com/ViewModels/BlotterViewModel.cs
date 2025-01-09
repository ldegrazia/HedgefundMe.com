using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using System.ComponentModel.DataAnnotations;
namespace HedgefundMe.com.ViewModels
{
    public class BlotterViewModel
    {
        private ProjectEntities _context;
        private MarketDataService _mdService;
         
        BlotterManager bm; 
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        public DateTime LastSignalRun { get; set; } 
        public List<BlotterEntry> Longs { get; set; }
        public List<BlotterEntry> Shorts { get; set; }
        public List<TradeHistoryEntry> CompletedTrades { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosedPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OpenLongPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosedLongPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OpenShortPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosedShortPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double TotalCurrentOpenValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double FinalGain { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double FinalGainPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OpenPnl { get; set; } 
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ClosedLongShareCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ClosedShortShareCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int OpenLongShareCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int OpenShortShareCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalShareCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalLongShareCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int LongWins { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int LongLosses { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ShortWins { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ShortLosses { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalShortShareCount { get; set; }


        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CurrentCommissionCost { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double GainAfterCommision { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double TargetPriceChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:C3}")]
        public double CommissionCostPerShare { get; set; }

        public bool BlotterIsMaxed { get; set; }

        public SiteAuditRecord LastPricingFetch { get; set; }
        public SiteAuditRecord LastSignalFetch { get; set; }
        public BlotterViewModel(ProjectEntities context, MarketDataService mdService, TradeSignalService tsService)
        {
            _context = context;
            bm = new BlotterManager(_context, mdService, tsService);
            _mdService = mdService;
            Init();
        }
        public void Init()
        {
            GetLongs();
            GetShorts();
            GetCompletedTrades(_mdService.StartOn); 
            LastSignalRun = bm.LastSignalRun();
            TargetPriceChange = bm.TargetPriceChange();
            CommissionCostPerShare = bm.CurrentSettings.CommissionCostPerShare; 
            RecalcPnL();
          
        }
        private void GetCompletedTrades(DateTime dateTime)
        {
            CompletedTrades = bm.GetCompletedTrades(dateTime);
        }
        private void GetLongs()
        { 
            Longs = bm.GetCurrentBlotter().Where(t => t.Strategy == Strings.Strategies.IntradayMomentumLong).ToList(); 
        }
        private void CancelTrade(string tradeId)
        {
            bm.CancelTrade(tradeId);
            Init();
        }
        private void GetShorts()
        {            
            Shorts = bm.GetCurrentBlotter().Where(t => t.Strategy == Strings.Strategies.IntradayMomentumShort).ToList(); 
        }
        /// <summary>
        /// Recalcs longs ans shorts pnl
        /// </summary>
        public void RecalcPnL()
        {
             
            ResetPnl();
            //recalc completed trades
            //get the longs
            var longs = CompletedTrades.Where(t => t.CloseAction == Strings.Opinions.Sell).ToList();
            var shorts = CompletedTrades.Where(t => t.CloseAction == Strings.Opinions.Cover).ToList();
            
            //open and close
            longs.ForEach(t =>
            {
                ClosedPnl += t.GainLoss;
                ClosedLongPnl += t.GainLoss;
                ClosedLongShareCount += (t.Shares * 2);
            });

            shorts.ForEach(t =>
            {
                ClosedPnl += t.GainLoss;
                ClosedShortPnl+= t.GainLoss;
                ClosedShortShareCount += (t.Shares * 2);
            });
            
         
            BlotterIsMaxed = bm.IsBlotterMaxed();

            //recalc open trades
           
            //open  
            Longs.ForEach(t =>
            {
                TotalCurrentOpenValue += t.CurrentValue;
                OpenPnl += t.GainLoss;
                OpenLongPnl += t.GainLoss;
                
                OpenLongShareCount += t.Shares;
            });
            
            Shorts.ForEach(t =>
            {
                TotalCurrentOpenValue += t.CurrentValue  ;
                OpenPnl += t.GainLoss;
                OpenShortPnl += t.GainLoss;
                 
                OpenShortShareCount += t.Shares;
            });
            FinalGain = OpenPnl + ClosedPnl;
            TotalShortShareCount = ClosedShortShareCount + OpenShortShareCount;
            TotalLongShareCount = ClosedLongShareCount + OpenLongShareCount;
            TotalShareCount = TotalShortShareCount + TotalLongShareCount;
            CurrentCommissionCost = TotalShareCount * CommissionCostPerShare;
            GainAfterCommision = FinalGain - CurrentCommissionCost;
            if(FinalGain!=0 && bm.CurrentSettings.MaxBlotterValue !=0)
                FinalGainPct = FinalGain/ bm.CurrentSettings.MaxBlotterValue;
            LastPricingFetch = _mdService.GetLatestPricingFetch();
            LastSignalFetch = _mdService.GetLatestSignalFetch();
        }
 public void ResetPnl()
        {
            TotalCurrentOpenValue = 0;
                OpenLongShareCount =0;
                ClosedLongShareCount = 0;
                OpenPnl = 0;
                OpenLongPnl = 0;
                ClosedShortPnl = 0;
                OpenShortPnl = 0;
                ClosedLongPnl = 0;
                OpenShortShareCount =0;
                ClosedShortShareCount = 0;
                FinalGain = 0;
                ClosedPnl = 0;
            TotalShareCount =0;
            GainAfterCommision = 0;
            CurrentCommissionCost = 0;
     TotalLongShareCount = 0;
     TotalShortShareCount = 0;
     FinalGainPct = 0;  
        }
        /// <summary>
        /// Gets the market data again and updates the blotter prices, then pnl
        /// </summary>
        public async Task RefreshAsync()
        { 
            await bm.RefreshAsync();
            GetLongs();
            GetShorts();
            RecalcPnL();
        }
        /// <summary>
        /// Maunally updates the incorrect trade values in the trade history
        /// </summary>
        public void UpdateClosedValues()
        {
            bm.UpdateClosedValues();
        }
    }
}