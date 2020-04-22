using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Activity
{
    public partial class ActivityUserTT
    {
        public System.Guid ID { get; set; }
        public System.Guid ActivityID { get; set; }
        public System.Guid UserID { get; set; }
         
    }
    public class ActivityUserDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ActivityID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }

        public int? DeLevelID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
    }
}
