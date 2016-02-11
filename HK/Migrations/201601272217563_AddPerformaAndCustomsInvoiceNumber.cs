namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPerformaAndCustomsInvoiceNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "PerformaInvoiceNumber", c => c.String());
            AddColumn("dbo.Containers", "CustomsInvoiceNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Containers", "CustomsInvoiceNumber");
            DropColumn("dbo.Containers", "PerformaInvoiceNumber");
        }
    }
}
