namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomsProductUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TmpContainerItems", "CustomsProductUnit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TmpContainerItems", "CustomsProductUnit");
        }
    }
}
