using HK.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class BaseController : Controller
    {
        protected ImportManagerContext db = new ImportManagerContext();
        protected int CurrentContainerID
        {
            get
            {
                if (System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["CurrentContainerID"] == null)
                {
                    var containerID = db.Containers.OrderByDescending(c => c.ContainerID).First().ContainerID;
                    System.Web.HttpContext.Current.Session["CurrentContainerID"] = containerID;
                    return containerID;
                }
                else
                {
                    return Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentContainerID"]);
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}