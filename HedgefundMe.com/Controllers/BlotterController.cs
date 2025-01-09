using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
namespace HedgefundMe.com.Controllers
{
      [Authorize]
    public class BlotterController : Controller
    { 
        private ProjectEntities db = new ProjectEntities();
        RebootRoleProvider _roleProvider;
        UserService _userService;
        public ActionResult Index()
        {
            _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            var usr = _userService.Get(User.Identity.Name);
            //see if the user is admin
            _roleProvider = new RebootRoleProvider(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            ViewBag.IsAdmin = (_roleProvider.IsUserInRole(usr.UserName, Constants.AdministratorsRole));
             MarketDataService ms = new MarketDataService(db);
             TradeSignalService ts = new TradeSignalService(db, ms); 
            BlotterViewModel bvm = new BlotterViewModel(db, ms, ts); 
            return View(bvm);
        }
        public async Task<ActionResult> RefreshAsync()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalService ts  = new TradeSignalService(db, ms); 
            BlotterViewModel bvm = new BlotterViewModel(db, ms, ts);
            await bvm.RefreshAsync();
            return RedirectToAction("Index");
        }
        public ActionResult History()
        {
            MarketDataParameters para = new MarketDataParameters(); 
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TodaysHistory();
            return View(hvm);
        }
        public ActionResult Open()
        {
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TodaysHistory();
            hvm.TodaysTrades = hvm.TodaysTrades.Where(t => t.State == Strings.Opinions.Open).OrderByDescending(t => t.OpenDate).ToList();
            return View(hvm);
        }
        [HttpPost]
        public ActionResult Open(string ticker)
        {
            //can we find this ticker?
            if (string.IsNullOrEmpty(ticker))
            {
                return RedirectToAction("Open");
            }
            ticker = ticker.ToUpper().Trim();
            //only show the data for this ticker
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TodaysHistory();
            hvm.TodaysTrades = hvm.TodaysTrades.Where(t => t.State == Strings.Opinions.Open && t.Ticker == ticker).OrderByDescending(t => t.OpenDate).ToList();
            hvm.CompletedTrades = hvm.TodaysTrades.Where(t => t.State == Strings.Opinions.Open && t.Ticker == ticker).OrderByDescending(t => t.CloseDate).ToList();
            hvm.ReclacPnl();
            ViewBag.Ticker = ticker;
            return View(hvm);
        }
        public ActionResult Closed()
        {
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TodaysHistory();
            hvm.TodaysTrades = hvm.TodaysTrades.Where(t => t.State == Strings.Opinions.Closed).ToList();
            return View(hvm);
        }
        public ActionResult Details(int id)
        {
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TradeDetails(id); 
            return View(hvm);
        }
        public ActionResult SignalDetails(string id)
        {
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.SignalDetails(id);
            return RedirectToAction("Details", new { id = hvm.TradeEntryDetails.ID });
        }
        public ActionResult Edit(int id)
        {
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TradeDetails(id);
            return View(hvm.TradeEntryDetails);
        }
          [HttpPost]
        public ActionResult Edit(TradeHistoryEntry item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //we need to recalc
                    item.CloseValue = item.Shares * item.ClosePrice;
                    item.OpenValue = item.Shares * item.OpenPrice;
                    item.CalculateGainLoss();
                    //now save it
                    db.Entry(item).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Result = "Saved Changes";
                }
                catch(Exception ex)
                {
                    ViewBag.Result = ex.Message; 
                } 
            }
            return View(item);
        }
        [HttpPost]
        public ActionResult Closed(string ticker)
        {
            //can we find this ticker?
            if(string.IsNullOrEmpty(ticker))
            {
                return RedirectToAction("Closed");
            }
            ticker = ticker.ToUpper().Trim();
            //only show the data for this ticker
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.TodaysHistory();
            hvm.TodaysTrades = hvm.TodaysTrades.Where(t => t.State == Strings.Opinions.Closed && t.Ticker==ticker).OrderByDescending(t=>t.OpenDate).ToList();
            hvm.CompletedTrades = hvm.TodaysTrades.Where(t => t.State == Strings.Opinions.Closed && t.Ticker == ticker).OrderByDescending(t => t.CloseDate).ToList();
            hvm.ReclacPnl();
            ViewBag.Ticker = ticker;
            return View(hvm);
        }
        public ActionResult All(int? page)
        {
            MarketDataParameters para = new MarketDataParameters();
            para.PageNumber = (page ?? 1);
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.GetHistory(para);
            return View(hvm);
        }
        public ActionResult ClearAllTrades()
        {
            DataManager dm = new DataManager(db);
            dm.ClearBlotter();
            return RedirectToAction("Index");
        }
       
        public ActionResult ClearOpenHistoricalTrades()
        {
            MarketDataParameters para = new MarketDataParameters();
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            hvm.ClearOpenHistoricalTrades();
            return RedirectToAction("History");
        }
        public async Task<ActionResult> Run()
        {
            DataManager dm = new DataManager(db);
            await dm.GetDataAsync();
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> CloseOutPositions()
        {
            DataManager dm = new DataManager(db);
            await dm.CloseOutPositions();
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> CloseOutTradeHistoryPositions()
        {
            DataManager dm = new DataManager(db);
            await dm.CloseOutTradeHistoryPositions();
            return RedirectToAction("History");
        }
        public ActionResult UpdateClosedValues()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalService ts = new TradeSignalService(db, ms);
            BlotterViewModel bvm = new BlotterViewModel(db, ms, ts);
            bvm.UpdateClosedValues();
            return RedirectToAction("History");
        }
        public ActionResult ResetTodaysOpenData()
        {
            MarketDataService ms = new MarketDataService(db);
            TradeSignalViewModel tsvm = new TradeSignalViewModel(db, ms);
            MarketDataParameters para = new MarketDataParameters();
            DataManager dm = new DataManager(db);
            TradeHistoryViewModel hvm = new TradeHistoryViewModel(para, db);
            tsvm.ClearNewTradeSignals();
            hvm.ClearOpenHistoricalTrades();
            dm.ClearBlotter(); 
            return RedirectToAction("History");
        }
        [HttpGet]
        public ActionResult Cancel(string tradeidOpen,string tradeidClose )
        {
            try
            {
                var trade = db.TradeSignalHistory.Where(t => t.TradeId == tradeidOpen).ToList();
                foreach (var datapoint in trade)
                {
                    db.Entry(datapoint).State = EntityState.Deleted;
                }
                db.SaveChanges();
                var tradeclose = db.TradeSignalHistory.Where(t => t.TradeId == tradeidClose).ToList();
                foreach (var datapoint in tradeclose)
                {
                    db.Entry(datapoint).State = EntityState.Deleted;
                }
                db.SaveChanges();
                //find the trade int he trade history and delete it
                var tHistory = db.TradeHistory.Where(t => t.TradeIdOpen == tradeidOpen).ToList();
                foreach (var datapoint in tHistory)
                {
                    db.Entry(datapoint).State = EntityState.Deleted; 
                }
                db.SaveChanges();
               
            }
            catch  
            {
                throw;
            }
            return RedirectToAction("Closed");

        }
    }
}
