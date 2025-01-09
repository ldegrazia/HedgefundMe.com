namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPortfolioEntries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PortfolioEntries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Url = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PortfolioEntries");
        }
    }
}
