namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionalRegionOnRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "Region_Id", c => c.Int());
            CreateIndex("dbo.Requests", "Region_Id");
            AddForeignKey("dbo.Requests", "Region_Id", "dbo.Regions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "Region_Id", "dbo.Regions");
            DropIndex("dbo.Requests", new[] { "Region_Id" });
            DropColumn("dbo.Requests", "Region_Id");
        }
    }
}
