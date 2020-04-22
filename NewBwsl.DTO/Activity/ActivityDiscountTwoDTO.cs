using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public    class ActivityDiscountTwoTT
    {
      
        public System.Guid ID { get; set; }
        public System.Guid ActivityID { get; set; }
        public System.Guid ProductID { get; set; }
         
    }
    public class ActivityDiscountTwoDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ActivityID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
