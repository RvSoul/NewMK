using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Record
{
    public class RatioDTO
    {
        public string EndDate { get; set; }
        public decimal TotalPrice_Valid { get; set; }
        public decimal TotalPv_Valid { get; set; }
        public decimal TotalPrice_InValid { get; set; }
        public decimal TotalPv_InValid { get; set; }
        public decimal Awardamount { get; set; }
        public decimal rat_amount { get; set; }
        public decimal PV_amount { get; set; }
    }
}
