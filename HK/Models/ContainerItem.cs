using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class ContainerItem
    {
        public int ContainerItemID {get; set;}
        public int ContainerID { get; set; }
        public string CartonNumber { get; set; }
        public int BuyerID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public ProductUnit QuantityType { get; set; }
        public int Cartons { get; set; }
        public Buyer Buyer { get; set; }
        public Product Product { get; set; }
        public Container Container { get; set; }

        public ContainerItem() {
            Cartons = 1;
        }
    }
}