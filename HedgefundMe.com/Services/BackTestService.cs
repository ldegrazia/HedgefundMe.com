using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class BackTestService
    {
        public const int ShareCount = 100;
        ProjectEntities _context;
        StockService scanservice;
        public BackTestService(ProjectEntities context)
        {
            _context = context;
            scanservice = new StockService(context);
        }
        public List<Position> BackTest(BackTestParameters parameters)
        {
            List<Position> positions = new List<Position>();

            StringBuilder sb = new StringBuilder();
            //create a position
            Position p = new Position { Ticker = parameters.Ticker.ToUpper(), Parameters = parameters }; 
            //get the scans from from to to
            var startResults = scanservice.GetAllStockValuesSince(p.Ticker, parameters.From()).OrderBy(t=>t.Date);
            //now for each result let's look at the opinion
            List<DateTime> tested = new List<DateTime>();
            foreach(var scanresult in startResults)
            { 
                if(tested.Contains(scanresult.Date))
                {
                    continue;
                }
                tested.Add(scanresult.Date); 
                if(scanresult.Opinion == Strings.Opinions.Buy)
                {
                    p.AddToPosition(scanresult.Price, scanresult.Date, Strings.Opinions.Buy);
                    continue;
                }
                if(scanresult == null || scanresult.Opinion == null)
                {
                    continue;
                }
                if (scanresult.Opinion.Contains(Strings.Opinions.Sell))
                {
                    if(!p.Trades.Any())
                    {
                        continue;
                    }
                    //p.SellPosition(scanresult.Date, scanresult.Price);
                    //positions.Add(p);
                    //make a new position
                   p = new Position { Ticker = parameters.Ticker.ToUpper(), Parameters = parameters };
                    continue;
                } 
            } 
            if(p.Trades.Any()) //close out
            {
                var resul = scanservice.GetLatest(p.Ticker);
                p.SellPosition(resul.Price,resul.Date);
                positions.Add(p);
            } 
            
            return positions;
        }
        static object locker = new object();
        public List<BestBet> BestBets(BackTestParameters parameters)
        {
            DateTime since = DateTime.Now.AddDays(-parameters.DaysBack);
            var latest = scanservice.GetLatest();
            //now lets run the back test on each
            //var all = scanservice.GetAllStockValuesSince(since).Where(t => t.Opinion == Strings.Opinions.Buy && !t.Opinion.Contains(Strings.Opinions.Sell)).ToList();
            //var all = scanservice.GetAllStockValuesSince(since).ToList();
            //foreach of these results, get each stock
            List<BestBet> bb = new List<BestBet>();
            Dictionary<string, List<Position>> results = new Dictionary<string, List<Position>>();
             Parallel.ForEach(latest, stock =>
                {
                 var parms = new BackTestParameters { Ticker = stock.Ticker , DaysBack = parameters.DaysBack, BuyAmount=parameters.BuyAmount, BuyInDollarAmount =parameters.BuyInDollarAmount, DollarCostAvg = parameters.DollarCostAvg }; 
                 //now run this and return the position then add it to the list
                 List<Position> positions;
                 lock (locker)
                 {
                   positions = BackTest(parms);
                 }
                 foreach (var p in positions) //if the latest data is negative ignore it if it is zero dont add it
                 {
                      //if (p.CalcPnl() <= 0)
                     //    continue;
                     BestBet b = new BestBet { Ticker = p.Ticker };
                     b.Pnl = p.CalcPnl();
                     b.PcnlPercentage = p.CalcPnlPercentage();
                     b.Positions.Add(p);
                     bb.Add(b);
                 }
             });
            return bb.OrderByDescending(t => t.PcnlPercentage).ToList();
        }
        /// <summary>
        /// Gets the best bets given the parameters and returns them
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<BestBetResult> RunBestBets(BackTestParameters parameters)
        {
            List<BestBetResult> bbs = new List<BestBetResult>(); 
            var results = BestBets(parameters);
            foreach(var b in results)
            {
                BestBetResult bbr = new BestBetResult();
                 bbr.BuyAmount=parameters.BuyAmount;
                 bbr.BuyInDollarAmount = Convert.ToBoolean(parameters.BuyInDollarAmount);
                 bbr.Date = DateTime.Now;
                 bbr.DaysBack = parameters.DaysBack;
                 bbr.DollarCostAvg = parameters.DollarCostAvg;
                 bbr.Ticker = b.Ticker;
                 bbr.PcnlPercentage = b.PcnlPercentage;
                 bbr.Pnl = b.Pnl;
                 bbs.Add(bbr);
            }
            return bbs;
        }
        /// <summary>
        /// Deletes existing best bets and adds the new ones
        /// </summary>
        public void UpdateBestBets(List<BestBetResult> bbs)
        {
            //need to know the latest best bets with this daysback date
            if(!bbs.Any())
            {
                return;
            }
            //get the latest bbs with this days back
            var allExistingDaysBack = _context.BestBets.ToList();
            foreach(var db in allExistingDaysBack)
            {
                _context.Entry(db).State = EntityState.Deleted;
            }
            _context.SaveChanges();
            //now lets add these new ones
            foreach(var tt in bbs)
            {
                _context.BestBets.Add(tt);
            }
            _context.SaveChanges();
             
        }
        /// <summary>
        /// Gets the latest bet bets in the database
        /// </summary>
        /// <returns></returns>
        public List<BestBetResult> GetLatestBestBets()
        {
            return _context.BestBets.ToList();
        }
        /// <summary>
        /// Reruns the pnl for 15 days back
        /// </summary>
        public void UpdateTodaysBestBets()
        {
            BackTestParameters parameters = new BackTestParameters { DaysBack = 15 };
            BackTestService bs = new BackTestService(_context);
            List<BestBetResult> results = new List<BestBetResult>();
            try
            {
                var r = bs.RunBestBets(parameters);
                bs.UpdateBestBets(r); 
            }
            catch (Exception ex)
            {
                Logger.WriteLine(MessageType.Error, ex.Message);
            }
             
        }
        
        /// <summary>
        /// Gets the strategy with the id
        /// </summary>
        /// <param name="strategyId"></param>
        /// <returns></returns>
        public Strategy GetStrategy(int strategyId)
        { 
            if (!_context.Strategies.Any(t => t.StrategyID == strategyId))
            {
                throw new ArgumentException("Unknown startegy Id" + strategyId);
            }
            //get the strategy and return it
            return _context.Strategies.First(p => p.StrategyID == strategyId); 
        }

        public Portfolio Strategy1()
        {
            //create a portfolio
            //add a new strategy
            string nam = "Normal Strategy";
            Strategy strategy;
            if (!_context.Strategies.Any(t => t.Name == nam))
            {

                strategy = new Strategy();
                strategy.Name = nam;
                strategy.Description = "Adhere to all Signals, sell all drops";
                _context.Strategies.Add(strategy);
                _context.SaveChanges();
            }
            strategy = _context.Strategies.First(f => f.Name == nam);

            strategy.DataPoints.ToList().ForEach(r => _context.Entry(r).State= EntityState.Deleted);
            strategy.Trades.ToList().ForEach(r => _context.Entry(r).State = EntityState.Deleted);
            _context.Entry(strategy).State = EntityState.Modified;
            _context.SaveChanges();
            Portfolio p = new Portfolio();
            p.Name = strategy.Name;
            p.MarginFactor = 1;
            p.Description = strategy.Description;
            int daysBack = -150;
            //start from the days back and run through each day
            //on each day
            //if its a buy, buy it
            //if its a sell, sell it
            //let init everything as a buy
            p.StartDate = DateTime.Now.AddDays(daysBack);
            var startDay = scanservice.GetStocksOn(p.StartDate);
            p.EndDate = DateTime.Now;
            PortfolioDataPoint mm = new PortfolioDataPoint();
            mm.Date = startDay.First().Date;
            mm.Value = 0.0;
            p.DailyValues.Add(mm); 
            foreach (var stock in startDay)
            {               
                if( string.IsNullOrEmpty(stock.CurrentSide ))
                {
                    continue;
                } 
                //lets buy it
                Trade t = new Trade { Shares = Strings.BackTest.BuyAmount, Ticker = stock.Ticker,  Price = stock.Price, Date = stock.Date };
                if(stock.CurrentSide == Strings.Side.Long)
                {
                    t.Action = Strings.Opinions.Buy;
                }
                else if(stock.CurrentSide == Strings.Side.Short)
                {
                    t.Action = Strings.Opinions.Sell;
                }
                p.Make(t); 
                //update the pnl daily value 
                p.UpdateCurrentMark(stock.Ticker, stock.Price); 
                p.DailyValues.First(y => y.Date == stock.Date).Value = p.CurrentTotalValue();//need to split this up and print it 
            }
            var startResults = scanservice.GetAllStockValuesSince(DateTime.Now.AddDays(daysBack)).OrderBy(t => t.Date);
            List<DateTime> dates = new List<DateTime>();
            //away we go
            //now for each result let's look at the opinion 
            foreach (var scanresult in startResults)
            {
                if (!p.DailyValues.Any(j => j.Date == scanresult.Date))
                {
                    //update the date and pnl value here
                    //this is a new date
                    //we need to find stocks that are dropped
                    PortfolioDataPoint m = new PortfolioDataPoint();
                    m.Date = scanresult.Date;
                    m.Value = 0.0;
                    p.DailyValues.Add(m);
                    var current = scanservice.GetStocksOn(scanresult.Date);
                    var dropped = scanservice.GetDrops(current);
                    //we need to sell out of these positions
                    var openDropped = p.Positions.Where(f => !string.IsNullOrEmpty(f.CurrentSide)).ToList();
                    foreach (var d in dropped)
                    {
                        if (openDropped.Any(pp => pp.Ticker == d.Ticker))
                        { 
                            p.UpdateCurrentMark(d.Ticker, d.Price);
                            //close the position out 
                            var lastDroppedPrice = openDropped.First(yy => yy.Ticker == d.Ticker); 
                            lastDroppedPrice.CloseLastPosition( d.Price,d.Date);
                             //update the amount here..if i shorted then get the toal and if i was long it the normal total
                            p.UpdateWorkingCaptial(d.Ticker); 
                            //update this pnl daily value
                            p.DailyValues.First(y => y.Date == scanresult.Date).Value = p.CurrentTotalValue();
                        }
                    }
                }
                if (scanresult == null || scanresult.Opinion == null || string.IsNullOrEmpty(scanresult.Opinion))
                {
                    continue;
                }
                p.UpdateCurrentMark(scanresult.Ticker, scanresult.Price);
                Trade t = new Trade { Shares = Strings.BackTest.BuyAmount, Ticker = scanresult.Ticker, Action = scanresult.Opinion, Price = scanresult.Price, Date = scanresult.Date };
                p.Make(t);
                //update the pnl daily value
                p.DailyValues.First(y => y.Date == scanresult.Date).Value = p.CurrentTotalValue();
            }
            //close out positions 
            var open = p.Positions.Where(t => !String.IsNullOrEmpty(t.CurrentSide)).ToList();
            //get the latest
            var latest = scanservice.GetLatest();
            //for each open position find this position and get the latest price
            foreach (var o in open)
            {
                if (!latest.Any(t => t.Ticker == o.Ticker))
                {
                    //close this out anyway
                    //get the last trade and sell at that price
                    var last = o.LastTrade();
                    p.UpdateCurrentMark(last.Ticker, o.CurrentMark);
                    o.CloseLastPosition(o.CurrentMark,last.Date);
                    p.UpdateWorkingCaptial(o.Ticker);
                    //update this pnl daily value
                    p.DailyValues.First(y => y.Date == last.Date.Date).Value = p.CurrentTotalValue();
                    continue;
                }
                var lastPrice = latest.First(t => t.Ticker == o.Ticker);
                p.UpdateCurrentMark(o.Ticker, lastPrice.Price);
                o.CloseLastPosition(lastPrice.Price, lastPrice.Date);
                p.UpdateWorkingCaptial(o.Ticker);
                p.DailyValues.First(y => y.Date == lastPrice.Date).Value = p.CurrentTotalValue();
            } 
            
            foreach(var pdp in p.DailyValues)
            {
                //add this to the strategy
                strategy.DataPoints.Add(pdp);
            }
            foreach(var trd in p.Positions)
            {
                foreach(var tr in trd.Trades)
                {
                    strategy.Trades.Add(tr);
                }
            }
            _context.Entry(strategy).State = EntityState.Modified;
            _context.SaveChanges();
            p.Positions = p.Positions.OrderBy(t => t.Trades.Count()).ToList();
            return p;
        }
        public Strategy GetStrategy(string name)
        {
            return _context.Strategies.First(t => t.Name == name); 
        }
        /// <summary>
        /// Get all avalable strategies
        /// </summary>
        /// <returns></returns>
         public List<Strategy> GetStrategies()
        {
            return _context.Strategies.ToList();
        }
         public Portfolio ShortStrategy()
         {
             //create a portfolio
             //add a new strategy
             string nam = "Short Strategy";
             Strategy strategy;
             if (!_context.Strategies.Any(t => t.Name == nam))
             {

                 strategy = new Strategy();
                 strategy.Name = nam;
                 strategy.Description = "Shorts, cover all drops";
                 _context.Strategies.Add(strategy);
                 _context.SaveChanges();
             }
             strategy = _context.Strategies.First(f => f.Name == nam);

             strategy.DataPoints.ToList().ForEach(r => _context.Entry(r).State = EntityState.Deleted);
             strategy.Trades.ToList().ForEach(r => _context.Entry(r).State = EntityState.Deleted);
             _context.Entry(strategy).State = EntityState.Modified;
             _context.SaveChanges();
             Portfolio p = new Portfolio();
             p.BuyAmount = Strings.BackTest.ShortAmount;
             p.Name = strategy.Name;
             p.MarginFactor = 1;
             p.Description = strategy.Description;
             int daysBack = -50;
             //start from the days back and run through each day
             //on each day
             //if its a buy, buy it
             //if its a sell, sell it
             //let init everything as a buy
            
             p.EndDate = DateTime.Now;
             PortfolioDataPoint mm = new PortfolioDataPoint();
             List<BottomStock> startDay = new List<BottomStock>();
             do
             {
                 p.StartDate = DateTime.Now.AddDays(daysBack);
                 startDay = scanservice.GetShortStocksOn(p.StartDate);
                 daysBack++;
             } while (!startDay.Any());
             mm.Date = startDay.First().Date;
             daysBack++;
             mm.Value = 0.0;
             p.DailyValues.Add(mm);
             foreach (var stock in startDay)
             {
                 if (string.IsNullOrEmpty(stock.CurrentSide))
                 {
                     continue;
                 }
                 //lets short it
                 Trade t = new Trade { Shares = Strings.BackTest.ShortAmount, Ticker = stock.Ticker, Price = stock.Price, Date = stock.Date };
                 if (stock.CurrentSide == Strings.Side.Short)
                 {
                     t.Action = Strings.Opinions.Short;
                 }
                 p.Make(t);
                 //update the pnl daily value 
                 p.UpdateCurrentMark(stock.Ticker, stock.Price);
                 p.DailyValues.First(y => y.Date == stock.Date).Value = p.CurrentTotalValue();//need to split this up and print it 
             }
             var startResults = scanservice.GetAllShortStockValuesSince(DateTime.Now.AddDays(daysBack)).OrderBy(t => t.Date);
             List<DateTime> dates = new List<DateTime>();
             //away we go
             //now for each result let's look at the opinion 
             foreach (var scanresult in startResults)
             {
                 if (!p.DailyValues.Any(j => j.Date == scanresult.Date))
                 {
                     //update the date and pnl value here
                     //this is a new date
                     //we need to find stocks that are dropped
                     PortfolioDataPoint m = new PortfolioDataPoint();
                     m.Date = scanresult.Date;
                     m.Value = 0.0;
                     p.DailyValues.Add(m);
                     var current = scanservice.GetShortStocksOn(scanresult.Date);
                     var dropped = scanservice.GetDroppedShorts(current);
                     //we need to sell out of these positions
                     var openDropped = p.Positions.Where(f => !string.IsNullOrEmpty(f.CurrentSide)).ToList();
                     foreach (var d in dropped)
                     {
                         if (openDropped.Any(pp => pp.Ticker == d.Ticker))
                         {
                             p.UpdateCurrentMark(d.Ticker, d.Price);
                             //close the position out 
                             var lastDroppedPrice = openDropped.First(yy => yy.Ticker == d.Ticker);
                             lastDroppedPrice.CloseLastPosition(d.Price, d.Date);
                             //update the amount here..if i shorted then get the toal and if i was long it the normal total
                             p.UpdateWorkingCaptial(d.Ticker);
                             //update this pnl daily value
                             p.DailyValues.First(y => y.Date == scanresult.Date).Value = p.CurrentTotalValue();
                         }
                     }
                 }
                 if (scanresult == null || scanresult.Opinion == null || string.IsNullOrEmpty(scanresult.Opinion))
                 {
                     continue;
                 }
                
                 Trade t = new Trade { Shares = Strings.BackTest.ShortAmount, Ticker = scanresult.Ticker, Action = scanresult.Opinion, Price = scanresult.Price, Date = scanresult.Date };
                 p.Make(t);
                 p.UpdateCurrentMark(scanresult.Ticker, scanresult.Price);
                 //update the pnl daily value
                 p.DailyValues.First(y => y.Date == scanresult.Date).Value = p.CurrentTotalValue();
             }
             //close out positions 
             var open = p.Positions.Where(t => !String.IsNullOrEmpty(t.CurrentSide)).ToList();
             //get the latest
             var latest = scanservice.GetLatestShorts();
             //for each open position find this position and get the latest price
             foreach (var o in open)
             {
                 if (!latest.Any(t => t.Ticker == o.Ticker))
                 {
                     //close this out anyway
                     //get the last trade and sell at that price
                     var last = o.LastTrade();
                     p.UpdateCurrentMark(last.Ticker, o.CurrentMark);
                     o.CloseLastPosition(o.CurrentMark, last.Date);
                     p.UpdateWorkingCaptial(o.Ticker);
                     //update this pnl daily value
                     p.DailyValues.First(y => y.Date == last.Date.Date).Value = p.CurrentTotalValue();
                     continue;
                 }
                 var lastPrice = latest.First(t => t.Ticker == o.Ticker);
                 p.UpdateCurrentMark(o.Ticker, lastPrice.Price);
                 o.CloseLastPosition(lastPrice.Price, lastPrice.Date);
                 p.UpdateWorkingCaptial(o.Ticker);
                 p.DailyValues.First(y => y.Date == lastPrice.Date).Value = p.CurrentTotalValue();
             }

             foreach (var pdp in p.DailyValues)
             {
                 //add this to the strategy
                 strategy.DataPoints.Add(pdp);
             }
             foreach (var trd in p.Positions)
             {
                 foreach (var tr in trd.Trades)
                 {
                     strategy.Trades.Add(tr);
                 }
             }
             _context.Entry(strategy).State = EntityState.Modified;
             _context.SaveChanges();
             p.Positions = p.Positions.OrderBy(t => t.Trades.Count()).ToList();
             return p;
         }
    }
}