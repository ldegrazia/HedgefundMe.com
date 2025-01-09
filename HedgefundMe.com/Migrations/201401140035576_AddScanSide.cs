namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScanSide : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Scans", "ScanSide", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Scans", "ScanSide");
        }
    }
}
