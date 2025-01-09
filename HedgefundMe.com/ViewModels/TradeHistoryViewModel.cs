using System;
using PagedList;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using System.ComponentModel.DataAnnotations;
namespace HedgefundMe.com.ViewModels
{
    public class TradeHistoryViewModel
    {
        private readonly ProjectEntities _context;
        private readonly MarketDataService mdservice;
        public TradingSettings CurrentSettings { get; set; }
        public MarketDataParameters Parameters { get; set; }
        private readonly TradingSettingsService tradingSettingsService; 
        public IPagedList<TradeHistoryEntry> Data { get; set; }
        public List<TradeHistoryEntry> TodaysTrades { get; set; }
        public List<TradeHistoryEntry> CompletedTrades { get; set; }

        public  TradeHistoryEntry  TradeEntryDetails { get; set; }
        public List<TradeSignalHistory> TradeSignalDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosedPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double LongPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ShortPnl { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PortfolioTotal { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int BuyShareCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")] 
        public int ShortShareCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalShareCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CurrentCommissionCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double GainAfterCommision { get; set; }
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double FinalGainPct { get; set; }
        [DisplayFormat(DataFormatString = "{0:C3}")]
        public double CommissionCostPerShare { get; set; }
        public TradeHistoryViewModel(MarketDataParameters parameters, ProjectEntities context)
        {
            _context = context;
            Parameters = parameters;
            mdservice = new MarketDataService(context);
            CompletedTrades = GetCompletedTrades(mdservice.StartOn);
            tradingSettingsService = new TradingSettingsService(_context);
            CurrentSettings = tradingSettingsService.GetCurrent();
            CommissionCostPerShare = CurrentSettings.CommissionCostPerShare; 
            ReclacPnl(); 
        }
        
        /// <summary>
        /// Gets the trades for today that have been completed
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<TradeHistoryEntry> GetCompletedTrades(DateTime onDate)
        {
            if (!_context.TradeHistory.Any())
            {
                TodaysTrades = new List<TradeHistoryEntry>();
                return TodaysTrades;
            }
            return _context.TradeHistory.Where(t => t.CloseDate.Day == onDate.Day && t.CloseDate.Year == onDate.Year && t.CloseDate.Month == onDate.Month).OrderByDescending(t => t.OpenDate).ToList();
        }
        /// <summary>
        /// Gets only todays trades
        /// </summary>
        public void TodaysHistory()
        { 
            var today = mdservice.StartOn;
            TodaysTrades = _context.TradeHistory.Where(t => t.OpenDate.Day == today.Day && t.OpenDate.Month == today.Month && t.OpenDate.Year == today.Year).OrderByDescending(t => t.OpenDate).ToList();
         }
        /// <summary>
        /// Gets the trades with paging 
        /// </summary>
        /// <param name="parameters"></param>
        public void GetHistory(MarketDataParameters parameters)
        {
            if(!_context.TradeHistory.Any())
            {
                var d = new List<TradeHistoryEntry>();
                Data = d.ToPagedList(parameters.PageNumber, parameters.PageSize);
                return;
            }
            var data = _context.TradeHistory.OrderByDescending(t => t.OpenDate).ThenBy(t => t.Strategy).ToList();
            Data= data.ToPagedList(parameters.PageNumber, parameters.PageSize);
        }
        public void ClearHistoricalTrades()
        {
            foreach (var closed in _context.TradeHistory)
            {
                _context.Entry(closed).State = EntityState.Deleted;
            }
            _context.SaveChanges();
        }
        /// <summary>
        /// Gets all open trades and removes them from the history, used during testing
        /// </summary>
        public void ClearOpenHistoricalTrades()
        {
            var opn = _context.TradeHistory.Where(t => t.State == Strings.Opinions.Open).ToList();
            foreach (var closed in opn)
            {
                _context.Entry(closed).State = EntityState.Deleted;
            }
            _context.SaveChanges();
        }
        /// <summary>
        /// Recalcs PNL and gets share count
        /// </summary>
        public void ReclacPnl()
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
                LongPnl += t.GainLoss;
                BuyShareCount += (t.Shares * 2);
            });

            shorts.ForEach(t =>
            {
                ClosedPnl += t.GainLoss;
                ShortPnl += t.GainLoss;
                ShortShareCount += (t.Shares * 2);
            });
            
            TotalShareCount = BuyShareCount + ShortShareCount;
            CurrentCommissionCost = TotalShareCount * CommissionCostPerShare;
            GainAfterCommision = ClosedPnl - CurrentCommissionCost;
            PortfolioTotal = CurrentSettings.MaxBlotterValue + ClosedPnl;
            if (ClosedPnl != 0 && CurrentSettings.MaxBlotterValue != 0)
                FinalGainPct = ClosedPnl /  CurrentSettings.MaxBlotterValue;
        }
        public void ResetPnl()
        { 
                BuyShareCount =0;
                LongPnl = 0;
                ShortPnl = 0;
                PortfolioTotal = 0;
                ShortShareCount =0;

                ClosedPnl = 0;
            TotalShareCount =0;
            GainAfterCommision = 0;
            FinalGainPct = 0;
        }
        public void TradeDetails(int id)
        {
            //get all the signals and the details of this trade
            TradeEntryDetails = _context.TradeHistory.Where(t => t.ID == id).First();
            TradeSignalDetails = _context.TradeSignalHistory.Where(t => t.TradeId == TradeEntryDetails.TradeIdOpen || t.TradeId == TradeEntryDetails.TradeIdClose).OrderBy(t => t.Date).ToList();

        }
        /// <summary>
        /// Uses the signal to get the dailts
        /// </summary>
        /// <param name="id"></param>
        public void SignalDetails(string id)
        {
          

            var tradeDetails = _context.TradeSignalHistory.Where(t => t.TradeId == id).OrderBy(t => t.Date).First();
            var entry = _context.TradeHistory.Where(t => t.TradeIdOpen == tradeDetails.TradeId || t.TradeIdClose == tradeDetails.TradeId).First();

            TradeDetails(entry.ID);

        }
    }
}