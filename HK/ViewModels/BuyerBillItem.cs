using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.ViewModels
{
    public class BuyerBillItem
    {
         public String Party {get; set;}
         public String Marka { get; set; }
         public String Product {get; set;}
         public decimal Rate {get; set;}
         public decimal Quantity {get; set;}
         public string Unit { get; set; } 
         public decimal Total { get; set; }
    }
}