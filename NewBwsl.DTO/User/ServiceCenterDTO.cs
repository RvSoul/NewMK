using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    public class Request_ServiceCenter : ModelDTO
    {

    }
    public class ServiceCenterModel
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public string ServiceCenterName { get; set; }
        public Nullable<System.DateTime> ApplyDate { get; set; }
        public Nullable<System.DateTime> AuditDate { get; set; }
        public Nullable<int> AuditResult { get; set; }
        public Nullable<int> SCType { get; set; }
        public string CManager { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> CountyId { get; set; }
        public string Consignee { get; set; }
        public string ReceivingCall { get; set; }
        public string ReceivingAddress { get; set; }
        public string ZipCode { get; set; }
        public string OldName { get; set; }
        public string OldID { get; set; }
        public Nullable<bool> IFOrder { get; set; }
        public Nullable<int> IsHealthCheckRight { get; set; }
        public string ParentDealerCode { get; set; }
    }
    public class ServiceCenterDTO : ServiceCenterModel
    {
        public string yzm { get; set; }
        public string UserCode { get; set; }
    }
}
