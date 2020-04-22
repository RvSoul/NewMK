using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Notice
{
    public class Request_NoticeDTO : ModelDTO
    {
        [SelectField("and", "=", "Guid")]
        public Nullable<System.Guid> NoticeTypeID { get; set; }

        [SelectField("and", "=", "Guid")]
        public Nullable<System.Guid> AdminUserID { get; set; }

        [SelectField("and", "in", "string")]
        public string Title { get; set; }
        [SelectField("and", "=", "bool")]
        public Nullable<bool> State { get; set; }
    }
    public class NoticeModel
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> NoticeTypeID { get; set; }
        public Nullable<System.Guid> AdminUserID { get; set; }
        public string Title { get; set; }
        public string Descn { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> ReadQuantity { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<bool> State { get; set; }
    }
    public class NoticeDTO : NoticeModel
    {
        public string AdminUserName { get; set; }
        public string NoticeTypeName { get; set; }
    }
}
