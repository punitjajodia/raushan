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
using ClosedXML.Excel;
using System.IO;

namespace HK.Controllers
{
    public class ContainerItemsController : BaseController
    {
        public IQueryable<ContainerItem> GetContainerItemsQuery() {
            return db.ContainerItems.Include(c => c.Buyer).Include(c => c.Product).Where(c => c.ContainerID == CurrentContainerID);
        }

        public List<ContainerItem> GetContainerItems()
        {
            return GetContainerItemsQuery().ToList();
        }

        public ActionResult Index()
        {
            return View(GetContainerItems());
        }

        // GET: ContainerItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContainerItem containerItem = db.ContainerItems.Find(id);
            if (containerItem == null)
            {
                return HttpNotFound();
            }
            return View(containerItem);
        }

        // GET: ContainerItems/Create
        public ActionResult Create()
        {
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode");
            return View();
        }

        public PartialViewResult CreateAndView()
        {
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode");
            return PartialView();
        }

        public PartialViewResult List()
        {
            return PartialView(GetContainerItems());
        }

        // POST: ContainerItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult CreateAndView([Bind(Include = "ContainerItemID,CartonNumber, BuyerID,ProductID,Quantity,QuantityType,Cartons")] ContainerItem containerItem)
        {
            containerItem.ContainerID = CurrentContainerID;
            if (ModelState.IsValid)
            {
                db.ContainerItems.Add(containerItem);
                db.SaveChanges();
                containerItem = new ContainerItem();
            }
            else
            {
                Response.AppendHeader("X-Error", "true");
            }

            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode", containerItem.BuyerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", containerItem.ProductID);
            return PartialView("CreateAndView", containerItem);
        }

        // GET: ContainerItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContainerItem containerItem = db.ContainerItems.Find(id);
            if (containerItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode", containerItem.BuyerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", containerItem.ProductID);
            return View(containerItem);
        }

        // POST: ContainerItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContainerItemID,BuyerID,ProductID,Quantity,QuantityType,Cartons")] ContainerItem containerItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(containerItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuyerID = new SelectList(db.Buyers, "BuyerID", "BuyerCode", containerItem.BuyerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode", containerItem.ProductID);
            return View(containerItem);
        }

        // GET: ContainerItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContainerItem containerItem = db.ContainerItems.Find(id);
            if (containerItem == null)
            {
                return HttpNotFound();
            }
            return View(containerItem);
        }

        // POST: ContainerItems/Delete/5
        [HttpPost, ActionName("Delete")]
      //  [ValidateAntiForgeryToken]
        public void DeleteConfirmed(int id)
        {
            ContainerItem containerItem = db.ContainerItems.Find(id);
            db.ContainerItems.Remove(containerItem);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Export()
        {
            var containerItems = GetContainerItemsQuery().Select(c => new { 
                   c.Buyer.BuyerCode,
                   c.Product.ProductCode,
                   c.Quantity,
                   c.QuantityType,
                   c.Cartons
            });
                

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Packing List");



            ws.Cell("A1").InsertTable(containerItems);

            //table.ShowTotalsRow = true;
            //table.Field(0).TotalsRowFunction = XLTotalsRowFunction.Sum;
            //// Just for fun let's add the text "Sum Of Income" to the totals row
            //table.Field(1).TotalsRowLabel = "Sum Of Income";

            ws.Columns().AdjustToContents();

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=\"Packing List.xlsx\"");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();

            return RedirectToAction("Index");
        }
        
    }
}
