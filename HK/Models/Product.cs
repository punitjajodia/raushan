using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public ProductUnit ProductUnit { get; set; }
        public virtual ICollection<ContainerItem> ContainerItems { get; set; }
    }
}