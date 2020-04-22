using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Activity;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers
{
    [Authorize]
    public class ActivityController : ApiControllerBase
    {
         
        ActivityDM dm = new ActivityDM();
        #region 活动操作

        //ModelState.IsValid


        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivity")]
        public ResultEntity<List<ActivityDTO>> GetActivity()
        {
            int count = 0;
            return new ResultEntityUtil<List<ActivityDTO>>().Success(dm.GetActivity(out count),count);
             
        }

        /// <summary>
        /// 添加活动
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivity")]
        [AllowAnonymous]
        public ResultEntity<bool> AddActivity([FromUri]ActivityTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivity(dto));
             
        }


        /// <summary>
        /// 查询单个活动信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityFirst")]
        public ResultEntity<ActivityDTO> GetActivityFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityDTO>().Success(dm.GetActivityFirst(id));

             
        }

        /// <summary>
        /// 修改活动名称
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivity")]
        public ResultEntity<bool> UpActivity([FromUri]ActivityTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivity(dto));
             
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivity")]
        public ResultEntity<bool> deActivity(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivity(id));
             
        }
        #endregion

        #region 活动产品操作
        /// <summary>
        /// 获取活动产品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityProduct")]
        public ResultEntity<List<ActivityProductDTO>> GetActivityProduct(Guid id)
        {
            return new ResultEntityUtil<List<ActivityProductDTO>>().Success(dm.GetActivityProduct(id));
             
        }

        /// <summary>
        /// 添加活动产品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityProduct")]
        public ResultEntity<bool> AddActivityProduct([FromUri]ActivityProductTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityProduct(dto));
 
        }

        /// <summary>
        /// 查询单个活动产品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityProductFirst")]
        public ResultEntity<ActivityProductDTO> GetActivityProductFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityProductDTO>().Success(dm.GetActivityProductFirst(id));
 
        }

        /// <summary>
        /// 修改活动产品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityProduct")]
        public ResultEntity<bool> UpActivityProduct([FromUri]ActivityProductTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityProduct(dto));
             
        }

        /// <summary>
        /// 删除活动产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityProduct")]
        public ResultEntity<bool> deActivityProduct(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityProduct(id));
 
        }

        #endregion

        #region 活动赠品操作

        /// <summary>
        /// 获取活动赠品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityGifts")]
        public ResultEntity<List<ActivityGiftsDTO>> GetActivityGifts(Guid id)
        {
            return new ResultEntityUtil<List<ActivityGiftsDTO>>().Success(dm.GetActivityGifts(id));
             
        }

        /// <summary>
        /// 添加活动赠品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityGifts")]
        public ResultEntity<bool> AddActivityGifts([FromUri]ActivityGiftsTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityGifts(dto));
             
        }

        /// <summary>
        /// 查询单个活动赠品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityGiftsFirst")]
        public ResultEntity<ActivityGiftsDTO> GetActivityGiftsFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityGiftsDTO>().Success(dm.GetActivityGiftsFirst(id));
             
        }

        /// <summary>
        /// 修改活动赠品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityGifts")]
        public ResultEntity<bool> UpActivityGifts([FromUri]ActivityGiftsTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityGifts(dto));
             
        }

        /// <summary>
        /// 删除活动赠品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityGifts")]
        public ResultEntity<bool> deActivityGifts(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityGifts(id));
 
        }
        #endregion

        #region 折上折类型2

        #region 折上折类型2数据
        /// <summary>
        /// 获取折上折类型2数据列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityDiscountTwo")]
        public ResultEntity<List<ActivityDiscountTwoDTO>> GetActivityDiscountTwo(Guid id)
        {
            return new ResultEntityUtil<List<ActivityDiscountTwoDTO>>().Success(dm.GetActivityDiscountTwo(id));
             
        }

        /// <summary>
        /// 添加折上折类型2数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityDiscountTwo")]
        public ResultEntity<bool> AddActivityDiscountTwo([FromUri]ActivityDiscountTwoTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityDiscountTwo(dto));

             
        }

        /// <summary>
        /// 查询单个折上折类型2数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityDiscountTwoFirst")]
        public ResultEntity<ActivityDiscountTwoDTO> GetActivityDiscountTwoFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityDiscountTwoDTO>().Success(dm.GetActivityDiscountTwoFirst(id));

            
        }

        /// <summary>
        /// 修改折上折类型2数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityDiscountTwo")]
        public ResultEntity<bool> UpActivityDiscountTwo([FromUri]ActivityDiscountTwoTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityDiscountTwo(dto));
             
        }

        /// <summary>
        /// 删除折上折类型2数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityDiscountTwo")]
        public ResultEntity<bool> deActivityDiscountTwo(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityDiscountTwo(id));
 
        }
        #endregion

        #region 折上折数值
        /// <summary>
        /// 获取折上折数值列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetDiscountTwoInfo")]
        public ResultEntity<List<DiscountTwoInfoDTO>> GetDiscountTwoInfo(Guid id)
        {
            return new ResultEntityUtil<List<DiscountTwoInfoDTO>>().Success(dm.GetDiscountTwoInfo(id));
 
        }

        /// <summary>
        /// 添加折上折数值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddDiscountTwoInfo")]
        public ResultEntity<bool> AddDiscountTwoInfo([FromUri]DiscountTwoInfoTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddDiscountTwoInfo(dto));

             
        }

        /// <summary>
        /// 查询单个折上折数值信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetDiscountTwoInfoFirst")]
        public ResultEntity<DiscountTwoInfoDTO> GetDiscountTwoInfoFirst(Guid id)
        {
            return new ResultEntityUtil<DiscountTwoInfoDTO>().Success(dm.GetDiscountTwoInfoFirst(id));
 
        }

        /// <summary>
        /// 修改折上折数值
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpDiscountTwoInfo")]
        public ResultEntity<bool> UpDiscountTwoInfo([FromUri]DiscountTwoInfoTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpDiscountTwoInfo(dto));
             
        }

        /// <summary>
        /// 删除折上折数值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deDiscountTwoInfo")]
        public ResultEntity<bool> deDiscountTwoInfo(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deDiscountTwoInfo(id));
 
        }
        #endregion

        #endregion

        #region 折上折类型1
        /// <summary>
        /// 获取折上折类型1列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityDiscountOneInfo")]
        public ResultEntity<List<ActivityDiscountOneInfoDTO>> GetActivityDiscountOneInfo(Guid id)
        {
            return new ResultEntityUtil<List<ActivityDiscountOneInfoDTO>>().Success(dm.GetActivityDiscountOneInfo(id));
             
        }

        /// <summary>
        /// 添加折上折类型1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityDiscountOneInfo")]
        public ResultEntity<bool> AddActivityDiscountOneInfo([FromUri]ActivityDiscountOneInfoTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityDiscountOneInfo(dto));
             
        }

        /// <summary>
        /// 查询单个折上折类型1信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityDiscountOneInfoFirst")]
        public ResultEntity<ActivityDiscountOneInfoDTO> GetActivityDiscountOneInfoFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityDiscountOneInfoDTO>().Success(dm.GetActivityDiscountOneInfoFirst(id));
             
        }

        /// <summary>
        /// 修改折上折类型1
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityDiscountOneInfo")]
        public ResultEntity<bool> UpActivityDiscountOneInfo([FromUri]ActivityDiscountOneInfoTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityDiscountOneInfo(dto));
 
        }

        /// <summary>
        /// 删除折上折类型1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityDiscountOneInfo")]
        public ResultEntity<bool> deActivityDiscountOneInfo(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityDiscountOneInfo(id));
 
        }

        #endregion

        #region 活动用户列表
        /// <summary>
        /// 获取活动用户列表数据列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityUser")]
        public ResultEntity<List<ActivityUserDTO>> GetActivityUser(Guid id)
        {
            return new ResultEntityUtil<List<ActivityUserDTO>>().Success(dm.GetActivityUser(id));
             
        }

        /// <summary>
        /// 添加活动用户列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityUser")]
        public ResultEntity<bool> AddActivityUser([FromUri]ActivityUserTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityUser(dto));
             
        }

        /// <summary>
        /// 查询单个活动用户列表数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityUserFirst")]
        public ResultEntity<ActivityUserDTO> GetActivityUserFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityUserDTO>().Success(dm.GetActivityUserFirst(id));
             
        }

        /// <summary>
        /// 修改活动用户列表数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityUser")]
        public ResultEntity<bool> UpActivityUser([FromUri]ActivityUserTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityUser(dto));
             
        }

        /// <summary>
        /// 删除活动用户列表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityUser")]
        public ResultEntity<bool> deActivityUser(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityUser(id));
             
        }
        #endregion

        #region 活动用户类型列表
        /// <summary>
        /// 获取活动用户列表类型数据列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityUserType")]
        public ResultEntity<List<ActivityUserTypeDTO>> GetActivityUserType(Guid id)
        {
            return new ResultEntityUtil<List<ActivityUserTypeDTO>>().Success(dm.GetActivityUserType(id));
             
        }

        /// <summary>
        /// 添加活动用户列表类型数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityUserType")]
        public ResultEntity<bool> AddActivityUserType([FromUri]ActivityUserTypeTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityUserType(dto));
             
        }

        /// <summary>
        /// 查询单个活动用户列表类型数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityUserTypeFirst")]
        public ResultEntity<ActivityUserTypeDTO> GetActivityUserTypeFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityUserTypeDTO>().Success(dm.GetActivityUserTypeFirst(id));
 
        }

        /// <summary>
        /// 修改活动用户列表类型数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityUserType")]
        public ResultEntity<bool> UpActivityUserType([FromUri]ActivityUserTypeTT dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityUserType(dto));
             
        }

        /// <summary>
        /// 删除活动用户列表类型数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityUserType")]
        public ResultEntity<bool> deActivityUserType(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityUserType(id));
             
        }
        #endregion

        #region 活动订单类型列表
        /// <summary>
        /// 获取活动订单列表类型数据列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityOrderType")]
        public ResultEntity<List<ActivityOrderTypeDTO>> GetActivityOrderType(Guid id)
        {
            return new ResultEntityUtil<List<ActivityOrderTypeDTO>>().Success(dm.GetActivityOrderType(id));
             
        }

        /// <summary>
        /// 添加活动订单列表类型数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddActivityOrderType")]
        public ResultEntity<bool> AddActivityOrderType([FromUri]ActivityOrderTypeModel dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddActivityOrderType(dto));
             
        }

        /// <summary>
        /// 查询单个活动订单列表类型数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivityOrderTypeFirst")]
        public ResultEntity<ActivityOrderTypeDTO> GetActivityOrderTypeFirst(Guid id)
        {
            return new ResultEntityUtil<ActivityOrderTypeDTO>().Success(dm.GetActivityOrderTypeFirst(id));
             
        }

        /// <summary>
        /// 修改活动订单列表类型数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpActivityOrderType")]
        public ResultEntity<bool> UpActivityOrderType([FromUri]ActivityOrderTypeModel dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpActivityOrderType(dto));
             
        }

        /// <summary>
        /// 删除活动订单列表类型数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deActivityOrderType")]
        public ResultEntity<bool> deActivityOrderType(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deActivityOrderType(id));
             
        }
        #endregion
    }
}
