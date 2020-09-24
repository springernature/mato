namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class titlepromoToRequestFromEvent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "PromotedTitle_Id", "dbo.PromotedTitles");
            DropIndex("dbo.Events", new[] { "PromotedTitle_Id" });
            AddColumn("dbo.Requests", "PromotedTitle_Id", c => c.Int());
            CreateIndex("dbo.Requests", "PromotedTitle_Id");
            AddForeignKey("dbo.Requests", "PromotedTitle_Id", "dbo.PromotedTitles", "Id");
            DropColumn("dbo.Events", "PromotedTitle_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "PromotedTitle_Id", c => c.Int());
            DropForeignKey("dbo.Requests", "PromotedTitle_Id", "dbo.PromotedTitles");
            DropIndex("dbo.Requests", new[] { "PromotedTitle_Id" });
            DropColumn("dbo.Requests", "PromotedTitle_Id");
            CreateIndex("dbo.Events", "PromotedTitle_Id");
            AddForeignKey("dbo.Events", "PromotedTitle_Id", "dbo.PromotedTitles", "Id");
        }
    }
}
