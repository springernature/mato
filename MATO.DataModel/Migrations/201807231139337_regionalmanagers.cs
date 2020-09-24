namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class regionalmanagers : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "Region_Id" });
            AlterColumn("dbo.AspNetUsers", "Region_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "Region_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "Region_Id" });
            AlterColumn("dbo.AspNetUsers", "Region_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "Region_Id");
        }
    }
}
