using EcoStore.Models;
using EcoStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace EcoStore.Controllers
{
    public class OrdersController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();
        public ActionResult OrdersList()
        {
            var orders = (from order in db.Orders
                          join orderdet in db.OrderDetails on order.OrderId equals orderdet.OrderId
                          join product in db.Products on orderdet.ProductId equals product.ProductId
                          select new OrderViewModel
                          {
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

                              OrderId = orderdet.OrderId,
                              Qty = orderdet.Qty,
                              Price = orderdet.Price,

                              ProductName = product.ProductName,
                              ImgPath = product.ImgPath
                          }).ToList();

            return View(orders);
        }
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orders = db.OrderDetails.Where(x => x.OrderId == id).Include(x=>x.Order).Include(x=>x.Product).FirstOrDefault();
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }
    }
}