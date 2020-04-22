using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    public class OrderProductDTO
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> OrderID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public Nullable<decimal> ProductPrice { get; set; }
        public Nullable<int> ProductNum { get; set; }
        public Nullable<int> ShoppingProductType { get; set; }
    }
    public class OrderProductDTOQD
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> OrderID { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public Nullable<decimal> ProductPrice { get; set; }
        public Nullable<int> ProductNum { get; set; }
        public Nullable<int> ShoppingProductType { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string zt { get; set; }
    }
}
