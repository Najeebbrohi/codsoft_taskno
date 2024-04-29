using EcoStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Nullable<int> Role { get; set; }
        public string ImgPath { get; set; }
        public bool Rememberme { get; set; }
        public HttpPostedFileBase File { get; set; }

        public virtual Role Roles { get; set; }
    }
}