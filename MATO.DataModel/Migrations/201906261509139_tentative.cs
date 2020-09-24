namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tentative : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "TentativeReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "TentativeReason");
        }
    }
}
