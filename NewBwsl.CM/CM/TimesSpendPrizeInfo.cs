//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewMK.Model.CM
{
    using System;
    using System.Collections.Generic;
    
    public partial class TimesSpendPrizeInfo
    {
        public string Id { get; set; }
        public int BalanceNumber { get; set; }
        public string DealerId { get; set; }
        public string SubDealerId { get; set; }
        public int SubDealerLayer { get; set; }
        public decimal SubDealerReduceTotalPrize { get; set; }
        public decimal Percentum { get; set; }
        public Nullable<double> Prize { get; set; }
    }
}
