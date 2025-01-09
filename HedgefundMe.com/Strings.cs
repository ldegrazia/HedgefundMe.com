using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HedgefundMe.com 
{
    public static class Strings
    {
        public static class Directions
        {
            public const string NEW = "New";
            public const string DOWN = "Down";
            public const string EVEN ="Even";
            public const string UP= "Up";
            public const string DROPPED = "Dropped";
        }
        public static class Colors
        {
            public const string RED = "#710000";  
            public const string BLACK= "#292A2B";  
            public const string GREEN= "#134e13";  
        }
        public static class ScanNames
        { 
            public const string BigRankIncrease = "BigRankIncrease";
            public const string BigPriceIncrease = "BigPriceIncrease";
            public const string BigRankDrops = "BigRankDrops";
            public const string BigPriceDrops = "BigPriceDrops"; 
            public const string Top5RankChanges = "Top5RankChanges";
            public const string Top5PriceChanges = "Top5PriceChanges";
            public const string UpOnHighVolume = "UpOnHighVolume";
            public const string DownOnHighVolume ="DownOnHighVolume"; 
            public const string Adds = "Adds";
            public const string Drops = "Drops";
            public const string NewPriceHighs = "NewPriceHighs";
            public const string NewRankHighs = "NewRankHighs"; 
            public const string Buys ="Buys";
            public const string Sells = "Sells";
            public const string SellStops = "SellStops";
            public static List<string> AllScanNames()
            {
                return new List<string>
                  {
                     
                      BigPriceIncrease,
                      BigRankIncrease,
                      BigRankDrops,
                      BigPriceDrops,
                      Top5RankChanges ,
                      Top5PriceChanges,
                      UpOnHighVolume ,
                      DownOnHighVolume,
                      Adds,
                      Drops,
                      NewPriceHighs,
                      NewRankHighs, 
                      Buys,
                      Sells,
                      SellStops
                  };
            }
            public static List<string> GoodScans()
            {
                return new List<string>
                  {
                       
                      BigRankIncrease,
                      BigPriceIncrease,
                       UpOnHighVolume ,
                       NewPriceHighs,
                      NewRankHighs,
                       Buys 
                  };
            }
           
            public static List<string> BadScans()
            {
                return new List<string>
                  {
                      
                      BigRankDrops,
                      BigPriceDrops,
                      DownOnHighVolume,
                      Sells,
                      SellStops
                  };
            }
            public static List<string> BadScanTrends()
            {
                return new List<string>
                  {  
                      DownOnHighVolume,
                      Sells,
                      SellStops
                  };
            }
            public static List<string> GoodScanTrends()
            {
                return new List<string>
                  { 
                      UpOnHighVolume ,
                       NewPriceHighs,
                       Buys 
                  };
            }
            public static List<string> NeutralScanNames()
            {
                return new List<string>
                  {
                  
                      Top5RankChanges ,
                      Top5PriceChanges,
                      Adds,
                      Drops 
                  };
            }
            public static string DescribeScan(string name)
            {
                switch (name)
                {
                     
                   
                    case Top5PriceChanges:
                        {
                           return  "[Top 5 %Gains in Price]";
                             
                        }
                    case  Top5RankChanges:
                        {
                            return "[Top 5 Rank Increases]";
                              
                        }
                    case  BigPriceDrops:
                        {
                           return "[Price falls more than 10%]";
                            
                        }
                    case BigRankDrops:
                        {
                            return "[Rank falls more than 10]";

                        }
                    case  BigPriceIncrease: //
                        {
                            return "[Up 8.00% in Price]";
                            
                        }
                    case BigRankIncrease: //
                        {
                            return "[Up 10 in Rank and up in price]";

                        }
                    case  NewPriceHighs: //
                        {
                           return "[The day's closing price is the highest recorded]";
                             
                        }
                    case  NewRankHighs: //
                        {
                            return "[The day's closing rank is the highest recorded]";
                            
                        }
                    case  Buys: //
                        {
                            return  "Buy Signals";
                             
                        }
                    case  Sells: //
                        {
                            return "Sell Signals";
                          
                        }
                    case  SellStops: //
                        {
                           return "Sell Stops";
                            
                        }
                    case Adds: //
                        {
                            return "Adds";

                        }
                    default:
                        {
                            return name;
                        }
                }
            }
             public static string TypeOfScan(string scan)
             {
                 if(ScanNames.GoodScans().Contains(scan))
                 {
                     return ScanSides.Good;
                 }
                  if(ScanNames.BadScans().Contains(scan))
                 {
                     return ScanSides.Bad;
                 }
                  return ScanSides.Neutral;
            }
             public static List<string> AllScanOverviewTypes()
             {
                 return new List<string>
            {                 
                 BigPriceIncrease,
                 BigRankIncrease,
                 UpOnHighVolume,
                 DownOnHighVolume,
                 BigPriceDrops,
                 BigRankDrops, 
                 NewPriceHighs,
                 NewRankHighs,
                 Sells,
                 Buys,
                 SellStops
            };
             }
        }
        public static class ScanSides
        { 
              public const string  Neutral = "Neutral";
              public const string  Good ="Good";
              public const string  Bad = "Bad";
              public static List<string> AllScanSides()
              {
                  return new List<string>
                  {
                      Neutral,
                      Good,
                      Bad
                  };
              }
        }
        public static class Strategies
        {
            public const string IntradayMomentumLong = "Long";
            public const string IntradayMomentumShort = "Short";
            public const string None = "";
        }
        public static class Side
        {
            public const string Long = "Long";
            public const string Short = "Short";
            public const string None = "";
            /// <summary>
            /// Looks at the opinion to determine the side
            /// </summary>
            public static string GetSide(string opinion)
            {
                if (opinion == Strings.Opinions.Sell)
                {                     
                    return Strings.Side.Short;
                }
                if (opinion == Strings.Opinions.Buy)
                {
                    return Strings.Side.Long;
                }
                return Strings.Side.None; 
            }
        }
        public static class Opinions
        {
            public const string NEW = "New";
            public const string Even = "Even";
            public const string Trade = "Trade";
            public const string NoTrade = "NoTrade";
            public const string Good = "Good";
            public const string Bad = "Bad";
            public const string NoOpinion = "";
            public const string Buy = "Buy";
            public const string Cover = "Cover";
            public const string Short = "Short";
            public const string Sell = "Sell";
            public const string SellStop = "Sell Stop";
            public const string BuyStop = "Buy Stop";
            public const string Liquidate = "Liquidate";
            public const string Hold = "Hold";
            public const string Win = "Win";
            public const string Loss = "Loss";
            public const string Open = "Open";
            public const string Closed = "Closed";
            /// <summary>
            /// Retruns true if the two opinions are the same
            /// </summary>
            /// <param name="oldOpinion"></param>
            /// <param name="newOpinion"></param>
            /// <returns></returns>
            public static bool IsDifferent(string oldOpinion, string newOpinion)
            {
                return oldOpinion != newOpinion;
            }
        }

        public static class BackTest
        {
            public const string BuyInDollars = "Dollar amounts";
            public const string BuyInShareQty = "Share amounts";
            public const string ShareQuantity =  "Share Quantity";
            public const string DollarQuantity = "Dollar Amount";
            public const string Shares = " shares";
            public const string Dollars = " dollars";
            public const int BuyAmount = 2200;
            public const int ShortAmount = 9200;
            public const double StartingCapital = 250000;
            public const int DaysBack = -150;
        }
        public static class LongShorts
        {
            public static List<string> Longs = new List<string>
            {
                "QQQ", 
                "DIA",
                "SPY",
                "GLD",
                 "UYG",
                "RXL",
                "UXI",
                "URE",
                "USD",
                "DIG",
                "ROM",
                "UPW" 
                
            };
            public static List<string> Shorts = new List<string>
            {
                "PSQ",
                "DOG",
                "SH",
                "DGZ",
                "SKF",
                "RXD",
                "SIJ",
                "SRS",
                "SSG",
                "DUG",
                "REW",
                "SDP" 
            };
        }
        
    }
}