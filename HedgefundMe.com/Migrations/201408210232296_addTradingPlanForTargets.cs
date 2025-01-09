namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTradingPlanForTargets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TradeHistoryEntries", "TradingPlan", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TradeHistoryEntries", "TradingPlan");
        }
    }
}
