namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddJobNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TmpContainerItems", "JobNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TmpContainerItems", "JobNumber");
        }
    }
}
