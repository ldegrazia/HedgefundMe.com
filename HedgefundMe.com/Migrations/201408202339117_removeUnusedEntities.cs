namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUnusedEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Trades", "StrategyID", "dbo.Strategies");
            DropForeignKey("dbo.PortfolioDataPoints", "StrategyID", "dbo.Strategies");
            DropForeignKey("dbo.StockPicks", "Strategy_StockPickingStrategyID", "dbo.StockPickingStrategies");
            DropForeignKey("dbo.TopStockPerformanceScans", "TopStock_ID", "dbo.TopStocks");
            DropForeignKey("dbo.TopStockPerformanceScans", "PerformanceScan_ID", "dbo.PerformanceScans");
            DropIndex("dbo.Trades", new[] { "StrategyID" });
            DropIndex("dbo.PortfolioDataPoints", new[] { "StrategyID" });
            DropIndex("dbo.StockPicks", new[] { "Strategy_StockPickingStrategyID" });
            DropIndex("dbo.TopStockPerformanceScans", new[] { "TopStock_ID" });
            DropIndex("dbo.TopStockPerformanceScans", new[] { "PerformanceScan_ID" });
            AlterColumn("dbo.BlotterEntries", "StopPrice", c => c.Double(nullable: false));
            AlterColumn("dbo.BlotterEntries", "TargetPrice", c => c.Double(nullable: false));
            AlterColumn("dbo.TradeSignals", "StopPrice", c => c.Double(nullable: false));
            AlterColumn("dbo.TradeSignals", "TargetPrice", c => c.Double(nullable: false));
            DropTable("dbo.TodaysTrades");
            DropTable("dbo.LatestTradeSignals");
            DropTable("dbo.TodaysShortTrades");
            DropTable("dbo.LatestShortSignals");
            DropTable("dbo.TopStocks");
            DropTable("dbo.PerformanceScans");
            DropTable("dbo.BottomStocks");
            DropTable("dbo.BestBetResults");
            DropTable("dbo.Strategies");
            DropTable("dbo.Trades");
            DropTable("dbo.PortfolioDataPoints");
            DropTable("dbo.Stocks");
            DropTable("dbo.StockPickingStrategies");
            DropTable("dbo.StockPicks");
            DropTable("dbo.TopStockPerformanceScans");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TopStockPerformanceScans",
                c => new
                    {
                        TopStock_ID = c.Int(nullable: false),
                        PerformanceScan_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TopStock_ID, t.PerformanceScan_ID });
            
            CreateTable(
                "dbo.StockPicks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Opinion = c.String(maxLength: 4000),
                        Strategy_StockPickingStrategyID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StockPickingStrategies",
                c => new
                    {
                        StockPickingStrategyID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 140),
                        Description = c.String(maxLength: 240),
                    })
                .PrimaryKey(t => t.StockPickingStrategyID);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Price = c.Double(nullable: false),
                        AvgVol = c.Double(),
                        Volume = c.Double(),
                        CurrentRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        Direction = c.String(maxLength: 50),
                        RankChange = c.Int(nullable: false),
                        Gain = c.Double(nullable: false),
                        Color = c.String(maxLength: 50),
                        VolumeChange = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PortfolioDataPoints",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StrategyID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                        JsonDate = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Trades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        StrategyID = c.Int(nullable: false),
                        Action = c.String(maxLength: 40),
                        Price = c.Double(nullable: false),
                        Ticker = c.String(maxLength: 40),
                        Date = c.DateTime(nullable: false),
                        Shares = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Strategies",
                c => new
                    {
                        StrategyID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 140),
                        Description = c.String(maxLength: 240),
                    })
                .PrimaryKey(t => t.StrategyID);
            
            CreateTable(
                "dbo.BestBetResults",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(maxLength: 4000),
                        DaysBack = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DollarCostAvg = c.Boolean(nullable: false),
                        BuyAmount = c.Int(nullable: false),
                        Pnl = c.Double(),
                        PcnlPercentage = c.Double(),
                        BuyInDollarAmount = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.BottomStocks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Price = c.Double(nullable: false),
                        AvgVol = c.Double(),
                        Volume = c.Double(),
                        CurrentRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        Direction = c.String(maxLength: 50),
                        RankChange = c.Int(nullable: false),
                        Gain = c.Double(nullable: false),
                        DaysInList = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        StartPrice = c.Double(nullable: false),
                        GainTillNow = c.Double(nullable: false),
                        Color = c.String(maxLength: 50),
                        VolumeChange = c.Double(nullable: false),
                        Opinion = c.String(maxLength: 50),
                        CurrentSide = c.String(maxLength: 10),
                        StopPrice = c.Double(),
                        TargetPrice = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PerformanceScans",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 140),
                        Description = c.String(maxLength: 140),
                        ScanSide = c.String(maxLength: 140),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TopStocks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Price = c.Double(nullable: false),
                        AvgVol = c.Double(),
                        Volume = c.Double(),
                        CurrentRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        Direction = c.String(maxLength: 50),
                        RankChange = c.Int(nullable: false),
                        Gain = c.Double(nullable: false),
                        DaysInList = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        StartPrice = c.Double(nullable: false),
                        GainTillNow = c.Double(nullable: false),
                        Color = c.String(maxLength: 50),
                        VolumeChange = c.Double(nullable: false),
                        Opinion = c.String(maxLength: 50),
                        CurrentSide = c.String(maxLength: 10),
                        StopPrice = c.Double(),
                        TargetPrice = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LatestShortSignals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        StopPrice = c.Double(nullable: false),
                        TargetPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TodaysShortTrades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        SellPrice = c.Double(nullable: false),
                        TargetPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                        RankChange = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        VolumeChange = c.Double(nullable: false),
                        TwoWeekPnl = c.Double(nullable: false),
                        TwoWeekPnlPercent = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LatestTradeSignals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        StopPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        TargetPrice = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TodaysTrades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        SellPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        TargetPrice = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                        RankChange = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        VolumeChange = c.Double(nullable: false),
                        TwoWeekPnl = c.Double(nullable: false),
                        TwoWeekPnlPercent = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("dbo.TradeSignals", "TargetPrice", c => c.Double());
            AlterColumn("dbo.TradeSignals", "StopPrice", c => c.Double());
            AlterColumn("dbo.BlotterEntries", "TargetPrice", c => c.Double());
            AlterColumn("dbo.BlotterEntries", "StopPrice", c => c.Double());
            CreateIndex("dbo.TopStockPerformanceScans", "PerformanceScan_ID");
            CreateIndex("dbo.TopStockPerformanceScans", "TopStock_ID");
            CreateIndex("dbo.StockPicks", "Strategy_StockPickingStrategyID");
            CreateIndex("dbo.PortfolioDataPoints", "StrategyID");
            CreateIndex("dbo.Trades", "StrategyID");
            AddForeignKey("dbo.TopStockPerformanceScans", "PerformanceScan_ID", "dbo.PerformanceScans", "ID", cascadeDelete: true);
            AddForeignKey("dbo.TopStockPerformanceScans", "TopStock_ID", "dbo.TopStocks", "ID", cascadeDelete: true);
            AddForeignKey("dbo.StockPicks", "Strategy_StockPickingStrategyID", "dbo.StockPickingStrategies", "StockPickingStrategyID");
            AddForeignKey("dbo.PortfolioDataPoints", "StrategyID", "dbo.Strategies", "StrategyID", cascadeDelete: true);
            AddForeignKey("dbo.Trades", "StrategyID", "dbo.Strategies", "StrategyID", cascadeDelete: true);
        }
    }
}
