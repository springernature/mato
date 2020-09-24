namespace MATO.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaymentType_IdIntas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TitleAuthorAssociations", "PaymentType_Id", c => c.Int());
            CreateIndex("dbo.TitleAuthorAssociations", "PaymentType_Id");
            AddForeignKey("dbo.TitleAuthorAssociations", "PaymentType_Id", "dbo.PaymentTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TitleAuthorAssociations", "PaymentType_Id", "dbo.PaymentTypes");
            DropIndex("dbo.TitleAuthorAssociations", new[] { "PaymentType_Id" });
            DropColumn("dbo.TitleAuthorAssociations", "PaymentType_Id");
        }
    }
}
