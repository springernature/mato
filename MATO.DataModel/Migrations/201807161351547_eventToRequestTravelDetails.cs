namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventToRequestTravelDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "InboundDate", c => c.DateTime());
            AddColumn("dbo.Requests", "OutboundDate", c => c.DateTime());
            AddColumn("dbo.Requests", "Inbound", c => c.String());
            AddColumn("dbo.Requests", "Outbound", c => c.String());
            DropColumn("dbo.Events", "ArrivalDate");
            DropColumn("dbo.Events", "DepartureDate");
            DropColumn("dbo.Events", "ArriveAt");
            DropColumn("dbo.Events", "DepartFrom");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "DepartFrom", c => c.String());
            AddColumn("dbo.Events", "ArriveAt", c => c.String());
            AddColumn("dbo.Events", "DepartureDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "ArrivalDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Requests", "Outbound");
            DropColumn("dbo.Requests", "Inbound");
            DropColumn("dbo.Requests", "OutboundDate");
            DropColumn("dbo.Requests", "InboundDate");
        }
    }
}
