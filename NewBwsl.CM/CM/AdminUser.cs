//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewMK.Model.CM
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdminUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdminUser()
        {
            this.Messages = new HashSet<Messages>();
            this.AdminUserOperationLog = new HashSet<AdminUserOperationLog>();
            this.Notice = new HashSet<Notice>();
        }
    
        public System.Guid RoleID { get; set; }
        public System.Guid ID { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserCode { get; set; }
        public string UserPwd { get; set; }
        public string SystemName { get; set; }
    
        public virtual AdminRole AdminRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Messages> Messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminUserOperationLog> AdminUserOperationLog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notice> Notice { get; set; }
    }
}
