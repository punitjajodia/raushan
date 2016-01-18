namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBillInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TmpContainerItems", "PartyPhone", c => c.String());
            AddColumn("dbo.TmpContainerItems", "BillOnBoardingDate", c => c.String());
            AddColumn("dbo.TmpContainerItems", "BillDeliveryDate", c => c.String());
            AddColumn("dbo.TmpContainerItems", "BillNumber", c => c.String());
            AddColumn("dbo.TmpContainerItems", "BillTTDAPNumber", c => c.String());
            AddColumn("dbo.TmpContainerItems", "BillTTDAPDate", c => c.String());
            AddColumn("dbo.TmpContainerItems", "LotSize", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TmpContainerItems", "LotSize");
            DropColumn("dbo.TmpContainerItems", "BillTTDAPDate");
            DropColumn("dbo.TmpContainerItems", "BillTTDAPNumber");
            DropColumn("dbo.TmpContainerItems", "BillNumber");
            DropColumn("dbo.TmpContainerItems", "BillDeliveryDate");
            DropColumn("dbo.TmpContainerItems", "BillOnBoardingDate");
            DropColumn("dbo.TmpContainerItems", "PartyPhone");
        }
    }
}
