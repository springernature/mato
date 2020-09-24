namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tweaks : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AuthorFiles", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AuthorFiles", "Active", c => c.String());
        }
    }
}
