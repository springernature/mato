namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class webinarCalendars : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebinarCalendars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WebinarRoomName = c.String(),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WebinarCalendars");
        }
    }
}
