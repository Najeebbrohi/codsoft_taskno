using EcoStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcoStore.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private EcostoreEntities db = new EcostoreEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContactList(Contact contact)
        {
            return View(db.Contacts.ToList());
        }
    }
}