using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}