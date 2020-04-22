using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Msg
{
    public class MessagesDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid MessageTypeID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public Nullable<System.Guid> AdminID { get; set; }
        public string MessageContent { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public string ReplyContent { get; set; }
        public int DataType { get; set; }
        public int MsgLevel { get; set; }
        public Nullable<System.Guid> PID { get; set; }
        public string Title { get; set; }
        public Nullable<int> MessageState { get; set; }

        public string UserCode { get; set; }
        public string AdminUserCode { get; set; }
        public string TypeName { get; set; }
    }
}
