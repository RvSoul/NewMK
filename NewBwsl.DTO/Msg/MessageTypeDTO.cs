using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Msg
{
    public class MessageTypeDTO: GetMapperDTO
    {

        public System.Guid ID { get; set; }
        public string TypeName { get; set; }
        public Nullable<int> PX { get; set; }
    }
}
