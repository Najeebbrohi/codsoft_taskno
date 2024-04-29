using EcoStore.Models;
using EcoStore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace EcoStore.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        EcostoreEntities db = new EcostoreEntities();

        public class PasswordEncryption
        {
            public string EncryptPassword(string password)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // Password ko byte array mein convert karna
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                    // Byte array ko hexadecimal string mein convert karna
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            public static bool VerifyPassword(string userInputPassword, string storedPasswordHash)
            {
                // Compute hash of user input password
                PasswordEncryption passwordEncryption = new PasswordEncryption();
                string userInputHash = passwordEncryption.EncryptPassword(userInputPassword);

                // Compare hashed passwords
                return userInputHash == storedPasswordHash;
            }
        }
        public ActionResult Index()
        {
            var user = db.Users.ToList();
            return View(user);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var user = db.Users.Single(x => x.Id == id);
            if(user == null)
            {
                return HttpNotFound();
            }
            return View(user);

        }
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Password ko byte array mein convert karna
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Byte array ko hexadecimal string mein convert karna
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserViewModel u)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cookie = new HttpCookie("User");
                
                var user = db.Users.Where(x => x.Email == u.Email).SingleOrDefault();
                if (user != null)
                {
                    // Password ko verify karein
                    bool isAuthenticated = PasswordEncryption.VerifyPassword(u.Password, user.Password);

                    if (isAuthenticated)
                    {
                        // Authentication successful
                        FormsAuthentication.SetAuthCookie(u.Email, false);
                        return RedirectToAction("Index", "Admin");
                    }
                }

                // Agar user nahi mila ya password match nahi hua
                ModelState.AddModelError("Message", "Invalid User");
            }
            return View();
        }
        public ActionResult Signup()
        {
            ViewBag.Roles = new SelectList(db.Roles, "Id", "RoleName");
            return View();
        }
        [HttpPost]
        public ActionResult Signup(UserViewModel userMV)
        {
            if (ModelState.IsValid)
            {
                if (userMV.File != null && userMV.File.ContentLength < 1000000)
                {
                    string fullfilename = Path.GetFileName(userMV.File.FileName);
                    string _filename = Guid.NewGuid().ToString() + fullfilename;
                    string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/"), _filename);
                    userMV.ImgPath = "~/Content/BeckendAssets/uploaded/" + _filename;

                    // Password ko encrypt karne ke liye PasswordEncryption class ka istemal karna
                    PasswordEncryption passwordEncryption = new PasswordEncryption();
                    string encryptedPassword = passwordEncryption.EncryptPassword(userMV.Password);

                    User user = new User
                    {
                        FirstName = userMV.FirstName,
                        LastName = userMV.LastName,
                        Username = userMV.Username,
                        Email = userMV.Email,
                        Password = encryptedPassword, // Encrypt kiya hua password ko set karna,
                        Gender = userMV.Gender,
                        Role = userMV.Role,
                        Phone = userMV.Phone,
                        DateOfBirth = userMV.DateOfBirth,
                        Address = userMV.Address,
                        ImgPath = userMV.ImgPath
                    };

                    db.Users.Add(user);
                    if (db.SaveChanges() > 0)
                    {
                        userMV.File.SaveAs(path);
                        ViewBag.Msg = "User Added Successfully";
                        ModelState.Clear();
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Error adding user to database";
                    }
                    ViewBag.Roles = new SelectList(db.Roles, "Id", "RoleName", user.Role);
                }
                else
                {
                    ViewBag.ErrorMsg = "Invalid file or file size exceeds 1MB";
                }
            }
            else
            {
                ViewBag.ErrorMsg = "Invalid input data";
            }
            return View();
        }
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Where(x => x.Id == id).FirstOrDefault();

            UserViewModel userVM = new UserViewModel();
            userVM.Id = user.Id;
            userVM.FirstName = user.FirstName;
            userVM.LastName = user.LastName;
            userVM.Username = user.Username;
            userVM.Gender = user.Gender;
            userVM.Email = user.Email;
            userVM.Password = user.Password;
            userVM.DateOfBirth = user.DateOfBirth;
            userVM.Phone = user.Phone;
            userVM.Address = user.Address;
            userVM.ImgPath = user.ImgPath;
            userVM.Role = user.Role;

            Session["ImgPath"] = userVM.ImgPath;

            ViewBag.Roles = new SelectList(db.Roles, "Id", "RoleName");
            return View(userVM);
        }
        [HttpPost]
        public ActionResult Edit(UserViewModel userMV)
        {
            User user = new User();
            if (ModelState.IsValid)
            {
                if(userMV.File != null)
                {
                    if (userMV.File.ContentLength < 1000000)
                    {
                        string fullfilename = Path.GetFileName(userMV.File.FileName);
                        string _filename = Guid.NewGuid().ToString() + fullfilename;
                        string path = Path.Combine(Server.MapPath("~/Content/BeckendAssets/uploaded/"), _filename);
                        userMV.ImgPath = "~/Content/BeckendAssets/uploaded/" + _filename;

                        // Password ko encrypt karne ke liye PasswordEncryption class ka istemal karna
                        PasswordEncryption passwordEncryption = new PasswordEncryption();
                        string encryptedPassword = passwordEncryption.EncryptPassword(userMV.Password);

                        user.Id = userMV.Id;
                        user.FirstName = userMV.FirstName;
                        user.LastName = userMV.LastName;
                        user.Username = userMV.Username;
                        user.Email = userMV.Email;
                        user.Password = encryptedPassword; // Encrypt kiya hua password ko set karna,
                        user.Gender = userMV.Gender;
                        user.Role = userMV.Role;
                        user.Phone = userMV.Phone;
                        user.DateOfBirth = userMV.DateOfBirth;
                        user.Address = userMV.Address;
                        user.ImgPath = userMV.ImgPath;

                        string OldImage = Request.MapPath(Session["ImgPath"].ToString());

                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            if (System.IO.File.Exists(OldImage))
                            {
                                System.IO.File.Delete(OldImage);
                            }
                            userMV.File.SaveAs(path);
                            ViewBag.Msg = "User Updated Successfully";
                            ModelState.Clear();
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "Error adding user to database";
                        }
                        ViewBag.Roles = new SelectList(db.Roles, "Id", "RoleName", user.Role);
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Invalid file or file size exceeds 1MB";
                    }
                }
                else
                {
                    user.Id = userMV.Id;
                    user.FirstName = userMV.FirstName;
                    user.LastName = userMV.LastName;
                    user.Username = userMV.Username;
                    user.Gender = userMV.Gender;
                    user.Email = userMV.Email;
                    user.Password = userMV.Password;
                    user.DateOfBirth = userMV.DateOfBirth;
                    user.Phone = userMV.Phone;
                    user.Address = userMV.Address;
                    user.Role = userMV.Role;
                    user.ImgPath = Session["ImgPath"].ToString();

                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index","Accounts");
                }
            }
            ViewBag.Roles = new SelectList(db.Roles, "Id", "RoleName", user.Role);
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Accounts");
        }
    }
}