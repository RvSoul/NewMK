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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.ActivePeriodRecord = new HashSet<ActivePeriodRecord>();
            this.ActivityUser = new HashSet<ActivityUser>();
            this.BrowseRecordsUrl = new HashSet<BrowseRecordsUrl>();
            this.ExChange = new HashSet<ExChange>();
            this.Messages = new HashSet<Messages>();
            this.Order = new HashSet<Order>();
            this.ProductCollection = new HashSet<ProductCollection>();
            this.ProductComment = new HashSet<ProductComment>();
            this.ServiceCenter = new HashSet<ServiceCenter>();
            this.ShoppingCart = new HashSet<ShoppingCart>();
            this.UserAddress = new HashSet<UserAddress>();
            this.UserCoupons = new HashSet<UserCoupons>();
            this.UserPwdCode = new HashSet<UserPwdCode>();
        }
    
        public System.Guid ID { get; set; }
        public int DeLevelID { get; set; }
        public Nullable<int> HonLevelID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string Phone { get; set; }
        public bool Sex { get; set; }
        public string CardCode { get; set; }
        public System.DateTime AddTime { get; set; }
        public Nullable<System.Guid> PID { get; set; }
        public int UserState { get; set; }
        public string OpenID { get; set; }
        public Nullable<int> Source { get; set; }
        public decimal EleMoney { get; set; }
        public decimal BonusMoney { get; set; }
        public int BonusState { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> CountyId { get; set; }
        public int StageCount { get; set; }
        public string LastIP { get; set; }
        public Nullable<System.DateTime> LastTime { get; set; }
        public int TPoint_Point { get; set; }
        public int RPoint_Point { get; set; }
        public string PayPwd { get; set; }
        public string IsBak { get; set; }
        public decimal PVSurplus { get; set; }
        public string BankNumber { get; set; }
        public string BakAddress { get; set; }
        public string BakName { get; set; }
        public string WeiXinName { get; set; }
        public string WeiXinUrl { get; set; }
        public Nullable<bool> IsVip { get; set; }
        public decimal EleMoneyFrozen { get; set; }
        public string Unionid { get; set; }
        public string BakBranchAddress { get; set; }
        public Nullable<int> VersionNum { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivePeriodRecord> ActivePeriodRecord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityUser> ActivityUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BrowseRecordsUrl> BrowseRecordsUrl { get; set; }
        public virtual DeLevel DeLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExChange> ExChange { get; set; }
        public virtual HonLevel HonLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Messages> Messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductCollection> ProductCollection { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductComment> ProductComment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceCenter> ServiceCenter { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCart> ShoppingCart { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAddress> UserAddress { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserCoupons> UserCoupons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserPwdCode> UserPwdCode { get; set; }
    }
}
