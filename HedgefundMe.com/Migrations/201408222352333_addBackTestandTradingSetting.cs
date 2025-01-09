namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBackTestandTradingSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TradingSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        Enabled = c.Boolean(nullable: false),
                        Rating = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 4000),
                        LastDataLoad = c.DateTime(nullable: false),
                        MinimumShortVolume = c.Double(nullable: false),
                        MinimumLongVolume = c.Double(nullable: false),
                        LastSignalRun = c.DateTime(nullable: false),
                        DollarAmount = c.Int(nullable: false),
                        MaxBlotterValue = c.Double(nullable: false),
                        TargetPriceChangeLong = c.Double(nullable: false),
                        TargetPriceChangeShort = c.Double(nullable: false),
                        SellStopChange = c.Double(nullable: false),
                        BuyStopChange = c.Double(nullable: false),
                        ReEnterWinningTrades = c.Boolean(nullable: false),
                        SameLosingTradeMaximum = c.Int(nullable: false),
                        AvgReturn = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.BackTestDatas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Side = c.String(nullable: false, maxLength: 50),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Opinion = c.String(maxLength: 50),
                        Price = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        PriceChangePcnt = c.Double(nullable: false),
                        Volume = c.Double(),
                        AvgVol = c.Double(),
                        VolumeChange = c.Double(nullable: false),
                        DayHigh = c.Double(nullable: false),
                        DayLow = c.Double(nullable: false),
                        YearHigh = c.Double(nullable: false),
                        YearLow = c.Double(nullable: false),
                        CurrentRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        Direction = c.String(maxLength: 50),
                        RankChange = c.Int(nullable: false),
                        MA50 = c.Double(nullable: false),
                        MA200 = c.Double(nullable: false),
                        Dividend = c.Double(nullable: false),
                        DividendPct = c.Double(nullable: false),
                        YearAgoPrice = c.Double(nullable: false),
                        YearPriceChange = c.Double(nullable: false),
                        SixMonAgoPrice = c.Double(nullable: false),
                        SixMonPriceChange = c.Double(nullable: false),
                        ThreeMonAgoPrice = c.Double(nullable: false),
                        ThreeMonPriceChange = c.Double(nullable: false),
                        Beta = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.BlotterEntries", "TradingSettings", c => c.Int(nullable: false));
            AddColumn("dbo.TradeHistoryEntries", "TradingSettings", c => c.Int(nullable: false));
            AddColumn("dbo.TradeSignals", "TradingSettings", c => c.Int(nullable: false));
            AddColumn("dbo.TradeSignalHistories", "TradingSettings", c => c.Int(nullable: false));
            AddColumn("dbo.MarketDatas", "Beta", c => c.Double(nullable: false));
            DropColumn("dbo.TradeHistoryEntries", "TradingPlan");
            DropColumn("dbo.MarketDatas", "Opinion");
            DropTable("dbo.SiteConfigurationSettings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SiteConfigurationSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SellStopChange = c.Double(nullable: false),
                        BadPriceChange = c.Double(nullable: false),
                        VolumeChangeTrigger = c.Double(nullable: false),
                        LastDataLoad = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.MarketDatas", "Opinion", c => c.String(maxLength: 50));
            AddColumn("dbo.TradeHistoryEntries", "TradingPlan", c => c.String(maxLength: 50));
            DropColumn("dbo.MarketDatas", "Beta");
            DropColumn("dbo.TradeSignalHistories", "TradingSettings");
            DropColumn("dbo.TradeSignals", "TradingSettings");
            DropColumn("dbo.TradeHistoryEntries", "TradingSettings");
            DropColumn("dbo.BlotterEntries", "TradingSettings");
            DropTable("dbo.BackTestDatas");
            DropTable("dbo.TradingSettings");
        }
    }
}
