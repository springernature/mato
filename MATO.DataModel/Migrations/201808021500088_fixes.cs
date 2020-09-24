namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "AdminApproval", c => c.Boolean(nullable: false));
            AddColumn("dbo.Requests", "AdminApprovalDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "AdminApprovalDate");
            DropColumn("dbo.Requests", "AdminApproval");
        }
    }
}
