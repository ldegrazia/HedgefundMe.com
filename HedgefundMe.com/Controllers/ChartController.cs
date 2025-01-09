using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using HedgefundMe.com.Models;
using HedgefundMe.com.ViewModels;
namespace HedgefundMe.com.Controllers
{
     [Authorize]
    public class ChartController : Controller
    {
        public ActionResult Index(int? page, string exchange, string startsWith)
        {
            ChartFetchParameters para = new ChartFetchParameters();
                para.PageNumber = (page ?? 1);
                para.Exchange = exchange;
                para.StartsWith = startsWith;
                ChartViewModel cvm = new ChartViewModel(para);
                return View(cvm); 
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Exchange,StartsWith,ItemsPerPage,CurrentPage")]ChartFetchParameters parameters)
        {
            
            ChartViewModel vm = new ChartViewModel(parameters);
            return View(vm);
        }
      
    }
}