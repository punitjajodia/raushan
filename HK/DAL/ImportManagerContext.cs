using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HK.DAL
{
    public class ImportManagerContext : DbContext
    {

        public System.Data.Entity.DbSet<HK.Models.Buyer> Buyers { get; set; }

        public System.Data.Entity.DbSet<HK.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<HK.Models.ContainerItem> ContainerItems { get; set; }

        public System.Data.Entity.DbSet<HK.ViewModels.InvoiceItem> InvoiceItems { get; set; }

        public System.Data.Entity.DbSet<HK.Models.Container> Containers { get; set; }

        public System.Data.Entity.DbSet<HK.Models.Exporter> Exporters { get; set; }

        public System.Data.Entity.DbSet<HK.Models.Importer> Importers { get; set; }

        public System.Data.Entity.DbSet<HK.Models.ProductContainerPrice> ProductContainerPrices { get; set; }

        public System.Data.Entity.DbSet<HK.Models.ProductContainerBuyerPrice> ProductContainerBuyerPrices { get; set; }

        public System.Data.Entity.DbSet<HK.ViewModels.BuyerInvoiceItem> BuyerInvoiceItems { get; set; }

        public System.Data.Entity.DbSet<HK.Models.TmpContainerItem> TmpContainerItems { get; set; }


    }
}