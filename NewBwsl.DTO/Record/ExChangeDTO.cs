using AutoMapper;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Record
{
    public class ExChangeDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int MoneyType { get; set; }
        public int ZMoneyType { get; set; }
        public decimal BeforeChangeMoney { get; set; }
        public decimal ChangeMoney { get; set; }
        public decimal AfterChangeMoney { get; set; }
        public System.DateTime ChangeTime { get; set; }
        public string ChangeMarks { get; set; }
        public string OrderNum { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<decimal> FeeRat { get; set; }
        public Nullable<decimal> FeeAmount { get; set; }
        public Nullable<decimal> CashAmount { get; set; }
        public Nullable<int> ExChangeType { get; set; }
    }
    public class ExChangeBonus_CashDTO : ExChangeDTO
    {
        public string BankNumber { get; set; }
        public string BakAddress { get; set; }
        public string BakName { get; set; }
        public string BakBranchAddress { get; set; }
    }
    public class Request_ExChangeDTO : ModelDTO
    {
        [SelectField("and", "=", "Guid")]
        public Guid? UserID { get; set; }

        [SelectField("and", "=", "string")]
        public string UserCode { get; set; }

        [SelectField("and", "=", "string")]
        public string UserName { get; set; }

        [SelectField("and", "=", "int")]
        public int? MoneyType { get; set; }

        [SelectField("and", "=", "int")]
        public int? ZMoneyType { get; set; }
         
        public int? ExChangeType { get; set; }

        [SelectField("and", "=", "string")]
        public string OrderNum { get; set; }

        public DateTime? stime { get; set; }
        public DateTime? etime { get; set; }
    }
    public class Request_ExChangeBonus_CashDTO : ModelDTO
    {
        [SelectField("and", "=", "Guid")]
        public Guid? UserID { get; set; }

        [SelectField("and", "=", "string")]
        public string UserCode { get; set; }

        [SelectField("and", "=", "string")]
        public string UserName { get; set; }

        [SelectField("and", "=", "string")]
        public string OrderNum { get; set; }
        
        public int? State { get; set; }

        public DateTime? stime { get; set; }
        public DateTime? etime { get; set; }
    }

}
