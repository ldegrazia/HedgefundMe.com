namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameactionpricetosellprice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TodaysTrades", "SellPrice", c => c.Double(nullable: false));
            DropColumn("dbo.TodaysTrades", "ActionPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TodaysTrades", "ActionPrice", c => c.Double(nullable: false));
            DropColumn("dbo.TodaysTrades", "SellPrice");
        }
    }
}
