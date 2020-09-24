namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class regionAsInt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Region_Id", "dbo.Regions");
            DropIndex("dbo.AspNetUsers", new[] { "Region_Id" });
            AddColumn("dbo.AspNetUsers", "Region", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "Region_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Region_Id", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "Region");
            CreateIndex("dbo.AspNetUsers", "Region_Id");
            AddForeignKey("dbo.AspNetUsers", "Region_Id", "dbo.Regions", "Id", cascadeDelete: true);
        }
    }
}
