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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ActivityDiscountTwo = new HashSet<ActivityDiscountTwo>();
            this.ActivityGifts = new HashSet<ActivityGifts>();
            this.ActivityGiftsPurchased = new HashSet<ActivityGiftsPurchased>();
            this.ActivityProduct = new HashSet<ActivityProduct>();
            this.ActivityProductPurchased = new HashSet<ActivityProductPurchased>();
            this.BrowseRecordsUrl = new HashSet<BrowseRecordsUrl>();
            this.GiftProduct = new HashSet<GiftProduct>();
            this.GiftProduct1 = new HashSet<GiftProduct>();
            this.OrderConfig = new HashSet<OrderConfig>();
            this.OrderProduct = new HashSet<OrderProduct>();
            this.ProductAttribute = new HashSet<ProductAttribute>();
            this.ProductComment = new HashSet<ProductComment>();
            this.ProductCollection = new HashSet<ProductCollection>();
            this.ProductImage = new HashSet<ProductImage>();
            this.ShoppingCart = new HashSet<ShoppingCart>();
            this.ProductStockHistory1 = new HashSet<ProductStockHistory>();
        }
    
        public System.Guid ID { get; set; }
        public System.Guid ProductTypeID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public System.DateTime AddTime { get; set; }
        public string SalesVolume { get; set; }
        public string KeyWord { get; set; }
        public string ProductWeight { get; set; }
        public string Unit { get; set; }
        public decimal PV { get; set; }
        public decimal Price { get; set; }
        public bool State { get; set; }
        public int ProductNature { get; set; }
        public string CrProductId { get; set; }
        public Nullable<int> PX { get; set; }
        public Nullable<int> Mulriple { get; set; }
        public bool IsBy { get; set; }
        public Nullable<System.DateTime> STime { get; set; }
        public Nullable<System.DateTime> ETime { get; set; }
        public int StockNumber { get; set; }
        public Nullable<bool> IsStarProduct { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityDiscountTwo> ActivityDiscountTwo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityGifts> ActivityGifts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityGiftsPurchased> ActivityGiftsPurchased { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityProduct> ActivityProduct { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityProductPurchased> ActivityProductPurchased { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BrowseRecordsUrl> BrowseRecordsUrl { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiftProduct> GiftProduct { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiftProduct> GiftProduct1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderConfig> OrderConfig { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        public virtual ProductType ProductType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductAttribute> ProductAttribute { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductComment> ProductComment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductCollection> ProductCollection { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCart> ShoppingCart { get; set; }
        public virtual ProductStockHistory ProductStockHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductStockHistory> ProductStockHistory1 { get; set; }
    }
}
