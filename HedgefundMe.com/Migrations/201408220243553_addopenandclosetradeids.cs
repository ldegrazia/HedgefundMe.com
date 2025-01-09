namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addopenandclosetradeids : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TradeHistoryEntries", "TradeIdOpen", c => c.String(maxLength: 100));
            AddColumn("dbo.TradeHistoryEntries", "TradeIdClose", c => c.String(maxLength: 100));
            DropColumn("dbo.TradeHistoryEntries", "TradeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TradeHistoryEntries", "TradeId", c => c.String(maxLength: 100));
            DropColumn("dbo.TradeHistoryEntries", "TradeIdClose");
            DropColumn("dbo.TradeHistoryEntries", "TradeIdOpen");
        }
    }
}
