using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required]
        public Nullable<int> SubCategoryId { get; set; }
        [Required]
        public Nullable<int> BrandId { get; set; }
        [Required]
        public Nullable<int> ColorId { get; set; }
        [Required]
        public Nullable<int> SizeId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        [Required]
        public Nullable<double> Price { get; set; }
        public string ImgPath { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<bool> HotProducts { get; set; }
        public Nullable<bool> NewArrivals { get; set; }
        public Nullable<bool> PolularProducts { get; set; }
        public Nullable<bool> Sale { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> ProductRating { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}