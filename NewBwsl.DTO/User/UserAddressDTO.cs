using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    public class UserAddressDTO
    {
        public System.Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string ReceivingAddress { get; set; }
        public string ReceivingName { get; set; }
        public string ReceivingPhone { get; set; }
        public bool IsDefault { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public string ConsigneeProvince { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeCounty { get; set; }
    }
}
