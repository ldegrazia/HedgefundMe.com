using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HedgefundMe.com;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
using HedgefundMe.com.Services;
namespace HedgefundMe.com.Controllers
{
    
    public class ApiController : Controller
    {
        //
        // GET: /Api/
        private ProjectEntities db = new ProjectEntities(); 
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult LatestTrades()
        {
            try
            {
                MarketDataService mdService = new MarketDataService(db);
                TradeSignalService service = new TradeSignalService(db, mdService);
                return Content(service.GetLatestLongTradesAsXml(), Constants.XmlResponse); 
            }
            catch (Exception ex)
            {
                return Content(ex.Message); 
            }
        }
        public ActionResult LatestShortTrades()
        {
            try
            {
                MarketDataService mdService = new MarketDataService(db);
                TradeSignalService service = new TradeSignalService(db, mdService);
                return Content(service.GetLatestShortTradesAsXml(), Constants.XmlResponse);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        /// <summary>
        /// Returns the market data for today as xml
        /// </summary>
        /// <returns></returns>
        public ActionResult Universe()
        {
            try
            {
                MarketDataService mdService = new MarketDataService(db);
                return Content(mdService.GetUniverseAsXml(), Constants.XmlResponse);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
         
    }
}
