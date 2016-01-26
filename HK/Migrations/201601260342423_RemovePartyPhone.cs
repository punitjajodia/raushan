namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePartyPhone : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TmpContainerItems", "PartyPhone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TmpContainerItems", "PartyPhone", c => c.String());
        }
    }
}
