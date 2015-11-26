using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class Buyer
    {
        public int BuyerID { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }

        public virtual ICollection<ContainerItem> ContainerItems { get; set; }
    }
}