using HK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.ViewModels
{
    public class BuyerInvoiceItem
    {
        public int BuyerInvoiceItemID { get; set; }
        public ProductContainerBuyerPrice ProductContainerBuyerPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cartons { get; set; }
    }
}