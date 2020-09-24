namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthorFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthorFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileLocation = c.String(),
                        FileName = c.String(),
                        Active = c.String(),
                        Author_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .Index(t => t.Author_Id);
            
            AddColumn("dbo.AuthorDetails", "AuthorPicture", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthorFiles", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AuthorFiles", new[] { "Author_Id" });
            DropColumn("dbo.AuthorDetails", "AuthorPicture");
            DropTable("dbo.AuthorFiles");
        }
    }
}
