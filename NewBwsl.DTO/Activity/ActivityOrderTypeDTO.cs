using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public class ActivityOrderTypeModel
    {
        public System.Guid ID { get; set; }
        public Nullable<int> OrderTypeID { get; set; }
        public Nullable<System.Guid> ActivityID { get; set; }
    }
    public class ActivityOrderTypeDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<int> OrderTypeID { get; set; }
        public Nullable<System.Guid> ActivityID { get; set; }
        public string OrderTypeName{ get; set; }
    }
}
