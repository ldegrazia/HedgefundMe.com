namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class opinionColumnLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Scans", "TimeFrame", c => c.String(maxLength: 100));
            AlterColumn("dbo.ScanResults", "Direction", c => c.String(maxLength: 50));
            AlterColumn("dbo.ScanResults", "Color", c => c.String(maxLength: 50));
            AlterColumn("dbo.ScanResults", "Opinion", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScanResults", "Opinion", c => c.String(maxLength: 4000));
            AlterColumn("dbo.ScanResults", "Color", c => c.String(maxLength: 4000));
            AlterColumn("dbo.ScanResults", "Direction", c => c.String(maxLength: 4000));
            AlterColumn("dbo.Scans", "TimeFrame", c => c.String(maxLength: 4000));
        }
    }
}
