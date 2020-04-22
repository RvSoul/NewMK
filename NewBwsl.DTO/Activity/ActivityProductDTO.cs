using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public class ActivityProductTT
    {
        public System.Guid ID { get; set; }
        public System.Guid ActivityID { get; set; }
        public System.Guid ProductID { get; set; }
        public int NumBase { get; set; }
        public Nullable<int> NumMax { get; set; }
        public Nullable<int> NumMaxAll { get; set; }
        public Nullable<double> Discount { get; set; }
    }
    public class ActivityProductDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ActivityID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public Nullable<int> NumBase { get; set; }
        public Nullable<int> NumMax { get; set; }
        public Nullable<int> NumMaxAll { get; set; }
        public Nullable<double> Discount { get; set; }


        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
