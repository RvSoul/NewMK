using AutoMapper;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO
{
    public class GetMapperDTO
    {

        public static List<ModelDTO> GetDTOList<Model, ModelDTO>(List<Model> data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model, ModelDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<Model>, List<ModelDTO>>(data);
        }

        public static Model SetModel<Model, ModelDTO>(ModelDTO data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ModelDTO, Model>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<ModelDTO, Model>(data);
        }

        public static ModelDTO GetDTO<Model, ModelDTO>(Model data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model, ModelDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<Model, ModelDTO>(data);
        }


    }
}
