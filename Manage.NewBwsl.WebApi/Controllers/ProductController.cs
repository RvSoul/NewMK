using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.DTO;
using NewMK.DTO.ManageData;
using NewMK.DTO.Order;
using NewMK.DTO.Product;
using NewMK.Model.CM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers
{
    [Authorize]
    public class ProductController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ProductDM dm = new ProductDM();
        //产品图片上传路径
        private string uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ProductImages/";

        #region 产品类型操作
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
        /// 添加产品类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddProductType")]
        public ResultEntity<bool> AddProductType(string name, int? px)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddProductType(name, px));

        }

        /// <summary>
        /// 查询单个产品类型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductTypeFirst")]
        public ResultEntity<ProductTypeDTO> GetProductTypeFirst(Guid id)
        {
            return new ResultEntityUtil<ProductTypeDTO>().Success(dm.GetProductTypeFirst(id));

        }

        /// <summary>
        /// 修改产品类型名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="BgColor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpProductType")]
        public ResultEntity<bool> UpProductType(Guid id, string name, int? px)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpProductType(id, name, px));

        }

        /// <summary>
        /// 删除产品类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deProductType")]
        public ResultEntity<bool> deProductType(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deProductType(id));

        }
        #endregion

        #region 产品操作
        /// <summary>
        /// 产品查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductList")]
        public ResultEntity<List<ProductDTO>> GetProductList([FromUri] Request_ProductDTO dto)
        {
            int pagcount = 0;
            return new ResultEntityUtil<List<ProductDTO>>().Success(dm.GetProductListHD(dto, out pagcount), pagcount);

        }
        /// <summary>
        /// 根据产品ID查产品详细
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProduct")]
        public ResultEntity<ProductDTO> GetProduct(Guid? Pid)
        {
            return new ResultEntityUtil<ProductDTO>().Success(dm.GetProductHD(Pid));

        }

        /// <summary>
        /// 产品添加
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddProduct")]
        public ResultEntity<bool> AddProduct([FromUri] Product pageDataList)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddProduct(pageDataList, CurrentUserId));

        }

        [HttpGet]
        [Route("api/UpProduct")]
        public ResultEntity<bool> UpProduct([FromUri] Product pageDataList)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpProduct(pageDataList, CurrentUserId));

        }

        /// <summary>
        /// 产品出入库
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="stockNum"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpProduct")]
        public ResultEntity<bool> InOutProduct(Guid productId, int stockNum, string message)
        {
            return new ResultEntityUtil<bool>().Success(dm.InOutProduct(productId, stockNum, message, CurrentUserId));
        }

        #endregion

        #region 产品图片操作

        /// <summary>
        /// [已实现] 上传图片
        /// </summary>
        /// <returns>成功返回图片URL</returns>
        [HttpPost]
        [Route("api/UpImages")]
        public ResultEntity<string> UpImages()
        {
            ResultEntity<string> result = new ResultEntity<string>();
            try
            {
                result.IsSuccess = false;
                string imgPath = string.Empty;

                HttpRequest request = System.Web.HttpContext.Current.Request;
                HttpFileCollection fileCollection = request.Files;

                // 判断是否有文件
                if (fileCollection.Count > 0)
                {
                    // 获取图片文件
                    HttpPostedFile httpPostedFile = fileCollection[0];

                    //文件大小
                    int fileSize = httpPostedFile.ContentLength;
                    if (fileSize > 8 * 1024 * 1024)
                    {
                        result.Msg = "上传错误，图片大小不能超过1M";
                        return result;
                    }
                    // 文件扩展名
                    string fileExtension = Path.GetExtension(httpPostedFile.FileName);
                    // 图片名称
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    // 图片上传路径
                    string filePath = uploadPath + fileName;
                    //httpPostedFile.FileName;

                    // 验证图片格式
                    if (fileExtension.Contains(".jpg") || fileExtension.Contains(".png"))
                    {
                        // 如果目录不存在则要先创建
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // 保存新的图片文件
                        while (File.Exists(filePath))
                        {
                            fileName = Guid.NewGuid().ToString() + fileExtension;
                            filePath = uploadPath + fileName;
                        }
                        httpPostedFile.SaveAs(filePath);

                        int a = filePath.Length;
                        // 返回图片URL
                        result.Data = "ProductImages/" + fileName;
                        result.Msg = "图片上传成功";
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.Msg = "请选择jpg/png/bmp格式的图片";
                    }
                }
                else
                {
                    result.Msg = "请先选择图片！";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Msg = e.Message.ToString();
            }
            return result;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SaveProductImg")]
        public ResultEntity<bool> SaveProductImg(dynamic obj)
        {
            string pageDataList = Convert.ToString(obj.pageDataList);
            string tguid = Convert.ToString(obj.ProductID);
            Guid ProductID = new Guid(tguid);

            List<SaveProductImgDTO> objs = JsonConvert.DeserializeObject<List<SaveProductImgDTO>>(pageDataList);
            return new ResultEntityUtil<bool>().Success(dm.SaveProductImg(objs, ProductID));

        }

        [HttpGet]
        [Route("api/SaveProductImg2")]
        public ResultEntity<bool> SaveProductImg2(string pageDataList, Guid ProductID)
        {
            List<SaveProductImgDTO> objs = JsonConvert.DeserializeObject<List<SaveProductImgDTO>>(pageDataList);
            return new ResultEntityUtil<bool>().Success(dm.SaveProductImg(objs, ProductID));

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

        #endregion

        #region 产品属性

        /// <summary>
        /// 获取产品所有属性
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
        /// 添加产品属性
        /// </summary>
        /// <param name="pageDataList"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddAttribute")]
        public ResultEntity<bool> AddAttribute([FromUri] ProductAttributeTT pageDataList)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddAttribute(pageDataList));

        }

        /// <summary>
        /// 删除产品属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deProductAttribute")]
        public ResultEntity<bool> deProductAttribute(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deProductAttribute(id));

        }
        #endregion

        #region 产品，礼包赠品

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
        /// 添加产品礼包赠品
        /// </summary>
        /// <param name="pageDataList"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddGiftProduct")]
        public ResultEntity<bool> AddGiftProduct([FromUri] GiftProductTT pageDataList)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddGiftProduct(pageDataList));

        }

        /// <summary>
        /// 删除产品礼包赠品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deGiftProduct")]
        public ResultEntity<bool> deGiftProduct(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deGiftProduct(id));

        }
        #endregion

        #region 订单产品配置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderConfigList")]
        public ResultEntity<List<OrderConfigDTO>> GetOrderConfigList([FromUri] Request_OrderConfigDTO pageDataList)
        {
            int pagcount = 0;
            return new ResultEntityUtil<List<OrderConfigDTO>>().Success(dm.GetOrderConfigList(pageDataList, out pagcount), pagcount);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddOrderConfig")]
        public ResultEntity<bool> AddOrderConfig([FromUri] OrderConfigDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddOrderConfig(dto));

        }

        ///
        [HttpGet]
        [Route("api/deOrderConfig")]
        public ResultEntity<bool> deOrderConfig(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deOrderConfig(id));

        }
        #endregion

        #region  首页图片管理

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

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AddIndexImagesManage")]
        public ResultEntity<bool> AddIndexImagesManage([FromBody]IndexImagesManageDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddIndexImagesManage(dto));

        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UpIndexImagesManage")]
        public ResultEntity<bool> UpIndexImagesManage([FromBody]IndexImagesManageDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpIndexImagesManage(dto));

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deIndexImagesManage")]
        public ResultEntity<bool> deIndexImagesManage(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deIndexImagesManage(id));


        }



        #endregion
    }
}
