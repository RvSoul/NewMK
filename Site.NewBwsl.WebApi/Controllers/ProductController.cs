using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Activity;
using NewMK.DTO.ManageData;
using NewMK.DTO.Order;
using NewMK.DTO.Product;
using NewMK.Model.CM;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Site.NewMK.WebApi.Controllers
{
    [AllowAnonymous]
    public class ProductController : ApiControllerBase
    {


        ProductDM dm = new ProductDM();
        /// <summary>
        /// 获取产品类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductType")]
        public ResultEntity<List<ProductTypeDTO>> GetProductType()
        {
            return new ResultEntityUtil<List<ProductTypeDTO>>().Success(dm.GetProductType());

        }


        /// <summary>
        /// 产品查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductList")]
        public ResultEntity<List<OrderConfigProductDTO>> GetProductList([FromUri] Request_OrderConfigDTO dto)
        {
            int pagcount = 0;
            List<OrderConfigProductDTO> list = dm.GetProductList(dto, out pagcount);
            return new ResultEntityUtil<List<OrderConfigProductDTO>>().Success(list, pagcount);


        }


        /// <summary>
        /// 根据产品ID查产品详细
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProduct")]
        public ResultEntity<OrderConfigProductDTO> GetProduct(Guid? Pid, int? OrderTypeID)
        {
            return new ResultEntityUtil<OrderConfigProductDTO>().Success(dm.GetProduct(Pid, OrderTypeID));

        }

        /// <summary>
        /// 获取产品所有礼包赠品
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetGiftProduct")]
        public ResultEntity<List<GiftProductDTO>> GetGiftProduct(Guid ProductID)
        {
            return new ResultEntityUtil<List<GiftProductDTO>>().Success(dm.GetGiftProduct(ProductID));

        }

        /// <summary>
        /// 获取产品活动
        /// </summary>
        /// <param name="Pid"></param>
        /// <param name="OrderTypeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductActivity")]
        public ResultEntity<List<ActivityDTO>> GetProductActivity(Guid? Pid, int OrderTypeID)
        {
            return new ResultEntityUtil<List<ActivityDTO>>().Success(dm.GetProductActivity(Pid, OrderTypeID));

        }

        /// <summary>
        /// 根据产品ID获取产品所有属性
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductAttribute")]
        public ResultEntity<List<ProductAttributeDTO>> GetProductAttribute(Guid ProductID)
        {
            return new ResultEntityUtil<List<ProductAttributeDTO>>().Success(dm.GetProductAttribute(ProductID));

        }
        /// <summary>
        /// 根据产品ID获取产品图片
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductImg")]
        public ResultEntity<List<ProductImgDTO>> GetProductImg(Guid ProductID)
        {
            return new ResultEntityUtil<List<ProductImgDTO>>().Success(dm.GetProductImg(ProductID));


        }


        /// <summary>
        /// 根据类型获取图片列表
        /// </summary>
        /// <param name="ImgType">1.PC轮播，2.PC明星产品，3.APP轮播</param>
        ///  <param name="DelevelID">用户级别ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetIndexImagesManage")]
        public ResultEntity<List<IndexImagesManageDTO>> GetIndexImagesManage(int? ImgType, int? DelevelID)
        {
            return new ResultEntityUtil<List<IndexImagesManageDTO>>().Success(dm.GetIndexImagesManage(ImgType, DelevelID));

        }
    }
}
