namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocalAuthorContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "LocalAuthorContact", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "LocalAuthorContact");
        }
    }
}
