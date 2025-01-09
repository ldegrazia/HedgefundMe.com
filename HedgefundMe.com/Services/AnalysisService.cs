using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HedgefundMe.com.Models;
namespace HedgefundMe.com.Services
{
    public class AnalysisService
    {
       
        private readonly ProjectEntities db;
        private readonly MarketDataService mdService;
        public AnalysisService(ProjectEntities context, MarketDataService mdservice)
        {
            db = context;
            mdService = mdservice;
        }
        /// <summary>
        /// Analyses longs, then shorts and updates the market data
        /// </summary>
        public void Analyze()
        {
            List<MarketData> longs = AnalyzeLongs(mdService.StartOn);
            List<MarketData> shorts = AnalyzeShorts(mdService.StartOn);
            mdService.Update(longs);
            mdService.Update(shorts);
        }
        
        /// <summary>
        /// Runs a performance scan for all tickers for the date, including opinions.
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<MarketData> AnalyzeLongs(DateTime onDate)
        {
            List<MarketData> current = mdService.GetDataOn(onDate, Strings.Side.Long).ToList();
            List<MarketData> past = mdService.GetDataOn(onDate.AddDays(-1), Strings.Side.Long).ToList();
            if (!past.Any())
            {
                return current;
            }
            List<MarketData> perf = new List<MarketData>();
            foreach (MarketData performance in current)
            {
                try
                {
                    var previousranks = (from t in past where t.Ticker.TrimEnd() == performance.Ticker.TrimEnd() select t);
                    #region newstock
                    if (previousranks.Count() == 0)//its new
                    {
                        performance.PreviousRank = 0;
                        performance.RankChange = 0;
                        performance.Direction = Strings.Directions.NEW;
                    }
                    #endregion
                    #region existing stock
                    else
                    {
                        MarketData previousrank = previousranks.First();
                        performance.PreviousRank = previousrank.CurrentRank;
                        performance.RankChange = (previousrank.CurrentRank - performance.CurrentRank);
                        performance.VolumeChange = FinanceHelper.VolumeChange(performance.AvgVol, performance.Volume);
                        performance.Direction = FinanceHelper.Direction(performance.RankChange); 
                    }
                    #endregion
                    perf.Add(performance);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, "Error running AnalyzeLongs for " + performance.Ticker + " :" + ex.Message);
                }
            }
            return perf.ToList();
        }

        /// <summary>
        /// Analyzes short market data.
        /// </summary>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public List<MarketData> AnalyzeShorts(DateTime onDate)
        {
            List<MarketData> current = mdService.GetDataOn(onDate,Strings.Side.Short).ToList();
            List<MarketData> past = mdService.GetDataOn(onDate.AddDays(-1),Strings.Side.Short).ToList();
            if (!past.Any())
            {
                return current;
            }
            List<MarketData> perf = new List<MarketData>();
            foreach (MarketData performance in current)
            {
                try
                {
                    var previousranks = (from t in past where t.Ticker.TrimEnd() == performance.Ticker.TrimEnd() select t);
                    #region newstock
                    if (previousranks.Count() == 0)//its new
                    {
                        performance.PreviousRank = 0;
                        performance.RankChange = 0;
                        performance.Direction = Strings.Directions.NEW;
                        
                    }
                    #endregion
                    #region existing stock
                    else
                    {
                        MarketData previousrank = previousranks.First();
                        performance.PreviousRank = previousrank.CurrentRank;
                        performance.RankChange = (previousrank.CurrentRank - performance.CurrentRank);
                        performance.VolumeChange = FinanceHelper.VolumeChange(performance.AvgVol, performance.Volume);
                        performance.Direction = FinanceHelper.Direction(performance.RankChange); 
                    }
                    #endregion 
                    perf.Add(performance);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(MessageType.Error, "Error running AnalyzeShorts for " + performance.Ticker + " :" + ex.Message);
                }
            }
            return perf.ToList();
        }
    }
}