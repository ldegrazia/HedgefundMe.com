namespace HedgefundMe.com.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateIndexTicker : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IndexTickers", "Index", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IndexTickers", "Index", c => c.Int(nullable: false));
        }
    }
}
