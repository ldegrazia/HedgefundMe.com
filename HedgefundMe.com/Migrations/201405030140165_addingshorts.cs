namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingshorts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BottomStocks",
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
                        CurrentSide = c.String(maxLength: 10),
                        StopPrice = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BottomStocks");
        }
    }
}
