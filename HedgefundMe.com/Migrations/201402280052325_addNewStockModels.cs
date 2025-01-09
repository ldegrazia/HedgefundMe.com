namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewStockModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TopStocks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
                        Rank = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        AvgVol = c.Double(),
                        Volume = c.Double(),
                        CurrentRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        Direction = c.String(maxLength: 50),
                        RankChange = c.Int(nullable: false),
                        Gain = c.Double(nullable: false),
                        DaysInList = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        StartPrice = c.Double(nullable: false),
                        GainTillNow = c.Double(nullable: false),
                        Color = c.String(maxLength: 50),
                        VolumeChange = c.Double(nullable: false),
                        Opinion = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PerformanceScans",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 40),
                        Description = c.String(maxLength: 140),
                        Date = c.DateTime(nullable: false),
                        ScanSide = c.String(maxLength: 140),
                        TopStock_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TopStocks", t => t.TopStock_ID)
                .Index(t => t.TopStock_ID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PerformanceScans", new[] { "TopStock_ID" });
            DropForeignKey("dbo.PerformanceScans", "TopStock_ID", "dbo.TopStocks");
            DropTable("dbo.PerformanceScans");
            DropTable("dbo.TopStocks");
        }
    }
}
