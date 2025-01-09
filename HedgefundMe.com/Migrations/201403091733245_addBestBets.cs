namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBestBets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BestBetResults",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(maxLength: 4000),
                        DaysBack = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DollarCostAvg = c.Boolean(nullable: false),
                        BuyAmount = c.Int(nullable: false),
                        Pnl = c.Double(),
                        PcnlPercentage = c.Double(),
                        BuyInDollarAmount = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BestBetResults");
        }
    }
}
