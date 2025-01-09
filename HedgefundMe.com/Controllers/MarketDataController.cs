using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
namespace HedgefundMe.com.Controllers
{
      [Authorize]
    public class MarketDataController : Controller
    {
        //
        // GET: /MarketData/
        private ProjectEntities db = new ProjectEntities();
        public  ActionResult  Index(int? page)
        { 
            MarketDataParameters para = new MarketDataParameters();
            para.PageNumber = (page ?? 1);
            MarketDataViewModel mvm = new MarketDataViewModel(para,db); 
            if(!db.Portfolios.Any())
            {
                //seed protfolios
                PortfolioEntry pe = new PortfolioEntry
                {
                    Name = "Top100",
                    Url = @"https://finance.yahoo.com/portfolio/pf_576/view/v2"
                };
                PortfolioEntry pe2 = new PortfolioEntry
                {
                    Name = "Bottom100",
                    Url = @"https://finance.yahoo.com/portfolio/pf_577/view/v2"
                };
                db.Portfolios.Add(pe);
                db.Portfolios.Add(pe2);
                db.SaveChanges();
            }
            return View(mvm);
        }
        public async Task<ActionResult> Refresh()
        {
            try
            {
                MarketDataParameters para = new MarketDataParameters();
                MarketDataViewModel mvm = new MarketDataViewModel(para, db);
                ViewBag.Result = await mvm.RefreshAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;
            }
            return RedirectToAction("Ranks");
        }
        public async Task<ActionResult> LoadData()
        {
             try
             {
                 MarketDataParameters para = new MarketDataParameters();
                 para.PageNumber = 1;
                 MarketDataViewModel mvm = new MarketDataViewModel(para, db);
                 ViewBag.Result = await mvm.GetDataAsync();
                 mvm.Analyze();
             }
             catch (Exception ex)
             {
                 ViewBag.Result = ex.Message; 
             }
             return View(); 
        }
        
        public ActionResult Ranks()
        {
            MarketDataParameters para = new MarketDataParameters();
            para.PageNumber = 1;
            MarketDataViewModel mvm = new MarketDataViewModel(para, db);
            return View(mvm);
        }
        
    
    }
}
