using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HedgefundMe.com;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class TradeSignalService
    {
       
        private TradingSettings currentSettings;
        private readonly ProjectEntities db;
        private readonly MarketDataService mdService;
        private readonly TradingSettingsService tsettings;
        public TradeSignalService(ProjectEntities context, MarketDataService mdservice)
        {    
               db = context;
               mdService = mdservice;
               tsettings = new TradingSettingsService(db);
               currentSettings = tsettings.GetCurrent();
        }
        /// <summary>
        /// Clears the existing new signals, gets new signals and updates the new signals in the new signals table
        /// </summary>
        public void RunNewSignals()
        {
            ResetNewSignals();//we must delete the new signals 
            var allsignals = new List<TradeSignal>(); 
            //filter the signals here
            allsignals.AddRange(GetSignals()); 
            AddNewTradeSignals(allsignals);  //now we update the database
        }
         
        /// <summary>
        /// Clears out any existing trade signals
        /// </summary>
        public void ResetNewSignals()
        {
            ClearNewTradeSignals(Strings.Strategies.IntradayMomentumLong);
            ClearNewTradeSignals(Strings.Strategies.IntradayMomentumShort);
        } 
        /// <summary>
        /// Gets signals both long and short signals and filters for min volume
        /// </summary>
        /// <returns></returns>
        public List<TradeSignal> GetSignals()
        {
            //loop over all the longs for today and get the signals
            List<TradeSignal> allSignals = new List<TradeSignal>();
            List<MarketData> alldata = new List<MarketData>();
            alldata.AddRange(mdService.GetDataOn(mdService.StartOn, Strings.Side.Long));
            alldata.AddRange(mdService.GetDataOn(mdService.StartOn, Strings.Side.Short));
            alldata = alldata.OrderByDescending(t => t.VolumeChange).ToList();//order by the highest avg vol%
            //loop over all the data for today and get the signals 
            foreach (var datapoint in alldata)
            {
                OpinionParameter r = new OpinionParameter(datapoint);
                r.GainChange = GetGainChange(datapoint.PriceChangePcnt);
               // r.RankChange = GetRankChange(datapoint.RankChange);
                r.VolumeChange = GetVolumeChange(datapoint.VolumeChange);
                var signal = GetTradeSignal(r);
                if (signal == Strings.Opinions.NoOpinion)
                {
                    continue;
                }
                if (signal == Strings.Opinions.Buy)
                {
                    if(datapoint.Volume < currentSettings.MinimumLongVolume) //filter longs
                    {
                        continue;
                    }
                    var stop = GetStopPrice(datapoint, true);
                    var target = GetTargetPrice(datapoint, true);
                    var shares = FinanceHelper.GetShareAmount(currentSettings.DollarAmount, datapoint.Price);
                    if (datapoint.Price == 0 || shares == 0)
                    {
                        Logger.WriteLine(MessageType.Error, datapoint.Ticker + " has a price of 0");
                        continue;
                    }
                    var value = shares * datapoint.Price;
                    allSignals.Add(new TradeSignal
                    {
                        Action = signal,
                        Date = DateTime.Now,
                        Price = datapoint.Price,
                        Ticker = datapoint.Ticker,
                        Strategy = Strings.Strategies.IntradayMomentumLong,
                        Shares = shares,
                        Value = value,
                        StopPrice = stop,
                        TargetPrice = target,
                        TradeId = Guid.NewGuid().ToString(),
                        TradingSettings = currentSettings.ID,
                        Details = string.Format("{0} {5} Shares of {1} @ {2}, Target: {3}, Stop: {4}, Risk: {6}.", signal, datapoint.Ticker, datapoint.Price.ToString("C2"), target.ToString("C2"), stop.ToString("C2"),
                        shares, value.ToString("C2"))
                    });
                    continue;
                }
                if (signal == Strings.Opinions.Short)
                {
                    if (datapoint.Volume < currentSettings.MinimumShortVolume) //filter shorts
                    {
                        continue;
                    }
                    var stop = GetStopPrice(datapoint, false);
                    var target = GetTargetPrice(datapoint, false);
                    var shares = FinanceHelper.GetShareAmount(currentSettings.DollarAmount, datapoint.Price);
                    if (datapoint.Price == 0 || shares == 0)
                    {
                        Logger.WriteLine(MessageType.Error, datapoint.Ticker + " has a price of 0");
                        continue;
                    }
                    var value = shares * datapoint.Price;
                    allSignals.Add(new TradeSignal
                    {
                        Action = signal,
                        Date = DateTime.Now,
                        Price = datapoint.Price,
                        Ticker = datapoint.Ticker,
                        Strategy = Strings.Strategies.IntradayMomentumShort,
                        Shares = shares,
                        Value = value,
                        StopPrice = stop,
                        TargetPrice = target,
                        TradeId = Guid.NewGuid().ToString(),
                        TradingSettings = currentSettings.ID,
                        Details = string.Format("{0} {5} Shares of {1} @ {2}, Target: {3}, Stop: {4}, Risk: {6}.", signal, datapoint.Ticker, datapoint.Price.ToString("C2"), target.ToString("C2"), stop.ToString("C2"),
                        shares, value.ToString("C2"))
                    });
                    continue;
                }
            }
            return allSignals;
        }
        /// <summary>
        /// Creates a new buy signal
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="currentPrice"></param>
        /// <returns></returns>
        public TradeSignal NewBuySignal(string ticker, double currentPrice)
        {
            if(currentPrice == 0)
            {
                Logger.WriteLine(MessageType.Error, ticker + " has a price of 0");
                return null;
            }
            var shares = FinanceHelper.GetShareAmount(currentSettings.DollarAmount, currentPrice);
            var value = shares * currentPrice;
            var signal=  new TradeSignal
                    {
                        Action = Strings.Opinions.Buy,
                        Date = DateTime.Now,
                        Price = currentPrice,
                        Ticker =ticker,
                        Strategy = Strings.Strategies.IntradayMomentumLong,
                        Shares = shares,
                        Value = value,
                        StopPrice = GetStopPrice(currentPrice, true),
                        TargetPrice = GetTargetPrice(currentPrice, true),
                        TradeId = Guid.NewGuid().ToString(),
                        TradingSettings = currentSettings.ID,
                   };
            signal.UpdateValue();
            signal.Details = string.Format("{0} {5} Shares of {1} @ {2}, Target: {3}, Stop: {4}, Risk: {6}.", signal.Action, ticker, signal.Price.ToString("C2"), signal.TargetPrice.ToString("C2"), signal.StopPrice.ToString("C2"),
            shares, value.ToString("C2"));
            return signal;
        }
        /// <summary>
        /// Creates a new sell signal
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="currentPrice"></param>
        /// <returns></returns>
        public   TradeSignal NewShortSignal(string ticker, double currentPrice)
        {
            if (currentPrice == 0)
            {
                Logger.WriteLine(MessageType.Error, ticker + " has a price of 0");
                return null;
            }
            var shares = FinanceHelper.GetShareAmount(currentSettings.DollarAmount, currentPrice);
            var value = shares * currentPrice;
            var signal = new TradeSignal
            {
                Action = Strings.Opinions.Short,
                Date = DateTime.Now,
                Price = currentPrice,
                Ticker = ticker,
                Strategy = Strings.Strategies.IntradayMomentumShort,
                Shares = shares,
                Value = value,
                StopPrice = GetStopPrice(currentPrice, false),
                TargetPrice = GetTargetPrice(currentPrice, false),
                TradeId = Guid.NewGuid().ToString(),
                TradingSettings = currentSettings.ID
            };
            signal.UpdateValue();
            signal.Details = string.Format("{0} {5} Shares of {1} @ {2}, Target: {3}, Stop: {4}, Risk: {6}.", signal.Action, ticker, signal.Price.ToString("C2"), signal.TargetPrice.ToString("C2"), signal.StopPrice.ToString("C2"),
            shares, value.ToString("C2"));
            return signal;
        }
        /// <summary>
        /// Creates a cover signal at the current price
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="atPrice"></param>
        /// <returns></returns>
        public TradeSignal NewCoverSignal(string ticker, double atPrice, int shares)
        {
            if (atPrice == 0)
            {
                Logger.WriteLine(MessageType.Error, ticker + " has a price of 0");
                return null;
            }
            var value = shares * atPrice;
            var signal = new TradeSignal
            {
                Action = Strings.Opinions.Cover,
                Date = DateTime.Now,
                Price = atPrice,
                Ticker = ticker,
                Strategy = Strings.Strategies.IntradayMomentumShort,
                Shares = shares,
                Value = value,
                StopPrice = atPrice,
                TargetPrice = atPrice,
                TradeId = Guid.NewGuid().ToString(),
                TradingSettings = currentSettings.ID
            };
            signal.UpdateValue();
            signal.Details = string.Format("{0} {3} Shares of {1} @ {2}.", signal.Action, ticker, signal.Price.ToString("C2"),shares);
            return signal;
        }
        /// <summary>
        /// Creates a sell signal at the current price
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="atPrice"></param>
        /// <returns></returns>
        public TradeSignal NewSellSignal(string ticker, double atPrice, int shares)
        {
            if (atPrice == 0)
            {
                Logger.WriteLine(MessageType.Error, ticker + " has a price of 0");
                return null;
            }
            var value = shares * atPrice;
            var signal = new TradeSignal
            {
                Action = Strings.Opinions.Sell,
                Date = DateTime.Now,
                Price = atPrice,
                Ticker = ticker,
                Strategy = Strings.Strategies.IntradayMomentumLong,
                Shares = shares,
                Value = value,
                StopPrice = atPrice,
                TargetPrice = atPrice,
                TradeId = Guid.NewGuid().ToString(),
                TradingSettings = currentSettings.ID
            };
            signal.UpdateValue();
            signal.Details = string.Format("{0} {3} Shares of {1} @ {2}.", signal.Action, ticker, signal.Price.ToString("C2"), shares);
            return signal;
        }
        /// <summary>
        /// If the stock is long, we set a sellstop
        /// If the stock is short, we set a buystop
        /// </summary>
        /// <param name="stock"></param>
        public   double GetStopPrice(MarketData stock, bool isLong)
        {
            if(isLong)
            { 
                return   stock.Price + (stock.Price *  currentSettings.SellStopChange); //now 5% 
            }
            return stock.Price + (stock.Price * currentSettings.BuyStopChange);  
        }
        public   double GetStopPrice(double currentPrice, bool isLong)
        {
            if(isLong)
            {
                return currentPrice + (currentPrice * currentSettings.SellStopChange); //now 5% 
            }
            return currentPrice + (currentPrice * currentSettings.BuyStopChange);  
        }
        /// <summary>
        /// If the stock is long, we set a target price
        /// If the stock is short, we set a target price
        /// </summary>
        /// <param name="stock"></param>
        public   double GetTargetPrice(MarketData stock, bool isLong)
        {
            if (isLong)
            {
                return stock.Price + (stock.Price * currentSettings.TargetPriceChangeLong);
                
            }
            return stock.Price + (stock.Price * currentSettings.TargetPriceChangeShort);
             
        }
        public   double GetTargetPrice(double currentPrice, bool isLong)
        {
            if (isLong)
            {
                return currentPrice + (currentPrice * currentSettings.TargetPriceChangeLong);
                
            }
            return currentPrice + (currentPrice * currentSettings.TargetPriceChangeShort);
             
        }
        public  string GetRankChange(double rankChange )
        {
             
            if (rankChange == 0)
            {
                return Strings.Opinions.Even;
            }
            if (rankChange < 0)
            {
                return Strings.Opinions.Bad;
            }
            return Strings.Opinions.Good;
        }
        public  string GetGainChange(double priceChange)
        {
            if (priceChange <= currentSettings.BadPriceChange)
            {
                return Strings.Opinions.Bad;
            }
            if (priceChange >= currentSettings.GoodPriceChange)
            {
                return Strings.Opinions.Good;
            }
            return Strings.Opinions.Even;
        }
        public   string GetVolumeChange(double volumeChange)
        {
            if (volumeChange <= DateHelper.GetVolumeChangeTrigger(CurrentServer.IsLocal))
            {
                return Strings.Opinions.NoTrade;
            }           
            return Strings.Opinions.Trade;
        }
        /// <summary>
        /// Can return buy or short or no opinion on any stock using rank
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetSignal(OpinionParameter parameter)
        {
            if (IsNoTrade(parameter)) { return Strings.Opinions.NoOpinion; }
            switch (parameter.RankChange)
            {
                case Strings.Opinions.Bad:
                    {
                        #region currentgain
                        switch (parameter.GainChange)
                        {
                            case Strings.Opinions.Good://rank change Bad, GainChange Good
                            case Strings.Opinions.Even://rank change Bad, GainChange Even
                                {
                                    return Strings.Opinions.NoOpinion; //this could be buy
                                }
                            default: //rank change Bad, GainChange bad
                                {
                                    return Strings.Opinions.Short;
                                }
                        }
                        #endregion
                    }
                default: //rank change good
                    #region currentgain
                    switch (parameter.GainChange)
                    {
                        case Strings.Opinions.Good: //rank change good, GainChange Good
                            {
                                return Strings.Opinions.Buy;
                            }
                        default: //rank change good, GainChange even
                            {   //rank change good, GainChange bad
                                return Strings.Opinions.NoOpinion;
                            }
                    }
                    #endregion
            }
        }
        /// <summary>
        /// Can return buy or short or no opinion on any stock ignores Rank
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetTradeSignal(OpinionParameter parameter)
        {
            if (IsNoVolumeTrade(parameter)) { return Strings.Opinions.NoOpinion; }
            #region currentgain
            switch (parameter.GainChange)
            {
                case Strings.Opinions.Good: //GainChange Good
                    {
                         return Strings.Opinions.Buy;
                    }
                case Strings.Opinions.Even:// GainChange Even
                    {
                        return Strings.Opinions.NoOpinion; 
                    }
                default: // GainChange bad
                    {
                        return Strings.Opinions.Short;
                    }
            }
            #endregion
        }
        public static bool IsNoTrade(OpinionParameter parameter)
        {
            return (parameter.VolumeChange == Strings.Opinions.NoTrade || parameter.GainChange == Strings.Opinions.Even || parameter.RankChange == Strings.Opinions.Even);
        }
        public static bool IsNoVolumeTrade(OpinionParameter parameter)
        {
            return (parameter.VolumeChange == Strings.Opinions.NoTrade || parameter.GainChange == Strings.Opinions.Even );
        }
        /// <summary>
        /// Deletes any existing new trade signals
        /// </summary>
        public void ClearNewTradeSignals( )
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            foreach (var datapoint in db.NewTradeSignals)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Deletes any existing new trade signals for the strategy
        /// </summary>
        public void ClearNewTradeSignals(string strategy)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            var signals = db.NewTradeSignals.Where(t => t.Strategy == strategy);
            foreach (var datapoint in signals)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Deletes any existing new trade signals for the strategy
        /// </summary>
        public void DeleteNewTradeSignal(TradeSignal signal)
        {
            db.Configuration.AutoDetectChangesEnabled = false;            
            db.Entry(signal).State = EntityState.Deleted;             
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Deletes any existing new trade signals for the strategy
        /// </summary>
        public void DeleteNewTradeSignals(List<TradeSignal> signals)
        {
            db.Configuration.AutoDetectChangesEnabled = false; 
            foreach (var datapoint in signals)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Adds the new trade signals to the new trade signals table
        /// </summary>
        /// <param name="signals"></param>
        public void AddNewTradeSignals(List<TradeSignal> signals)
        {
            
            foreach (var signal in signals)
            {
                db.NewTradeSignals.Add(signal);
            }
            db.SaveChanges(); 
        }
        /// <summary>
        /// Deletes trade signal history
        /// </summary>
        public void DeleteTradeSignalHistory()
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            var signals = db.TradeSignalHistory.ToList();
            foreach (var datapoint in signals)
            {
                db.Entry(datapoint).State = EntityState.Deleted;
            }
            db.SaveChanges();
            db.ChangeTracker.DetectChanges();
        }
        /// <summary>
        /// Gets the historical signals
        /// </summary>
        /// <returns></returns>
        public List<TradeSignalHistory> GetTradeSignalHistory()
        {
            return db.TradeSignalHistory.OrderByDescending(t => t.Date).ThenBy(t => t.Strategy).ToList();
        }
        /// <summary>
        /// Gets the historical signals
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<TradeSignalHistory> GetTradeSignalHistory(DateTime onDate)
        {
            return db.TradeSignalHistory.Where(t=>t.Date== onDate).OrderByDescending(t => t.Strategy).ThenBy(t => t.Action).ToList();
        }
        /// <summary>
        /// Adds each new Trade Signal to the Trade Signal History table
        /// </summary>
        public void UpdateTradeSignalHistory()
        {
            //we then add them all to the history of signals
            //we need to make a tradesignal into a historical signal and add to the database
            foreach (var signal in db.NewTradeSignals)
            {
                db.TradeSignalHistory.Add(new TradeSignalHistory
                {
                    Action = signal.Action,
                    Date = signal.Date,
                    Details = signal.Details,
                    Price = signal.Price,
                    Shares = signal.Shares,
                    StopPrice = signal.StopPrice,
                    Strategy = signal.Strategy,
                    TargetPrice = signal.TargetPrice,
                    Ticker = signal.Ticker,
                    Value = signal.Value,
                    TradingSettings = signal.TradingSettings,
                    TradeId = signal.TradeId
                });
            }
            db.SaveChanges();
        }
       
        public void UpdateTradeSignalHistory(List<TradeSignal> newTradeSignals)
        {
            //we then add them all to the history of signals
            //we need to make a tradesignal into a historical signal and add to the database
            foreach (var signal in newTradeSignals)
            {
                db.TradeSignalHistory.Add(new TradeSignalHistory
                {
                    Action = signal.Action,
                    Date = signal.Date,
                    Details = signal.Details,
                    Price = signal.Price,
                    Shares = signal.Shares,
                    StopPrice = signal.StopPrice,
                    Strategy = signal.Strategy,
                    TargetPrice = signal.TargetPrice,
                    Ticker = signal.Ticker,
                    Value = signal.Value,
                    TradingSettings = signal.TradingSettings,
                    TradeId = signal.TradeId
                });
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Returns the new signlas for the strategy
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public List<TradeSignal> GetNewTradesFor(string strategy)
        {
            return db.NewTradeSignals.Where(t => t.Strategy == strategy).OrderBy(t=>t.Action).ToList();
        }
        /// <summary>
        /// Returns the new signlas for longs from the new trade signals table
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public List<TradeSignal> GetNewLongSignals()
        {
            return GetNewTradesFor(Strings.Strategies.IntradayMomentumLong);
        }
        /// <summary>
        /// Returns the new signlas for shorts from the new trade signals table
        /// </summary>
        /// <returns></returns>
        public List<TradeSignal> GetNewShortSignals()
        {
            return GetNewTradesFor(Strings.Strategies.IntradayMomentumShort);
        }
        /// <summary>
        /// Returns all the new signals for shorts and longs from the new trade signals table
        /// </summary>
        /// <returns></returns>
        public List<TradeSignal> GetNewSignals()
        {
            return db.NewTradeSignals.OrderBy(t=>t.Strategy).ThenBy(t=>t.Action).ToList();
        }
        /// <summary>
        /// Returns all the new long signls from the new trade signals table
        /// </summary>
        /// <returns></returns>
        public List<TradeSignal> GetOnlyLongNewSignals()
        {
            return db.NewTradeSignals.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumLong).OrderBy(t=>t.ID).ToList();
        }
        /// <summary>
        /// Returns all the new long signls from the new trade signals table
        /// </summary>
        /// <returns></returns>
        public List<TradeSignal> GetOnlyShortNewSignals()
        {
            return db.NewTradeSignals.Where(t => t.Strategy == Strings.Strategies.IntradayMomentumShort).OrderBy(t => t.ID).ToList();
        }
     
       /// <summary>
        ///  Checks the current price with the stop price, if triggered, a sell or cover signal is returned to close 
       /// </summary>
       /// <param name="currentPrice"></param>
       /// <param name="stopPrice"></param>
       /// <param name="ticker"></param>
       /// <param name="isLong"></param>
       /// <returns></returns>
        public TradeSignal GetStopSignal(double currentPrice, double stopPrice, string ticker, bool isLong)
        {
            //this will check if the stop is hit for a long
            //if it is hit then we return the trade signal to sell.
            //if it is not hit then there is no signal
            if(!HitSellStop(currentPrice,stopPrice, isLong))
            {
                return new TradeSignal { Action = Strings.Opinions.NoOpinion };
            }
            if (isLong)
            {
                return new TradeSignal
                {
                    Action = Strings.Opinions.Sell,
                    Date = DateTime.Now,
                    Price = stopPrice,
                    Ticker = ticker,
                    StopPrice = stopPrice,
                    Strategy = Strings.Strategies.IntradayMomentumLong,
                    TradeId = Guid.NewGuid().ToString(),
                    TradingSettings = currentSettings.ID,
                    Details = string.Format("{0} {1} @ {2}, Hit stop price.", Strings.Opinions.Sell, ticker, stopPrice.ToString("C2"))
                };
            }
            //we are short
            return new TradeSignal
            {
                Action = Strings.Opinions.Cover,
                Date = DateTime.Now,
                Price = stopPrice,
                Ticker = ticker,
                StopPrice = stopPrice,
                Strategy = Strings.Strategies.IntradayMomentumShort,
                TradeId = Guid.NewGuid().ToString(),
                TradingSettings = currentSettings.ID,
                Details = string.Format("{0} {1} @ {2}, Hit stop price @ {3}", Strings.Opinions.Cover, ticker, stopPrice.ToString("C2"), stopPrice.ToString("C2"))
            };
        }
        /// <summary>
        /// Checks if the stop is hit
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="stopPrice"></param>
        /// <returns></returns>
        public static bool HitSellStop(double currentPrice, double stopPrice, bool isLong)
        {
            if (isLong)
                return (currentPrice < stopPrice);
            return (currentPrice > stopPrice);
        }
        /// <summary>
        /// Checks the current price with the target price, if triggered, a sell signal is returned to sell 
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="targetPrice"></param>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public TradeSignal GetTargetSignal(double currentPrice, double targetPrice, string ticker, bool isLong)
        {
            //this will check if the stop is hit for a long
            //if it is hit then we return the trade signal to sell.
            //if it is not hit then there is no signal
            if (!HitTarget(currentPrice, targetPrice,isLong))
            {
                return new TradeSignal { Action = Strings.Opinions.NoOpinion };
            }
            if(isLong)
            {
                return new TradeSignal
                {
                    Action = Strings.Opinions.Sell,
                    Date = DateTime.Now,
                    Price = currentPrice,
                    Ticker = ticker,
                    TargetPrice = targetPrice,
                    Strategy = Strings.Strategies.IntradayMomentumLong,
                    TradeId = Guid.NewGuid().ToString(),
                    TradingSettings = currentSettings.ID,
                    Details = string.Format("{0} {1} @ {2}, Hit target price.", Strings.Opinions.Sell, ticker, currentPrice.ToString("C2"))
                };
            }
            //we are short
            return new TradeSignal
            {
                Action = Strings.Opinions.Cover,
                Date = DateTime.Now,
                Price = currentPrice,
                Ticker = ticker,
                TargetPrice = targetPrice,
                Strategy = Strings.Strategies.IntradayMomentumShort,
                TradeId = Guid.NewGuid().ToString(),
                TradingSettings = currentSettings.ID,
                Details = string.Format("{0} {1} @ {2}, Hit target price.", Strings.Opinions.Cover, ticker, currentPrice.ToString("C2"))
            };
           
        }
       /// <summary>
        /// Checks if the target is hit
       /// </summary>
       /// <param name="currentPrice"></param>
       /// <param name="targetPrice"></param>
       /// <param name="isLong"></param>
       /// <returns></returns>
        public static bool HitTarget(double currentPrice, double? targetPrice, bool isLong)
        {
            if(isLong)
                return (currentPrice > targetPrice);
            return (currentPrice < targetPrice);
        }
        /// <summary>
        /// Returns XML for the latest trades only
        /// </summary>
        /// <returns></returns>
        public string GetLatestLongTradesAsXml()
        {
            return SerializerService.SerializeTradeSignals(GetOnlyLongNewSignals());
        }
        /// <summary>
        /// Returns XML for the latest short trades only
        /// </summary>
        /// <returns></returns>
        public string GetLatestShortTradesAsXml()
        {
            return SerializerService.SerializeTradeSignals(GetOnlyShortNewSignals());
        }
    }
}