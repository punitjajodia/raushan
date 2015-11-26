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
    public class ImportersController : Controller
    {
        private ImportManagerContext db = new ImportManagerContext();

        // GET: Importers
        public async Task<ActionResult> Index()
        {
            return View(await db.Importers.ToListAsync());
        }

        // GET: Importers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Importer importer = await db.Importers.FindAsync(id);
            if (importer == null)
            {
                return HttpNotFound();
            }
            return View(importer);
        }

        // GET: Importers/Create
        public PartialViewResult Create()
        {
            return PartialView();
        }

        // POST: Importers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Create([Bind(Include = "ImporterID,ImporterName,ImporterAddress,TaxCertificateNumber")] Importer importer)
        {
            if (ModelState.IsValid)
            {
                db.Importers.Add(importer);
                await db.SaveChangesAsync();
            }
            else
            {
                Response.AppendHeader("X-Error", "true");
            }

            return PartialView(importer);
        }

        // GET: Importers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Importer importer = await db.Importers.FindAsync(id);
            if (importer == null)
            {
                return HttpNotFound();
            }
            return View(importer);
        }

        // POST: Importers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ImporterID,ImporterName,ImporterAddress,TaxCertificateNumber")] Importer importer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(importer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(importer);
        }

        // GET: Importers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Importer importer = await db.Importers.FindAsync(id);
            if (importer == null)
            {
                return HttpNotFound();
            }
            return View(importer);
        }

        // POST: Importers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Importer importer = await db.Importers.FindAsync(id);
            db.Importers.Remove(importer);
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
