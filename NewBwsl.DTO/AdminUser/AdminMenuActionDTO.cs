using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.AdminUser
{
    public class AdminMenuActionDTO
    {
        public Guid ID { get; set; }
        public Guid MenuID { get; set; }
        public string Action { get; set; }

        public string Comment { get; set; }
    }
}
