namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requestsTweaks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requests", "AdminAction_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Requests", "NextAction_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Requests", new[] { "AdminAction_Id" });
            DropIndex("dbo.Requests", new[] { "NextAction_Id" });
            AddColumn("dbo.Requests", "Declined", c => c.Boolean(nullable: false));
            AddColumn("dbo.Requests", "DeclinedDate", c => c.DateTime());
            DropColumn("dbo.Requests", "TripFeedbackRatingByAuthor");
            DropColumn("dbo.Requests", "TripFeedbackNotesByAuthor");
            DropColumn("dbo.Requests", "TripFeedbackRatingBySalesRep");
            DropColumn("dbo.Requests", "TripFeedbackNotesBySalesRep");
            DropColumn("dbo.Requests", "AdminAction_Id");
            DropColumn("dbo.Requests", "NextAction_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Requests", "NextAction_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Requests", "AdminAction_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Requests", "TripFeedbackNotesBySalesRep", c => c.String());
            AddColumn("dbo.Requests", "TripFeedbackRatingBySalesRep", c => c.String());
            AddColumn("dbo.Requests", "TripFeedbackNotesByAuthor", c => c.String());
            AddColumn("dbo.Requests", "TripFeedbackRatingByAuthor", c => c.String());
            DropColumn("dbo.Requests", "DeclinedDate");
            DropColumn("dbo.Requests", "Declined");
            CreateIndex("dbo.Requests", "NextAction_Id");
            CreateIndex("dbo.Requests", "AdminAction_Id");
            AddForeignKey("dbo.Requests", "NextAction_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Requests", "AdminAction_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
