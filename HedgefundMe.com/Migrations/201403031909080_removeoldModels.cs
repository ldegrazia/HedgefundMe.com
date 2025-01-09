namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeoldModels : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.ScanResults", "ScanID", "dbo.Scans");
            //DropIndex("dbo.ScanResults", new[] { "ScanID" });
            DropTable("dbo.Rankings");
            DropTable("dbo.ScanResults");
            DropTable("dbo.Scans");
           
            DropTable("dbo.IndexTickers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.IndexTickers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ticker = c.String(maxLength: 4000),
                        Index = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScanResults",
                c => new
                    {
                        ScanResultID = c.Int(nullable: false, identity: true),
                        Ticker = c.String(maxLength: 4000),
                        Date = c.DateTime(nullable: false),
                        CurrentRank = c.Int(nullable: false),
                        PreviousRank = c.Int(nullable: false),
                        Direction = c.String(maxLength: 50),
                        Price = c.Double(nullable: false),
                        RankChange = c.Int(nullable: false),
                        Gain = c.Double(nullable: false),
                        DaysInList = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        StartPrice = c.Double(nullable: false),
                        GainTillNow = c.Double(nullable: false),
                        Color = c.String(maxLength: 50),
                        AvgVol = c.Double(),
                        Volume = c.Double(),
                        VolumeChange = c.Double(nullable: false),
                        Opinion = c.String(maxLength: 50),
                        ScanID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScanResultID);
            
            CreateTable(
                "dbo.Scans",
                c => new
                    {
                        ScanID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 40),
                        Description = c.String(maxLength: 4000),
                        ScanDate = c.DateTime(nullable: false),
                        TimeFrame = c.String(maxLength: 100),
                        ScanSide = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ScanID);
            
            CreateTable(
                "dbo.Rankings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 4000),
                        Rank = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        AvgVol = c.Double(),
                        Volume = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.ScanResults", "ScanID");
            AddForeignKey("dbo.ScanResults", "ScanID", "dbo.Scans", "ScanID", cascadeDelete: true);
        }
    }
}
