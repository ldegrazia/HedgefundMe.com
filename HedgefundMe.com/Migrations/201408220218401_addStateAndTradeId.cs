namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStateAndTradeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlotterEntries", "TradeId", c => c.String(maxLength: 100));
            AddColumn("dbo.TradeHistoryEntries", "TradeId", c => c.String(maxLength: 100));
            AddColumn("dbo.TradeHistoryEntries", "State", c => c.String(maxLength: 50));
            AddColumn("dbo.TradeSignals", "TradeId", c => c.String(maxLength: 100));
            AddColumn("dbo.TradeSignalHistories", "TradeId", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TradeSignalHistories", "TradeId");
            DropColumn("dbo.TradeSignals", "TradeId");
            DropColumn("dbo.TradeHistoryEntries", "State");
            DropColumn("dbo.TradeHistoryEntries", "TradeId");
            DropColumn("dbo.BlotterEntries", "TradeId");
        }
    }
}
