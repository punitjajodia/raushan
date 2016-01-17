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
    public class TmpContainerItemsController : BaseController
    {
        private ImportManagerContext db = new ImportManagerContext();

        // GET: TmpContainerItems
        public PartialViewResult Index()
        {
            var tmpContainerItems = db.TmpContainerItems
                .Where(t => t.ContainerID == CurrentContainerID)
                .Include(t => t.Container);

            return PartialView(tmpContainerItems.ToList());
        }

        // GET: TmpContainerItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TmpContainerItem tmpContainerItem = db.TmpContainerItems.Find(id);
            if (tmpContainerItem == null)
            {
                return HttpNotFound();
            }
            return View(tmpContainerItem);
        }

        // GET: TmpContainerItems/Create
        public ActionResult Create()
        {
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber");
            return View();
        }

        // POST: TmpContainerItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TmpContainerItemID,ContainerID,CartonNumber,BuyerName, ProductCustomsName,ProductBuyerName,ProductUnit,Quantity,Cartons,BuyerCurrency,BuyerUnitPrice,CustomsCurrency,CustomsUnitPrice")] TmpContainerItem tmpContainerItem)
        {
            if (ModelState.IsValid)
            {
                db.TmpContainerItems.Add(tmpContainerItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", tmpContainerItem.ContainerID);
            return View(tmpContainerItem);
        }

        // GET: TmpContainerItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TmpContainerItem tmpContainerItem = db.TmpContainerItems.Find(id);
            if (tmpContainerItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", tmpContainerItem.ContainerID);
            return View(tmpContainerItem);
        }

        // POST: TmpContainerItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TmpContainerItemID,ContainerID,CartonNumber,BuyerName,ProductCustomsName,ProductBuyerName,ProductUnit,Quantity,Cartons,BuyerCurrency,BuyerUnitPrice,CustomsCurrency,CustomsUnitPrice")] TmpContainerItem tmpContainerItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tmpContainerItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", tmpContainerItem.ContainerID);
            return View(tmpContainerItem);
        }

        // GET: TmpContainerItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TmpContainerItem tmpContainerItem = db.TmpContainerItems.Find(id);
            if (tmpContainerItem == null)
            {
                return HttpNotFound();
            }
            return View(tmpContainerItem);
        }

        // POST: TmpContainerItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TmpContainerItem tmpContainerItem = db.TmpContainerItems.Find(id);
            db.TmpContainerItems.Remove(tmpContainerItem);
            db.SaveChanges();
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
