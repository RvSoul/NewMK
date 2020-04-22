using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.ManageData
{
    public class ImgDeLevelModel
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ImgID { get; set; }
        public Nullable<int> DeLevelID { get; set; }
    }
    public class ImgDeLevelDTO: ImgDeLevelModel
    { 
        public string DeLevelName { get; set; }
    }
}
