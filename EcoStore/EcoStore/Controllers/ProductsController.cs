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
    public class ProductsController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Brand).Include(p => p.Color).Include(p => p.Size).Include(p => p.Sub_categories);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            Product product = new Product();

            ProductViewModel productVM = new ProductViewModel();

            productVM.SubCategoryId = product.SubCategoryId;
            productVM.BrandId = product.BrandId;
            productVM.ColorId = product.ColorId;
            productVM.SizeId = product.SizeId;
            productVM.ProductName = product.ProductName;
            productVM.Price = product.Price;
            productVM.ShortDescription = product.ShortDescription;
            productVM.LongDescription = product.LongDescription;
            productVM.ImgPath = product.ImgPath;
            productVM.NewArrivals = product.NewArrivals;
            productVM.HotProducts = product.HotProducts;
            productVM.PolularProducts = product.PolularProducts;
            productVM.Qty = product.Qty;
            productVM.Sale = product.Sale;
            productVM.Status = product.Status;

            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name");
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "ColorName");
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Name");
            ViewBag.SubCategoryId = new SelectList(db.Sub_categories, "SubCategoryId", "Name");
            return View(productVM);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel productVM)
        {
            Product product = new Product();
            if (ModelState.IsValid && productVM.File != null)
            {
                string filename = Path.GetFileName(productVM.File.FileName);
                string _file = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/product/"), _file);
                productVM.ImgPath = "~/Content/BeckendAssets/uploaded/product/" + _file;

                product.SubCategoryId = productVM.SubCategoryId;
                product.BrandId = productVM.BrandId;
                product.ColorId = productVM.ColorId;
                product.SizeId = productVM.SizeId;
                product.ProductName = productVM.ProductName;
                product.Price = productVM.Price;
                product.ShortDescription = productVM.ShortDescription;
                product.LongDescription = productVM.LongDescription;
                product.ImgPath = productVM.ImgPath;

                product.NewArrivals = productVM.NewArrivals;
                product.HotProducts = productVM.HotProducts;
                product.PolularProducts = productVM.PolularProducts;
                product.Qty = productVM.Qty;
                product.Sale = productVM.Sale;
                product.Status = productVM.Status;

                _ = db.Products.Add(product);

                if(db.SaveChanges() > 0)
                {
                    if(productVM.File.ContentLength <= 1000000)
                    {
                        productVM.File.SaveAs(path);
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

            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", product.BrandId);
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "ColorName", product.ColorId);
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Name", product.SizeId);
            ViewBag.SubCategoryId = new SelectList(db.Sub_categories, "SubCategoryId", "Name", product.SubCategoryId);
            return View(productVM);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            ProductViewModel productVM = new ProductViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);

            productVM.ProductId = product.ProductId;
            productVM.ProductName = product.ProductName;
            productVM.Price = product.Price;
            productVM.ShortDescription = product.ShortDescription;
            productVM.LongDescription = product.LongDescription;
            productVM.ImgPath = product.ImgPath;

            productVM.NewArrivals = product.NewArrivals;
            productVM.HotProducts = product.HotProducts;
            productVM.PolularProducts = product.PolularProducts;
            productVM.Qty = product.Qty;
            productVM.Sale = product.Sale;
            productVM.Status = product.Status;

            Session["Image"] = product.ImgPath;
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", product.BrandId);
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "ColorName", product.ColorId);
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Name", product.SizeId);
            ViewBag.SubCategoryId = new SelectList(db.Sub_categories, "SubCategoryId", "Name", product.SubCategoryId);
            return View(productVM);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel productVM)
        {
            Product product = new Product();
            if (ModelState.IsValid)
            {
                if(productVM.File != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(productVM.File.FileName);
                    string extension = Path.GetExtension(productVM.File.FileName);
                    filename = Guid.NewGuid().ToString() + filename + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/product/"), filename);
                    productVM.ImgPath = "~/Content/BeckendAssets/uploaded/product/" + filename;

                    product.ProductId = productVM.ProductId;
                    product.ProductName = productVM.ProductName;
                    product.Price = productVM.Price;
                    product.ShortDescription = productVM.ShortDescription;
                    product.LongDescription = productVM.LongDescription;
                    product.SizeId = productVM.SizeId;
                    product.SubCategoryId = productVM.SubCategoryId;
                    product.ColorId = productVM.ColorId;
                    product.BrandId = productVM.BrandId;
                    product.ImgPath = productVM.ImgPath;

                    product.NewArrivals = productVM.NewArrivals;
                    product.HotProducts = productVM.HotProducts;
                    product.PolularProducts = productVM.PolularProducts;
                    product.Qty = productVM.Qty;
                    product.Sale = productVM.Sale;
                    product.Status = productVM.Status;

                    string OldImg = Request.MapPath(Session["Image"].ToString());

                    db.Entry(product).State = EntityState.Modified;

                    if (extension == ".jpeg" || extension == ".png" || extension == ".jpg")
                    {
                        if(productVM.File.ContentLength < 1000000)
                        {
                            if (System.IO.File.Exists(OldImg))
                            {
                                System.IO.File.Delete(OldImg);
                            }
                            if(db.SaveChanges() > 0)
                            {
                                productVM.File.SaveAs(path);
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
                    product.ProductId = productVM.ProductId;
                    product.ProductName = productVM.ProductName;
                    product.Price = productVM.Price;
                    product.ShortDescription = productVM.ShortDescription;
                    product.LongDescription = productVM.LongDescription;
                    product.SizeId = productVM.SizeId;
                    product.SubCategoryId = productVM.SubCategoryId;
                    product.ColorId = productVM.ColorId;
                    product.BrandId = productVM.BrandId;
                    product.ImgPath = Session["Image"].ToString();

                    product.NewArrivals = productVM.NewArrivals;
                    product.HotProducts = productVM.HotProducts;
                    product.PolularProducts = productVM.PolularProducts;
                    product.Qty = productVM.Qty;
                    product.Sale = productVM.Sale;
                    product.Status = productVM.Status;

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", product.BrandId);
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "ColorName", product.ColorId);
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Name", product.SizeId);
            ViewBag.SubCategoryId = new SelectList(db.Sub_categories, "SubCategoryId", "Name", product.SubCategoryId);
            return View();
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            string CurrentImg = Request.MapPath(product.ImgPath);
            db.Products.Remove(product);
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
