namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveExporterID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Containers", "ExporterID", "dbo.Exporters");
            DropIndex("dbo.Containers", new[] { "ExporterID" });
            DropColumn("dbo.Containers", "ExporterID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Containers", "ExporterID", c => c.Int(nullable: false));
            CreateIndex("dbo.Containers", "ExporterID");
            AddForeignKey("dbo.Containers", "ExporterID", "dbo.Exporters", "ExporterID", cascadeDelete: true);
        }
    }
}
