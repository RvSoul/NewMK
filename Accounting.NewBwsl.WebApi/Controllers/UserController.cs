using Accounting.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Record;
using NewMK.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Accounting.NewMK.WebApi.Controllers
{
    [AllowAnonymous]
    public class UserController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        UserDM dm = new UserDM();
        RecordDM recordDM = new RecordDM();

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="dto">用户电话，可以为空</param> 
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserList")]
        public ResultEntity<List<UserDTO>> GetUserList([FromUri]Request_User dto)
        {
            ResultEntity<List<UserDTO>> result = new ResultEntity<List<UserDTO>>();

            try
            {
                int count = 0;
                result.Data = dm.GetUserList(dto, out count);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 获取安置关系团队
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_GetPRelation_Dealers")]
        public ResultEntity<List<UserRelation>> GetPro_GetPRelation_Dealers(string UserCode, string num)
        {
            ResultEntity<List<UserRelation>> result = new ResultEntity<List<UserRelation>>();


            try
            {
                result.Data = dm.GetPro_GetPRelation_Dealers(UserCode, num);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 获取推荐关系团队
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_GetRRelation_Dealers")]
        public ResultEntity<List<UserRelation>> GetPro_GetRRelation_Dealers(string UserCode, string num)
        {
            ResultEntity<List<UserRelation>> result = new ResultEntity<List<UserRelation>>();


            try
            {
                result.Data = dm.GetPro_GetRRelation_Dealers(UserCode, num);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
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
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                result.Data = dm.UpUserState(userId, 1);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
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
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                result.Data = dm.UpUserState(userId, 2);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
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
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                result.Data = dm.UpBonusState(userId, 1);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
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
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                result.Data = dm.UpBonusState(userId, 0);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }
    }
}
