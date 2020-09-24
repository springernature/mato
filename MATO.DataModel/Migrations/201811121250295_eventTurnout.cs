namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventTurnout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "ExpectedTurnout", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "ExpectedTurnout");
        }
    }
}
