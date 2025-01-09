using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HedgefundMe.com.Services;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Controllers
{
    [Authorize]
    public class DatabaseController : Controller
    {
        
        // GET: /Database/
        private ProjectEntities db = new ProjectEntities();
        public ActionResult Index()
        {
            return View();
        }
     
        public ActionResult MockTrades()
        {
            MarketDataService mdService = new MarketDataService(db);
            List<TradeSignal> allSignals = new List<TradeSignal>();
            List<MarketData> alldata = new List<MarketData>();
            alldata.AddRange(mdService.GetDataOn(mdService.StartOn, Strings.Side.Long));
            alldata.AddRange(mdService.GetDataOn(mdService.StartOn, Strings.Side.Short));
            //create new buy signals
            //create new sell signals
            for (int i = 0; i < 12; i++)
            {
                var price = alldata[i].Price + (-.2);
                var shares = FinanceHelper.GetShareAmount(5000, alldata[i].Price + (-.2));
                var value = shares * price;
                var stop = price - (price * 0.01);
                var target = price + (price * 0.01);
                allSignals.Add(new TradeSignal
                        {
                            Action = Strings.Opinions.Buy,
                            Date = DateTime.Now,
                            Price = price,
                            Ticker = alldata[i].Ticker,
                            Strategy = Strings.Strategies.IntradayMomentumLong,
                            Shares =shares,
                            Value = value,
                            StopPrice = stop,
                            TargetPrice = target,
                            TradeId = Guid.NewGuid().ToString(),
                            TradingSettings = 2,
                            Details = string.Format("MOCKED {0} {5} Shares of {1} @ {2}, Target: {3}, Stop: {4}, Risk: {6}.", Strings.Opinions.Buy, alldata[i].Ticker, price.ToString("C2"), target.ToString("C2"), stop.ToString("C2"),
                            shares, value.ToString("C2"))
                        });
            }
            for (int i = 12; i < 24; i++)
            {
                var price = alldata[i].Price + (0.2);
                var shares = FinanceHelper.GetShareAmount(5000, alldata[i].Price + (0.2));
                var value = shares * price;
                var stop = price +( price * 0.01);
                var target = price - (price * 0.01);
                allSignals.Add(new TradeSignal
                {
                    Action = Strings.Opinions.Short,
                    Date = DateTime.Now,
                    Price = price,
                    Ticker = alldata[i].Ticker,
                    Strategy = Strings.Strategies.IntradayMomentumShort,
                    Shares = shares,
                    Value = value,
                    StopPrice = stop,
                    TargetPrice = target,
                    TradeId = Guid.NewGuid().ToString(),
                    TradingSettings = 2,
                    Details = string.Format("MOCKED {0} {5} Shares of {1} @ {2}, Target: {3}, Stop: {4}, Risk: {6}.", Strings.Opinions.Buy, alldata[i].Ticker, price.ToString("C2"), target.ToString("C2"), stop.ToString("C2"),
                    shares, value.ToString("C2"))
                });
            }
            TradeSignalService tsService = new TradeSignalService(db, mdService);
            tsService.AddNewTradeSignals(allSignals);
            BlotterManager bm = new BlotterManager(db, mdService, tsService);
            bm.UpdatePositions();
            mdService.RefreshTodaysPortfolioData();
            bm.UpdateBlotterData();
            //add the trade to the blotter
            //done
            ViewBag.Result = "Mocked "+ alldata.Count() + " Trades";
            return View();
        }
        public ActionResult DeleteMockedTrades()
        {
            var mockedSignals = db.TradeSignalHistory.Where(t => t.Details.Contains("MOCKED")).ToList();
            var tradeEntries = new List<TradeHistoryEntry>();
            var newTradeSignals = new List<TradeSignal>();
            foreach(var tt in mockedSignals)
            {
                db.Entry(tt).State = EntityState.Deleted;
                tradeEntries.AddRange(db.TradeHistory.Where(t => t.TradeIdOpen == tt.TradeId || t.TradeIdClose == tt.TradeId));
                //delete this in trade history 
            }
            foreach(var entry in tradeEntries)
            {
                db.Entry(entry).State = EntityState.Deleted;
            }
            db.SaveChanges();
            MarketDataService mdService = new MarketDataService(db);
            TradeSignalService tsService = new TradeSignalService(db, mdService);
            BlotterManager bm = new BlotterManager(db, mdService, tsService);
            bm.ClearBlotter();
            tsService.ClearNewTradeSignals();
            ViewBag.Result = "Deleted Mocked Trades";
            return View();
        }
         
    }
}
