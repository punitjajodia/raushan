using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class Settings
    {
        public static int CurrentContainerID
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["CurrentContainerID"]); }
            set { ConfigurationManager.AppSettings["CurrentContainerID"] = value.ToString(); }
        }
    }
}