using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Notice
{
    public class NoticeTypeDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> PX { get; set; }
    }
}
