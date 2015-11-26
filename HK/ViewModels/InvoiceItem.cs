using HK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.ViewModels
{
    public class InvoiceItem
    {
        

        public int InvoiceItemID { get; set; }

        public int ProductID { get; set; }
        public string ProductCode { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public ProductUnit Unit { get; set; }

        public ProductContainerPrice ProductContainerPrice { get; set; }
    }
}