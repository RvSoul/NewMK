using AutoMapper;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.ManageData
{
    public partial class IndexImagesManageDTO
    {
        public static List<IndexImagesManageDTO> GetIndexImagesManageDTOList(List<IndexImagesManage> data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndexImagesManage, IndexImagesManageDTO>();
                cfg.CreateMap<ImgDeLevel, ImgDeLevelDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<IndexImagesManage>, List<IndexImagesManageDTO>>(data);
        }
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ImgType { get; set; }
        public string ImgURL { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public Nullable<System.Guid> ProductTypeID { get; set; }
        public Nullable<int> PX { get; set; }

        public List<ImgDeLevelDTO> ImgDeLevel { get; set; }

        public string AddImgDeLevel { get; set; }
    }
}
