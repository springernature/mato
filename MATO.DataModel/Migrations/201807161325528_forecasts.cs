namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forecasts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TitleForecasts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year1 = c.Int(nullable: false),
                        Year2 = c.Int(nullable: false),
                        Year3 = c.Int(nullable: false),
                        PromotedTitle_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PromotedTitles", t => t.PromotedTitle_Id)
                .Index(t => t.PromotedTitle_Id);
            
            AddColumn("dbo.Requests", "TitleForecast_Id", c => c.Int());
            CreateIndex("dbo.Requests", "TitleForecast_Id");
            AddForeignKey("dbo.Requests", "TitleForecast_Id", "dbo.TitleForecasts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "TitleForecast_Id", "dbo.TitleForecasts");
            DropForeignKey("dbo.TitleForecasts", "PromotedTitle_Id", "dbo.PromotedTitles");
            DropIndex("dbo.TitleForecasts", new[] { "PromotedTitle_Id" });
            DropIndex("dbo.Requests", new[] { "TitleForecast_Id" });
            DropColumn("dbo.Requests", "TitleForecast_Id");
            DropTable("dbo.TitleForecasts");
        }
    }
}
