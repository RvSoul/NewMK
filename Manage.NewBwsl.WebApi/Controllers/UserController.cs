using Infrastructure.Utility;
using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Msg;
using NewMK.DTO.User;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class UserController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        UserDM dm = new UserDM();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetDeLevelList")]
        public ResultEntity<List<DeLevelDTO>> GetDeLevelList()
        {
            return new ResultEntityUtil<List<DeLevelDTO>>().Success(dm.GetDeLevelList());

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetHonLevelList")]
        public ResultEntity<List<HonLevelDTO>> GetHonLevelList()
        {
            return new ResultEntityUtil<List<HonLevelDTO>>().Success(dm.GetHonLevelList());

        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="dto">用户电话，可以为空</param> 
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserList")]
        public ResultEntity<List<UserDTO>> GetUserList([FromUri]Request_User dto)
        {
            int count = 0;
            return new ResultEntityUtil<List<UserDTO>>().Success(dm.GetUserList(dto, out count), count);

        }


        /// <summary>
        /// 修改用户级别
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUserType")]
        public ResultEntity<bool> UpUserType([FromUri]UserDTO dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpUserType(dto));

        }

        /// <summary>
        /// 解绑微信
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DeWeiXinOpenId")]
        public ResultEntity<bool> DeWeiXinOpenId(string usercode)
        {
            return new ResultEntityUtil<bool>().Success(dm.DeWeiXinOpenId(usercode));

        }


        /// <summary>
        /// 获取留言类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetMessageTypeList")]
        public ResultEntity<List<MessageTypeDTO>> GetMessageTypeList()
        {
            return new ResultEntityUtil<List<MessageTypeDTO>>().Success(dm.GetMessageTypeList());
             
        }
        /// <summary>
        /// 获取留言列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetMessageList")]
        public ResultEntity<List<MessagesDTO>> GetMessageList(string usercode, int PageIndex, int PageSize)
        {
            log.Info($"接口输入参数，usercode:{usercode},PageIndex:{PageIndex},PageSize:{PageSize}");
            int count = 0;
            return new ResultEntityUtil<List<MessagesDTO>>().Success(dm.GetMessageList(usercode, PageIndex, PageSize, out count),count);
             
        }

        /// <summary>
        /// 获取留言回复列表
        /// </summary>
        /// <param name="messagesId">留言ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetMessageList2")]
        public ResultEntity<List<MessagesDTO>> GetMessageList2(Guid messagesId)
        {
            return new ResultEntityUtil<List<MessagesDTO>>().Success(dm.GetMessageList2(messagesId));
             
        }

        /// <summary>
        /// 添加留言回复
        /// </summary>
        /// <param name="MessageID">留言ID</param>
        /// <param name="replyContent">回复内容</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddMessages2")]
        public ResultEntity<bool> AddMessages2(Guid MessageID, string replyContent)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddMessages3(CurrentUserId, MessageID, replyContent));
             
        }

        /// <summary>
        /// 留言关闭
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MessageClose")]
        public ResultEntity<bool> MessageClose(Guid ID)
        {
            return new ResultEntityUtil<bool>().Success(dm.MessageClose(ID));
             
        }

        /// <summary>
        /// 添加店铺
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddServiceCenter")]
        public ResultEntity<bool> AddServiceCenter([FromUri]ServiceCenterDTO dto)
        {
            //短信验证暂时屏蔽
            //if (dto.yzm != null && dto.yzm != "" && dto.yzm != "undefined")
            //{
            //    string cookyzmtt = HttpRuntime.Cache["yzm"] == null ? "" : HttpRuntime.Cache["yzm"].ToString();
            //    if (cookyzmtt != dto.yzm)
            //    {
            //        result.Msg = "验证码无效！";
            //        return result;
            //    }
            //    else
            //    {
            //        HttpRuntime.Cache.Remove("yzm");
            //    }
            //}
            //else
            //{
            //    result.Msg = "验证码无效!";
            //    return result;
            //}
            return new ResultEntityUtil<bool>().Success(dm.AddServiceCenter(dto));
 
        }

        /// <summary>
        /// 获取店铺列表
        /// </summary>
        /// <param name="rdto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetServiceCenterList")]
        public ResultEntity<List<ServiceCenterDTO>> GetServiceCenterList([FromUri]Request_ServiceCenter rdto)
        {
            int num = 0;
            return new ResultEntityUtil<List<ServiceCenterDTO>>().Success(dm.GetServiceCenterList(rdto, out num), num);
             
        }

        /// <summary>
        /// 获取单个店铺
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetServiceCenter")]
        public ResultEntity<ServiceCenterDTO> GetServiceCenter(Guid id)
        {
            return new ResultEntityUtil<ServiceCenterDTO>().Success(dm.GetServiceCenter(id) );
             
        }

        /// <summary>
        /// 修改店铺资料
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpServiceCenter")]
        public ResultEntity<bool> UpServiceCenter([FromUri]ServiceCenterDTO dto)
        {
            //短信验证暂时屏蔽
            //if (dto.yzm != null && dto.yzm != "" && dto.yzm != "undefined")
            //{
            //    string cookyzmtt = HttpRuntime.Cache["yzm"] == null ? "" : HttpRuntime.Cache["yzm"].ToString();
            //    if (cookyzmtt != dto.yzm)
            //    {
            //        result.Msg = "验证码无效！";
            //        return result;
            //    }
            //    else
            //    {
            //        HttpRuntime.Cache.Remove("yzm");
            //    }
            //}
            //else
            //{
            //    result.Msg = "验证码无效!";
            //    return result;
            //}
            return new ResultEntityUtil<bool>().Success(dm.UpServiceCenter(dto));

             
        }

        /// <summary>
        /// 店铺审核
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AuditResult"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SHServiceCenter")]
        public ResultEntity<bool> SHServiceCenter(Guid ID, int AuditResult)
        {
            return new ResultEntityUtil<bool>().Success(dm.SHServiceCenter(ID, AuditResult));
             
        }

        /// <summary>
        /// 解锁登录状态
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUserStateTrue")]
        public ResultEntity<bool> UpUserStateTrue(Guid userId)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpUserState(userId, 1));
 
        }

        /// <summary>
        /// 锁定登录状态
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUserStateFalse")]
        public ResultEntity<bool> UpUserStateFalse(Guid userId)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpUserState(userId, 2));
             
        }

        /// <summary>
        /// 解锁奖励积分状态
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpBonusStateTrue")]
        public ResultEntity<bool> UpBonusStateTrue(Guid userId)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpBonusState(userId, 1));
             
        }

        /// <summary>
        /// 锁定奖励积分状态
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpBonusStateFalse")]
        public ResultEntity<bool> UpBonusStateFalse(Guid userId)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpBonusState(userId, 0));
             
        }
        /// <summary>
        /// 获取当前用户银行卡信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserBak")]
        public ResultEntity<UserBak> GetUserBak(Guid userid)
        {
            return new ResultEntityUtil<UserBak>().Success(dm.GetUserBak(userid));
             
        }
        /// <summary>
        /// 更换银行卡
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddUserBak")]
        public ResultEntity<bool> AddUserBak([FromUri]UserBak dto, Guid userid)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddUserBak(dto, userid));
             
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="yzm">验证码</param>
        /// <param name="userPwd">密码</param>
        /// <param name="usercode">用户编号</param>
        /// <param name="level">1.登录密码，2.支付密码，3.全部</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUserPwd")]
        public ResultEntity<bool> UpUserPwd(string phone, string yzm, string userPwd, string usercode, string level)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpUserPwdHT(userPwd, usercode, level));
             
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Sex"></param> 
        /// <param name="yzm"></param>
        /// <param name="UserName"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUser")]
        public ResultEntity<bool> UpUser(string Phone, bool Sex, string yzm, string UserName, Guid UserID, string CardCode)
        { 
            //短信验证暂时屏蔽
            //if (yzm != null && yzm != "" && yzm != "undefined")
            //{
            //    string cookyzmtt = HttpRuntime.Cache["yzm"] == null ? "" : HttpRuntime.Cache["yzm"].ToString();
            //    if (cookyzmtt != yzm)
            //    {
            //        //result.Msg = "验证码无效！";
            //        //return result;
            //    }
            //    else
            //    {
            //        HttpRuntime.Cache.Remove("yzm");
            //    }
            //}
            //else
            //{
            //    result.Msg = "验证码无效!";
            //    return result;
            //}
            return new ResultEntityUtil<bool>().Success(dm.UpUser(UserID, Phone, Sex, UserName, CardCode));
             
        }

        /// <summary>
        /// 验证是否有位置关系
        /// </summary>
        /// <param name="code1">上级编号</param>
        /// <param name="code2">下级编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/CKRelation")]
        public ResultEntity<CKRelationDTO> CKRelation(string code1, string code2)
        {
            return new ResultEntityUtil<CKRelationDTO>().Success(dm.CKRelation(code1, code2));
             
        }

        /// <summary>
        /// 获取当前登录用户下面安置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserPlaceList")]
        public ResultEntity<List<UserDTO>> GetUserPlaceList(Guid? userid)
        {
            return new ResultEntityUtil<List<UserDTO>>().Success(dm.GetUserPlaceList(userid));
             
        }

        /// <summary>
        /// 获取当前登录用户下面推荐信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserRecomList")]
        public ResultEntity<List<UserDTO>> GetUserRecomList(Guid? userid)
        {
            return new ResultEntityUtil<List<UserDTO>>().Success(dm.GetUserRecomList(userid));
             
        }

        /// <summary>
        /// 获取用户地址列表
        /// </summary>
        /// <param name="userid">用户ID，传空获取自己的</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserAddressList")]
        public ResultEntity<List<UserAddressDTO>> GetUserAddressList(Guid? userid)
        {
            return new ResultEntityUtil<List<UserAddressDTO>>().Success(dm.GetUserAddressList(userid));
        }

        /// <summary>
        /// 推荐业绩查询
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Get_RecomCase_Query")]
        public ResultEntity<List<Pro_Get_RecomCase_QueryDTO>> GetPro_Get_RecomCase_Query(DateTime? time1, DateTime? time2, string code)
        {
            return new ResultEntityUtil<List<Pro_Get_RecomCase_QueryDTO>>().Success(dm.GetPro_Get_RecomCase_Query(time1, time2, code));
        }

        #region 前端业务
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetIdUser")]
        public ResultEntity<UserDTO> GetIdUser(Guid? userid)
        {
            return new ResultEntityUtil<UserDTO>().Success(dm.GetIduser(userid));
             
        }

        /// <summary>
        /// 根据编号获取用户信息
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserInfo")]
        public ResultEntity<UserDTO> GetUserInfo(string usercode)
        {
            return new ResultEntityUtil<UserDTO>().Success(dm.GetUserInfo(usercode));
             
        }

        /// <summary>
        /// 获取业绩
        /// </summary>
        /// <param name="PCDealerCode"></param>
        /// <param name="DeptSelect"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Get_BranchAch")]
        public ResultEntity<UserYeJi> GetPro_Get_BranchAch(string PCDealerCode, string DeptSelect)
        {
            return new ResultEntityUtil<UserYeJi>().Success(dm.GetPro_Get_BranchAch(PCDealerCode, DeptSelect));
             
        }

        /// <summary>
        /// 获取下滑
        /// </summary>
        /// <param name="PCDealerCode"></param>
        /// <param name="DeptSelect"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Get_RegCust_ManageNode")]
        public ResultEntity<UserXiahua> GetPro_Get_RegCust_ManageNode(string PCDealerCode, string DeptSelect)
        {
            return new ResultEntityUtil<UserXiahua>().Success(dm.GetPro_Get_RegCust_ManageNode(PCDealerCode, DeptSelect));
             
        }

        /// <summary>
        /// 验证安置人是否在推荐下
        /// </summary>
        /// <param name="PPdealercode"></param>
        /// <param name="PCdealercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_PP_PCRelation_Check")]
        public ResultEntity<UserIsShanXia> GetPro_PP_PCRelation_Check(string PPdealercode, string PCdealercode)
        {
            return new ResultEntityUtil<UserIsShanXia>().Success(dm.GetPro_PP_PCRelation_Check(PPdealercode, PCdealercode));
             
        }

        /// <summary>
        /// 重新获取AB区
        /// </summary>
        /// <param name="PCDealerCode"></param>
        /// <param name="PPDealerCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Get_AreaNameByManageCode")]
        public ResultEntity<UserABqu> GetPro_Get_AreaNameByManageCode(string PCDealerCode, string PPDealerCode)
        {
            return new ResultEntityUtil<UserABqu>().Success(dm.GetPro_Get_AreaNameByManageCode(PCDealerCode, PPDealerCode));
             
        }

        /// <summary>
        /// 获取店铺
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Get_StoreNo")]
        public ResultEntity<UserDianPu> GetPro_Get_StoreNo(string UserCode)
        {
            return new ResultEntityUtil<UserDianPu>().Success(dm.GetPro_Get_StoreNo(UserCode));
             
        }


        /// <summary>
        /// 代购注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RegisterWeiXin")]
        public ResultEntity<string> RegisterWeiXin([FromUri]UserDTO model)
        {
            return new ResultEntityUtil<string>().Success(dm.Register(model, CurrentUserId));
             
        }

        /// <summary>
        /// 后台升级用户接口
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PCDealerCode"></param>
        /// <param name="PPDealerCode"></param>
        /// <param name="DeptSelect"></param>
        /// <param name="DeLevelID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AdminUserUpUser")]
        public ResultEntity<string> AdminUserUpUser(string UserCode, string PCDealerCode, string PPDealerCode, string DeptSelect, int DeLevelID)
        {
            return new ResultEntityUtil<string>().Success(dm.AdminUserUpUser(UserCode, PCDealerCode, PPDealerCode, DeptSelect, DeLevelID, CurrentUserId));
             
        }
         
        #endregion
    }
}
