using EcoStore.Models;
using EcoStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace EcoStore.Controllers
{
    public class HomeController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();

        // Action to display the home page with products and mini cart
        public ActionResult Index()
        {
            // Retrieve cart items from session
            var cartItems = Session["Cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Retrieve other data required for the view
            var model = new HomeViewModel();
            model.Categories = db.Categories.ToList();
            model.Products = db.Products.Where(x => x.Status == true).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int qty)
        {
            // Retrieve product details from database
            var product = db.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                // Retrieve cart items from session
                var cartItems = Session["Cart"] as List<CartViewModel> ?? new List<CartViewModel>();
                // Check if the product is already in the cart
                var existingCartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
                if (existingCartItem != null)
                {
                    // If the product is already in the cart, update the quantity
                    existingCartItem.Qty += qty;
                }
                else
                {
                    // If the product is not in the cart, add it to the cart with the specified quantity
                    cartItems.Add(new CartViewModel { ProductId = productId, ProductName = product.ProductName, Price = product.Price, Qty = qty, ImgPath = product.ImgPath });
                }
                // Update session with modified cart items
                Session["Cart"] = cartItems;
            }
            // Determine the URL of the referring page
            string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsolutePath : "/";

            // Return to the referring page
            return Redirect(returnUrl);
        }
        //[HttpPost]
        //public ActionResult AddToWishlist(int productId, int qty)
        //{
        //    var product = db.Products.FirstOrDefault(x => x.ProductId == productId);
        //    if (product != null)
        //    {
        //        var cartItems = Session["wishlist"] as List<CartViewModel> ?? new List<CartViewModel>();
        //        var existingCartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
        //        if (existingCartItem != null)
        //        {
        //            existingCartItem.Qty += qty;
        //        }
        //        else
        //        {
        //            cartItems.Add(new CartViewModel { ProductId = productId, ProductName = product.ProductName, Price = product.Price, Qty = qty, ImgPath = product.ImgPath });
        //        }
        //        Session["wishlist"] = cartItems;
        //    }

        //    string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsolutePath : "/";
        //    return Redirect(returnUrl);
        //}

        // Action to view product details
        public ActionResult ProductDetails(int? id)
        {
            var product = db.Products.FirstOrDefault(x => x.ProductId == id);
            var gallery = db.Galleries.Where(x => x.ProductId == id).ToList();
            var ratings = db.Ratings.Where(x => x.ProductId == product.ProductId).ToList();
            ViewBag.Galleries = gallery;
            ViewBag.Rating = ratings;

            if (product != null)
            {
                return View(product);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            var cartItems = Session["Cart"] as List<CartViewModel>;
            if (cartItems != null)
            {
                // Remove the product from the cart based on the product ID
                var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    Session["Cart"] = cartItems; // Update session with modified cart items
                }
            }
            return Json(new { success = true }); // Return a JSON response indicating success
        }

        //[HttpPost]
        //public ActionResult RemoveFromCart(int productId, int quantityToRemove)
        //{
        //    var cartItems = Session["Cart"] as List<CartViewModel>;
        //    if (cartItems != null)
        //    {
        //        // Find the product in the cart based on the product ID
        //        var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
        //        if (itemToRemove != null)
        //        {
        //            // Check if the quantity to remove is greater than or equal to the quantity in the cart
        //            if (quantityToRemove >= itemToRemove.Qty)
        //            {
        //                // If the quantity to remove is greater than or equal to the quantity in the cart, remove the product from the cart
        //                cartItems.Remove(itemToRemove);
        //            }
        //            else
        //            {
        //                // If the quantity to remove is less than the quantity in the cart, decrement the quantity
        //                itemToRemove.Qty -= quantityToRemove;

        //                // Check if quantity is now 0, if so, remove the product from the cart
        //                if (itemToRemove.Qty == 0)
        //                {
        //                    cartItems.Remove(itemToRemove);
        //                }
        //            }
        //            Session["Cart"] = cartItems; // Update session with modified cart items
        //        }
        //    }
        //    return Json(new { success = true }); // Return a JSON response indicating success
        //}

        public ActionResult Shop()
        {
            var model = new HomeViewModel();
            try
            {
                using (var db = new EcostoreEntities())
                {
                    model.Sizes = db.Sizes.ToList();
                    model.Colors = db.Colors.ToList();
                    model.Categories = db.Categories.ToList();
                    model.SubCategories = db.Sub_categories.ToList();
                    model.Brands = db.Brands.ToList();
                    model.Products = db.Products.Where(x => x.Status == true).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ModelState.AddModelError("", "An error occurred while processing your request.");
            }

            return View(model);
        }
        
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(ContactViewModel model)
        {
            Contact contact = new Contact();
            string toEmail = "Najeebbrohi9477@gmail.com";
            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.From = new MailAddress(model.Email);
            mail.Subject = model.Subject;
            mail.Body = $"Name : {model.Name} \n {model.Email} \n {model.Message}";

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("najeebbrohi9477@gmail.com", "bpeb oeyk qaws dbcm");
                smtp.Port = 587;
                try
                {
                    contact.Name = model.Name;
                    contact.Email = model.Email;
                    contact.Subject = model.Subject;
                    contact.Message = model.Message;
                    db.Contacts.Add(contact);
                    if(db.SaveChanges() > 0)
                    {
                        smtp.Send(mail);
                        ViewBag.Msg = "Message has been sent";
                        ModelState.Clear();
                        string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsolutePath : "/";
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View();
        }

        public ActionResult SortBy(string SortBy)
        {
            return View();
        }
        public ActionResult Ratings(int ProductId, string Comment, string Name, string Email, int Rating)
        {
            Rating rating = new Rating();
            rating.ProductId = ProductId;
            rating.Name = Name;
            rating.Comment = Comment;
            rating.Email = Email;
            rating.Rating1 = Rating;
            rating.PostedDate = DateTime.Now;
            db.Ratings.Add(rating);
            db.SaveChanges();
            string returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsolutePath : "/";
            return Redirect(returnUrl);
        }
    }
}
