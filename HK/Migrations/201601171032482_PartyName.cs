namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartyName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TmpContainerItems", "PartyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TmpContainerItems", "PartyName");
        }
    }
}
