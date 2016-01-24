using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HK.DAL;
using HK.Models;
using HK.ViewModels;

namespace HK.Controllers
{
    public class TmpContainersController : BaseController
    {

        // GET: Containers

        public ContainerFilters GetContainerFilters()
        {
            var containerFilters = new ContainerFilters();
            containerFilters.SelectedExporterIDs = String.IsNullOrEmpty(Request.QueryString["exporter"]) ? new List<int>() : Request.QueryString["exporter"].Split(',').Select(Int32.Parse).ToList();
            containerFilters.SelectedImporterIDs = String.IsNullOrEmpty(Request.QueryString["importer"]) ? new List<int>() : Request.QueryString["importer"].Split(',').Select(Int32.Parse).ToList();
            containerFilters.Exporters = db.Exporters.Distinct().ToList();
            containerFilters.Importers = db.Importers.Distinct().ToList();
            return containerFilters;
        }


        public ActionResult Index()
        {
          //  var containerFilters = GetContainerFilters();
            var containers = db.Containers.ToList();

            //if (containerFilters.SelectedExporterIDs.Count > 0)
            //{
            //    containers = containers.Where(c => containerFilters.SelectedExporterIDs.Contains(c.ExporterID)).ToList();
            //}

            //if (containerFilters.SelectedImporterIDs.Count > 0)
            //{
            //    containers = containers.Where(c => containerFilters.SelectedImporterIDs.Contains(c.ImporterID)).ToList();
            //}

            return View(containers);
        }

        public ActionResult List()
        {
            return View(db.Containers.ToList());
        }

        public PartialViewResult Filters()
        {
            var containerFilters = GetContainerFilters();
            return PartialView(containerFilters);
        }

        public Container GetCurrentContainer()
        {
            var currentContainer = db.Containers.Where(c => c.ContainerID == CurrentContainerID)
                .FirstOrDefault();

            return currentContainer;
        }



        public ActionResult Load(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["CurrentContainerID"] = Convert.ToString(id);
            return RedirectToAction("Index", "TmpDashboard");
        }

        // GET: Containers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Container container = db.Containers.Find(id);
            if (container == null)
            {
                return HttpNotFound();
            }
            return View(container);
        }

        // GET: Containers/Create
        public PartialViewResult Create()
        {
            ViewBag.Exporters = new SelectList(db.Containers.Select(c => c.ExporterName).Distinct());
            ViewBag.Importers = new SelectList(db.Containers.Select(c => c.ImporterName).Distinct());

            ViewBag.ImporterID = new SelectList(db.Importers, "ImporterID", "ImporterName");
            ViewBag.ExporterID = new SelectList(db.Exporters, "ExporterID", "ExporterName");
            return PartialView();
        }

        // POST: Containers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public PartialViewResult Create([Bind(Include = "ContainerID,ContainerNumber,Date,ShippedPer,OnAbout,From,AirwayBillNumber,LetterOfCreditNumber,DrawnUnder,ImporterID, ExporterID")] Container container)
        {
            if (ModelState.IsValid)
            {
                db.Containers.Add(container);
                db.SaveChanges();
                Session["CurrentContainerID"] = container.ContainerID;
                ViewBag.ImporterID = new SelectList(db.Importers, "ImporterID", "ImporterName");
                ViewBag.ExporterID = new SelectList(db.Exporters, "ExporterID", "ExporterName");
                return PartialView("Edit", container);
            }
            else
            {
                Response.AppendHeader("X-Error", "true");
                ViewBag.ImporterID = new SelectList(db.Importers, "ImporterID", "ImporterName");
                ViewBag.ExporterID = new SelectList(db.Exporters, "ExporterID", "ExporterName");
                return PartialView("Create", container);
            }

            
        }

        public ActionResult CreateNew()
        {
            var container = new Container();
            container.Date = DateTime.Now;
            db.Containers.Add(container);
            db.SaveChanges();
            Session["CurrentContainerID"] = container.ContainerID;
            return RedirectToAction("Index", "TmpDashboard");
        }

        public PartialViewResult EditContainer()
        {
            ViewBag.Exporters = new SelectList(db.Containers.Select(c => c.ExporterName).Distinct());
            ViewBag.Importers = new SelectList(db.Containers.Select(c => c.ImporterName).Distinct());
            ViewBag.ImporterID = new SelectList(db.Importers, "ImporterID", "ImporterName");
            return PartialView("Edit", db.Containers.Find(CurrentContainerID));
        }

        //// GET: Containers/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Container container = db.Containers.Find(id);
        //    return Edit(container);   
        //}

        // POST: Containers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public PartialViewResult Edit([Bind(Include = "ContainerID,ContainerNumber,Date,ShippedPer,OnAbout,From,AirwayBillNumber,LetterOfCreditNumber,DrawnUnder, CostsIncluded, HarmonicCodes, TotalGrossWeight, TotalCartons, CountryOfOrigin, BeneficiaryBank, BeneficiarySwift, BeneficiaryUsdAccount, ImporterName, ExporterName")] Container container)
        {
            container.ContainerID = CurrentContainerID;
            if (ModelState.IsValid)
            {
                db.Entry(container).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            else
            {
                container = new TmpContainersController().GetCurrentContainer();
            }

            ViewBag.Exporters = new SelectList(db.Containers.Select(c => c.ExporterName).Distinct());
            ViewBag.Importers = new SelectList(db.Containers.Select(c => c.ImporterName).Distinct());

            ViewBag.ImporterID = new SelectList(db.Importers, "ImporterID", "ImporterName");
            ViewBag.ExporterID = new SelectList(db.Exporters, "ExporterID", "ExporterName");
            return PartialView("Edit", container);
        }

        // GET: Containers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Container container = db.Containers.Find(id);
            if (container == null)
            {
                return HttpNotFound();
            }
            return View(container);
        }

        // POST: Containers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Container container = db.Containers.Find(id);
            db.Containers.Remove(container);
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["CurrentContainerID"] = null;
            return RedirectToAction("Index");
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
