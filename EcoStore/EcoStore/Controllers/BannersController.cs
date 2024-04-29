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
    public class BannersController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // GET: Banners
        public ActionResult Index()
        {
            var banners = db.Banners.Include(b => b.Category);
            return View(banners.ToList());
        }
        // GET: Banners/Create
        public ActionResult Create()
        {
            Banner banner = new Banner();
            BannerViewModel bannerVM = new BannerViewModel();
            bannerVM.BannerId = banner.BannerId;
            bannerVM.BannerName = banner.BannerName;
            bannerVM.CategoryId = banner.CategoryId;
            bannerVM.ImgPath = banner.ImgPath;
            bannerVM.Status = banner.Status;

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View(bannerVM);
        }

        // POST: Banners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BannerViewModel bannerVM)
        {
            Banner banner = new Banner();
            if (ModelState.IsValid && bannerVM.File != null)
            {
                string filename = Path.GetFileName(bannerVM.File.FileName);
                string _file = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/banner/"), _file);
                bannerVM.ImgPath = "~/Content/BeckendAssets/uploaded/banner/" + _file;

                banner.BannerId = bannerVM.BannerId;
                banner.BannerName = bannerVM.BannerName;
                banner.CategoryId = bannerVM.CategoryId;
                banner.ImgPath = bannerVM.ImgPath;
                banner.Status = bannerVM.Status;

                db.Banners.Add(banner);

                if (db.SaveChanges() > 0)
                {
                    if (bannerVM.File.ContentLength <= 1000000)
                    {
                        bannerVM.File.SaveAs(path);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Msg = "File must be less than or equal to 1MB";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Message", "Invalid Data");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", banner.CategoryId);
            return View(banner);
        }

        // GET: Banners/Edit/5
        public ActionResult Edit(int? id)
        {
            BannerViewModel bannerVM = new BannerViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);

            bannerVM.BannerId = banner.BannerId;
            bannerVM.BannerName = banner.BannerName;
            bannerVM.CategoryId = banner.CategoryId;
            bannerVM.ImgPath = banner.ImgPath;
            bannerVM.Status = banner.Status;

            Session["Image"] = banner.ImgPath;
            if (banner == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", banner.CategoryId);
            return View(bannerVM);
        }

        // POST: Banners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BannerViewModel bannerVM)
        {
            Banner banner = new Banner();
            if (ModelState.IsValid)
            {
                if (bannerVM.File != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(bannerVM.File.FileName);
                    string extension = Path.GetExtension(bannerVM.File.FileName);
                    filename = Guid.NewGuid().ToString() + filename + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/banner/"), filename);
                    bannerVM.ImgPath = "~/Content/BeckendAssets/uploaded/banner/" + filename;

                    banner.BannerId = bannerVM.BannerId;
                    banner.BannerName = bannerVM.BannerName;
                    banner.CategoryId = bannerVM.CategoryId;
                    banner.ImgPath = bannerVM.ImgPath;
                    banner.Status = bannerVM.Status;

                    string OldImg = Request.MapPath(Session["Image"].ToString());

                    db.Entry(banner).State = EntityState.Modified;

                    if (extension == ".jpeg" || extension == ".png" || extension == ".jpg")
                    {
                        if (bannerVM.File.ContentLength < 1000000)
                        {
                            if (System.IO.File.Exists(OldImg))
                            {
                                System.IO.File.Delete(OldImg);
                            }
                            if (db.SaveChanges() > 0)
                            {
                                bannerVM.File.SaveAs(path);
                            }
                        }
                        else
                        {
                            ViewBag.Msg = "Image Size must be less than or equal to 1MB";
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Image Extension is invalid";
                    }

                }
                else
                {
                    banner.BannerId = bannerVM.BannerId;
                    banner.BannerName = bannerVM.BannerName;
                    banner.CategoryId = bannerVM.CategoryId;
                    banner.ImgPath = bannerVM.ImgPath;
                    banner.Status = bannerVM.Status;

                    db.Entry(banner).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", banner.CategoryId);
            return View(banner);
        }

        // GET: Banners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: Banners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banner banner = db.Banners.Find(id);
            string CurrentImg = Request.MapPath(banner.ImgPath);
            db.Banners.Remove(banner);
            if (db.SaveChanges() > 0)
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
