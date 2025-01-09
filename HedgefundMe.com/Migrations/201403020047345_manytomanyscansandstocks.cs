namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manytomanyscansandstocks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PerformanceScans", "TopStock_ID", "dbo.TopStocks");
            DropIndex("dbo.PerformanceScans", new[] { "TopStock_ID" });
            CreateTable(
                "dbo.TopStockPerformanceScans",
                c => new
                    {
                        TopStock_ID = c.Int(nullable: false),
                        PerformanceScan_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TopStock_ID, t.PerformanceScan_ID })
                .ForeignKey("dbo.TopStocks", t => t.TopStock_ID, cascadeDelete: true)
                .ForeignKey("dbo.PerformanceScans", t => t.PerformanceScan_ID, cascadeDelete: true)
                .Index(t => t.TopStock_ID)
                .Index(t => t.PerformanceScan_ID);
            
            DropColumn("dbo.PerformanceScans", "TopStock_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PerformanceScans", "TopStock_ID", c => c.Int());
            DropIndex("dbo.TopStockPerformanceScans", new[] { "PerformanceScan_ID" });
            DropIndex("dbo.TopStockPerformanceScans", new[] { "TopStock_ID" });
            DropForeignKey("dbo.TopStockPerformanceScans", "PerformanceScan_ID", "dbo.PerformanceScans");
            DropForeignKey("dbo.TopStockPerformanceScans", "TopStock_ID", "dbo.TopStocks");
            DropTable("dbo.TopStockPerformanceScans");
            CreateIndex("dbo.PerformanceScans", "TopStock_ID");
            AddForeignKey("dbo.PerformanceScans", "TopStock_ID", "dbo.TopStocks", "ID");
        }
    }
}
