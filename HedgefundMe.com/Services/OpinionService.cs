using System;
using System.Web;
using System.Linq;
using HedgefundMe.com.Models;
using System.Collections.Generic;
using System.Data;

namespace HedgefundMe.com.Services
{
    public class OpinionService
    {
        public static double MinimumShortVolume = 100000;  
        public static int DollarAmount = 1000;  //ths dollar amount for the trade emails
        public static double TargetPriceChangeLong = 0.03; //put this in the datbase
        public static double TargetPriceChangeShort = -0.03; //put this in the datbase
        public static double SellStopChange = -0.03; //put this in the datbase
        public static double BuyStopChange = 0.03; //put this in the datbase
        public static double LargeSellStopChange = 0.8; //put this in the datbase
        public static double LargeBuyStopChange = -0.8; //put this in the datbase
        public static double BadPriceChange = -0.03; //put this in the databse
        //public static double VolumeChangeTrigger = 0.011; //put this in the databse
        public const string OpinionsFile = "~/App_Data/opinions.txt";
        public const string SellStopsFile = "~/App_Data/sellstops.txt";
        private readonly ProjectEntities db;
        StockService service;
       
        public  OpinionService(ProjectEntities context)
        {
            db = context; 
           service = new StockService(db);
           
        }
        /// <summary>
        /// Updates the Stop Price and details depending on the opinion
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TodaysTrade UpdateSellPriceDetails(TopStock r,TodaysTrade t)
        {
            t.Price = r.Price;
            if (t.Action == Strings.Opinions.Buy ||t.Action == Strings.Opinions.Short )
            {
                //get the sell stop
                t.SellPrice = r.StopPrice.Value;
                t.TargetPrice = r.TargetPrice ?? 0;
                //get the sell stop 
                t.Details = string.Format("{0} {5} Shares of {1} @ {2}, Target price is {3}, Stop @ {4}, Risk ${6}.", t.Action, r.Ticker, t.Price.ToString("C2"), t.TargetPrice.ToString("C2"), t.SellPrice.ToString("C2"),
                    FinanceHelper.GetShareAmount(DollarAmount,t.Price),DollarAmount);
            }             
            if (t.Action.Contains("Stop")) //this means cover short or long
            { 
                t.SellPrice = r.StopPrice.Value;
                t.Details = string.Format("{0} hit {1} @ {2}", r.Ticker, r.Opinion, t.SellPrice.ToString("C2"));
            }
            return t;
        }
        /// <summary>
        /// Updates the Stop Price and details depending on the opinion
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TodaysShortTrade UpdateSellPriceDetails(BottomStock r, TodaysShortTrade t)
        {
            t.Price = r.Price;
            if (t.Action == Strings.Opinions.Buy || t.Action == Strings.Opinions.Short)
            {
                //get the sell stop
                t.SellPrice = r.StopPrice.Value;
                t.TargetPrice = r.TargetPrice ?? 0;
                //get the sell stop 
                t.Details = string.Format("{0} {5} Shares of {1} @ {2}, Target price is {3}, Stop @ {4}, Risk ${6}.", t.Action, r.Ticker, t.Price.ToString("C2"), t.TargetPrice.ToString("C2"), t.SellPrice.ToString("C2"),
                     FinanceHelper.GetShareAmount(DollarAmount, t.Price),DollarAmount);
            }
            if (t.Action.Contains("Stop")) //this means cover short or long
            {
                t.SellPrice = r.StopPrice.Value;
                t.Details = string.Format("{0} hit {1} @ {2}",r.Ticker, r.Opinion, t.SellPrice.ToString("C2"));
            }
            return t;
        }
        /// <summary>
        /// Updates the details with the dropped price
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TodaysTrade UpdateDropDetails(TodaysTrade t)
       { 
            //get the sell stop
               t.SellPrice = t.Price;
               t.Action = Strings.Opinions.Liquidate; 
               t.Details = string.Format("{0} {1}, {2} @ {3}", t.Ticker,Strings.Directions.DROPPED, t.Action, t.SellPrice.ToString("C2"));
            
           return t;
       }
        /// <summary>
        /// Updates the details with the dropped price
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TodaysShortTrade UpdateShortDropDetails(TodaysShortTrade t)
        {
            //get the sell stop
            t.SellPrice = t.Price;
            t.Action = Strings.Opinions.Liquidate;
            t.Details = string.Format("{0} {1}, {2} @ {3}",t.Ticker, Strings.Directions.DROPPED,  t.Action, t.SellPrice.ToString("C2"));

            return t;
        }
        /// <summary>
        /// New Method that gets opinions, checks stops and set stops all in one
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public void GetOpinion(TopStock stock)
        {
            
            OpinionParameter r = new OpinionParameter(stock); 
            r.GainChange = GetGainChange(stock.Gain);
            r.RankChange = GetGainChange(stock.RankChange);
            r.VolumeChange = GetVolumeChange(stock.VolumeChange); 
            if (stock.CurrentRank == 1 || stock.Direction == Strings.Directions.NEW) //default the first rank to good, and the new entries
            {
                r.RankChange = Strings.Opinions.Good;
            }
            if (stock.HitStop()) //check sell stop first
            {
                stock.Opinion = Strings.Opinions.BuyStop;
                if (stock.IsLong())
                {
                    stock.Opinion = Strings.Opinions.SellStop;
                }
                stock.CurrentSide = Strings.Side.None;
                return;
            }
            stock.Opinion = RunOpinion(r);
            if(string.IsNullOrEmpty(stock.Opinion))
            {
                   ProtectGains(stock); //lets force the new stop
            } 
            if (stock.Opinion==Strings.Opinions.Buy)
            {
                //we need to update the current side now it changed
                stock.CurrentSide = Strings.Side.Long;
                SetStopPrice(stock);//set the stops
                SetTargetPrice(stock);
            }
            if (stock.Opinion == Strings.Opinions.Sell)
            {                 
                if(stock.CurrentSide != Strings.Side.Long)//if we are not long then this is invalid signal
                {
                    stock.Opinion = Strings.Opinions.NoOpinion;
                }
                stock.CurrentSide = Strings.Side.None; //do not short 
            } 
        }

        /// <summary>
        /// New Method that gets opinions, checks stops and set stops all in one
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public void GetOpinion(BottomStock stock)
        {

            OpinionParameter r = new OpinionParameter(stock);
            r.GainChange = GetGainChange(stock.Gain);
            r.RankChange = GetGainChange(stock.RankChange);
            r.VolumeChange = GetVolumeChange(stock.VolumeChange);
            if (stock.Volume < MinimumShortVolume) //only short above 100,000 shares
            {
                  stock.Opinion = Strings.Opinions.NoOpinion; 
                  stock.CurrentSide = Strings.Side.None;
                  return;
            }
            if (stock.CurrentRank == 1 || stock.Direction == Strings.Directions.NEW) //default the first rank to good, and the new entries
            {
                r.RankChange = Strings.Opinions.Good;
            }
            if (stock.HitStop()) //check sell stop first
            {
                stock.Opinion = Strings.Opinions.BuyStop;
                if (stock.IsLong())
                {
                    stock.Opinion = Strings.Opinions.SellStop;
                }
                stock.CurrentSide = Strings.Side.None;
                return;
            }
            stock.Opinion = RunShortOpinion(r);
            if (string.IsNullOrEmpty(stock.Opinion))
            {
                ProtectGains(stock); //lets force the new stop
            }
            if (stock.Opinion == Strings.Opinions.Short)
            {
                //we need to update the current side now it changed
                stock.CurrentSide = Strings.Side.Short;
                SetStopPrice(stock);//set the stops
                SetTargetPrice(stock);
            }
            if (stock.Opinion == Strings.Opinions.Cover)
            {
                if (stock.CurrentSide != Strings.Side.Short)//if we are not short then this is invalid signal
                {
                    stock.Opinion = Strings.Opinions.NoOpinion;
                }
                stock.CurrentSide = Strings.Side.None;  
            } 
        }
        #region stops
         
        /// <summary>
        /// Returns true or false if the current price is way above the stop, below the stop to protect the gains
        /// </summary>
        /// <returns></returns>
        public bool WayAboveStop(ITopStock stock)
        { 
            if(!stock.StopPrice.HasValue)
            {
                return false;
            }
            //is the opinion a buy?
            if (stock.IsLong())
            {
                //get the the price change see if it is less that the stop price
                var priceChange = FinanceHelper.PriceGain(stock.StopPrice.Value, stock.Price);
                return (priceChange > OpinionService.LargeSellStopChange);
            }
            if (stock.IsShort())
            {
                var priceChange = FinanceHelper.PriceGain(stock.StopPrice.Value, stock.Price);
                return (priceChange < OpinionService.LargeBuyStopChange);
            }
            return false;
        }
        /// <summary>
        /// Checks if the stock is way above the protective stop
        /// </summary>
        /// <param name="stock"></param>
        public void ProtectGains(ITopStock stock)
        {
            if (WayAboveStop(stock))
            {
                if (stock.IsLong())
                {
                    stock.Opinion = Strings.Opinions.Buy; 
                    return;
                }
                if (stock.IsShort())
                {
                    stock.Opinion = Strings.Opinions.Short;
                    
                }
            } 
        }
        /// <summary>
        /// If the stock is long, we set a sellstop
        /// If the stock is short, we set a buystop
        /// </summary>
        /// <param name="stock"></param>
        public void SetStopPrice(ITopStock stock)
        {
            if(stock.IsLong())
            { 
                 stock.StopPrice = stock.Price + (stock.Price * OpinionService.SellStopChange); //now 5%
                 return;  
            }
            if (stock.IsShort())
            {
                stock.StopPrice = stock.Price + (stock.Price * OpinionService.BuyStopChange); 
            }  
        }
        /// <summary>
        /// If the stock is long, we set a target price
        /// If the stock is short, we set a target price
        /// </summary>
        /// <param name="stock"></param>
        public void SetTargetPrice(ITopStock stock)
        {
            if (stock.IsLong())
            {
                stock.TargetPrice = stock.Price + (stock.Price * OpinionService.TargetPriceChangeLong);
                return;
            }
            if (stock.IsShort())
            {
                stock.TargetPrice = stock.Price + (stock.Price * OpinionService.TargetPriceChangeShort);
            }
        }
        /// <summary>
        /// Checks of long or short, then finds stop price change
        /// </summary>
        /// <param name="onDate"></param>
        public bool HitStop(TopStock current)
        {
            //is the opinion a buy?
            if (current.IsLong())
            {
                //get the the price change see if it is less that the stop price
                return HitSellStop(current);
            }
            if (current.IsShort())
            {
                return HitBuyStop(current);
            }
            return false;
        }
        /// <summary>
        /// Compares the change to the stop price for longs
        /// </summary>
        /// <param name="priceChange"></param>
        /// <returns></returns>
        public static bool HitSellStop(double priceChange)
        {
            return (priceChange < SellStopChange);
        }
        public static bool HitSellStop(TopStock stock)
        {
            if(stock.StopPrice.HasValue)
            {
                return (stock.Price < stock.StopPrice.Value);
            }
            return false;
        }
        /// <summary>
        /// Compares the change to the stop price for shorts
        /// </summary>
        /// <param name="priceChange"></param>
        /// <returns></returns>
        public static bool HitBuyStop(double priceChange)
        {

            return (priceChange > BuyStopChange);
        }
        public static bool HitBuyStop(TopStock stock)
        {
            if (stock.StopPrice.HasValue)
            {
                return (stock.Price > stock.StopPrice);
            }
            return false;
        }
        #endregion
        public static string GetRankChange(double rankChange )
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
        public static string GetGainChange(double priceChange)
        {             
            if (priceChange < BadPriceChange)
            {
                return Strings.Opinions.Bad;
            }
            if (priceChange > 0)
            {
                return Strings.Opinions.Good;
            }
            return Strings.Opinions.Even;
        }
        public static string GetVolumeChange(double volumeChange)
        {
            if (volumeChange < DateHelper.GetVolumeChangeTrigger(false))
            {
                return Strings.Opinions.NoTrade;
            }           
            return Strings.Opinions.Trade;
        }
        /// <summary>
        /// Determines the opinion for longs
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string RunOpinion(OpinionParameter parameter)
        { 
            /*case
            when ([Gain] > 0 good  and [RankChange] > 0 good and (VolumeChange * 100 > 1)) THEN 'Buy'
            when ([Gain] < 0 bad and [RankChange] < 0 bad and (VolumeChange * 100 > 1)) then 'Sell' 
            else ' '
            end as [Trade],[price] from ScanResults where (Gain != 0) and RankChange != 0 and (VolumeChange * 100 > 1)
            */
            //check sell stop first
            
            if(parameter.VolumeChange == Strings.Opinions.NoTrade || parameter.GainChange == Strings.Opinions.Even || parameter.RankChange == Strings.Opinions.Even)
            {
                return Strings.Opinions.NoOpinion;
            }  
            switch(parameter.RankChange)
            { 
                case Strings.Opinions.Bad:
                    {
                        #region currentgain
                        switch (parameter.GainChange)
                        {                            
                            case Strings.Opinions.Bad: //rank change Bad, GainChange bad
                                {                                    
                                    return Strings.Opinions.Sell;
                                }
                            default: //rank change Bad, GainChange good
                                {
                                    return Strings.Opinions.NoOpinion;
                                }
                        }
                        #endregion
                    }
                default: //rank change good
                    #region currentgain
                    switch (parameter.GainChange)
                    {
                        case Strings.Opinions.Bad: //rank change good, GainChange bad
                        case Strings.Opinions.Even://rank change good, GainChange even
                            {
                                return Strings.Opinions.NoOpinion;
                            }                        
                        default: //rank change good, GainChange good
                            { 
                                return Strings.Opinions.Buy; 
                            }
                    }
                    #endregion
            }
        }
        public static string RunShortOpinion(OpinionParameter parameter)
        {
            /*case
            when ([Gain] > 0 good  and [RankChange] > 0 good and (VolumeChange * 100 > 1)) THEN 'Buy'
            when ([Gain] < 0 bad and [RankChange] < 0 bad and (VolumeChange * 100 > 1)) then 'Sell' 
            else ' '
            end as [Trade],[price] from ScanResults where (Gain != 0) and RankChange != 0 and (VolumeChange * 100 > 1)
            */
            //check sell stop first

            if (parameter.VolumeChange == Strings.Opinions.NoTrade || parameter.GainChange == Strings.Opinions.Even || parameter.RankChange == Strings.Opinions.Even)
            {
                return Strings.Opinions.NoOpinion;
            }
            switch (parameter.RankChange)
            {
                case Strings.Opinions.Bad:
                    {
                        #region currentgain
                        switch (parameter.GainChange)
                        {
                            case Strings.Opinions.Good: //rank change Bad, GainChange Good
                                {
                                    return Strings.Opinions.Cover;
                                }
                            default: //rank change Bad, GainChange bad
                                {
                                    return Strings.Opinions.NoOpinion;
                                }
                        }
                        #endregion
                    }
                default: //rank change good
                    #region currentgain
                    switch (parameter.GainChange)
                    {
                        case Strings.Opinions.Good: //rank change good, GainChange Good
                        case Strings.Opinions.Even://rank change good, GainChange even
                            {
                                return Strings.Opinions.NoOpinion;
                            }
                        default: //rank change good, GainChange bad
                            {
                                return Strings.Opinions.Short;
                            }
                    }
                    #endregion
            }
        }

          
    }
}