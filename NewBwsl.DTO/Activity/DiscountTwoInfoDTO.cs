using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public partial class DiscountTwoInfoTT
    {
        public System.Guid ID { get; set; }
        public System.Guid DiscountTwoID { get; set; }
        public int Num { get; set; }
        public double Discount { get; set; }
         
    }
    public class DiscountTwoInfoDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> DiscountTwoID { get; set; }
        public Nullable<int> Num { get; set; }
        public Nullable<double> Discount { get; set; }
    }
}
