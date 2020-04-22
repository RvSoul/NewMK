using Accounting.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Accounting.NewMK.WebApi.Controllers
{
    [AllowAnonymous]
    public class RecordController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        RecordDM recordDM = new RecordDM();
        /// <summary>
        /// 获取资金记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetExChangeList")]
        public ResultEntity<List<ExChangeDTO>> GetExChangeList([FromUri]Request_ExChangeDTO dto)
        {
            ResultEntity<List<ExChangeDTO>> result = new ResultEntity<List<ExChangeDTO>>();

            try
            {
                int count = 0;
                result.Data = recordDM.GetExChangeList(dto, out count);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR; log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 获取提现列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetExChangeListBonus_Cash")]
        public ResultEntity<List<ExChangeBonus_CashDTO>> GetExChangeListBonus_Cash([FromUri]Request_ExChangeBonus_CashDTO dto)
        {
            ResultEntity<List<ExChangeBonus_CashDTO>> result = new ResultEntity<List<ExChangeBonus_CashDTO>>();

            try
            {
                int count = 0;
                result.Data = recordDM.GetExChangeListBonus_Cash(dto, out count);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR; log.Error(error.Message, error);
            }
            return result;
        }
        /// <summary>
        /// 获取提现列表统计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetExChangeListBonus_CashCount")]
        public ResultEntity<ExChangeBonus_CashDTO> GetExChangeListBonus_CashCount([FromUri]Request_ExChangeBonus_CashDTO dto)
        {
            ResultEntity<ExChangeBonus_CashDTO> result = new ResultEntity<ExChangeBonus_CashDTO>();

            try
            {
                int count = 0;
                result.Data = recordDM.GetExChangeListBonus_CashCount(dto);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = 200;
                result.Msg = error.Message;
            }
            return result;
        }
        /// <summary>
        /// 同意审批
        /// </summary>
        /// <param name="dto">ID集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Bonus_Cash_True")]
        public ResultEntity<bool> Bonus_Cash_True([FromBody]IDList dto)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                int count = 0;
                result.Data = recordDM.Bonus_Cash_True(dto, 2);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR; log.Error(error.Message, error);
            }
            return result;
        }
        /// <summary>
        /// 完成审批
        /// </summary>
        /// <param name="dto">ID集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Bonus_Cash_True2")]
        public ResultEntity<bool> Bonus_Cash_True2([FromBody]IDList dto)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                int count = 0;
                result.Data = recordDM.Bonus_Cash_True(dto, 3);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR; log.Error(error.Message, error);
            }
            return result;
        }
        /// <summary>
        /// 驳回审批
        /// </summary>
        /// <param name="dto">ID集合</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/Bonus_Cash_false")]
        public ResultEntity<bool> Bonus_Cash_false([FromBody]IDList2 dto)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();

            try
            {
                int count = 0;
                result.Data = recordDM.Bonus_Cash_false(dto);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR; log.Error(error.Message, error);
            }
            return result;
        }
        [HttpGet]
        [Route("api/GetPro_Tj_Award_Order_AllRat_Web")]
        public ResultEntity<List<RatioDTO>> GetPro_Tj_Award_Order_AllRat_Web(string begin, string end)
        {
            ResultEntity<List<RatioDTO>> result = new ResultEntity<List<RatioDTO>>();

            try
            {
                int count = 0;
                result.Data = recordDM.GetPro_Tj_Award_Order_AllRat_Web(begin, end);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = 200;
                result.Msg = error.Message;
            }
            return result;
        }

        /// <summary>
        /// 店补
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePrizeInfo")]
        public ResultEntity<List<ServiceCenterPrizeInfoDTO>> ServicePrizeInfo(int balancenumber, string dealercode)
        {
            ResultEntity<List<ServiceCenterPrizeInfoDTO>> result = new ResultEntity<List<ServiceCenterPrizeInfoDTO>>();

            try
            {
                int count = 0;
                result.Data = recordDM.ServicePrizeInfo(balancenumber, dealercode);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = 200;
                result.Msg = error.Message;
            }
            return result;
        }

        /// <summary>
        /// 零售积分（原主动消费积分）
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RecordPrizeInfo")]
        public ResultEntity<List<RecordPrizeInfoDTO>> RecordPrizeInfo(int balancenumber, string dealercode)
        {
            ResultEntity<List<RecordPrizeInfoDTO>> result = new ResultEntity<List<RecordPrizeInfoDTO>>();

            try
            {
                int count = 0;
                result.Data = recordDM.RecordPrizeInfo(balancenumber, dealercode);
                result.IsSuccess = true;
                result.Count = count;
                result.Msg = "查询成功！";
            }
            catch (Exception error)
            {
                result.ErrorCode = 200;
                result.Msg = error.Message;
            }
            return result;
        }
    }
}
