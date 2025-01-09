namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pnlToTodaysTrades : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TodaysTrades", "TwoWeekPnl", c => c.Double(nullable: false));
            AddColumn("dbo.TodaysTrades", "TwoWeekPnlPercent", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TodaysTrades", "TwoWeekPnlPercent");
            DropColumn("dbo.TodaysTrades", "TwoWeekPnl");
        }
    }
}
