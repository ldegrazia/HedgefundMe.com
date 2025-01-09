namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeScandatefromPerformanceScan : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PerformanceScans", "Name", c => c.String(maxLength: 140));
            DropColumn("dbo.PerformanceScans", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PerformanceScans", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PerformanceScans", "Name", c => c.String(maxLength: 40));
        }
    }
}
