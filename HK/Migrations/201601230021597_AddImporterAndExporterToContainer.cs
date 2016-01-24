namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImporterAndExporterToContainer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "ImporterName", c => c.String());
            AddColumn("dbo.Containers", "ImporterAddress", c => c.String());
            AddColumn("dbo.Containers", "ExporterName", c => c.String());
            AddColumn("dbo.Containers", "ExporterAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Containers", "ExporterAddress");
            DropColumn("dbo.Containers", "ExporterName");
            DropColumn("dbo.Containers", "ImporterAddress");
            DropColumn("dbo.Containers", "ImporterName");
        }
    }
}
