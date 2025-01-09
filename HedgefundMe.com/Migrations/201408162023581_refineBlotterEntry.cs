namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refineBlotterEntry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlotterEntries", "OpenValue", c => c.Double(nullable: false));
            AddColumn("dbo.BlotterEntries", "CurrentValue", c => c.Double(nullable: false));
            AddColumn("dbo.BlotterEntries", "GainLossPct", c => c.Double(nullable: false));
            DropColumn("dbo.BlotterEntries", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BlotterEntries", "Value", c => c.Double(nullable: false));
            DropColumn("dbo.BlotterEntries", "GainLossPct");
            DropColumn("dbo.BlotterEntries", "CurrentValue");
            DropColumn("dbo.BlotterEntries", "OpenValue");
        }
    }
}
