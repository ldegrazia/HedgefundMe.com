namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOpinionDate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ScanResults", "OpinionDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScanResults", "OpinionDate", c => c.DateTime(nullable: false));
        }
    }
}
