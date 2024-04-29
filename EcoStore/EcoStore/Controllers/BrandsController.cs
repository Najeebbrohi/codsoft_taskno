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
    public class BrandsController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // GET: Brands
        public ActionResult Index()
        {
            return View(db.Brands.ToList());
        }

        // GET: Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BrandViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string filename = Path.GetFileName(viewModel.File.FileName);
                string _file = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/brand/"), _file);
                viewModel.ImgPath = "~/Content/BeckendAssets/uploaded/brand/" + _file;

                Brand brand = new Brand
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    ImgPath = viewModel.ImgPath,
                };

                db.Brands.Add(brand);
                if (viewModel.File.ContentLength < 1000000)
                {
                    if (db.SaveChanges() > 0)
                    {

                        viewModel.File.SaveAs(path);
                        ViewBag.Msg = "Brand Added Successfully";
                    }
                }
                else
                {
                    ViewBag.Msg = "Image must be less than or equal to 1MB";
                }
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: Brands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);

            BrandViewModel brandMV = new BrandViewModel();
            brandMV.BrandId = brand.BrandId;
            brandMV.Name = brand.Name;
            brandMV.Description = brand.Description;
            brandMV.ImgPath = brand.ImgPath;

            Session["Image"] = brandMV.ImgPath;
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brandMV);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BrandViewModel viewModel)
        {
            Brand brand = new Brand();
            if (ModelState.IsValid)
            {
                if (viewModel.File == null)
                {
                    brand.BrandId = viewModel.BrandId;
                    brand.Name = viewModel.Name;
                    brand.Description = viewModel.Description;
                    brand.ImgPath = viewModel.ImgPath;

                    brand.ImgPath = Session["Image"].ToString();
                    db.Entry(brand).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    string filename = Path.GetFileName(viewModel.File.FileName);
                    string _file = Guid.NewGuid().ToString() + filename;
                    string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/brand/"), _file);
                    viewModel.ImgPath = "~/Content/BeckendAssets/uploaded/brand/" + _file;

                    brand.BrandId = viewModel.BrandId;
                    brand.Name = viewModel.Name;
                    brand.Description = viewModel.Description;
                    brand.ImgPath = viewModel.ImgPath;

                    string OldImage = Request.MapPath(Session["Image"].ToString());

                    db.Entry(brand).State = EntityState.Modified;

                    if (db.SaveChanges() > 0)
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
            return View(brand);
        }

        // GET: Brands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = db.Brands.Find(id);
            db.Brands.Remove(brand);
            string CurrentImg = Request.MapPath(brand.ImgPath);
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
