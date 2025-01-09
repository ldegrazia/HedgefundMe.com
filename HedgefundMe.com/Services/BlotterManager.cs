using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HedgefundMe.com.Models;
using System.Threading.Tasks;
namespace HedgefundMe.com.Services
{
    public class BlotterManager
    {
         
        private readonly ProjectEntities db;
        private readonly MarketDataService mdService;
        private readonly TradeSignalService tsService;
        private readonly TradingSettingsService sService;
        public TradingSettings CurrentSettings { get; set; }
        public bool BlotterIsMaxed {get;set;}
        public BlotterManager(ProjectEntities context, MarketDataService mdservice, TradeSignalService tservice)
        {
            db = context;
            mdService = mdservice;
            tsService = tservice;
            sService = new TradingSettingsService(context);
            CurrentSettings = sService.GetCurrent();
        }
        public double TargetPriceChange()
        {
            return CurrentSettings.TargetPriceChangeLong;
        } 
        /// <summary>
        /// Main routine, this gets the new signals and updates the blotter
        /// then it refreshes the blotter data, then checks the stops and targets in the blotter
        /// </summary>
        public void UpdateTrades()
        {
            UpdateBlotterData(); 
            if (BlotterIsMaxed)
            {
                tsService.ClearNewTradeSignals(); //we are ignoring the new trade signals we got
            }
            else
            {

                UpdatePositions();//we are not maxed so process more signals.
            }
            UpdateBlotterData(); 
            UpdateStops();//can't do these first
            UpdateTargets();
            UpateTradeSignalHistory();
        }
        /// <summary>
        /// Cancels the trade in the blotter and the trade history
        /// </summary>
        /// <param name="tradeOpenId"></param>
        public void CancelTrade(string tradeOpenId)
        {
            //find this id and delete from everything
            var trade = db.TradeBlotter.Where(t => t.TradeId == tradeOpenId).ToList();
            foreach (var datapoint in trade)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            }
            db.SaveChanges();
            //find the trade int he trade hisstory and delete it
            var tHistory = db.TradeHistory.Where(t => t.TradeIdOpen == tradeOpenId).ToList();
            foreach (var datapoint in tHistory)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// All commited trade signals are saved to the table
        /// </summary>
        public void UpateTradeSignalHistory()
        {
            tsService.UpdateTradeSignalHistory();
        }
        /// <summary>
        /// For the last half hour, this refreshes the blotter data, then checks the stops and targets in the blotter
        /// </summary>
        public async Task UpdateBlotterStopsAndTargetsAsync()
        {
            await RefreshAsync();
            UpdateStops();
            UpdateTargets();
        }
        /// <summary>
        /// Will handle buy and short for the current blotter positions
        /// </summary>
        public void UpdatePositions()
        {
            var allPositions = db.TradeBlotter.ToList();
            var completed = GetCompletedTrades(mdService.StartOn);
            var allSignals = tsService.GetNewSignals();
            List<TradeSignal> ignored = new List<TradeSignal>();
            List<BlotterEntry> newPositions = new List<BlotterEntry>();
            List<TradeHistoryEntry> newTradehistoryEntries = new List<TradeHistoryEntry>();
            List<BlotterEntry> closedPositions = new List<BlotterEntry>();
            List<TradeSignal> closeSignals = new List<TradeSignal>();
            foreach (var signal in allSignals)//for each new trade
            {
                if (signal.Action == Strings.Opinions.Buy)
                {
                    if (IsSameLosingTradeMoreThanMax(completed, signal)) //this is a losing reentry, ignore it
                    {
                        ignored.Add(signal);
                        continue;
                    }
                    //if the current trade is a buy and the trade signal is buy, ignore the trade and remove it
                    if (allPositions.Any(t => t.Side == Strings.Side.Long && t.Ticker == signal.Ticker))
                    {
                        ignored.Add(signal);//we have the trade in the blotter
                        continue;
                    }
                    //if we have the trade as buy and it is short, we must cover first
                    if (allPositions.Any(t => t.Side == Strings.Side.Short && t.Ticker == signal.Ticker))
                    {
                        var shortPosition = allPositions.Where(t => t.Side == Strings.Side.Short && t.Ticker == signal.Ticker).First();
                        var coverfirstSignal = tsService.NewCoverSignal(shortPosition.Ticker, shortPosition.CurrentPrice, shortPosition.Shares);//we need a cover signal
                        if(coverfirstSignal == null)
                        {
                            continue;
                        } 
                        closedPositions.Add(shortPosition);                       
                        closeSignals.Add(coverfirstSignal); //lets Cover the stock                         
                    }                    
                    newPositions.Add(NewBuy(signal));//lets make the trade
                    newTradehistoryEntries.Add(NewLongPosition(signal));
                    continue;
                }
                if (signal.Action == Strings.Opinions.Sell)
                {
                    //if the current trade is a buy and the trade signal is sell, we must sell the stock at this price
                    if (allPositions.Any(t => t.Side == Strings.Side.Long && t.Ticker == signal.Ticker))
                    {
                        //lets sell the stock
                        var longPosition = allPositions.Where(t => t.Side == Strings.Side.Long && t.Ticker == signal.Ticker).First();
                        closedPositions.Add(longPosition);
                        signal.Shares = longPosition.Shares;
                        signal.UpdateValue();
                        closeSignals.Add(signal);//in order to sell the stock we must keep the trade signal 
                        continue;
                    }
                    ignored.Add(signal);  //its a sell, we dont have it in the blotter, so we remove it
                }
                if (signal.Action == Strings.Opinions.Short)
                {
                    if (IsSameLosingTradeMoreThanMax(completed, signal)) //this is a losing reentry, ignore it
                    {
                        ignored.Add(signal);
                        continue;
                    }
                    //if the current trade is a Short and the trade signal is Short, ignore the trade and remove it
                    if (allPositions.Any(t => t.Side == Strings.Side.Short && t.Ticker == signal.Ticker))
                    {                        
                        ignored.Add(signal);//remove the new trade we dont want to save it we have it already
                        continue;
                    }
                    //if the current position is a long and the signal is a short we must sell it first 
                    if (allPositions.Any(t => t.Side == Strings.Side.Long && t.Ticker == signal.Ticker))
                    {
                        //lets Sell the stock first
                        var longPosition = allPositions.Where(t => t.Side == Strings.Side.Long && t.Ticker == signal.Ticker).First();
                        
                        var sellfirstSignal = tsService.NewSellSignal(longPosition.Ticker, longPosition.CurrentPrice, longPosition.Shares);//we need a cover signal 
                        if(sellfirstSignal == null)
                        {
                            continue;
                        }
                        closedPositions.Add(longPosition);
                        closeSignals.Add(sellfirstSignal); //lets sell the stock
                    }
                    //its a new trade we dont have it, lets make the trade
                    newPositions.Add(NewShort(signal));
                    newTradehistoryEntries.Add(NewShortPosition(signal));
                    continue;
                }
                if (signal.Action == Strings.Opinions.Cover)
                {
                    //if the current trade is a short and the trade signal is Cover, we must Cover the stock at this price
                    if (allPositions.Any(t => t.Side == Strings.Side.Short && t.Ticker == signal.Ticker))
                    {
                        //lets Cover the stock
                        var shortPosition = allPositions.Where(t => t.Side == Strings.Side.Short && t.Ticker == signal.Ticker).First();
                        closedPositions.Add(shortPosition);
                        signal.Shares = shortPosition.Shares;
                        signal.UpdateValue();
                        closeSignals.Add(signal);//in order to Cover the stock we must keep the trade signal 
                        continue;
                    }
                    ignored.Add(signal);  //its a Cover, we dont have it in the blotter, so we remove it
                }
            } 
            tsService.DeleteNewTradeSignals(ignored); //remove the ignored signals from the new trade signals 
            AddNewEntriesToBlotter(newPositions);//make the new buys, these are added to the blotter
            AddNewEntriesToTradeBlotterHistory(newTradehistoryEntries);//we need to add these trades to the tradeblotter history
            CloseTradeInBlotter(closedPositions);//now we need to sell the longs, which means we sell the trades in the blotter by removing them
            CloseTradesInTradeHistory(closeSignals);//then we update the tradehistoryentry for this ticker and close it out
        }
        /// <summary>
        /// Creates a new buy for the blotter using the signal
        /// </summary>
        /// <param name="longSignal"></param>
        /// <returns></returns>
        public BlotterEntry NewBuy(TradeSignal longSignal)
        {
            BlotterEntry newBuy = new BlotterEntry
            {
                Date = longSignal.Date,
                Side = Strings.Side.Long,
                Ticker = longSignal.Ticker,
                PurchasePrice = longSignal.Price,
                Shares = longSignal.Shares,
                StopPrice = longSignal.StopPrice,
                TargetPrice = longSignal.TargetPrice,
                Strategy = Strings.Strategies.IntradayMomentumLong,
                TradeId = longSignal.TradeId,
                TradingSettings = CurrentSettings.ID
            };
            newBuy.CalculateOpenValue();
            return newBuy;
        }
        /// <summary>
        /// Creates a new short for the blotter using the signal
        /// </summary>
        /// <param name="shortSignal"></param>
        /// <returns></returns>
        public static BlotterEntry NewShort(TradeSignal shortSignal)
        {
            BlotterEntry newShort = new BlotterEntry
            {
                Date = shortSignal.Date,
                Side = Strings.Side.Short,
                Ticker = shortSignal.Ticker,
                PurchasePrice = shortSignal.Price,
                Shares = shortSignal.Shares,
                StopPrice = shortSignal.StopPrice,
                TargetPrice = shortSignal.TargetPrice,
                Strategy = Strings.Strategies.IntradayMomentumShort,
                TradeId = shortSignal.TradeId,
                TradingSettings = shortSignal.TradingSettings
            };
            newShort.CalculateOpenValue();
            return newShort;
        }
        /// <summary>
        /// Creates a new long trade history entry from the long signal
        /// </summary>
        /// <param name="longSignal"></param>
        /// <returns></returns>
        public static TradeHistoryEntry NewLongPosition(TradeSignal longSignal)
        {
            TradeHistoryEntry newBuy = new TradeHistoryEntry
            {
                TradeIdOpen = longSignal.TradeId,
                OpenAction = longSignal.Action,
                OpenDate = longSignal.Date,
                OpenDetails = longSignal.Details,
                OpenPrice = longSignal.Price,
                OpenValue = longSignal.Value,
                Shares = longSignal.Shares,
                Side = Strings.Side.Long,
                Strategy = longSignal.Strategy,
                Ticker = longSignal.Ticker,
                TradingSettings = longSignal.TradingSettings
            };
            return newBuy;
        }
        /// <summary>
        /// Creates a new short trade history entry from the long signal
        /// </summary>
        /// <param name="longSignal"></param>
        /// <returns></returns>
        public static TradeHistoryEntry NewShortPosition(TradeSignal shortSignal)
        {
            TradeHistoryEntry newShort = new TradeHistoryEntry
            {
                TradeIdOpen = shortSignal.TradeId,
                OpenAction = shortSignal.Action,
                OpenDate = shortSignal.Date,
                OpenDetails = shortSignal.Details,
                OpenPrice = shortSignal.Price,
                OpenValue = shortSignal.Value,
                Shares = shortSignal.Shares,
                Side = Strings.Side.Short,
                Strategy = shortSignal.Strategy,
                Ticker = shortSignal.Ticker,
                TradingSettings = shortSignal.TradingSettings

            };
            return newShort;
        }
        /// <summary>
        /// Finds all the blotter entries and updates the current data
        /// </summary>
        public void UpdateBlotterData()
        {
            var blotterTrades = db.TradeBlotter.ToList();
            var todaysData = mdService.GetDataOn(mdService.StartOn);
            foreach(var openTrade in blotterTrades)
            {
                if(!todaysData.Any(t=>t.Ticker == openTrade.Ticker))//todo
                {
                    Logger.WriteLine(MessageType.Error, "No market data for blotter position " + openTrade.Ticker);
                    continue; //we need to get this data
                }
                //get this data point and update the blotter entries current price
                var marketData= todaysData.Where(t => t.Ticker == openTrade.Ticker).First();
                openTrade.AvgVol = marketData.AvgVol;
                openTrade.CurrentPrice = marketData.Price;
                openTrade.Date = marketData.Date;
                openTrade.DayHigh = marketData.DayHigh;
                openTrade.DayLow = marketData.DayLow;
                openTrade.YearHigh = marketData.YearHigh;
                openTrade.YearLow = marketData.YearLow;
                openTrade.PriceChange = marketData.PriceChange;
                openTrade.PriceChangePcnt = marketData.PriceChangePcnt;
                openTrade.Volume = marketData.Volume;
                openTrade.VolumeChange = marketData.VolumeChange;
                openTrade.AvgVol = marketData.AvgVol;
                openTrade.CalculateGainLoss();
            }
            Update(blotterTrades);
            UpdateOpenTradeData();
            IsBlotterMaxed();
        }

        /// <summary>
        /// Finds all the open entries and updates the current data
        /// </summary>
        public void UpdateOpenTradeData()
        {
            var openTrades = GetOpenTrades(mdService.StartOn).Where(t => t.State == Strings.Opinions.Open).ToList();
            var todaysData = mdService.GetDataOn(mdService.StartOn);
            foreach (var openTrade in openTrades)
            {
                if (!todaysData.Any(t => t.Ticker == openTrade.Ticker))//todo
                {
                    continue; //we need to get this data
                }
                //get this data point and update the blotter entries current price
                var marketData = todaysData.Where(t => t.Ticker == openTrade.Ticker).First();
                openTrade.ClosePrice = marketData.Price;
                openTrade.CloseDate = DateTime.Now;
                openTrade.CloseValue = openTrade.Shares * marketData.Price;
                openTrade.CalculateGainLoss();
            }
            Update(openTrades);
        }
        /// <summary>
        /// Updates the blotter history trades
        /// </summary>
        public void UpdateTradeHistoryPrices()
        {
            var openTrades = db.TradeHistory.Where(t => string.IsNullOrEmpty(t.CloseAction)).ToList();
            var todaysData = mdService.GetDataOn(mdService.StartOn);
            foreach (var openTrade in openTrades)
            {
                if (!todaysData.Any(t => t.Ticker == openTrade.Ticker) )//todo
                {
                    continue; //we need to get this data
                }
                //get this data point and update the blotter entries current price
                var marketData = todaysData.Where(t => t.Ticker == openTrade.Ticker).First();
                openTrade.ClosePrice = marketData.Price; 
            }
            Update(openTrades);
        }
        /// <summary>
      /// Updates the blotter entries
      /// </summary>
      /// <param name="blotterData"></param>
        public void Update(List<BlotterEntry> blotterData)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            foreach (var datapoint in blotterData)
            {
                db.Entry(datapoint).State = EntityState.Modified;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Updates the Trade History Entry
        /// </summary>
        /// <param name="data"></param>
        public void Update(List<TradeHistoryEntry> data)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            foreach (var datapoint in data)
            {
                db.Entry(datapoint).State = EntityState.Modified;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Adds the new buys to the blotter entry        
        /// </summary>
        /// <param name="newTrades"></param>
        public void AddNewEntriesToBlotter(List<BlotterEntry> newTrades)
        {
            foreach(var newTrade in newTrades)
            {
                db.TradeBlotter.Add(newTrade);
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Adds the new entry to the trade history blotter        
        /// </summary>
        /// <param name="newTrades"></param>
        public void AddNewEntriesToTradeBlotterHistory(List<TradeHistoryEntry> newTrades)
        {
            foreach (var newTrade in newTrades)
            {
                db.TradeHistory.Add(newTrade);
            }
            db.SaveChanges();
        }
        /// <summary>
        /// For each entry that we are closing, we remove from the blotter that is currently open
        /// </summary>
        /// <param name="closedEntry"></param>
        public void CloseTradeInBlotter(List<BlotterEntry> closedEntry)
        {
            foreach (var closed in closedEntry)
            {
                db.Entry(closed).State = EntityState.Deleted;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// This resets the blotter to no trades
        /// </summary>
        public void ClearBlotter()
        {
            var longs = db.TradeBlotter.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumLong || t.Strategy == Strings.Strategies.IntradayMomentumShort);
            foreach (var closed in longs)
            {
                db.Entry(closed).State = EntityState.Deleted;
            }
            
            db.SaveChanges();
        }
        /// <summary>
        /// This clears the trade history
        /// </summary>
        public void ClearTradeHistory()
        {
             foreach (var closed in db.TradeHistory)
            {
                db.Entry(closed).State = EntityState.Deleted;
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Tries to find the blotter entry in the history and then updates the closing details
        /// </summary>
        /// <param name="closeSignal"></param>
        public void CloseTradeInTradeHistory(TradeSignal closeSignal)
        {
            //lets find this open position in the trade history
            //if we don't have it then we have a problem
            if (!db.TradeHistory.Any(t => t.Ticker == closeSignal.Ticker && string.IsNullOrEmpty(t.CloseAction)))
            {
                //no matching trade
                Logger.WriteLine(MessageType.Warning, "Could not find trade in history blotter for " + closeSignal.Ticker);
                return;
            }
            //get this open trade
            var trade = db.TradeHistory.Where(t => t.Ticker == closeSignal.Ticker && string.IsNullOrEmpty(t.CloseAction)).First();
            //lets update the details of this entry and save it
            trade.TradeIdClose = closeSignal.TradeId; //new id for the close
            trade.CloseAction = closeSignal.Action;//we close the long
            trade.CloseDate = closeSignal.Date;
            trade.CloseDetails = closeSignal.Details;
            trade.ClosePrice = closeSignal.Price;
            trade.CloseValue = closeSignal.Price * closeSignal.Shares;
            trade.CalculateGainLoss();//we need to do some math here for the price and value
            trade.MarkClosed();
            //we update the trade blotter history with the pnl here 
            db.Entry(trade).State = EntityState.Modified;  
            db.SaveChanges();
        }
        /// <summary>
        /// Tries to find each blotter entry in the history and then updates the closing details
        /// </summary>
        /// <param name="closeSignals"></param>
        public void CloseTradesInTradeHistory(List<TradeSignal> closeSignals)
        {
            closeSignals.ForEach(t => CloseTradeInTradeHistory(t));
        }
        /// <summary>
        /// Checks the positions in the blotter for stops that hit.
        /// If they are hit, the signal is added to the newtrade signals, 
        /// the history and the blotter is updated with the trades
        /// </summary>
        public void UpdateStops()
        {
            //we need to check all the open blotter entries
            //get all the open trades in teh blotter
            var currentLongs = db.TradeBlotter.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumLong);
            List<BlotterEntry> soldLongs = new List<BlotterEntry>();
            List<TradeSignal> sellSignals = new List<TradeSignal>();
            foreach (var entry in currentLongs)
            {
                //get the signal
                var signal = tsService.GetStopSignal(entry.CurrentPrice, entry.StopPrice, entry.Ticker,true);
                if(signal.Action != Strings.Opinions.NoOpinion)
                {
                    //lets sell the stock 
                    signal.Shares = entry.Shares;
                    signal.Price = signal.Price;
                    signal.UpdateValue(); 
                    sellSignals.Add(signal);
                    soldLongs.Add(entry);//get this entry for deletion
                }
            }
            //each one of these is a sell 
            tsService.AddNewTradeSignals(sellSignals);//now we need to add these new signals to the newtradesignals
            CloseTradeInBlotter(soldLongs);//now we need to sell the longs, which means we sell the trades in the blotter by removing them
            CloseTradesInTradeHistory(sellSignals);//then we update the tradehistoryentry for this ticker and close it out 

            var currentShorts = db.TradeBlotter.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumShort);
            List<BlotterEntry> coveredShorts = new List<BlotterEntry>();
            List<TradeSignal> coverSignals = new List<TradeSignal>();

            foreach (var entry in currentShorts)
            {
                //get the signal
                var signal = tsService.GetStopSignal(entry.CurrentPrice, entry.StopPrice, entry.Ticker, false);
                if (signal.Action != Strings.Opinions.NoOpinion)
                {
                    signal.Shares = entry.Shares;
                    signal.Price = signal.Price;
                    signal.UpdateValue(); 
                    coverSignals.Add(signal);
                    coveredShorts.Add(entry);
                }
            }
            //each one of these is a cover 
            tsService.AddNewTradeSignals(coverSignals);//now we need to add these new signals to the newtradesignals
            CloseTradeInBlotter(coveredShorts);//now we need to sell the longs, which means we sell the trades in the blotter by removing them
            CloseTradesInTradeHistory(coverSignals);//then we update the tradehistoryentry for this ticker and close it out
        }
       /// <summary>
       /// Checks the positions in the blotter if the stops hit
       /// If they are hit, the signal is added to the new trade signals,
       /// the history and the blotter is updated with the trades
       /// </summary>
        public void UpdateTargets()
        {
            //we need to check all the open blotter entries
            //get all the open trades in the blotter
            var currentLongs = db.TradeBlotter.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumLong);
            List<BlotterEntry> soldLongs = new List<BlotterEntry>();
            List<TradeSignal> sellSignals = new List<TradeSignal>();
            List<TradeSignal> newEntries = new List<TradeSignal>();
            List<BlotterEntry> newPositions = new List<BlotterEntry>();
            List<TradeHistoryEntry> newTradehistoryEntries = new List<TradeHistoryEntry>();
            foreach (var entry in currentLongs)
            {
                //get the signal
                var signal = tsService.GetTargetSignal(entry.CurrentPrice, entry.TargetPrice, entry.Ticker, true);
                if (signal.Action != Strings.Opinions.NoOpinion)//we hit the target
                {
                    signal.Shares = entry.Shares;
                    signal.Price = signal.Price;
                    signal.UpdateValue(); 
                    sellSignals.Add(signal);
                    soldLongs.Add(entry);
                    if (CurrentSettings.ReEnterWinningTrades && !BlotterIsMaxed && !DateHelper.IsThreeFortyFive())
                    {
                        var newLong = tsService.NewBuySignal(signal.Ticker, entry.CurrentPrice);  //now we must re-enter the trade, it was a winner
                         if(newLong == null)
                         {
                             continue;
                         }
                        newEntries.Add(newLong);
                        newPositions.Add(NewBuy(newLong));
                        newTradehistoryEntries.Add(NewLongPosition(newLong));
                    } 
                }
            }
            
            tsService.AddNewTradeSignals(sellSignals);//now we need to add these new signals to the newtradesignals
            CloseTradeInBlotter(soldLongs);//now we need to sell the longs, which means we sell the trades in the blotter by removing them
            CloseTradesInTradeHistory(sellSignals);//then we update the tradehistoryentry for this ticker and close it out 
            
             
            //make the tades that hit the target
            var currentShorts = db.TradeBlotter.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumShort);
            List<BlotterEntry> coveredShorts = new List<BlotterEntry>();
            List<TradeSignal> coverSignals = new List<TradeSignal>(); 
            foreach (var entry in currentShorts)
            {
                //get the signal
                var signal = tsService.GetTargetSignal(entry.CurrentPrice, entry.TargetPrice, entry.Ticker, false);
                if (signal.Action != Strings.Opinions.NoOpinion)
                {
                    signal.Shares = entry.Shares;
                    signal.Price = signal.Price;
                    signal.UpdateValue(); 
                    coverSignals.Add(signal);
                    coveredShorts.Add(entry);
                    if (CurrentSettings.ReEnterWinningTrades && !BlotterIsMaxed && !DateHelper.IsThreeFortyFive()) //now we must re-enter the trade, it was a winner
                    {
                        var newShort = tsService.NewShortSignal(signal.Ticker, entry.CurrentPrice); 
                        if(newShort == null)
                        {
                            continue;
                        }
                        newEntries.Add(newShort);
                        newPositions.Add(NewShort(newShort));
                        newTradehistoryEntries.Add(NewShortPosition(newShort));
                    }
                }
            }
            //each one of these is a cover 
            tsService.AddNewTradeSignals(coverSignals);//now we need to add these new signals to the newtradesignals
            CloseTradeInBlotter(coveredShorts);//now we need to sell the longs, which means we sell the trades in the blotter by removing them
            CloseTradesInTradeHistory(coverSignals);//then we update the tradehistoryentry for this ticker and close it out
            if (CurrentSettings.ReEnterWinningTrades && !BlotterIsMaxed) //now we must re-enter the trade, it was a winner
            {
                tsService.AddNewTradeSignals(newEntries); //add the rentries here
                AddNewEntriesToBlotter(newPositions);//make the new buys, these are added to the blotter
                AddNewEntriesToTradeBlotterHistory(newTradehistoryEntries);//we need to add these trades to the tradeblotter history
            }
        }
        /// <summary>
        /// Retrieves all the trades inthe blotter
        /// </summary>
        /// <returns></returns>
        public List<BlotterEntry> GetCurrentBlotter()
        {
            return db.TradeBlotter.ToList();
        }
        /// <summary>
        /// Gets all the market data again from yahoo then updates the blotters prices
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
           var fetch= await mdService.RefreshTodaysPortfolioData();
           UpdateBlotterData();
        }
        public List<TradeHistoryEntry> GetHistoricalTrades()
        {
            return db.TradeHistory.OrderByDescending(t => t.OpenDate).ThenBy(t => t.Strategy).ToList();
        }
        public void ResetNewSignals()
        {
            tsService.ResetNewSignals();
        }
        /// <summary>
        /// Does not remove trades from the blotter, Fetches latest prices, then marks the prices and then closes open positions in history
        /// </summary>
        public async Task CloseOutTradeHistoryPositions()
        {
            ResetNewSignals();
            List<TradeSignal> closeSignals = new List<TradeSignal>();
            var fetch = await mdService.RefreshTodaysPortfolioData();
            UpdateBlotterData();
            UpdateStops();
            var blotterPositions = GetCurrentBlotter();
            var openTrades = db.TradeHistory.Where(t => string.IsNullOrEmpty(t.CloseAction)).ToList();
            var todaysData = mdService.GetDataOn(mdService.StartOn);
            foreach (var openTrade in openTrades)
            {
                openTrade.CloseDate = DateTime.Now;
                openTrade.CloseAction = Strings.Opinions.Cover;
              
                if (openTrade.OpenAction == Strings.Opinions.Buy)
                {
                    //we must sell it
                    openTrade.CloseAction = Strings.Opinions.Sell;
                }  
                if(!todaysData.Any(t => t.Ticker == openTrade.Ticker))
                {
                    //this is a problem
                    Logger.WriteLine(MessageType.Error, "Could not get close price for " + openTrade.Ticker);
                    continue;
                }
                var marketData = todaysData.Where(t => t.Ticker == openTrade.Ticker).First();
                openTrade.ClosePrice = marketData.Price;//normal close of day price
                openTrade.CloseDetails = "Closing Position";
                openTrade.CloseValue = openTrade.Shares * marketData.Price;
                openTrade.MarkClosed();
                openTrade.CalculateGainLoss();
                TradeSignal closeSignal;
                if(openTrade.CloseAction == Strings.Opinions.Sell) //add the signal
                {
                    closeSignal = tsService.NewSellSignal(openTrade.Ticker, openTrade.ClosePrice, openTrade.Shares); 
                }
                else
                {
                    closeSignal = tsService.NewCoverSignal(openTrade.Ticker, openTrade.ClosePrice, openTrade.Shares);
                }
                closeSignal.Details = closeSignal.Details + " CLOSED OUT";
                openTrade.TradeIdClose = closeSignal.TradeId;
                closeSignals.Add(closeSignal);
            }
           Update(openTrades);
           tsService.AddNewTradeSignals(closeSignals); //we need the close signals here
           UpateTradeSignalHistory();
           mdService.UpdatePricingFetchSiteAudit();
           mdService.UpdateSignalFetchSiteAudit();
        }
        /// <summary>
        /// Gets the trades for today that have been completed
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<TradeHistoryEntry> GetCompletedTrades(DateTime onDate)
        {  
            return db.TradeHistory.Where(t => t.CloseDate.Day == onDate.Day && t.CloseDate.Year== onDate.Year && t.CloseDate.Month == onDate.Month && t.State == Strings.Opinions.Closed).ToList(); 

        }

        /// <summary>
        /// Gets the trades for today that have been completed
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<TradeHistoryEntry> GetOpenTrades(DateTime onDate)
        {
            return db.TradeHistory.Where(t => t.OpenDate.Day == onDate.Day && t.OpenDate.Year == onDate.Year && t.OpenDate.Month == onDate.Month && t.State == Strings.Opinions.Open).ToList();

        }
        /// <summary>
        /// Gets the last open trade from the trade history
        /// </summary>
        /// <returns></returns>
        public DateTime LastSignalRun()
        {
            return db.TradeHistory.Max(t => t.OpenDate);
        }
        /// <summary>
        /// Maunally updates the incorrect trade values in the trade history
        /// </summary>
        public void UpdateClosedValues()
        {
            //get the market data
            var openTrades = db.TradeHistory.Where(t => t.CloseValue == 0 && t.ClosePrice !=0 &&  !string.IsNullOrEmpty(t.CloseAction)).ToList();
            foreach (var openTrade in openTrades)
            {  
                openTrade.ClosePrice = openTrade.ClosePrice;
                openTrade.CloseValue = openTrade.Shares * openTrade.ClosePrice;
                openTrade.CalculateGainLoss();
            }
            Update(openTrades);
        }
        /// <summary>
        /// Checks to see if we have made this trade already and it was a loser 
        /// </summary>
        /// <returns></returns>
        public bool IsLosingDoubleDip(TradeSignal signal)
        {
            var completed = GetCompletedTrades(mdService.StartOn);
           
            if(completed.Any(t=>t.Ticker == signal.Ticker && t.GainLoss <0))
                return true;
            return false;
        }
        /// <summary>
        ///  We can dip three times but only if its a winner
        /// </summary>
        /// <param name="completed"></param>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool IsQuadrupleDipOrLosingTripleDip(List<TradeHistoryEntry> completed, string ticker)
        {
            if (completed.Any(t => t.Ticker == ticker))
            {
                var completedTrades = completed.Where(t => t.Ticker == ticker).OrderBy(t=>t.CloseDate).ToList();
                
                
                if (completedTrades.Count() >= 3) //no more than three
                {
                    return true;
                }
                foreach(var trade in completedTrades)//we can dip three times but only if its a winner
                {
                    if(trade.GainLoss < 0) //if we have a loss for this trade don't go again
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// if the last trade was a losing trade, and this trade is the same side, we can't do this
        /// </summary>
        /// <param name="completed"></param>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool IsLosingReEntry(List<TradeHistoryEntry> completed, TradeSignal newsignal)
        {
            if (completed.Any(t => t.Ticker == newsignal.Ticker))
            {
                var lastTradeCompleted = completed.Where(t => t.Ticker == newsignal.Ticker).OrderByDescending(t => t.CloseDate).First();
                //get the last trade
                if(lastTradeCompleted.GainLoss < 0)
                {
                    //if the last trade was a cover, and this trade is a short, we can't do this
                    if((lastTradeCompleted.CloseAction == Strings.Opinions.Cover &&  newsignal.Action== Strings.Opinions.Short) ||
                     (lastTradeCompleted.CloseAction == Strings.Opinions.Sell && newsignal.Action == Strings.Opinions.Buy))
                    {
                        return true;
                    }
                } 
            }
            return false;
        }
        /// <summary>
        ///if the price now is higher than the price  on losing trade we can only go long
        ///if the price change now is less than the price on losing trade we can only go short
        /// </summary>
        /// <param name="completed"></param>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool IsSameWayLosingReEntry(List<TradeHistoryEntry> completed, TradeSignal newsignal)
        {
            if (completed.Any(t => t.Ticker == newsignal.Ticker))
            {
                //if we lost money on the last two trades we can't enter again
                var lastSetOfTrades = completed.Where(t => t.Ticker == newsignal.Ticker).OrderByDescending(t => t.CloseDate).Take(2);
                var lastTradeCompleted = completed.Where(t => t.Ticker == newsignal.Ticker).OrderByDescending(t => t.CloseDate).First(); //get the last trade               
                if (lastTradeCompleted.GainLoss < 0) //last trade was a loser
                {  
                    var priceDifference = newsignal.Price - lastTradeCompleted.ClosePrice;
                    if(lastTradeCompleted.OpenAction == Strings.Opinions.Short) //last trade was a short
                    {
                        if(newsignal.Action == Strings.Opinions.Short) //this trade is a short
                        {
                            //we can't short again if the price now is MORE than the closing price on losing trade
                            return (priceDifference > 0);
                        }   //we can go long,last trade was a short, we can always reverse                       
                    }
                    if(lastTradeCompleted.OpenAction == Strings.Opinions.Buy) //last trade was a buy
                    {
                        if (newsignal.Action == Strings.Opinions.Buy) //this trade is a buy
                        {
                            //we can't buy again if the price now is LESS than the closing price on losing trade
                            return (priceDifference < 0);
                        }   //we can go short,last trade was a long, we can always reverse
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Can't lose money more than the maximum amount on the same side
        /// </summary>
        /// <param name="completed"></param>
        /// <param name="newsignal"></param>
        /// <returns></returns>
        public bool IsSameLosingTradeMoreThanMax(List<TradeHistoryEntry> completed, TradeSignal newsignal)
        {
            /*can't lose money more than twice on the same side 
                we can continue to go long if we made money last trade
                we can continue to go short if we made money last trade
                we can always reverse */
            if (completed.Any(t => t.Ticker == newsignal.Ticker))
            {
                var lastTradeCompleted = completed.Where(t => t.Ticker == newsignal.Ticker).OrderByDescending(t => t.CloseDate).First(); //get the last trade
                if(lastTradeCompleted.OpenAction == newsignal.Action)
                {
                    //make sure last trade, and if there was one before that were winners
                    var lastTwoTrades =completed.Where(t => t.Ticker == newsignal.Ticker).OrderByDescending(t => t.CloseDate).Take(CurrentSettings.SameLosingTradeMaximum);
                    if (lastTwoTrades.Count() == CurrentSettings.SameLosingTradeMaximum)
                    {
                        return lastTwoTrades.Where(t => t.GainLoss < 0).Count() == CurrentSettings.SameLosingTradeMaximum;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checks all the positions in the blotter to see if they are maxed
        /// </summary>
        public bool IsBlotterMaxed()
        {
            
            var longs = GetCurrentBlotter().Where(t => t.Side == Strings.Side.Long).ToList();
            var shorts = GetCurrentBlotter().Where(t => t.Side == Strings.Side.Short).ToList();
            double currentLongValue = 0.0;
            double currentShortValue = 0.0;
            longs.ForEach(t => currentLongValue += t.CurrentValue);
            shorts.ForEach(t => currentShortValue += t.CurrentValue);
            var totalValue = currentLongValue + currentShortValue;
            BlotterIsMaxed = (totalValue > CurrentSettings.MaxBlotterValue);
            return BlotterIsMaxed;
        }
    }
}