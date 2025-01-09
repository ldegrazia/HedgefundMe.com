namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSiteAudts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteAuditRecords",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastSignalFetchBy = c.String(maxLength: 100),
                        LastSignalFetchDate = c.DateTime(nullable: false),
                        LastPricingFetchBy = c.String(maxLength: 100),
                        LastPricingFetchDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteAuditRecords");
        }
    }
}
