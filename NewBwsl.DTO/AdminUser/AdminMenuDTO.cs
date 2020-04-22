using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.AdminUser
{
    public class AdminMenuDTO
    {
        private bool issucess = false;

        public Guid ID { get; set; }
        public string MenuUrl { get; set; }
        public string MenuExplain { get; set; }
        public string MenuCode { get; set; }
        public Nullable<System.Guid> FID { get; set; }
        public bool isChecked { get { return issucess; } set { issucess = value; } }

        public bool IsShow { get; set; }

        public string SystemName { get; set; }

        public int Sort { get; set; }

        /// <summary>
        /// 0：菜单；1：菜单操作行为
        /// </summary>
        public byte Type { get; set; }

        public List<AdminMenuActionDTO> Action { get; set; }

    }
}
