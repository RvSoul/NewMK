using AutoMapper;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    public class DeLevelDTO : GetMapperDTO
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> MinPV { get; set; }
        public Nullable<decimal> MaxPV { get; set; }
        public Nullable<int> PX { get; set; }
    }
}
