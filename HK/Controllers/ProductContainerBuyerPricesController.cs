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
    public class ProductContainerBuyerPricesController : BaseController
    {

        // GET: ProductContainerBuyerPrices
        public async Task<ActionResult> Index()
        {
            var productContainerBuyerPrices = db.ProductContainerBuyerPrices.Include(p => p.Buyer).Include(p => p.Container).Include(p => p.Product);
            return View(await productContainerBuyerPrices.ToListAsync());
        }

        // GET: ProductContainerBuyerPrices/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductContainerBuyerPrice productContainerBuyerPrice = await db.ProductContainerBuyerPrices.FindAsync(id);
            if (productContainerBuyerPrice == null)
            {
                return HttpNotFound();
            }
            return View(productContainerBuyerPrice);
        }

        // GET: ProductContainerBuyerPrices/Create
        public ActionResult Create()
        {
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode");
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode");
            return View();
        }

        // POST: ProductContainerBuyerPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductContainerBuyerPriceID,ProductID,ContainerID,BuyerID,Unit,Price")] ProductContainerBuyerPrice productContainerBuyerPrice)
        {
            if (ModelState.IsValid)
            {
                db.ProductContainerBuyerPrices.Add(productContainerBuyerPrice);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode", productContainerBuyerPrice.BuyerID);
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", productContainerBuyerPrice.ContainerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", productContainerBuyerPrice.ProductID);
            return View(productContainerBuyerPrice);
        }

        // GET: ProductContainerBuyerPrices/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductContainerBuyerPrice productContainerBuyerPrice = await db.ProductContainerBuyerPrices.FindAsync(id);
            if (productContainerBuyerPrice == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode", productContainerBuyerPrice.BuyerID);
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", productContainerBuyerPrice.ContainerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", productContainerBuyerPrice.ProductID);
            return View(productContainerBuyerPrice);
        }

        // POST: ProductContainerBuyerPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "ProductContainerBuyerPriceID,ProductID,ContainerID,BuyerID,Unit,Price")] ProductContainerBuyerPrice productContainerBuyerPrice)
        {
            if (ModelState.IsValid)
            {
                if (productContainerBuyerPrice.ProductContainerBuyerPriceID == 0)
                {
                    db.ProductContainerBuyerPrices.Add(productContainerBuyerPrice);
                }
                else
                {
                    db.Entry(productContainerBuyerPrice).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode", productContainerBuyerPrice.BuyerID);
            ViewBag.ContainerID = new SelectList(db.Containers, "ContainerID", "ContainerNumber", productContainerBuyerPrice.ContainerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", productContainerBuyerPrice.ProductID);
            return View(productContainerBuyerPrice);
        }

        // GET: ProductContainerBuyerPrices/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductContainerBuyerPrice productContainerBuyerPrice = await db.ProductContainerBuyerPrices.FindAsync(id);
            if (productContainerBuyerPrice == null)
            {
                return HttpNotFound();
            }
            return View(productContainerBuyerPrice);
        }

        // POST: ProductContainerBuyerPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductContainerBuyerPrice productContainerBuyerPrice = await db.ProductContainerBuyerPrices.FindAsync(id);
            db.ProductContainerBuyerPrices.Remove(productContainerBuyerPrice);
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
