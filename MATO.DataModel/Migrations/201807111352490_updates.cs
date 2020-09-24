namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requests", "Address_Id", "dbo.Addresses");
            DropForeignKey("dbo.Requests", "Country_Id", "dbo.Countries");
            DropIndex("dbo.Requests", new[] { "Address_Id" });
            DropIndex("dbo.Requests", new[] { "Country_Id" });
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventName = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        ArrivalDate = c.DateTime(nullable: false),
                        DepartureDate = c.DateTime(nullable: false),
                        ArriveAt = c.String(),
                        DepartFrom = c.String(),
                        EventType_Id = c.Int(),
                        PromotedTitle_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventTypes", t => t.EventType_Id)
                .ForeignKey("dbo.PromotedTitles", t => t.PromotedTitle_Id)
                .Index(t => t.EventType_Id)
                .Index(t => t.PromotedTitle_Id);
            
            AddColumn("dbo.Requests", "EventOne_Id", c => c.Int());
            AddColumn("dbo.Requests", "EventThree_Id", c => c.Int());
            AddColumn("dbo.Requests", "EventTwo_Id", c => c.Int());
            CreateIndex("dbo.Requests", "EventOne_Id");
            CreateIndex("dbo.Requests", "EventThree_Id");
            CreateIndex("dbo.Requests", "EventTwo_Id");
            AddForeignKey("dbo.Requests", "EventOne_Id", "dbo.Events", "Id");
            AddForeignKey("dbo.Requests", "EventThree_Id", "dbo.Events", "Id");
            AddForeignKey("dbo.Requests", "EventTwo_Id", "dbo.Events", "Id");
            DropColumn("dbo.Requests", "Priority");
            DropColumn("dbo.Requests", "EventName");
            DropColumn("dbo.Requests", "EventDate");
            DropColumn("dbo.Requests", "ArrivalDate");
            DropColumn("dbo.Requests", "DepartureDate");
            DropColumn("dbo.Requests", "ArriveAt");
            DropColumn("dbo.Requests", "DepartFrom");
            DropColumn("dbo.Requests", "BackgroundInfo");
            DropColumn("dbo.Requests", "Address_Id");
            DropColumn("dbo.Requests", "Country_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Requests", "Country_Id", c => c.Int());
            AddColumn("dbo.Requests", "Address_Id", c => c.Int());
            AddColumn("dbo.Requests", "BackgroundInfo", c => c.String());
            AddColumn("dbo.Requests", "DepartFrom", c => c.String());
            AddColumn("dbo.Requests", "ArriveAt", c => c.String());
            AddColumn("dbo.Requests", "DepartureDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Requests", "ArrivalDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Requests", "EventDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Requests", "EventName", c => c.String());
            AddColumn("dbo.Requests", "Priority", c => c.Int(nullable: false));
            DropForeignKey("dbo.Requests", "EventTwo_Id", "dbo.Events");
            DropForeignKey("dbo.Requests", "EventThree_Id", "dbo.Events");
            DropForeignKey("dbo.Requests", "EventOne_Id", "dbo.Events");
            DropForeignKey("dbo.Events", "PromotedTitle_Id", "dbo.PromotedTitles");
            DropForeignKey("dbo.Events", "EventType_Id", "dbo.EventTypes");
            DropIndex("dbo.Events", new[] { "PromotedTitle_Id" });
            DropIndex("dbo.Events", new[] { "EventType_Id" });
            DropIndex("dbo.Requests", new[] { "EventTwo_Id" });
            DropIndex("dbo.Requests", new[] { "EventThree_Id" });
            DropIndex("dbo.Requests", new[] { "EventOne_Id" });
            DropColumn("dbo.Requests", "EventTwo_Id");
            DropColumn("dbo.Requests", "EventThree_Id");
            DropColumn("dbo.Requests", "EventOne_Id");
            DropTable("dbo.Events");
            CreateIndex("dbo.Requests", "Country_Id");
            CreateIndex("dbo.Requests", "Address_Id");
            AddForeignKey("dbo.Requests", "Country_Id", "dbo.Countries", "Id");
            AddForeignKey("dbo.Requests", "Address_Id", "dbo.Addresses", "Id");
        }
    }
}
