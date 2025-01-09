namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOpinionToScanResult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScanResults", "Opinion", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScanResults", "Opinion");
        }
    }
}
