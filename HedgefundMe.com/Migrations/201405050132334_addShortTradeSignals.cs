namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addShortTradeSignals : DbMigration
    {
        public override void Up()
        {
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
                "dbo.LatestShortSignals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        StopPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LatestShortSignals");
            DropTable("dbo.TodaysShortTrades");
        }
    }
}
