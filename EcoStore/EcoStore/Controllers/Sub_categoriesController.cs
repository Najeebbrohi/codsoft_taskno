using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EcoStore.Models;

namespace EcoStore.Controllers
{
    public class Sub_categoriesController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // GET: Sub_categories
        public ActionResult Index()
        {
            //var sub_categories = db.Sub_categories.ToList();
            var sub_categories = db.Sub_categories.Include(x => x.Category).ToList();
            return View(sub_categories.ToList());
        }

        // GET: Sub_categories/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Sub_categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sub_categories sub_categories)
        {
            if (ModelState.IsValid)
            {
                db.Sub_categories.Add(sub_categories);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", sub_categories.CategoryId);
            return View(sub_categories);
        }

        // GET: Sub_categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sub_categories sub_categories = db.Sub_categories.Find(id);
            if (sub_categories == null)
            {
                return HttpNotFound();
            }
            return View(sub_categories);
        }

        // POST: Sub_categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sub_categories sub_categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sub_categories).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", sub_categories.CategoryId);
            return View(sub_categories);
        }

        // GET: Sub_categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sub_categories sub_categories = db.Sub_categories.Find(id);
            if (sub_categories == null)
            {
                return HttpNotFound();
            }
            return View(sub_categories);
        }

        // POST: Sub_categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sub_categories sub_categories = db.Sub_categories.Find(id);
            db.Sub_categories.Remove(sub_categories);
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
