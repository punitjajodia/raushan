using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class ProductContainerBuyerPrice
    {
        public int ProductContainerBuyerPriceID { get; set; }

        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        public int ContainerID { get; set; }
        public virtual Container Container { get; set; }

        public int BuyerID { get; set; }
        public virtual Buyer Buyer { get; set; }

        [DefaultValue(ProductUnit.PCS)]
        public ProductUnit Unit { get; set; }

        public decimal Price { get; set; }

    }
}