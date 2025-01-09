namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeOldRankColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TopStocks", "Rank");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TopStocks", "Rank", c => c.Int(nullable: false));
        }
    }
}
