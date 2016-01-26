namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameBuyerNameToMarka : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.TmpContainerItems", "BuyerName", "Marka");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.TmpContainerItems", "Marka", "BuyerName");
        }
    }
}
