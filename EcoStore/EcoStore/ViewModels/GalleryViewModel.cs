using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class GalleryViewModel
    {
        public int GalleryId { get; set; }
        public string ImgPath { get; set; }
        public HttpPostedFileBase File { get; set; }
        public Nullable<int> ProductId { get; set; }
    }
}