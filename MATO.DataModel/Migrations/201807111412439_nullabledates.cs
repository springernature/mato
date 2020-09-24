namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullabledates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Requests", "FinalisedDate", c => c.DateTime());
            AlterColumn("dbo.Requests", "CancelledDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Requests", "CancelledDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Requests", "FinalisedDate", c => c.DateTime(nullable: false));
        }
    }
}
