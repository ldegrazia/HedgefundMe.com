using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
namespace HedgefundMe.com.Controllers
{
      [Authorize]
    public class TradeSignalsController : Controller
    {
        //
        // GET: /TradeSignal/
        private ProjectEntities db = new ProjectEntities();
        public ActionResult Index()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms); 
            return View(tsvm);
        }
        public ActionResult Longs()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            return View(tsvm);
        }
        public ActionResult Shorts()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            return View(tsvm);
        }
        public ActionResult Refresh()
        {
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> RunSignals()
        {
            //update the market data again
            //analyse the data
            //get the signals again
            MarketDataService ms = new MarketDataService(db);
            AnalysisService analService = new AnalysisService(db,ms);
            var result = await ms.LoadDataAsync(ms.StartOn);
            analService.Analyze();
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            tsvm.RunSignals();
            return RedirectToAction("Index");
        }
        public ActionResult History(int? page)
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            MarketDataParameters mdp = new MarketDataParameters();
            mdp.PageNumber = page ?? 1;
            tsvm.GetHistoricalSignals(mdp);
            return View(tsvm);
        }
        public ActionResult ClearNewTradeSignals()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            MarketDataParameters mdp = new MarketDataParameters();
            tsvm.ClearNewTradeSignals();
            return RedirectToAction("Index");
        }
        public ActionResult ClearAllSignals()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            MarketDataParameters mdp = new MarketDataParameters();
            tsvm.ClearTradeSignalHistory();
            return RedirectToAction("History");
        }

    }
}
