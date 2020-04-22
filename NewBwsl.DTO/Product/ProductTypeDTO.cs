using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Product
{
    public class ProductTypeDTO
    {
        public Guid ID { get; set; }
        public string TypeName { get; set; }
        public int? Px { get; set; }
    }
}
