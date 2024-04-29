using EcoStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Sub_categories> SubCategories { get; set; }
        public List<Color> Colors { get; set; }
        public List<Size> Sizes { get; set; }
        // New property to store cart items
        public List<Product> CartItems { get; set; }
        public List<Gallery> Galleries { get; set; }
    }
}