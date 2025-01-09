namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOpinionDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScanResults", "OpinionDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScanResults", "OpinionDate");
        }
    }
}
