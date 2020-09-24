namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lists : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PromotedTitle_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "PromotedTitle_Id");
            AddForeignKey("dbo.AspNetUsers", "PromotedTitle_Id", "dbo.PromotedTitles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "PromotedTitle_Id", "dbo.PromotedTitles");
            DropIndex("dbo.AspNetUsers", new[] { "PromotedTitle_Id" });
            DropColumn("dbo.AspNetUsers", "PromotedTitle_Id");
        }
    }
}
