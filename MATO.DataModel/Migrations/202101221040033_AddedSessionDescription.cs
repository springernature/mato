namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSessionDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "SessionDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "SessionDescription");
        }
    }
}
