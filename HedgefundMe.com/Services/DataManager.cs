using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HedgefundMe.com.Models;
using System.Threading.Tasks;
namespace HedgefundMe.com.Services
{
    public class DataManager
    {
        private readonly ProjectEntities _context;
        private readonly MarketDataService mdService;
        private readonly TradeSignalService tsService;
        private readonly AnalysisService analService;
        private readonly BlotterManager bm;

        public DataManager(ProjectEntities db)
        {
            _context = db;
            mdService = new MarketDataService(_context);
            tsService = new TradeSignalService(_context, mdService);
            analService = new AnalysisService(db, mdService);
            bm = new BlotterManager(_context, mdService, tsService);
        }
        /// <summary>
        /// Resets the blotter
        /// </summary>
        public void ClearBlotter()
        {
            bm.ClearBlotter();
        }
        /// <summary>
        /// Main routine to load data from yahoo, get trade signals, then updates the blotter
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetDataAsync()
        {
            var result = await mdService.LoadDataAsync2(DateTime.Now.Date); 
            analService.Analyze();
            tsService.RunNewSignals();
            bm.UpdateTrades();
            return result;
        }
        /// <summary>
        /// Dont remove trades from the blotter, just mark the
        /// prices and then clean up
        /// </summary>
        public async Task CloseOutTradeHistoryPositions()
        {            
            await bm.CloseOutTradeHistoryPositions();
        }
        /// <summary>
        /// Closes out the open positions using latest price then clears the blotter.
        /// </summary>
        public async Task CloseOutPositions()
        {
            await bm.CloseOutTradeHistoryPositions();
            bm.ClearBlotter();
        }
        /// <summary>
        /// Checks time of day and performs action based on the time
        /// </summary>
        /// <returns></returns>
        public async Task<string> Run()
        {
            try
            {
                
                if (DateHelper.IsWeekend())
                {
                    return " It is a weekend.";
                }
                if (DateHelper.IsHoliday())
                {
                    //Logger.WriteLine(MessageType.Information, "Fetch not run, it is a holiday.");
                    return " It is a holiday. ";
                }
                if (DateHelper.IsFirstRun())//lets check if its the first run if it is, we need to clear the blotter
                {
                    bm.ClearBlotter(); //clear the blotter
                    Logger.WriteLine(MessageType.Information, "Market is open. Blotter Cleared for trading.");
                    return " Market is open. Blotter Cleared for trading.";
                }
                if (DateHelper.IsBeforeTenAm())//lets check if its the first run if it is, we need to clear the blotter
                {
                    Logger.WriteLine(MessageType.Information, "Market is open. Its before 10 AM.");
                    return " Market is open. Its before 10 AM.";
                }               
                if (DateHelper.IsThreeFortyFive())//close all the open trades and calc pnl
                {
                    Logger.WriteLine(MessageType.Information, "Closing out all open positions.");
                    await bm.CloseOutTradeHistoryPositions();
                    EmailService.SendEndOfDayEmail();
                    bm.ClearBlotter(); //clear the blotter
                    return " Closed out all open positions.";
                }
                if (!DateHelper.IsMarketHours()) //the market is not open, its not a special case
                {
                    return " Market is not open.";
                }
                var result = await GetDataAsync();
                EmailService.SendTradeEmail(tsService.GetNewLongSignals());
                EmailService.SendShortTradeEmail(tsService.GetNewShortSignals());
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message; 
            }
        }
    }
}