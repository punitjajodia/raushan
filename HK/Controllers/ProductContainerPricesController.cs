using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HK.DAL;
using HK.Models;

namespace HK.Controllers
{
    public class ProductContainerPricesController : BaseController
    {
        private ImportManagerContext db = new ImportManagerContext();

        // GET: ProductContainerPrices
        public async Task<ActionResult> Index()
        {
            var productContainerPrices = db.ProductContainerPrices.Include(p => p.Container).Include(p => p.Product);
            return View(await productContainerPrices.ToListAsync());
        }

        // GET: ProductContainerPrices/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductContainerPrice productContainerPrice = await db.ProductContainerPrices.FindAsync(id);
            if (productContainerPrice == null)
            {
                return HttpNotFound();
            }
            return View(productContainerPrice);
        }

        // GET: ProductContainerPrices/Create
        public ActionResult Create()
        {
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode");
            return View();
        }

        // POST: ProductContainerPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductContainerPriceID,ProductID,Price,QuantityType,ContainerID")] ProductContainerPrice productContainerPrice)
        {
            if (ModelState.IsValid)
            {
                db.ProductContainerPrices.Add(productContainerPrice);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", productContainerPrice.ContainerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", productContainerPrice.ProductID);
            return View(productContainerPrice);
        }

        // GET: ProductContainerPrices/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductContainerPrice productContainerPrice = await db.ProductContainerPrices.FindAsync(id);
            if (productContainerPrice == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", productContainerPrice.ContainerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", productContainerPrice.ProductID);
            return View(productContainerPrice);
        }

        // POST: ProductContainerPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "ProductContainerPriceID,ProductID,Price,QuantityType")] ProductContainerPrice productContainerPrice)
        {
            productContainerPrice.ContainerID = CurrentContainerID;
            if (ModelState.IsValid)
            {
                if (productContainerPrice.ProductContainerPriceID == 0)
                {
                    db.ProductContainerPrices.Add(productContainerPrice);
                }
                else
                {
                    db.Entry(productContainerPrice).State = EntityState.Modified;
                }         
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", productContainerPrice.ContainerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", productContainerPrice.ProductID);
            return View(productContainerPrice);
        }

        // GET: ProductContainerPrices/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductContainerPrice productContainerPrice = await db.ProductContainerPrices.FindAsync(id);
            if (productContainerPrice == null)
            {
                return HttpNotFound();
            }
            return View(productContainerPrice);
        }

        // POST: ProductContainerPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductContainerPrice productContainerPrice = await db.ProductContainerPrices.FindAsync(id);
            db.ProductContainerPrices.Remove(productContainerPrice);
            await db.SaveChangesAsync();
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
