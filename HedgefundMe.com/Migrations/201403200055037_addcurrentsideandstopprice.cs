namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcurrentsideandstopprice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TopStocks", "CurrentSide", c => c.String(maxLength: 10));
            AddColumn("dbo.TopStocks", "StopPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TopStocks", "StopPrice");
            DropColumn("dbo.TopStocks", "CurrentSide");
        }
    }
}
