using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.AdminUser
{
    public class Request_AdminUserOperationLogDTO : ModelDTO
    {
        [SelectField("and", "in", "string")]
        public string AdminUserCode { get; set; }

        [SelectField("and", "in", "string")]
        public string OrderNumber { get; set; }

        [SelectField("and", "in", "string")]
        public string OrderTypeName { get; set; }

        [SelectField("and", "in", "string")]
        public string UserCode { get; set; }

        [SelectField("and", "in", "int")]
        public int? OperationType { get; set; }

        public Nullable<System.DateTime> STime { get; set; }
        public Nullable<System.DateTime> ETime { get; set; }
    }
    public class AdminUserOperationLogDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> AdminUserID { get; set; }
        public string AdminUserCode { get; set; }
        public string OrderNumber { get; set; }
        public string OrderTypeName { get; set; }
        public string UserCode { get; set; }
        public Nullable<decimal> MoneyProduct { get; set; }
        public Nullable<decimal> MoneyTransport { get; set; }
        public Nullable<decimal> OrderMoney { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public string OldLevel { get; set; }
        public string NewLevel { get; set; }
        public Nullable<int> OperationType { get; set; }
        public string PCDealerCode { get; set; }
        public string PPDealerCode { get; set; }
        public string DeptName { get; set; }
    }
}
