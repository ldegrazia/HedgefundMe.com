namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteConfigurations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteConfigurationSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SellStopChange = c.Double(nullable: false),
                        BadPriceChange = c.Double(nullable: false),
                        VolumeChangeTrigger = c.Double(nullable: false),
                        LastDataLoad = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteConfigurationSettings");
        }
    }
}
