namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomsQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TmpContainerItems", "CustomsQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TmpContainerItems", "CustomsQuantity");
        }
    }
}
