namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStockPickingStrategies : DbMigration
    {
        public override void Up()
        {
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
                "dbo.StockPickingStrategies",
                c => new
                    {
                        StockPickingStrategyID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 140),
                        Description = c.String(maxLength: 240),
                    })
                .PrimaryKey(t => t.StockPickingStrategyID);
            
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockPickingStrategies", t => t.Strategy_StockPickingStrategyID)
                .Index(t => t.Strategy_StockPickingStrategyID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockPicks", new[] { "Strategy_StockPickingStrategyID" });
            DropForeignKey("dbo.StockPicks", "Strategy_StockPickingStrategyID", "dbo.StockPickingStrategies");
            DropTable("dbo.StockPicks");
            DropTable("dbo.StockPickingStrategies");
            DropTable("dbo.Stocks");
        }
    }
}
