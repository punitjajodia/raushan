using HK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HK.ViewModels
{
    public class BuyerVM
    {
        public Buyer Buyer { get; set; }
        public List<Buyer> Buyers { get; set; }
    }
}