using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class ProductContainerPrice
    {
        public int ProductContainerPriceID { get; set; }

        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        public decimal Price { get; set; }

        [DefaultValue(ProductUnit.PCS)]
        public ProductUnit QuantityType { get; set; }

        public int ContainerID { get; set; }
        public virtual Container Container { get; set; }
    }
}