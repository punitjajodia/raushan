using HK.Controllers;
using HK.DAL;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Models
{
    public class DashboardController : BaseController
    {

        public PackingListVM GetPackingList()
        {
            var PackingList = new PackingListVM();
            PackingList.Container = new TmpContainersController().GetCurrentContainer();

            PackingList.Container.ContainerItems = PackingList.Container.ContainerItems.OrderBy(ci => ci.BuyerID).ToList();

            PackingList.InvoiceItems = new InvoicesController().GetInvoices(PackingList.Container);
            return PackingList;
        }

        public ActionResult Index()
        {
            return View(GetPackingList());
        }

        public PartialViewResult List()
        {
            return PartialView(GetPackingList());
        }
    }
}