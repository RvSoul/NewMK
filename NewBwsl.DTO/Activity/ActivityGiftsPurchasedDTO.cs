﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public partial class ActivityGiftsPurchasedTT
    {
        public System.Guid ID { get; set; }
        public System.Guid ActivityID { get; set; }
        public System.Guid GiftsID { get; set; }
        public int ProducNum { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public Nullable<System.Guid> OrderID { get; set; }
         
    }
    public class ActivityGiftsPurchasedDTO
    {
    }
}
