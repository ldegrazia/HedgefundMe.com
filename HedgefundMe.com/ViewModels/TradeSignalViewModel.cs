using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using System.ComponentModel.DataAnnotations;
namespace HedgefundMe.com.ViewModels
{
    public class TradeSignalViewModel
    {
        private ProjectEntities _context;
        public TradeSignalService Service;
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        public SiteAuditRecord LastSignalFetch { get; set; }
        public List<TradeSignal> Data { get; set; }
        public IPagedList<TradeSignalHistory> HistoricalData { get; set; }
        public TradeSignalViewModel(ProjectEntities context, MarketDataService mdService)
        {
            _context = context;
            Service = new TradeSignalService(_context, mdService);
            Data = Service.GetNewSignals();
            LastSignalFetch = GetLastSignalRun();
        }
        /// <summary>
        /// Gets the last open trade from the trade history
        /// </summary>
        /// <returns></returns>
        public SiteAuditRecord GetLastSignalRun()
        {
            MarketDataService ms = new MarketDataService(_context);
            return ms.GetLatestSignalFetch();
        }
        /// <summary>
        /// Clears the existing new signals then calls runs the new signals
        /// </summary>
        public void RunSignals()
        {
             Service.RunNewSignals();
            Data = Service.GetNewSignals();
        }
        /// <summary>
        /// Gets the historical signals for a date  
        /// </summary> 
        public void GetHistoricalSignals(MarketDataParameters parameters)
        {
            var data = Service.GetTradeSignalHistory(); 
            HistoricalData = data.ToPagedList(parameters.PageNumber, parameters.PageSize); ;
        }
        /// <summary>
        /// Deletes the trade signals from the history to start over
        /// </summary>
        public void ClearTradeSignalHistory()
        {
            Service.DeleteTradeSignalHistory();

        }
        /// <summary>
        ///  Deletes new trade signals from the table
        /// </summary>
        public void ClearNewTradeSignals()
        {
            Service.ClearNewTradeSignals();
        }
    }
}