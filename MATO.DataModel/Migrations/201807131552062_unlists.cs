namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unlists : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "PromotedTitle_Id", "dbo.PromotedTitles");
            DropIndex("dbo.AspNetUsers", new[] { "PromotedTitle_Id" });
            DropColumn("dbo.AspNetUsers", "PromotedTitle_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "PromotedTitle_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "PromotedTitle_Id");
            AddForeignKey("dbo.AspNetUsers", "PromotedTitle_Id", "dbo.PromotedTitles", "Id");
        }
    }
}
