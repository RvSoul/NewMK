using AutoMapper;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    public class HonLevelDTO
    {
        public static List<HonLevelDTO> GetDTO(List<HonLevel> data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HonLevel, HonLevelDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<HonLevel>, List<HonLevelDTO>>(data);
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> PX { get; set; }
    }
}
