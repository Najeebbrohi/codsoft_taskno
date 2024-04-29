using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EcoStore.Models;
using EcoStore.ViewModels;

namespace EcoStore.Controllers
{
    public class CategoriesController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryViewModel viewModel)
        {
            Category category = new Category();
            if (ModelState.IsValid && viewModel.File != null)
            {
                string filename = Path.GetFileName(viewModel.File.FileName);
                string _file = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/category/"), _file);
                viewModel.ImgPath = "~/Content/BeckendAssets/uploaded/category/" + _file;

                category.Name = viewModel.Name;
                category.Description = viewModel.Description;
                category.ImgPath = viewModel.ImgPath;
                

                db.Categories.Add(category);
                if(db.SaveChanges() > 0)
                {
                    if(viewModel.File.ContentLength < 1000000)
                    {
                        viewModel.File.SaveAs(path);
                    }
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);

            CategoryViewModel categoryVM = new CategoryViewModel();
            categoryVM.CategoryId = category.CategoryId;
            categoryVM.Name = category.Name;
            categoryVM.Description = category.Description;
            categoryVM.ImgPath = category.ImgPath;

            Session["Image"] = categoryVM.ImgPath;

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(categoryVM);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryViewModel viewModel)
        {
            Category category = new Category();
            if (ModelState.IsValid)
            {
                if(viewModel.File == null)
                {
                    category.CategoryId = viewModel.CategoryId;
                    category.Name = viewModel.Name;
                    category.Description = viewModel.Description;
                    category.ImgPath = viewModel.ImgPath;

                    category.ImgPath = Session["Image"].ToString();
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    string filename = Path.GetFileName(viewModel.File.FileName);
                    string _file = Guid.NewGuid().ToString() + filename;
                    string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/category/"), _file);
                    viewModel.ImgPath = "~/Content/BeckendAssets/uploaded/category/" + _file;

                    category.CategoryId = viewModel.CategoryId;
                    category.Name = viewModel.Name;
                    category.Description = viewModel.Description;
                    category.ImgPath = viewModel.ImgPath;

                    string OldImage = Request.MapPath(Session["Image"].ToString());

                    db.Entry(category).State = EntityState.Modified;

                    if(db.SaveChanges() > 0)
                    {
                        if (System.IO.File.Exists(OldImage))
                        {
                            System.IO.File.Delete(OldImage);
                        }
                        viewModel.File.SaveAs(path);
                    }    
                    return RedirectToAction("Index");
                }
                
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            string CurrentImg = Request.MapPath(category.ImgPath);
            if (System.IO.File.Exists(CurrentImg))
            {
                System.IO.File.Delete(CurrentImg);
            }
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
