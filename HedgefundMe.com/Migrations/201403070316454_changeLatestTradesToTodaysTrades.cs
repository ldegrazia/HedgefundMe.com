namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeLatestTradesToTodaysTrades : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TodaysTrades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 4000),
                        Action = c.String(maxLength: 4000),
                        Price = c.Double(nullable: false),
                        PriceChange = c.Double(nullable: false),
                        Details = c.String(maxLength: 4000),
                        RankChange = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        VolumeChange = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropTable("dbo.LatestTrades");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LatestTrades",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Ticker = c.String(nullable: false, maxLength: 50),
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
            
            DropTable("dbo.TodaysTrades");
        }
    }
}
