namespace HK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuyerInvoiceItems",
                c => new
                    {
                        BuyerInvoiceItemID = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cartons = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductContainerBuyerPrice_ProductContainerBuyerPriceID = c.Int(),
                    })
                .PrimaryKey(t => t.BuyerInvoiceItemID)
                .ForeignKey("dbo.ProductContainerBuyerPrices", t => t.ProductContainerBuyerPrice_ProductContainerBuyerPriceID)
                .Index(t => t.ProductContainerBuyerPrice_ProductContainerBuyerPriceID);
            
            CreateTable(
                "dbo.ProductContainerBuyerPrices",
                c => new
                    {
                        ProductContainerBuyerPriceID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        ContainerID = c.Int(nullable: false),
                        BuyerID = c.Int(nullable: false),
                        Unit = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductContainerBuyerPriceID)
                .ForeignKey("dbo.Buyers", t => t.BuyerID, cascadeDelete: true)
                .ForeignKey("dbo.Containers", t => t.ContainerID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.ContainerID)
                .Index(t => t.BuyerID);
            
            CreateTable(
                "dbo.Buyers",
                c => new
                    {
                        BuyerID = c.Int(nullable: false, identity: true),
                        BuyerCode = c.String(),
                        BuyerName = c.String(),
                    })
                .PrimaryKey(t => t.BuyerID);
            
            CreateTable(
                "dbo.ContainerItems",
                c => new
                    {
                        ContainerItemID = c.Int(nullable: false, identity: true),
                        ContainerID = c.Int(nullable: false),
                        CartonNumber = c.String(),
                        BuyerID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        QuantityType = c.Int(nullable: false),
                        Cartons = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ContainerItemID)
                .ForeignKey("dbo.Buyers", t => t.BuyerID, cascadeDelete: true)
                .ForeignKey("dbo.Containers", t => t.ContainerID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ContainerID)
                .Index(t => t.BuyerID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Containers",
                c => new
                    {
                        ContainerID = c.Int(nullable: false, identity: true),
                        ContainerNumber = c.String(),
                        Date = c.DateTime(nullable: false),
                        ShippedPer = c.String(),
                        OnAbout = c.String(),
                        From = c.String(),
                        AirwayBillNumber = c.String(),
                        LetterOfCreditNumber = c.String(),
                        DrawnUnder = c.String(),
                        ImporterID = c.Int(nullable: false),
                        ExporterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ContainerID)
                .ForeignKey("dbo.Exporters", t => t.ExporterID, cascadeDelete: true)
                .ForeignKey("dbo.Importers", t => t.ImporterID, cascadeDelete: true)
                .Index(t => t.ImporterID)
                .Index(t => t.ExporterID);
            
            CreateTable(
                "dbo.Exporters",
                c => new
                    {
                        ExporterID = c.Int(nullable: false, identity: true),
                        ExporterName = c.String(nullable: false),
                        ExporterAddress = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ExporterID);
            
            CreateTable(
                "dbo.Importers",
                c => new
                    {
                        ImporterID = c.Int(nullable: false, identity: true),
                        ImporterName = c.String(nullable: false),
                        ImporterAddress = c.String(nullable: false),
                        TaxCertificateNumber = c.String(),
                    })
                .PrimaryKey(t => t.ImporterID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductCode = c.String(),
                        ProductName = c.String(),
                        ProductUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductUnit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        InvoiceItemID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        ProductCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.Int(nullable: false),
                        ProductContainerPrice_ProductContainerPriceID = c.Int(),
                    })
                .PrimaryKey(t => t.InvoiceItemID)
                .ForeignKey("dbo.ProductContainerPrices", t => t.ProductContainerPrice_ProductContainerPriceID)
                .Index(t => t.ProductContainerPrice_ProductContainerPriceID);
            
            CreateTable(
                "dbo.ProductContainerPrices",
                c => new
                    {
                        ProductContainerPriceID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityType = c.Int(nullable: false),
                        ContainerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductContainerPriceID)
                .ForeignKey("dbo.Containers", t => t.ContainerID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.ContainerID);
            
            CreateTable(
                "dbo.TmpContainerItems",
                c => new
                    {
                        TmpContainerItemID = c.Int(nullable: false, identity: true),
                        ContainerID = c.Int(nullable: false),
                        CartonNumber = c.String(),
                        BuyerName = c.String(),
                        ProductCustomsName = c.String(),
                        ProductBuyerName = c.String(),
                        ProductUnit = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cartons = c.Int(nullable: false),
                        BuyerCurrency = c.String(),
                        BuyerUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomsCurrency = c.String(),
                        CustomsUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TmpContainerItemID)
                .ForeignKey("dbo.Containers", t => t.ContainerID, cascadeDelete: true)
                .Index(t => t.ContainerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TmpContainerItems", "ContainerID", "dbo.Containers");
            DropForeignKey("dbo.InvoiceItems", "ProductContainerPrice_ProductContainerPriceID", "dbo.ProductContainerPrices");
            DropForeignKey("dbo.ProductContainerPrices", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductContainerPrices", "ContainerID", "dbo.Containers");
            DropForeignKey("dbo.BuyerInvoiceItems", "ProductContainerBuyerPrice_ProductContainerBuyerPriceID", "dbo.ProductContainerBuyerPrices");
            DropForeignKey("dbo.ProductContainerBuyerPrices", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductContainerBuyerPrices", "ContainerID", "dbo.Containers");
            DropForeignKey("dbo.ProductContainerBuyerPrices", "BuyerID", "dbo.Buyers");
            DropForeignKey("dbo.ContainerItems", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Containers", "ImporterID", "dbo.Importers");
            DropForeignKey("dbo.Containers", "ExporterID", "dbo.Exporters");
            DropForeignKey("dbo.ContainerItems", "ContainerID", "dbo.Containers");
            DropForeignKey("dbo.ContainerItems", "BuyerID", "dbo.Buyers");
            DropIndex("dbo.TmpContainerItems", new[] { "ContainerID" });
            DropIndex("dbo.ProductContainerPrices", new[] { "ContainerID" });
            DropIndex("dbo.ProductContainerPrices", new[] { "ProductID" });
            DropIndex("dbo.InvoiceItems", new[] { "ProductContainerPrice_ProductContainerPriceID" });
            DropIndex("dbo.Containers", new[] { "ExporterID" });
            DropIndex("dbo.Containers", new[] { "ImporterID" });
            DropIndex("dbo.ContainerItems", new[] { "ProductID" });
            DropIndex("dbo.ContainerItems", new[] { "BuyerID" });
            DropIndex("dbo.ContainerItems", new[] { "ContainerID" });
            DropIndex("dbo.ProductContainerBuyerPrices", new[] { "BuyerID" });
            DropIndex("dbo.ProductContainerBuyerPrices", new[] { "ContainerID" });
            DropIndex("dbo.ProductContainerBuyerPrices", new[] { "ProductID" });
            DropIndex("dbo.BuyerInvoiceItems", new[] { "ProductContainerBuyerPrice_ProductContainerBuyerPriceID" });
            DropTable("dbo.TmpContainerItems");
            DropTable("dbo.ProductContainerPrices");
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.Products");
            DropTable("dbo.Importers");
            DropTable("dbo.Exporters");
            DropTable("dbo.Containers");
            DropTable("dbo.ContainerItems");
            DropTable("dbo.Buyers");
            DropTable("dbo.ProductContainerBuyerPrices");
            DropTable("dbo.BuyerInvoiceItems");
        }
    }
}
