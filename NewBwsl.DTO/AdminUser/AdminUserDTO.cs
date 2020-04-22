using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.AdminUser
{
    public class AdminUserDTO
    {
        public Guid RoleID { get; set; }
        public System.Guid ID { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserCode { get; set; }
        public string UserPwd { get; set; }
        public string RoleName { get; set; }
        public string SystemName { get; set; }
    }
}
