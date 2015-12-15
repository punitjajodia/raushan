namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExtraColumnsToContainer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "CostsIncluded", c => c.String());
            AddColumn("dbo.Containers", "HarmonicCodes", c => c.String());
            AddColumn("dbo.Containers", "TotalGrossWeight", c => c.String());
            AddColumn("dbo.Containers", "TotalCartons", c => c.String());
            AddColumn("dbo.Containers", "CountryOfOrigin", c => c.String());
            AddColumn("dbo.Containers", "BeneficiaryBank", c => c.String());
            AddColumn("dbo.Containers", "BeneficiarySwift", c => c.String());
            AddColumn("dbo.Containers", "BeneficiaryUsdAccount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Containers", "BeneficiaryUsdAccount");
            DropColumn("dbo.Containers", "BeneficiarySwift");
            DropColumn("dbo.Containers", "BeneficiaryBank");
            DropColumn("dbo.Containers", "CountryOfOrigin");
            DropColumn("dbo.Containers", "TotalCartons");
            DropColumn("dbo.Containers", "TotalGrossWeight");
            DropColumn("dbo.Containers", "HarmonicCodes");
            DropColumn("dbo.Containers", "CostsIncluded");
        }
    }
}
