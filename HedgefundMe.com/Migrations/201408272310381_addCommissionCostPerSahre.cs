namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCommissionCostPerSahre : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TradingSettings", "CommissionCostPerShare", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TradingSettings", "CommissionCostPerShare");
        }
    }
}
