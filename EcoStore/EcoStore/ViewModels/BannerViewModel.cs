using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoStore.ViewModels
{
    public class BannerViewModel
    {
        public int BannerId { get; set; }
        public string BannerName { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<bool> Status { get; set; }
        public string ImgPath { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}