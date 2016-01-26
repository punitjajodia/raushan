namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeProductUnitString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TmpContainerItems", "ProductUnit", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TmpContainerItems", "ProductUnit", c => c.Int(nullable: false));
        }
    }
}
