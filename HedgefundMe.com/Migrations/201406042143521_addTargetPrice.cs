namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTargetPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TodaysTrades", "TargetPrice", c => c.Double(nullable: false));
            AddColumn("dbo.LatestTradeSignals", "TargetPrice", c => c.Double(nullable: false));
            AddColumn("dbo.TodaysShortTrades", "TargetPrice", c => c.Double(nullable: false));
            AddColumn("dbo.LatestShortSignals", "TargetPrice", c => c.Double(nullable: false));
            AddColumn("dbo.TopStocks", "TargetPrice", c => c.Double());
            AddColumn("dbo.BottomStocks", "TargetPrice", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BottomStocks", "TargetPrice");
            DropColumn("dbo.TopStocks", "TargetPrice");
            DropColumn("dbo.LatestShortSignals", "TargetPrice");
            DropColumn("dbo.TodaysShortTrades", "TargetPrice");
            DropColumn("dbo.LatestTradeSignals", "TargetPrice");
            DropColumn("dbo.TodaysTrades", "TargetPrice");
        }
    }
}
