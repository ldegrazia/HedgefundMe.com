namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderToTrade : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trades", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trades", "Order");
        }
    }
}
