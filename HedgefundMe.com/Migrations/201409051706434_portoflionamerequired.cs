namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class portoflionamerequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PortfolioEntries", "Name", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PortfolioEntries", "Name", c => c.String(maxLength: 100));
        }
    }
}
