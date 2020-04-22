using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Product
{
    public class ProductImgDTO
    {
        public Nullable<System.Guid> ProductID { get; set; }
        public System.Guid ID { get; set; }
        public Nullable<int> ImgType { get; set; }
        public string Url { get; set; }

        public Nullable<int> PX { get; set; }
    }

    public class SaveProductImgDTO
    {
        public Guid ProductID { get; set; }
        public System.Guid ID { get; set; }
        public int ImgType { get; set; }
        public string Url { get; set; }

        public Nullable<int> PX { get; set; }
    }
}
