using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public partial class ActivityUserTypeTT
    {
        public System.Guid ID { get; set; }
        public System.Guid ActivityID { get; set; }
        public Nullable<int> DeLevelID { get; set; }
         
    }
    public class ActivityUserTypeDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ActivityID { get; set; }
        public Nullable<int> DeLevelID { get; set; }
        public string DeLevelName { get; set; }
    }
}
