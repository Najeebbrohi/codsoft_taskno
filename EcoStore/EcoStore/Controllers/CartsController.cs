using EcoStore.Models;
using EcoStore.ViewModels;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EcoStore.Controllers
{
    [HandleError(View = "Error")]
    public class CartsController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();
        // GET: Carts
        public ActionResult Cart()
        {
            return View();
        }
        public ActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PlaceOrder(Order order, string Payment)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now.Date;
                order.OrderTime = DateTime.Now.TimeOfDay;
                order.OrderState = "Pending";
                order.Payment = Payment;
                db.Orders.Add(order);
                db.SaveChanges();

                // Save order details to database
                if (Session["Cart"] != null)
                {
                    var cartItems = (List<CartViewModel>)Session["Cart"];
                    foreach (var cartItem in cartItems)
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = cartItem.ProductId,
                            Qty = cartItem.Qty,
                            Price = cartItem.Price
                        };
                        db.OrderDetails.Add(orderDetail);
                    }
                    db.SaveChanges();
                }
                // Clear the cart or perform any other necessary cleanup
                Session["Cart"] = null;

                // Send order confirmation email
                SendEmail(order);

                // Redirect to Thankyou action with orderId parameter
                return RedirectToAction("Thankyou", "Carts", new { orderId = order.OrderId });
            }
            // If model state is not valid, redirect to error page or handle as per your requirement
            return RedirectToAction("Error");
        }
        
        public ActionResult Thankyou(int? orderId)
        {
            // Check if orderId is null or negative
            if (orderId == null || orderId <= 0)
            {
                // Redirect to an error page or return a specific view
                return RedirectToAction("Index","Home");
            }

            // Check if order exists and is associated with the current user (example)
            var orderlist = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (orderlist == null)
            {
                // Redirect to an error page or return a specific view
                return RedirectToAction("Index", "Home");
            }

            // Get order details and display Thankyou page
            var orders = (from order in db.Orders
                          join orderdet in db.OrderDetails on order.OrderId equals orderdet.OrderId
                          join product in db.Products on orderdet.ProductId equals product.ProductId
                          where order.OrderId == orderId
                          select new OrderViewModel
                          {
                              OrderId = order.OrderId,
                              FirstName = order.FirstName,
                              LastName = order.LastName,
                              Email = order.Email,
                              Phone = order.Phone,
                              Address = order.Address,
                              Country = order.Country,
                              State = order.State,
                              City = order.City,
                              ZipCode = order.ZipCode,
                              OrderDate = order.OrderDate,
                              OrderTime = order.OrderTime,
                              OrderState = order.OrderState,
                              Note = order.Note,

                              Qty = orderdet.Qty,
                              Price = orderdet.Price,

                              ProductName = product.ProductName,
                              ImgPath = product.ImgPath
                          }).ToList();

            return View(orders);
        }

        [HttpPost]
        public ActionResult Update(FormCollection form)
        {
            var cartItems = (List<CartViewModel>)Session["Cart"];
            string[] Quanitity = form.GetValues("Qty");
            for (int i = 0; i < cartItems.Count; i++)
            {
                cartItems[i].Qty = Convert.ToInt32(Quanitity[i]);
            }
            Session["Cart"] = cartItems;
            return View("Cart");
        }
        
        public ActionResult StripePayment(int orderId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = (100 * 10), // Stripe accepts amount in cents, so multiply by 100
                Currency = "usd", // Change to your currency
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
        {
            { "OrderId", order.OrderId.ToString() }
        }
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);
            ViewBag.PaymentIntentClientSecret = paymentIntent.ClientSecret;

            // Pass the Publishable Key to the view
            ViewBag.StripePublishableKey = "pk_test_51P58UwIdeg02yrKOLxTPvjiroj5MeYZCKEWhFmgu2xyc2sXVgTNF0dOCfy2hjv3DtsNCpz6bGEXttxBfI92mU1aZ00DPjrOaBR"; // Replace "your_publishable_key" with your actual Stripe publishable key

            return View();
        }
        private void SendEmail(Order model)
        {
            //var orderDetails = db.OrderDetails.Where(od => od.OrderId == model.OrderId).ToList();
            var orderdet = (from order in db.Orders
                            join orderde in db.OrderDetails
                            on order.OrderId equals orderde.OrderId
                            where orderde.OrderId == model.OrderId
                            select new { order.FirstName, order.LastName, order.Address, order.OrderDate, order.OrderId, order.Phone, orderde.Product.ProductName, orderde.Qty, orderde.Price }
                            ).ToList();

            string toEmail = model.Email;
            string Subject = "Order Confirmation Message";
            string emailBody = $"Thank you for your order. \n\nYour Order Detials: \n";
            foreach (var detail in orderdet)
            {
                emailBody += $"Order Id: {detail.OrderId}\n";
                emailBody += $"Customer Name: {detail.FirstName} {detail.LastName}\n";
                emailBody += $"Customer Address: {detail.Address}\n";
                emailBody += $"Customer Contact: {detail.Phone}\n";
                emailBody += $"Order Date: {detail.OrderDate}\n";
                emailBody += $"Product Name: {detail.ProductName}\n";
                emailBody += $"Product Price: {detail.Price}\n";
                emailBody += $"Quantity: {detail.Qty}\n\n";
            }

            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.From = new MailAddress(model.Email);
            mail.Subject = Subject;
            mail.Body = emailBody;

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("najeebbrohi9477@gmail.com", "bpeb oeyk qaws dbcm");
                smtp.Port = 587;

                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    // Handle exception (log it, display an error message, etc.)
                    throw new Exception($"Error sending email: {ex.Message}", ex);
                }
            }
        }
    }
}