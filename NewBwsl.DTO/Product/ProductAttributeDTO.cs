using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Product
{
    public class ProductAttributeTT
    {
        public System.Guid ID { get; set; }
        public Guid ProductID { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
        public Nullable<int> PX { get; set; }
    }
    public class ProductAttributeDTO
    {
        public System.Guid ID { get; set; }
        public Guid ProductID { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
        public Nullable<int> PX { get; set; }
    }
}
