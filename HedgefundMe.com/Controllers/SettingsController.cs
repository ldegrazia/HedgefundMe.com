using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
using System.Data;
using System.Text;
using System.IO;
namespace HedgefundMe.com.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/
        private ProjectEntities db = new ProjectEntities();
        public ActionResult Index()
        {
            TradingSettingsService ts = new TradingSettingsService(db);
            var current = ts.Get();
            var _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            var usr = _userService.Get(User.Identity.Name);
             
            StockFileService s = new StockFileService(usr.UserName);

            ViewBag.FilePath = s.GetPhoto(string.Empty);
            return View(current);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            //we shoud get the file name here too
              var _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
              var usr = _userService.Get(User.Identity.Name);
             
              StockFileService s = new StockFileService(usr.UserName);
              ViewBag.FilePath = s.GetPhoto(string.Empty);
            try
            {
               var result = s.UpdatePhoto(Constants.StockDataFileName, Request.Files[Constants.File]);
               ViewBag.Result = "Saved " + result;
            }
            catch(Exception ex)
            {
                ViewBag.Result = ex.Message;
            }
            TradingSettingsService ts = new TradingSettingsService(db);
            var current = ts.Get();
            return View(current);
        }
        public ActionResult Details(int id)
        {
            TradingSettingsService ts = new TradingSettingsService(db);
            var current = ts.Get(id);
            return View(current);
        }
        public ViewResult Edit(int id)
        {
            TradingSettingsService ts = new TradingSettingsService(db);
             var _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            var current = ts.Get(id);
            var usr = _userService.Get(User.Identity.Name);
            current.CreatedBy = usr.UserName;
            return View(current);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TradingSettings settings)
        {
            if (ModelState.IsValid)
            {
                //save these settings
                TradingSettingsService ts = new TradingSettingsService(db);
                settings = ts.Update(settings);
                ViewBag.Result = "Saved changes";
            }
            return View(settings);
        }
        public ActionResult CreatePortfolio()
        {

            var portfolio = new PortfolioEntry();
            return View(portfolio);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePortfolio(PortfolioEntry portfolio)
        {
            if (ModelState.IsValid)
            {
                //save this portfolio
                try
                {
                    if (db.Portfolios.Any(t => t.Name == portfolio.Name))
                    {
                        throw new Exception(ErrorConstants.NameExists);
                    }
                    db.Portfolios.Add(portfolio);
                    db.SaveChanges();
                    ViewBag.Result = "Saved changes";
                }
                catch(Exception ex)
                {
                    ViewBag.Result = ex.Message;
                } 
            }
            return View(portfolio);
        }
        /// <summary>
        /// Returns the csv file
        /// </summary> 
        /// <returns></returns>
        public ActionResult GetFile()
        {
            var _userService = new UserService(HttpContext.Session[Constants.SessionAppNameKey].ToString());
            var usr = _userService.Get(User.Identity.Name);

            StockFileService s = new StockFileService(usr.UserName); 
            try
            {
                
              FileStream stream = new FileStream(s.GetPhoto(""), FileMode.Open,FileAccess.Read);
                 
                    FileStreamResult result = new FileStreamResult(stream, "text/csv");
                    result.FileDownloadName = "stocks.csv";
                    return result;
                
               
            }
            catch (Exception)
            {

                return null;
            }

        }
        public ViewResult Cancel(int? page)
        {
            MarketDataParameters para = new MarketDataParameters();
            para.PageNumber = (page ?? 1);
            SettingsViewModel sm = new SettingsViewModel(para, db);
            ViewBag.Result = string.Empty;
            return View(sm);
        }
        [HttpGet]
        public ViewResult Cancel(string id, int? page)
        { 
            try
            {
                
                 //find this id and delete from everything
            var trade = db.TradeBlotter.Where(t => t.TradeId == id).ToList();
            foreach (var datapoint in trade)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            } 
            db.SaveChanges();
            StringBuilder sb = new StringBuilder(); 
            //find the trade int he trade hisstory and delete it
            var tHistory = db.TradeHistory.Where(t => t.TradeIdOpen == id).ToList();
            foreach (var datapoint in tHistory)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
                sb.Append("Cancelled trade " + id + "<br/>");
            }
            db.SaveChanges();
            ViewBag.Result = sb.ToString();
            }
           catch(Exception ex)
            {
                ViewBag.Result = ex.Message;
            }
            MarketDataParameters para = new MarketDataParameters();
            para.PageNumber = (page ?? 1);
            SettingsViewModel sm = new SettingsViewModel(para, db); 
            return View(sm);
             
        }
        public ViewResult Portfolios()
        {
            var portfoiols = db.Portfolios.ToList();
            return View(portfoiols);
        }
        public ViewResult EditPortfolio(int id)
        {
            var portfolio= db.Portfolios.Single(t=>t.ID == id);
            return View(portfolio);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPortfolio(PortfolioEntry entry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Result = "Saved changes";
            }
            return View(entry);
        }
    }
}
