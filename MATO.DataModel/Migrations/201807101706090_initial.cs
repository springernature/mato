namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubmitDate = c.DateTime(nullable: false),
                        Finalised = c.Boolean(nullable: false),
                        FinalisedDate = c.DateTime(nullable: false),
                        Cancelled = c.Boolean(nullable: false),
                        CancelledDate = c.DateTime(nullable: false),
                        Priority = c.Int(nullable: false),
                        EventName = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        ArrivalDate = c.DateTime(nullable: false),
                        DepartureDate = c.DateTime(nullable: false),
                        ArriveAt = c.String(),
                        DepartFrom = c.String(),
                        BackgroundInfo = c.String(),
                        AuthorNotesBySalesRep = c.String(),
                        NonAuthorNotesBySalesRep = c.String(),
                        AuthorNotesByAdmin = c.String(),
                        NonAuthorNotesByAdmin = c.String(),
                        Complete = c.Boolean(nullable: false),
                        TripFeedbackRatingByAuthor = c.String(),
                        TripFeedbackNotesByAuthor = c.String(),
                        TripFeedbackRatingBySalesRep = c.String(),
                        TripFeedbackNotesBySalesRep = c.String(),
                        Address_Id = c.Int(),
                        AdminAction_Id = c.String(maxLength: 128),
                        Country_Id = c.Int(),
                        NextAction_Id = c.String(maxLength: 128),
                        RequestedAuthor_Id = c.String(maxLength: 128),
                        SubmitBy_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Address_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AdminAction_Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.NextAction_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.RequestedAuthor_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.SubmitBy_Id)
                .Index(t => t.Address_Id)
                .Index(t => t.AdminAction_Id)
                .Index(t => t.Country_Id)
                .Index(t => t.NextAction_Id)
                .Index(t => t.RequestedAuthor_Id)
                .Index(t => t.SubmitBy_Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsoCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "SubmitBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "RequestedAuthor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "NextAction_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.Requests", "AdminAction_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "Address_Id", "dbo.Addresses");
            DropIndex("dbo.Requests", new[] { "SubmitBy_Id" });
            DropIndex("dbo.Requests", new[] { "RequestedAuthor_Id" });
            DropIndex("dbo.Requests", new[] { "NextAction_Id" });
            DropIndex("dbo.Requests", new[] { "Country_Id" });
            DropIndex("dbo.Requests", new[] { "AdminAction_Id" });
            DropIndex("dbo.Requests", new[] { "Address_Id" });
            DropTable("dbo.Countries");
            DropTable("dbo.Requests");
            DropTable("dbo.EventTypes");
        }
    }
}
