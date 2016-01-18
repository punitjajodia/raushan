using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class TmpContainerItem
    {
        public int TmpContainerItemID { get; set; }
        public int ContainerID { get; set; }
        
        public string CartonNumber { get; set; }
        
        public string BuyerName { get; set; }

        public string PartyName { get; set; }
        public string PartyPhone { get; set; }
        public string BillOnBoardingDate { get; set; }
        public string BillDeliveryDate { get; set; }
        public string BillNumber { get; set; }
        public string BillTTDAPNumber { get; set; }
        public string BillTTDAPDate { get; set; }
        public string LotSize { get; set; }

        public string ProductCustomsName { get; set; }
        public string ProductBuyerName { get; set; }
        public ProductUnit ProductUnit { get; set; }

        public decimal Quantity { get; set; }

        public int Cartons { get; set; }
        public Container Container { get; set; }

        public string BuyerCurrency { get; set; }
        public decimal BuyerUnitPrice { get; set; }	
        public string CustomsCurrency { get; set; }	
        public decimal CustomsUnitPrice { get; set;}

        public TmpContainerItem()
        {
            Cartons = 1;
        }
    }
}