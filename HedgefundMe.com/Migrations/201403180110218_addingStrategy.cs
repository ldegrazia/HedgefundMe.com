namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingStrategy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Strategies",
                c => new
                    {
                        StrategyID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 140),
                        Description = c.String(maxLength: 240),
                    })
                .PrimaryKey(t => t.StrategyID);
            
            CreateTable(
                "dbo.Trades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StrategyID = c.Int(nullable: false),
                        Action = c.String(maxLength: 40),
                        Price = c.Double(nullable: false),
                        Ticker = c.String(maxLength: 40),
                        Date = c.DateTime(nullable: false),
                        Shares = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Strategies", t => t.StrategyID, cascadeDelete: true)
                .Index(t => t.StrategyID);
            
            CreateTable(
                "dbo.PortfolioDataPoints",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StrategyID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                        JsonDate = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Strategies", t => t.StrategyID, cascadeDelete: true)
                .Index(t => t.StrategyID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PortfolioDataPoints", new[] { "StrategyID" });
            DropIndex("dbo.Trades", new[] { "StrategyID" });
            DropForeignKey("dbo.PortfolioDataPoints", "StrategyID", "dbo.Strategies");
            DropForeignKey("dbo.Trades", "StrategyID", "dbo.Strategies");
            DropTable("dbo.PortfolioDataPoints");
            DropTable("dbo.Trades");
            DropTable("dbo.Strategies");
        }
    }
}
