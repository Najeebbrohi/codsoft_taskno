using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class BrandViewModel
    {
        public int BrandId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}