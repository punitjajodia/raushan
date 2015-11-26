using HK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.ViewModels
{
    public class InvoiceItemExportVM
    {
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }


        public InvoiceItemExportVM(InvoiceItem i)
        {
            ProductCode = i.ProductCode;
            Quantity = i.Quantity;
            Unit = i.ProductContainerPrice.QuantityType;
            UnitPrice = i.ProductContainerPrice.Price;
            Amount = i.Amount;
        }
    }
}