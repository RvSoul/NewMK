using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public class ActivityGiftsTT
    {
        public System.Guid ID { get; set; }
        public System.Guid GiftID { get; set; }
        public System.Guid ActivityID { get; set; }
        public int NumBase { get; set; }
        public Nullable<int> NumMax { get; set; }
        public Nullable<int> NumMaxAll { get; set; }
        public Nullable<decimal> MoneyMin { get; set; }
        public Nullable<decimal> MoneyMax { get; set; }

    }
    public class ActivityGiftsDTO: ActivityGiftsTT
    { 

        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        public string ProductImg { get; set; }
    }
}
