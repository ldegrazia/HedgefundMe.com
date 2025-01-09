namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexTickers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndexTickers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ticker = c.String(maxLength: 4000),
                        Index = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IndexTickers");
        }
    }
}
