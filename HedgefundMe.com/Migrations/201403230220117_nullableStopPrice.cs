namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableStopPrice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TopStocks", "StopPrice", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TopStocks", "StopPrice", c => c.Double(nullable: false));
        }
    }
}
