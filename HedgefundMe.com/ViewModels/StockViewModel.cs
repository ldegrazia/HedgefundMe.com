using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.ViewModels
{
    public class StockViewModel
    { 
        private StockService service; 
        public IEnumerable<Rss> Feeds { get; set; }
        public ITopStock ScanResult { get; set; }
        public string Ticker { get; set; }
        public List<ITopStock> BackTest { get; set; }
        public StockViewModel(string stock, ProjectEntities context )
        {
            service = new StockService(context);
            Ticker = stock.ToUpper();
            try
            {
                ScanResult = service.GetLatest(Ticker);
                if(ScanResult.ID == 0)
                {
                    //try the shorts
                    ScanResult = service.GetLatestShort(Ticker);
                    var bts = service.GetAllShortStockValuesSince(Ticker, DateTime.Now.AddDays(15)).ToList();
                    BackTest = new List<ITopStock>(bts.Cast<ITopStock>());
                }
                else
                {
                    //List<ISomeInterface> interfaceList = new List<ISomeInterface>(list.Cast<ISomeInterface>());
                    var bt = service.GetAllStockValuesSince(stock, DateTime.Now.AddDays(-15)).ToList();// as List<ITopStock>;
                    BackTest = new List<ITopStock>(bt.Cast<ITopStock>());
                }
                ScanResult.Ticker = Ticker;              
                Feeds = RssReader.GetRssFeed(Ticker);  //get the rss feeds
             
            }
            catch(Exception)
            {             
                 
                Feeds = RssReader.GetRssFeed(stock);
                ScanResult = new TopStock();
                ScanResult.Ticker = Ticker;
                BackTest = new List<ITopStock>();
                return;
            } 
        } 
        
    
    }
}