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
    public class GalleriesController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // GET: Galleries
        public ActionResult Index()
        {
            var galleries = db.Galleries.Include(g => g.Product);
            return View(galleries.ToList());
        }

        // GET: Galleries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        // GET: Galleries/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: Galleries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GalleryViewModel galleryVM)
        {
            Gallery gallery = new Gallery();

            if (ModelState.IsValid)
            {
                string filename = Path.GetFileName(galleryVM.File.FileName);
                string _file = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/gallery/"), _file);
                galleryVM.ImgPath = "~/Content/BeckendAssets/uploaded/gallery/" + _file;

                gallery.ImgPath = galleryVM.ImgPath;
                gallery.ProductId = galleryVM.ProductId;

                db.Galleries.Add(gallery);
                if(galleryVM.File.ContentLength <= 1000000)
                {
                    if(db.SaveChanges() > 0)
                    {
                        galleryVM.File.SaveAs(path);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Msg = "Image must be less than or equal to 1MB";
                }
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", gallery.ProductId);
            return View(gallery);
        }

        // GET: Galleries/Edit/5
        public ActionResult Edit(int? id)
        {
            GalleryViewModel galleryVM = new GalleryViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Find(id);
            galleryVM.GalleryId = gallery.GalleryId;
            galleryVM.ImgPath = gallery.ImgPath;
            galleryVM.ProductId = gallery.ProductId;
            Session["ImgPath"] = gallery.ImgPath;
            if (gallery == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", gallery.ProductId);
            return View(galleryVM);
        }

        // POST: Galleries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GalleryViewModel galleryVM)
        {
            Gallery gallery = new Gallery();
            if (ModelState.IsValid)
            {
                if(galleryVM.File != null)
                {
                    string filename = Path.GetFileName(galleryVM.File.FileName);
                    string _file = Guid.NewGuid().ToString() + filename;
                    string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/gallery/"), _file);
                    galleryVM.ImgPath = "~/Content/BeckendAssets/uploaded/gallery/" + _file;

                    gallery.GalleryId = galleryVM.GalleryId;
                    gallery.ImgPath = galleryVM.ImgPath;
                    gallery.ProductId = galleryVM.ProductId;

                    var OldImg = Request.MapPath(Session["ImgPath"].ToString());

                    db.Entry(gallery).State = EntityState.Modified;
                    if(galleryVM.File.ContentLength <= 1000000)
                    {
                        if(db.SaveChanges() > 0)
                        {
                            if (System.IO.File.Exists(OldImg))
                            {
                                System.IO.File.Delete(OldImg);
                            }
                            galleryVM.File.SaveAs(path);
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Msg = "Image must be less than or equal to 1MB";
                    }
                    
                }
                else
                {
                    gallery.GalleryId = galleryVM.GalleryId;
                    gallery.ImgPath = Session["ImgPath"].ToString();
                    gallery.ProductId = galleryVM.ProductId;
                    
                    db.Entry(gallery).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", gallery.ProductId);
            return View(gallery);
        }

        // GET: Galleries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        // POST: Galleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Gallery gallery = db.Galleries.Find(id);
            var CurrentImg = Request.MapPath(gallery.ImgPath);
            db.Galleries.Remove(gallery);
            if(db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(CurrentImg))
                {
                    System.IO.File.Delete(CurrentImg);
                }
            }
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
