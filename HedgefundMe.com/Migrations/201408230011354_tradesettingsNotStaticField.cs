namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tradesettingsNotStaticField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TradingSettings", "GoodPriceChange", c => c.Double(nullable: false));
            AddColumn("dbo.TradingSettings", "BadPriceChange", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TradingSettings", "BadPriceChange");
            DropColumn("dbo.TradingSettings", "GoodPriceChange");
        }
    }
}
