using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.DTO;
using NewMK.DTO.Msg;
using NewMK.DTO.Record;
using NewMK.DTO.User;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Site.NewMK.WebApi.Controllers
{
    //[Authorize]
    public class UserController : ApiControllerBase
    {
         
        UserDM dm = new UserDM();
        decimal sxfbl = 0.05M;

        [HttpGet]
        [Route("api/GetDeLevelList")]
        public ResultEntity<List<DeLevelDTO>> GetDeLevelList()
        {
            return new ResultEntityUtil<List<DeLevelDTO>>().Success(dm.GetDeLevelList());
        }

        /// <summary>
        /// 更新微信昵称和头像
        /// </summary>
        /// <param name="dto"></param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/UpWeiXinUser")]
        public ResultEntity<bool> UpWeiXinUser([FromBody] WeiXinUserinfo dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpWeiXinUser(CurrentUserId, dto));
        }

        /// <summary>
        /// 绑定手机号和名字
        /// </summary>
        /// <param name="Phone">手机号</param>
        /// <param name="name">名字</param>
        /// <param name="yzm">验证码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BDUserPhone")]
        public ResultEntity<bool> BDUserPhone(string Phone, string name, string yzm)
        {
            // 短信验证暂时屏蔽
            if (yzm != null && yzm != "" && yzm != "undefined")
            {
                string cookyzmtt = HttpRuntime.Cache[Phone + "-yzm"] == null ? "" : HttpRuntime.Cache[Phone + "-yzm"].ToString();
                if (cookyzmtt != yzm)
                {
                    throw new DMException("验证码无效！");

                }
                else
                {
                    HttpRuntime.Cache.Remove(Phone + "-yzm");
                }
            }
            else
            {
                throw new DMException("验证码无效！");

            }
            return new ResultEntityUtil<bool>().Success(dm.BDUserPhone(CurrentUserId, name, Phone));
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
            if (userid == null)
            {
                userid = CurrentUserId;
            }
            return new ResultEntityUtil<List<UserAddressDTO>>().Success(dm.GetUserAddressList(userid));
        }

        /// <summary>
        /// 添加地址
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddUserAddress")]
        public ResultEntity<bool> AddUserAddress([FromUri]UserAddressDTO dto)
        {
            dto.UserID = CurrentUserId;
            return new ResultEntityUtil<bool>().Success(dm.AddUserAddress(dto));
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deUserAddress")]
        public ResultEntity<bool> deUserAddress(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deUserAddress(id));
        }

        /// <summary>
        /// 设置默认用户地址
        /// </summary>
        /// <param name="id">地址ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUserAddressIfDefault")]
        public ResultEntity<bool> UpUserAddressIfDefault(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpUserAddressIfDefault(id, CurrentUserId));
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetIdUser")]
        public ResultEntity<UserDTO> GetIdUser()
        {
            return new ResultEntityUtil<UserDTO>().Success(dm.GetIduser(CurrentUserId));
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        [HttpGet]
        [Route("api/SignOutLogin")]
        public void SignOutLogin()
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();

            dm.SignOutLogin(CurrentUserId);
        }

        /// <summary>
        /// 获取收益
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserShouYI")]
        public ResultEntity<UserShouYiDTO> GetUserShouYI()
        {
            return new ResultEntityUtil<UserShouYiDTO>().Success(dm.GetUserShouYI(CurrentUserId));
        }

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
        /// 修改用户信息
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Sex"></param> 
        /// <param name="yzm"></param>
        /// <param name="UserName"></param>
        /// <param name="phone2"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUser")]
        public ResultEntity<bool> UpUser(string Phone, bool Sex, string yzm, string UserName, string phone2)
        {
            //短信验证暂时屏蔽
            if (yzm != null && yzm != "" && yzm != "undefined")
            {
                string cookyzmtt = HttpRuntime.Cache[phone2 + "-yzm"] == null ? "" : HttpRuntime.Cache[phone2 + "-yzm"].ToString();
                if (cookyzmtt != yzm)
                {
                    throw new DMException("验证码无效！");
                }
                else
                {
                    HttpRuntime.Cache.Remove(phone2 + "-yzm");
                }
            }
            else
            {
                throw new DMException("验证码无效！");
            }
            return new ResultEntityUtil<bool>().Success(dm.UpUser(CurrentUserId, Phone, Sex, UserName, ""));
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
        /// 转账
        /// </summary>
        /// <param name="money"></param>
        /// <param name="UserCode">要转给的用户</param>
        /// <param name="PayPwd">密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/TransferMoney")]
        public ResultEntity<bool> TransferMoney(Decimal money, string UserCode, string PayPwd)
        {
            log.Info($"接口输入参数：money({money},UserCode:{UserCode},PayPwd:{PayPwd})");
            return new ResultEntityUtil<bool>().Success(dm.TransferMoney(CurrentUserId, money, UserCode, PayPwd));
        }


        /// <summary>
        /// 奖励积分转电子币
        /// </summary>
        /// <param name="money"></param>
        /// <param name="PayPwd">密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Bonus_Ele")]
        public ResultEntity<bool> Bonus_Ele(Decimal money, string PayPwd)
        {
            return new ResultEntityUtil<bool>().Success(dm.Bonus_Ele(CurrentUserId, money, PayPwd));
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        /// <param name="money"></param>
        /// <param name="PayPwd"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Bonus_Cash")]
        public ResultEntity<bool> Bonus_Cash(Decimal money, string PayPwd)
        {
            return new ResultEntityUtil<bool>().Success(dm.Bonus_Cash(CurrentUserId, money, PayPwd, sxfbl));
        }

        /// <summary>
        /// 查看提现
        /// </summary>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">页大小</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetCash")]
        public ResultEntity<List<ExChangeDTO>> GetCash(int PageIndex, int PageSize)
        {
            int count = 0;
            return new ResultEntityUtil<List<ExChangeDTO>>().Success(dm.GetCash(CurrentUserId, PageIndex, PageSize, out count), count);
        }

        /// <summary>
        /// 验证支付密码
        /// </summary>
        /// <param name="PayPwd"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/IfPayPwd")]
        public ResultEntity<UserMoney> IfPayPwd([FromBody]dynamic PayPwd)
        {
            string Pwd = Convert.ToString(PayPwd.PayPwd);
            return new ResultEntityUtil<UserMoney>().Success(dm.IfPayPwd(CurrentUserId, Pwd));
        }

        /// <summary>
        /// 显示支付方式
        /// </summary>
        /// <param name="orderMoney"></param>
        /// <returns>-1：无积分，不显示积分支付和混合支付，0：混合支付，1：积分支付</returns>
        [HttpGet]
        [Route("api/PayType")]
        public ResultEntity<int> PayType(decimal orderMoney)
        {
            return new ResultEntityUtil<int>().Success(dm.PayType(CurrentUserId, orderMoney));
        }

        /// <summary>
        /// [已实现]获取推荐关系下的顾客
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetGet_DealerRecomBranch")]
        public ResultEntity<List<DealerRelationDTO>> GetGet_DealerRecomBranch()
        {
            return new ResultEntityUtil<List<DealerRelationDTO>>().Success(dm.GetGet_DealerRecomBranch(CurrentUserId));
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
        public ResultEntity<List<MessagesDTO>> GetMessageList()
        {
            return new ResultEntityUtil<List<MessagesDTO>>().Success(dm.GetMessageList(CurrentUserId));
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
        /// 添加留言
        /// </summary>
        /// <param name="MessageTypeID">留言类型</param>
        /// <param name="MessageContent">留言内容</param>
        /// <param name="Title">留言标题</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddMessages")]
        public ResultEntity<bool> AddMessages(Guid MessageTypeID, string MessageContent, string Title)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddMessages(CurrentUserId, MessageTypeID, MessageContent, Title));
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
            return new ResultEntityUtil<bool>().Success(dm.AddMessages2(CurrentUserId, MessageID, replyContent));
        }

        /// <summary>
        /// 获取当前登录用户下面安置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserPlaceList")]
        public ResultEntity<List<UserDTO>> GetUserPlaceList()
        {
            return new ResultEntityUtil<List<UserDTO>>().Success(dm.GetUserPlaceList(CurrentUserId));
        }

        /// <summary>
        /// 获取当前登录用户下面推荐信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserRecomList")]
        public ResultEntity<List<UserDTO>> GetUserRecomList()
        {
            return new ResultEntityUtil<List<UserDTO>>().Success(dm.GetUserRecomList(CurrentUserId));
        }

        /// <summary>
        /// 获取当前用户银行卡信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserBak")]
        public ResultEntity<UserBak> GetUserBak()
        {
            return new ResultEntityUtil<UserBak>().Success(dm.GetUserBak(CurrentUserId));
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        /// <param name="dto"></param> 
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddUserBak")]
        public ResultEntity<bool> AddUserBak([FromUri]UserBak dto)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddUserBak(dto, CurrentUserId));
        }

        /// <summary>
        /// 判断是不是店铺
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IsServiceCenter")]
        public ResultEntity<bool> IsServiceCenter()
        {
            return new ResultEntityUtil<bool>().Success(dm.IsServiceCenter(CurrentUserId));
        }

    }
}
