namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewTradeSignals : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LatestTradeSignals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        StopPrice = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LatestTradeSignals");
        }
    }
}
