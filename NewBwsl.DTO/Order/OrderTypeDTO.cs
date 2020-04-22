using AutoMapper;
using NewMK.DTO.User;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    public class OrderTypeDTO : GetMapperDTO
    {
        public int ID { get; set; }
        public string OrderTypeName { get; set; }
        public Nullable<int> PX { get; set; }
    }
}
