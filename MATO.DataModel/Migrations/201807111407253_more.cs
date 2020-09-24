namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class more : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "EventCity", c => c.String());
            AddColumn("dbo.Events", "TalkType_Id", c => c.Int());
            AddColumn("dbo.Events", "TargetSector_Id", c => c.Int());
            CreateIndex("dbo.Events", "TalkType_Id");
            CreateIndex("dbo.Events", "TargetSector_Id");
            AddForeignKey("dbo.Events", "TalkType_Id", "dbo.TalkTypes", "Id");
            AddForeignKey("dbo.Events", "TargetSector_Id", "dbo.TargetSectors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "TargetSector_Id", "dbo.TargetSectors");
            DropForeignKey("dbo.Events", "TalkType_Id", "dbo.TalkTypes");
            DropIndex("dbo.Events", new[] { "TargetSector_Id" });
            DropIndex("dbo.Events", new[] { "TalkType_Id" });
            DropColumn("dbo.Events", "TargetSector_Id");
            DropColumn("dbo.Events", "TalkType_Id");
            DropColumn("dbo.Events", "EventCity");
        }
    }
}
