namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedImporterID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Containers", "ImporterID", "dbo.Importers");
            DropIndex("dbo.Containers", new[] { "ImporterID" });
            AddColumn("dbo.Containers", "ImporterTaxCertificateNumber", c => c.String());
            DropColumn("dbo.Containers", "ImporterID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Containers", "ImporterID", c => c.Int(nullable: false));
            DropColumn("dbo.Containers", "ImporterTaxCertificateNumber");
            CreateIndex("dbo.Containers", "ImporterID");
            AddForeignKey("dbo.Containers", "ImporterID", "dbo.Importers", "ImporterID", cascadeDelete: true);
        }
    }
}
