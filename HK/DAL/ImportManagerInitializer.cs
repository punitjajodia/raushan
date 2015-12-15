using HK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HK.DAL
{
    public class ImportManagerInitializer : DropCreateDatabaseIfModelChanges<ImportManagerContext>
    {
        protected override void Seed(ImportManagerContext context)
        {
            var importers = new List<Importer> {
                new Importer {
                    ImporterName = "M/S. DHANALAXMI ENTERPRISES",
                    ImporterAddress = "KATHMANDU, NEPAL"
                },
                new Importer {
                    ImporterName = "M/S. PUNIT TRADERS",
                    ImporterAddress = "BHOTEBAHAL, KATHMANDU"
                }
            };

            importers.ForEach(c => context.Importers.Add(c));
            context.SaveChanges();

            var exporters = new List<Exporter> {
                new Exporter {
                    ExporterName = "SAI OVERSEAS (HK) LIMITED",
                    ExporterAddress = "ROOM 705, HO KING COMMERCIAL CENTRE\n2-16 FA YUAN STREET, MONGKOK\nKOWLOON, HONG KONG."
                }, 
                new Exporter {
                    ExporterName = "LAXMAN OVERSEAS (BK) LIMITED",
                    ExporterAddress = "HOUSE NO. 2843, BANGKOK CENTRAL COMMERCIAL CENTRE\nTIKIRO STREET\nBANGKOK, THAILAND."
                }
            };

            exporters.ForEach(c => context.Exporters.Add(c));
            context.SaveChanges();

            

            var containers = new List<Container> {
                new Container { 
                    ContainerNumber = "SI-150613/2015", 
                    Date = DateTime.Now, 
                    ShippedPer="Hello", 
                    OnAbout="", 
                    From="CHINA TO CALCUTTA SEA PORT OF INDIA IN TRANSIT TO NEPAL VIA BIRGUNJ CUSTOM OFFICE BIRGUNJ",
                    AirwayBillNumber = "",
                    LetterOfCreditNumber = "D/P SYSTEM",
                    DrawnUnder = "NEPAL CREDIT AND COMMERCE BANK LTD. KATHMANDU NP",
                    ImporterID = 1,
                    ExporterID = 1
                    //Importer = "M/S. DHANALAXMI ENTERPRISES KATHMANDU, NEPAL (TAX CERTIFICATE NO. 300704864)"
                },
                new Container { 
                    ContainerNumber = "SI-452324/2015",
                    Date = DateTime.Now, 
                    ShippedPer="Hello", 
                    OnAbout="", 
                    From="CHINA TO CALCUTTA SEA PORT OF INDIA IN TRANSIT TO NEPAL VIA BIRGUNJ CUSTOM OFFICE BIRGUNJ",
                    AirwayBillNumber = "",
                    LetterOfCreditNumber = "D/P SYSTEM",
                    DrawnUnder = "NEPAL CREDIT AND COMMERCE BANK LTD. KATHMANDU NP",
                    ImporterID = 1,
                    ExporterID = 2
                   // Importer = "M/S. DHANALAXMI ENTERPRISES KATHMANDU, NEPAL (TAX CERTIFICATE NO. 300704864)"
                }
            };
            containers.ForEach(c => context.Containers.Add(c));
            context.SaveChanges();

            var buyers = new List<Buyer>
            {
                new Buyer { BuyerCode="PWKR"},
                new Buyer { BuyerCode="NAMO"}
            };

            buyers.ForEach(b => context.Buyers.Add(b));

            var products = new List<Product>
            {
                new Product { ProductCode="OUTER", ProductUnitPrice = 5.00M, ProductUnit = ProductUnit.DOZ},
                new Product { ProductCode="GIRLS_VEST", ProductName="GIRLS VEST", ProductUnitPrice = 41.55M, ProductUnit = ProductUnit.PCS }
            };

            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            var containerItems = new List<ContainerItem>
            {
                new ContainerItem {ContainerID = 1, BuyerID = 1, ProductID = 1, Quantity=20, QuantityType=ProductUnit.DOZ },
                new ContainerItem {ContainerID = 1, BuyerID = 2, ProductID = 2, Quantity=100, QuantityType=ProductUnit.PCS },
                new ContainerItem {ContainerID = 1, BuyerID = 1, ProductID = 2, Quantity=50, QuantityType=ProductUnit.PCS },
                new ContainerItem {ContainerID = 1, BuyerID = 1, ProductID = 1, Quantity=20, QuantityType=ProductUnit.DOZ },
                new ContainerItem {ContainerID = 2, BuyerID = 2, ProductID = 2, Quantity=100, QuantityType=ProductUnit.PCS },
                new ContainerItem {ContainerID = 2, BuyerID = 1, ProductID = 2, Quantity=50, QuantityType=ProductUnit.PCS }
            };

            containerItems.ForEach(c => context.ContainerItems.Add(c));
            context.SaveChanges();

            Settings.CurrentContainerID = 1;
        }
    }
}