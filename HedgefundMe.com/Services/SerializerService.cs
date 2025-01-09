using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public static class SerializerService
    { 
        /// <summary>
        /// Returns the new xml of the latest trades
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public static string SerializeTradeSignals(List<TradeSignal> stocks)
        {
            // Create the stream.
            MemoryStream ms = new MemoryStream();
            ArrayOfTodaysTrade trades = new ArrayOfTodaysTrade();
            trades.TodaysTradesDto = new TodaysTradeDto[stocks.Count()];
            for (int i = 0; i < stocks.Count(); i++)
            {
                TodaysTradeDto dt = new TodaysTradeDto();
                dt.Action = stocks[i].Action;
                dt.Date = stocks[i].Date;
                dt.Details = stocks[i].Details;
                dt.ID = stocks[i].ID;
                dt.Shares = stocks[i].Shares;
                dt.Price = stocks[i].Price;
                dt.StopPrice = stocks[i].StopPrice;
                dt.TargetPrice = stocks[i].TargetPrice;
                dt.Ticker = stocks[i].Ticker;
                dt.TradeId = stocks[i].TradeId;
                dt.TradingSettings = stocks[i].TradingSettings;
                trades.TodaysTradesDto[i] = dt;
            }
            XmlSerializer s = new XmlSerializer(typeof(ArrayOfTodaysTrade));
            XmlTextWriter tw = new XmlTextWriter(ms, null);
            s.Serialize(tw, trades);

            // Rewind the Stream.
            ms.Seek(0, SeekOrigin.Begin);

            XmlDocument doc = new XmlDocument();

            // load from stream;
            doc.Load(ms);
            String xmlString = doc.OuterXml;
            return xmlString;
        }
        /// <summary>
        /// Returns the new xml of the universe
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public static string SerializeUniverse(List<MarketData> stocks)
        {
            // Create the stream.
            MemoryStream ms = new MemoryStream();
            ArrayOfMarketData trades = new ArrayOfMarketData();
            trades.TodaysTradesDto = new MarketDataDto[stocks.Count()];
            for (int i = 0; i < stocks.Count(); i++)
            {
                MarketDataDto dt = new MarketDataDto();
                
                dt.Date = stocks[i].Date;                
                dt.ID = stocks[i].ID;                
                dt.Price = stocks[i].Price;                
                dt.Ticker = stocks[i].Ticker;
                dt.AvgVol = stocks[i].AvgVol;
                dt.Volume = stocks[i].Volume;
                dt.PriceChange = stocks[i].PriceChange;
                dt.PriceChangePcnt = stocks[i].PriceChangePcnt;
                dt.VolumeChange = stocks[i].VolumeChange;
                dt.CurrentRank = stocks[i].CurrentRank;
                dt.PreviousRank = stocks[i].PreviousRank;
                dt.RankChange = stocks[i].RankChange;
                dt.Side = stocks[i].Side;
                dt.Direction = stocks[i].Direction;
                dt.Beta = stocks[i].Beta;
                trades.TodaysTradesDto[i] = dt;
            }
            XmlSerializer s = new XmlSerializer(typeof(ArrayOfMarketData));
            XmlTextWriter tw = new XmlTextWriter(ms, null);
            s.Serialize(tw, trades);

            // Rewind the Stream.
            ms.Seek(0, SeekOrigin.Begin);

            XmlDocument doc = new XmlDocument();

            // load from stream;
            doc.Load(ms);
            String xmlString = doc.OuterXml;
            return xmlString;
        }
    }
}