namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class introduceNewObjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlotterEntries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Side = c.String(nullable: false, maxLength: 50),
                        Strategy = c.String(maxLength: 50),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        PurchasePrice = c.Double(nullable: false),
                        CurrentPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        PriceChangePcnt = c.Double(nullable: false),
                        Volume = c.Double(),
                        AvgVol = c.Double(),
                        VolumeChange = c.Double(nullable: false),
                        Color = c.String(maxLength: 50),
                        DayHigh = c.Double(nullable: false),
                        DayLow = c.Double(nullable: false),
                        YearHigh = c.Double(nullable: false),
                        YearLow = c.Double(nullable: false),
                        Shares = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        GainLoss = c.Double(nullable: false),
                        StopPrice = c.Double(),
                        TargetPrice = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TradeHistoryEntries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Side = c.String(nullable: false, maxLength: 50),
                        OpenAction = c.String(maxLength: 50),
                        OpenPrice = c.Double(nullable: false),
                        OpenDate = c.DateTime(nullable: false),
                        OpenDetails = c.String(maxLength: 255),
                        CloseAction = c.String(maxLength: 50),
                        ClosePrice = c.Double(nullable: false),
                        CloseDate = c.DateTime(nullable: false),
                        CloseDetails = c.String(maxLength: 255),
                        Shares = c.Int(nullable: false),
                        Commision = c.Double(nullable: false),
                        OpenValue = c.Double(nullable: false),
                        CloseValue = c.Double(nullable: false),
                        GainLossPct = c.Double(nullable: false),
                        GainLoss = c.Double(nullable: false),
                        Strategy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TradeSignals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Action = c.String(maxLength: 50),
                        Price = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                        StopPrice = c.Double(),
                        TargetPrice = c.Double(),
                        Shares = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        Details = c.String(maxLength: 255),
                        Strategy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TradeSignalHistories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Action = c.String(maxLength: 50),
                        Price = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                        StopPrice = c.Double(),
                        TargetPrice = c.Double(),
                        Shares = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        Details = c.String(maxLength: 255),
                        Strategy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.MarketDatas",
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
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MarketDatas");
            DropTable("dbo.TradeSignalHistories");
            DropTable("dbo.TradeSignals");
            DropTable("dbo.TradeHistoryEntries");
            DropTable("dbo.BlotterEntries");
        }
    }
}
