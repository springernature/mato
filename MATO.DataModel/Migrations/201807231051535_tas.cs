namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TitleAuthorAssociations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author_Id = c.String(maxLength: 128),
                        Title_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .ForeignKey("dbo.PromotedTitles", t => t.Title_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Title_Id);
            
            AddColumn("dbo.PromotedTitles", "TargetSector_Id", c => c.Int());
            CreateIndex("dbo.PromotedTitles", "TargetSector_Id");
            AddForeignKey("dbo.PromotedTitles", "TargetSector_Id", "dbo.TargetSectors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TitleAuthorAssociations", "Title_Id", "dbo.PromotedTitles");
            DropForeignKey("dbo.TitleAuthorAssociations", "Author_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PromotedTitles", "TargetSector_Id", "dbo.TargetSectors");
            DropIndex("dbo.TitleAuthorAssociations", new[] { "Title_Id" });
            DropIndex("dbo.TitleAuthorAssociations", new[] { "Author_Id" });
            DropIndex("dbo.PromotedTitles", new[] { "TargetSector_Id" });
            DropColumn("dbo.PromotedTitles", "TargetSector_Id");
            DropTable("dbo.TitleAuthorAssociations");
        }
    }
}
