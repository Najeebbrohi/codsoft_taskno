using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class CartViewModel
    {
        public int ProductId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public double? Price { get; set; }
        public string ImgPath { get; set; }
        public int Qty { get; set; }
        public bool? HotProducts { get; set; }
        public bool? NewArrivals { get; set; }
        public bool? PolularProducts { get; set; }
        public bool? Sale { get; set; }
        public bool? Status { get; set; }
        public int Count { get; set; }
        public int Sum { get; set; }
        public double TotalPrice => (double)(Price * Qty); // Modify TotalPrice property to handle nullable Price

        public CartViewModel()
        {
            Qty = 1;
        }
    }

}