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
    public class ExportersController : Controller
    {
        private ImportManagerContext db = new ImportManagerContext();

        // GET: Exporters
        public async Task<ActionResult> Index()
        {
            return View(await db.Exporters.ToListAsync());
        }

        // GET: Exporters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exporter exporter = await db.Exporters.FindAsync(id);
            if (exporter == null)
            {
                return HttpNotFound();
            }
            return View(exporter);
        }

        // GET: Exporters/Create
        public PartialViewResult Create()
        {
            return PartialView();
        }

        // POST: Exporters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Create([Bind(Include = "ExporterID,ExporterName,ExporterAddress")] Exporter exporter)
        {
            if (ModelState.IsValid)
            {
                db.Exporters.Add(exporter);
                await db.SaveChangesAsync();
            }
            else
            {
                Response.AppendHeader("X-Error", "true");
            }

            return PartialView(exporter);
        }

        // GET: Exporters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exporter exporter = await db.Exporters.FindAsync(id);
            if (exporter == null)
            {
                return HttpNotFound();
            }
            return View(exporter);
        }

        // POST: Exporters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ExporterID,ExporterName,ExporterAddress")] Exporter exporter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exporter).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(exporter);
        }

        // GET: Exporters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exporter exporter = await db.Exporters.FindAsync(id);
            if (exporter == null)
            {
                return HttpNotFound();
            }
            return View(exporter);
        }

        // POST: Exporters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Exporter exporter = await db.Exporters.FindAsync(id);
            db.Exporters.Remove(exporter);
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
