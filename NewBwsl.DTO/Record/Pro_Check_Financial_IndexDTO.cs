using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Record
{
    public class Pro_Check_Financial_IndexDTO
    {
        public string UserCode { get; set; }
        public decimal AwardIni { get; set; }
        public decimal AwardMoney { get; set; }
        public decimal AwardMoneyToCash { get; set; }
        public decimal AwardMoneyToEleMoney { get; set; }
        public decimal Compute_QM { get; set; }
        public decimal Award_QM { get; set; }
        public decimal Diff_Award { get; set; }


        public decimal EleMoney_QC { get; set; }
        public decimal EleMoney_OnLineR { get; set; }
        public decimal EleMoney_OffLine { get; set; }
        public decimal EleMoney_WeChat { get; set; }
        public decimal EleMoney_99Bill { get; set; }
        public decimal EleMoney_AwardTo { get; set; }
        public decimal EleMoney_DelOrder { get; set; }
        public decimal EleMoney_OtherExIn { get; set; }
        public decimal EleMoney_Award { get; set; }
        public decimal EleMoney_OldSystemIn { get; set; }
        public decimal EleMoney_PayOrder { get; set; }
        public decimal EleMoney_OtherExOut { get; set; }
        public decimal EleMoney_ToCash { get; set; }
        public decimal EleMoney_QM { get; set; }
        public decimal Diff_EleMoney { get; set; }
    }
}
