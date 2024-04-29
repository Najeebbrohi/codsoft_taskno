using EcoStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class OrderViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.TimeSpan> OrderTime { get; set; }
        public string OrderState { get; set; }
        public string Note { get; set; }

        //Order Details
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> OrderId { get; set; }

        // Products Here
        public string ProductName { get; set; }
        public string ImgPath { get; set; }
    }
}