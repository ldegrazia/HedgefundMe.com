namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addActionPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TodaysTrades", "ActionPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TodaysTrades", "ActionPrice");
        }
    }
}
