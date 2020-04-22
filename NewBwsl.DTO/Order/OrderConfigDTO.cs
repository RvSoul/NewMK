using AutoMapper;
using NewMK.DTO.Product;
using NewMK.DTO.User;
using NewMK.DTO.Activity;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    public class OrderConfigDTO
    {
        public static List<OrderConfigDTO> GetOrderConfigDTOList(List<OrderConfig> data)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderConfig, OrderConfigDTO>();
                cfg.CreateMap<Model.CM.Product, ProductDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<OrderConfig>, List<OrderConfigDTO>>(data);
        }

        public System.Guid ID { get; set; }
        public System.Guid ProductID { get; set; }
        public int OrderTypeID { get; set; }
        public decimal Price { get; set; }
        public decimal PV { get; set; }
        public string LevelList { get; set; }
        public bool IsRequired { get; set; }
        public Nullable<decimal> PriceMin { get; set; }
        public Nullable<bool> IfCount { get; set; }
        public string OldLevelList { get; set; }
        public Nullable<System.DateTime> STime1 { get; set; }
        public Nullable<System.DateTime> ETime1 { get; set; }
        public ProductDTO Product { get; set; }

    }

    public class Request_OrderConfigDTO : ModelDTO
    {
        public System.Guid? ProductTypeID { get; set; }
        public string KeyWord { get; set; }
        public bool? IsStarProduct { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        [SelectField("and", "=", "int")]
        public int? OrderTypeID { get; set; }

        [SelectField("and", "in", "string")]
        public string LevelList { get; set; }

        [SelectField("and", "in", "string")]
        public string OldLevelList { get; set; }
    }
    public class OrderConfigProductDTO
    {

        public System.Guid ID { get; set; }
        public System.Guid ProductID { get; set; }
        public int OrderTypeID { get; set; }
        public decimal Price { get; set; }
        public decimal PV { get; set; }
        public string LevelList { get; set; }
        public bool IsRequired { get; set; }
        public Nullable<decimal> PriceMin { get; set; }
        public Nullable<bool> IfCount { get; set; }
        public string OldLevelList { get; set; }
        public Nullable<System.DateTime> STime1 { get; set; }
        public Nullable<System.DateTime> ETime1 { get; set; }
        public ProductDTO Product { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary>
        public string ProductTypeName { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public int ProductNum { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public string zt { get; set; }

        public List<ActivityDTO> ActivityList { get; set; }
        public System.Guid ProductTypeID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public System.DateTime AddTime { get; set; }
        public string SalesVolume { get; set; }
        public string KeyWord { get; set; }
        public string ProductWeight { get; set; }
        public string Unit { get; set; }
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
    }

}
