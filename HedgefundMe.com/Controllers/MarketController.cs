using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Text;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
using HedgefundMe.com.Services;

namespace HedgefundMe.com.Controllers
{
     [Authorize]
    public class MarketController : Controller
    {
        private ProjectEntities db = new ProjectEntities();

        public ActionResult Index()
        { 
            
            return View( );
        }
        [HttpPost]
        public JsonResult GetTrendsFor(string scan)
        {
            List<TrendModel> trends = new List<TrendModel>();
             var all = db.StockScans.Single(t => t.Name == scan);
             // how many for each day do we have?
              DateTime start = DateTime.Now.AddDays(-30);
              var stocks = db.Stocks.Where(t => t.Date >= start.Date && t.Scans.Any(f => f.Name == all.Name)).ToList();
                do
                { 
                    var tdp = new TrendModel { Label = start.ToShortDateString(), Value = stocks.Where(t=>t.Date== start.Date).Count() };
                     trends.Add(tdp); 
                    start = start.AddDays(1);
                    if(DateHelper.IsWeekend(start))
                    {
                        start = start.AddDays(2);
                    }
                }
                while (start <= DateTime.Now); 
            return Json(trends, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTodaysTrends(string goodBad)
        {
            if (string.IsNullOrEmpty(goodBad))
            {
                goodBad = "good";
            }
            var scans = Strings.ScanNames.GoodScanTrends();
            if (goodBad == "bad")
            {
                scans = Strings.ScanNames.BadScanTrends();
            }

            List<TrendModel> trends = new List<TrendModel>();
            var service = new StockService(db);
            foreach (var t in scans)
            {
                var mdp = service.GetTrendsFor(t, service.StartOn, service.StartOn);
                foreach (var dp in mdp.Values)
                {
                    var tdp = new TrendModel { Label = t, Value = dp.Value };
                    trends.Add(tdp);
                }
            }
            return Json(trends, JsonRequestBehavior.AllowGet);
        }
    }
}
