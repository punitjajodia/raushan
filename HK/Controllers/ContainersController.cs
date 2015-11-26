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

namespace HK.Controllers
{
    public class ContainersController : BaseController
    {

        // GET: Containers
        public ActionResult Index()
        {
            return View(db.Containers.ToList());
        }

        public ActionResult List()
        {
            return View(db.Containers.ToList());
        }

        public Container GetCurrentContainer()
        {
            var currentContainer = db.Containers.Where(c => c.ContainerID == CurrentContainerID)
                .Include(c => c.Exporter)
                .Include(c => c.Importer)
                .Include(c => c.ContainerItems.Select(ci => ci.Product))
                .Include(c => c.ContainerItems.Select(ci => ci.Buyer))
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
            return RedirectToAction("Index", "Dashboard");
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

        public PartialViewResult EditContainer(Container container)
        {
            ViewBag.ImporterID = new SelectList(db.Importers, "ImporterID", "ImporterName");
            ViewBag.ExporterID = new SelectList(db.Exporters, "ExporterID", "ExporterName");
            return PartialView("Edit", container);
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
        public PartialViewResult Edit([Bind(Include = "ContainerID,ContainerNumber,Date,ShippedPer,OnAbout,From,AirwayBillNumber,LetterOfCreditNumber,DrawnUnder,ImporterID, ExporterID")] Container container)
        {
            container.ContainerID = CurrentContainerID;
            if (ModelState.IsValid)
            {
                db.Entry(container).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            else
            {
                container = new ContainersController().GetCurrentContainer();
            }
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
