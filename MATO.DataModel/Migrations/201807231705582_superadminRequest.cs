namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class superadminRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "ManagerApproval", c => c.Boolean(nullable: false));
            AddColumn("dbo.Requests", "ManagerApprovalDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "ManagerApprovalDate");
            DropColumn("dbo.Requests", "ManagerApproval");
        }
    }
}
