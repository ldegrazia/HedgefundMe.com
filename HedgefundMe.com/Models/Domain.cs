using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
namespace HedgefundMe.com.Models
{
   /// <summary>
   /// Represents all the trade settings
   /// </summary>
    public class TradingSettings
    {

        [Key] 
        public int ID { get; set; }

        [Display(Name = "Name:")]
        [Required(ErrorMessage = "Value is required")]
        public string Name { get; set; }

        [Display(Name = "In use now?:")]
        public bool Enabled { get; set; }

        [Display(Name = "Rating:")]
        public int Rating { get; set; }
       

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy hh:mm tt}")]
        [Display(Name = "Settings Date:")]
        public DateTime Date { get; set; }

        [Display(Name = "Created By:")]
        [Required(ErrorMessage = "Value is required")]
        public string CreatedBy { get; set; }

        //market data
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy hh:mm tt}")]
        [Display(Name = "Last market data load:")]
        public DateTime LastDataLoad { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Minimum Short volume:")]
        [Required(ErrorMessage = "Value is required")]
        public double MinimumShortVolume { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Minimum Long volume:")]
        [Required(ErrorMessage = "Value is required")]
        public double MinimumLongVolume { get; set; } 
        
        //signals
         [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy hh:mm tt}")]
         [Display(Name = "Last signals ran on:")]
         public DateTime LastSignalRun { get; set; }

         [DisplayFormat(DataFormatString = "{0:P}")]
         [Display(Name = "% Price change for buys:")]
         [Required(ErrorMessage = "Value is required")]
         public double GoodPriceChange { get; set; }

         [DisplayFormat(DataFormatString = "{0:P}")]
         [Display(Name = "% Price change for shorts:")]
         [Required(ErrorMessage = "Value is required")]
         public double BadPriceChange { get; set; }  

         //trading behavior
         [Display(Name = "Trade in amounts of:")]
         [DisplayFormat(DataFormatString = "{0:C}")]
         [Required(ErrorMessage = "Value is required")]
         public int DollarAmount { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}")]
         [Display(Name = "Maximum blotter amount:")]
         [Required(ErrorMessage = "Value is required")]
         public double MaxBlotterValue { get; set; }

         [Display(Name = "Min % Price target for Longs:")]
         [Required(ErrorMessage = "Value is required")]
         [DisplayFormat(DataFormatString = "{0:P}")]
         public double TargetPriceChangeLong { get; set; } 

         [Display(Name = "Min % Price target for shorts:")]
         [DisplayFormat(DataFormatString = "{0:P}")]
         [Required(ErrorMessage = "Value is required")]
         public double TargetPriceChangeShort { get; set; }  

         [DisplayFormat(DataFormatString = "{0:P}")]
         [Required(ErrorMessage = "Value is required")]
         [Display(Name = "% Price Sell Stop for Longs:")]
         public double SellStopChange   { get; set; }  

         [DisplayFormat(DataFormatString = "{0:P}")]
         [Required(ErrorMessage = "Value is required")]
         [Display(Name = "% Price Sell Stop for Shorts:")]
         public double BuyStopChange   { get; set; } 

         [Display(Name = "Reenter trades when targets are met?:")]
         public bool ReEnterWinningTrades { get; set; }

         [Display(Name = "Maximum times to make same losing trade:")]
         public int SameLosingTradeMaximum { get; set; }

         [Display(Name = "Avgerage Return:")]
         [DisplayFormat(DataFormatString = "{0:P}")]
         public double AvgReturn { get; set; }

         [Display(Name = "Commission Cost Per Share:")]
         [DisplayFormat(DataFormatString = "{0:C3}")]
         public double CommissionCostPerShare { get; set; }

         public TradingSettings()
         {
               MaxBlotterValue = 100000;
               CreatedBy = "admin";
               BadPriceChange = -0.03; //put this in the database
               GoodPriceChange = 0.01; //put this in the database
               DollarAmount = 1000;  //ths dollar amount for the trade emails
               SellStopChange = -0.03; //put this in the database
               BuyStopChange = 0.03; //put this in the database
               TargetPriceChangeLong = 0.01; //put this in the datbase
               TargetPriceChangeShort = -0.01; //put this in the datbase
               ReEnterWinningTrades = true;
               SameLosingTradeMaximum = 2;
               MinimumShortVolume = 100000;
                MinimumLongVolume = 50000;  
               Date = DateTime.Now;
               LastDataLoad = DateTime.Now;
               LastSignalRun = DateTime.Now;
               CommissionCostPerShare = 0.003;
               Name = "Admin Trading Settings for " + DateTime.Now.ToLongDateString();
         }
    }
   public enum TimeFrame
    {
        ThreeMonth,
        OneMonth,
        SixMonth,
        OneYear,
        TenDay,
        TwentyDay,
        TwoMonth,
        NineMonth,
        OneDay,
        FiveDay,
        Max,
        Custom
    }  
    
    public class MarketDataParameters
    { 
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public MarketDataParameters()
        {
            PageSize = 50;
            PageNumber = 1;
        }
    }
    #region opinions
    public class OpinionParameter
    {
    public string Ticker  {get;set;}
	public string RankChange {get;set;}
	public string CurrentGain {get;set;}
    public string VolumeChange { get; set; }
	public string GainChange {get;set;}
    public string Opinion { get; set; }
    public OpinionParameter()
    { }
  
    public OpinionParameter(MarketData stock)
    {
        Ticker = stock.Ticker;
    }
    }
    #endregion
  
     
   
    /// <summary>
    /// class for serializing only
    /// </summary>
    public class ArrayOfTodaysTrade
    {

        [XmlArray("Signals")]

        [XmlArrayItem("TodaysTrades")]

        public TodaysTradeDto[] TodaysTradesDto;

    }

    public class TodaysTradeDto
    {

        public int ID { get; set; }

        public DateTime Date { get; set; }
         
        public string TradeId { get; set; }

        public string Ticker { get; set; }

        public string Action { get; set; }

        public int Shares { get; set; }

        public double Price { get; set; }

        public double? StopPrice { get; set; } 

        public double? TargetPrice { get; set; } 

        public string Details { get; set; }

        public int TradingSettings { get; set; } 

    }
    /// <summary>
    /// class for serializing only
    /// </summary>
    public class ArrayOfMarketData
    {
        [XmlArray("Universe")]
        [XmlArrayItem("MarketData")]
        public MarketDataDto[] TodaysTradesDto;
    }

    public class MarketDataDto
    { 
        public int ID { get; set; } 
        public string Side { get; set; } 
        public DateTime Date { get; set; } 
        public string Ticker { get; set; } 
        public double Price { get; set; } 
        public double PriceChange { get; set; } 
        public double PriceChangePcnt { get; set; } 
        public double? Volume { get; set; } 
        public double? AvgVol { get; set; } 
        public double VolumeChange { get; set; } 
        public double DayHigh { get; set; } 
        public double DayLow { get; set; } 
        public double YearHigh { get; set; } 
        public double YearLow { get; set; } 
        public int CurrentRank { get; set; } 
        public int PreviousRank { get; set; } 
        public string Direction { get; set; } 
        public int RankChange { get; set; } 
        public double MA50 { get; set; } 
        public double MA200 { get; set; } 
        public double Dividend { get; set; } 
        public double DividendPct { get; set; } 
        public double YearAgoPrice { get; set; } 
        public double YearPriceChange { get; set; } 
        public double SixMonAgoPrice { get; set; } 
        public double SixMonPriceChange { get; set; } 
        public double ThreeMonAgoPrice { get; set; } 
        public double ThreeMonPriceChange { get; set; } 
        public double Beta { get; set; } 
    } 
    /// <summary>
    /// Used for aduting the latest fetches and changes to the site data
    /// </summary>
    public class SiteAuditRecord
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(100)]
        [Display(Name = "Last Signal Fetched By")]
        public string LastSignalFetchBy { get; set; }
        [Display(Name = "Last Signal Fetched at")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy tt}")]
        public DateTime LastSignalFetchDate { get; set; }
         public string SignalFetchDateEasterTime()
        {
            return DateHelper.ToEasternStandardTime(LastSignalFetchDate).ToString("MM/dd/yyyy hh:mm tt");
        }
        [MaxLength(100)]
        [Display(Name = "Last Pricing Fetched By")]
        public string LastPricingFetchBy { get; set; }
        [Display(Name = "Last Pricing Fetched at")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy tt}")]
        public DateTime LastPricingFetchDate { get; set; }
        public string PricingFetchDateEasterTime()
        {
            return DateHelper.ToEasternStandardTime(LastPricingFetchDate).ToString("MM/dd/yyyy hh:mm tt");
        }
        public SiteAuditRecord()
        {
            LastPricingFetchDate = DateTime.Now.AddDays(-30);
            LastSignalFetchDate = DateTime.Now.AddDays(-30);
            LastSignalFetchBy = "System";
            LastPricingFetchBy = "System";
        }
    }
    /// <summary>
    /// The current trade in the blotter that is open
    /// </summary>
    public class BlotterEntry
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(100)]
        public string TradeId { get; set; }

        [Required(ErrorMessage = "Side is required")]
        [MaxLength(50)]
        public string Side { get; set; }

        [MaxLength(50)]
        public string Strategy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy tt}")] 
        public DateTime Date { get; set; }

        public string EasterTimeDate()
        {
            return DateHelper.ToEasternStandardTime(Date).ToString("MM/dd/yyyy hh:mm tt");
        }


        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }

        [Required(ErrorMessage = "Purchase Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PurchasePrice { get; set; }

        [Required(ErrorMessage = "Current Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")] 
        public double CurrentPrice { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PriceChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double PriceChangePcnt { get; set; } 

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Volume { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? AvgVol { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double VolumeChange { get; set; }

        [MaxLength(50)]
        public string Color { get; set; }
         
        [DisplayFormat(DataFormatString = "{0:C}")] 
        public double DayHigh { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DayLow { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearHigh { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearLow { get; set; }

        public int Shares { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OpenValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CurrentValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double GainLoss { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double GainLossPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double StopPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double TargetPrice { get; set; }

        public int TradingSettings { get; set; }
        public void CalculateOpenValue()
        {
            OpenValue = Shares * PurchasePrice;
        }
        public void CalculateGainLoss()
        {
            if (OpenValue == 0) return ;
            CurrentValue = (Shares * CurrentPrice);
            if(Strategy == Strings.Strategies.IntradayMomentumLong)
            {
                GainLossPct = (CurrentValue - OpenValue) / OpenValue ;
                GainLoss = CurrentValue - OpenValue;
                return;
            }
            GainLossPct = (CurrentValue - OpenValue) / OpenValue * -1;
            GainLoss = (CurrentValue - OpenValue) *-1; 
        }
    }
    /// <summary>
    /// Acts as P N L report for each set of trades, an open and a close trade
    /// </summary>
    public class TradeHistoryEntry
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(100)]
        public string TradeIdOpen { get; set; }

        [MaxLength(100)]
        public string TradeIdClose { get; set; }


        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }

        [Required(ErrorMessage = "Side is required")]
        [MaxLength(50)]
        public string Side { get; set; }

        [MaxLength(50)]
        public string OpenAction { get; set; }

        [Required(ErrorMessage = "Open Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OpenPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime OpenDate { get; set; }
        public string EasterTimeOpenDate()
        {
            return DateHelper.ToEasternStandardTime(OpenDate).ToString("MM/dd/yyyy hh:mm tt");
        }

        [MaxLength(255)]
        public string OpenDetails { get; set; }

        [MaxLength(50)]
        public string CloseAction { get; set; }

        [Required(ErrorMessage = "Open Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosePrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime CloseDate { get; set; }
        public string EasterTimeCloseDate()
        {
            return DateHelper.ToEasternStandardTime(CloseDate).ToString("MM/dd/yyyy hh:mm tt");
        }
        [MaxLength(255)]
        public string CloseDetails { get; set; }

        public int Shares { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Commision { get; set; }

        [Required(ErrorMessage = "Open Value is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OpenValue { get; set; }

        [Required(ErrorMessage = "Open Value is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CloseValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double GainLossPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double GainLoss { get; set; }

        [MaxLength(50)]
        public string Strategy { get; set; }

        public int TradingSettings { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        public bool IsOpen()
        {
            return string.IsNullOrEmpty(CloseAction);
        }
        /// <summary>
        /// Returns open or closed
        /// </summary>
        /// <returns></returns>
        public string Satus()
        {
           if(IsOpen())
           {
               return Strings.Opinions.Open;
           }
           return Strings.Opinions.Closed;
        }
        public string WinLoss()
        {
            if(GainLoss == 0)
            {
                return string.Empty;
            }
            if(GainLoss > 0)
            {
                return Strings.Opinions.Win;
            }
            return Strings.Opinions.Loss;
        }

        public TradeHistoryEntry()
        {
            OpenDate = DateTime.Now;
            CloseDate = DateTime.Now.AddDays(30);
            State = Strings.Opinions.Open;
        }
        /// <summary>
        /// Marks the state as closed
        /// </summary>
        public void MarkClosed()
        {
            State = Strings.Opinions.Closed;
        }
        /// <summary>
        /// Calculates the gain loss for the entry
        /// </summary> 
        public void CalculateGainLoss()
        {
            if (CloseValue == 0) return;//divide by 0
            if(Strategy==Strings.Strategies.IntradayMomentumLong)
            {
                //subtract close minus open thats the gain
                //calculate the percent value
                GainLoss = CloseValue - OpenValue;
                GainLossPct = ((CloseValue - OpenValue) / CloseValue);
                return;
            }
            //we are short so
            GainLoss = (CloseValue - OpenValue) * -1; //500 - 1000 = -500 * -1 = 500
            GainLossPct = ((CloseValue - OpenValue) / CloseValue) * -1;
        }
    }
    /// <summary>
    /// Represents a new signal to buy, sell, short or cover with details
    /// </summary>
    public class TradeSignal
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(100)]
        public string TradeId { get; set; }

        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }
        
        [MaxLength(50)]
        public string Action { get; set; }

        [Required(ErrorMessage = "Open Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime Date { get; set; }

        public string EasterTimeDate()
        {
            return DateHelper.ToEasternStandardTime(Date).ToString("MM/dd/yyyy hh:mm tt");
        }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double StopPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double TargetPrice { get; set; }

        public int Shares { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Value { get; set; }

        [MaxLength(255)]
        public string Details { get; set; }

        [MaxLength(50)]
        public string Strategy { get; set; }

        public int TradingSettings { get; set; }

        public TradeSignal()
        {
            Date = DateTime.Now;
        }
        public void UpdateValue()
        {
            Value = Shares * Price;
        }
    }
    /// <summary>
    /// Historical a signal to buy, sell, short or cover with details
    /// </summary>
    public class TradeSignalHistory
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(100)]
        public string TradeId { get; set; }

        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }

        [MaxLength(50)]
        public string Action { get; set; }

        [Required(ErrorMessage = "Open Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double? StopPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double? TargetPrice { get; set; }

        public int Shares { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Value { get; set; }

        [MaxLength(255)]
        public string Details { get; set; }

        [MaxLength(50)]
        public string Strategy { get; set; }

        public int TradingSettings { get; set; }

        public TradeSignalHistory()
        {
            Date = DateTime.Now;
        }
        public string EasterTimeDate()
        {
            return DateHelper.ToEasternStandardTime(Date).ToString("MM/dd/yyyy hh:mm tt");
        }
    }
    /// <summary>
    /// Represents the market data of a ticker
    /// </summary>
    public class MarketData
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Side is required")]
        [MaxLength(50)]
        public string Side { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }
          
        [Required(ErrorMessage = "Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Price { get; set; } 

        [Required(ErrorMessage = "Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PriceChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double PriceChangePcnt { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Volume { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? AvgVol { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double VolumeChange { get; set; } 

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DayHigh { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DayLow { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearHigh { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearLow { get; set; }

        public int CurrentRank { get; set; }

        public int PreviousRank { get; set; }

        [MaxLength(50)] 
        public string Direction { get; set; }

        public int RankChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double MA50 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double MA200 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Dividend { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double DividendPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearAgoPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double YearPriceChange { get; set; } 

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SixMonAgoPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double SixMonPriceChange { get; set; } 

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ThreeMonAgoPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ThreeMonPriceChange { get; set; } 
       
        public double Beta { get; set; } 
          
    }

    /// <summary>
    /// Represents the market data for back tests
    /// </summary>
    public class BackTestData
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Side is required")]
        [MaxLength(50)]
        public string Side { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }

        [MaxLength(50)]
        public string Opinion { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PriceChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double PriceChangePcnt { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? Volume { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double? AvgVol { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double VolumeChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DayHigh { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DayLow { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearHigh { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearLow { get; set; }

        public int CurrentRank { get; set; }

        public int PreviousRank { get; set; }

        [MaxLength(50)]
        public string Direction { get; set; }

        public int RankChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double MA50 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double MA200 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Dividend { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double DividendPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double YearAgoPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double YearPriceChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SixMonAgoPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double SixMonPriceChange { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ThreeMonAgoPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ThreeMonPriceChange { get; set; }

        public double Beta { get; set; } 

        public BackTestData(MarketData marketData)
        {
            this.AvgVol = marketData.AvgVol;
            this.Beta = marketData.Beta;
            this.CurrentRank = marketData.CurrentRank;
            this.Date = DateTime.Now;
            this.DayHigh = marketData.DayHigh;
            this.DayLow = marketData.DayLow;
            this.Direction = marketData.Direction;
            this.Dividend = marketData.Dividend;
            this.DividendPct = marketData.DividendPct;
            this.MA200 = marketData.MA200;
            this.MA50 = marketData.MA50;
            this.PreviousRank = marketData.PreviousRank;
            this.Price = marketData.Price;
            this.PriceChange = marketData.PriceChange;
            this.PriceChangePcnt = marketData.PriceChangePcnt;
            this.RankChange = marketData.RankChange;
            this.Side = marketData.Side;
            this.SixMonAgoPrice = marketData.SixMonAgoPrice;
            this.SixMonPriceChange = marketData.SixMonPriceChange;
            this.ThreeMonAgoPrice = marketData.ThreeMonAgoPrice;
            this.ThreeMonPriceChange = marketData.ThreeMonPriceChange;
            this.Ticker = marketData.Ticker;
            this.Volume = marketData.Volume;
            this.VolumeChange = marketData.VolumeChange;
            this.YearAgoPrice = marketData.YearAgoPrice;
            this.YearHigh = this.YearHigh;
            this.YearLow = this.YearLow;
            this.YearPriceChange = this.YearPriceChange;
             
        }

    }
    public class Stock
    {
        [Key]
        public int ID { get; set; }


        [Required(ErrorMessage = "Ticker is required")]
        [MaxLength(50)]
        public string Ticker { get; set; }
    }
     /// <summary>
    /// The yahoo list with the tickers
    /// </summary>
    public class PortfolioEntry
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(100)]
        [Display(Name = "Name of portfolio")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Url is required")]
        [MaxLength(250)]
        [Display(Name = "Url to the view of the data")] 
        public string Url { get; set; }

        
    }
}