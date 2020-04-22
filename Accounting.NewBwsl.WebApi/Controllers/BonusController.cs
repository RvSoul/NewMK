using Accounting.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Bonus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Accounting.NewMK.WebApi.Controllers
{
    public class BonusController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BonusDM dm = new BonusDM();

        /// <summary>
        /// 查询奖励积分列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BalanceWeekPrize")]
        public ResultEntity<List<WaitWeekBalance>> BalanceWeekPrize()
        {
            ResultEntity<List<WaitWeekBalance>> result = new ResultEntity<List<WaitWeekBalance>>();
            try
            {
                result.Data = dm.BalanceWeekPrize();
                result.IsSuccess = true;
                result.Count = result.Data.Count;
                result.Msg = "";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 计算周奖
        /// </summary>
        /// <param name="date"></param>
        [HttpGet]
        [Route("api/CountWeekPrize")]
        public ResultEntity<WaitWeekBalance> CountWeekPrize(DateTime date)
        {
            ResultEntity<WaitWeekBalance> result = new ResultEntity<WaitWeekBalance>();
            string adminname = "admin";
            try
            {
                adminname = RequestContext.Principal.Identity.Name;
            }
            catch
            {
                adminname = "admin";
            }
            try
            {
                result.Data = dm.BalanceWeekPrize("", date, adminname);
                result.IsSuccess = true;
                result.Msg = "";
                result.Count = 1;
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }
        /// <summary>
        /// 查询奖励积分结算状态
        /// </summary>
        /// <param name="banlance"></param>
        /// <param name="type">0.周奖，1.月奖</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/QueryBalance")]
        public ResultEntity<bool> QueryBalance(int banlance, int type)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();
            try
            {
                result.IsSuccess = true;
                result.Data = dm.QueryBalance(banlance, type);
            }
            catch (Exception error)
            {
                result.IsSuccess = true;
                result.Data = false;
            }
            return result;
        }

        /// <summary>
        /// 等待发放奖励积分的周奖记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WaitGrantWeekPrize")]
        public ResultEntity<List<WaitWeekBalance>> WaitGrantWeekPrize()
        {
            ResultEntity<List<WaitWeekBalance>> result = new ResultEntity<List<WaitWeekBalance>>();
            try
            {
                result.Data = dm.GetWaitGrantWeekBalanceList();
                result.IsSuccess = true;
                result.Count = result.Data.Count;
                result.Msg = "";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 发放周奖
        /// </summary>
        /// <param name="balancenumber"></param>
        [HttpGet]
        [Route("api/GrantWeekPrize")]
        public ResultEntity<string> GrantWeekPrize(int balancenumber)
        {
            ResultEntity<string> result = new ResultEntity<string>();
            string adminname = "admin";
            try
            {
                adminname = RequestContext.Principal.Identity.Name;
            }
            catch
            {
                adminname = "admin";
            }
            try
            {
                string msg = dm.GrantPrize("", balancenumber, adminname);
                if (string.IsNullOrWhiteSpace(msg))
                {
                    result.IsSuccess = true;
                    result.Data = "";
                    result.Msg = "";
                    result.Count = 1;
                }
                else
                {
                    result.ErrorCode = 113;
                    result.Msg = msg;
                }
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 已发周奖详情
        /// </summary>
        /// <param name="num"></param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结算时间</param>
        /// <param name="dealercode"></param>
        /// <param name="dealername"></param>
        /// <param name="page">页码</param>
        /// <param name="size">每页条数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WeekPrize")]
        public ResultEntity<List<WeekPrizeHSDTO>> WeekPrize(int? num, DateTime? start, DateTime? end, string dealercode, string dealername, int page, int size)
        {
            ResultEntity<List<WeekPrizeHSDTO>> result = new ResultEntity<List<WeekPrizeHSDTO>>();
            try
            {
                int totalcount = 0;
                result.Data = dm.GetWeekPrizeList(true, num, start, end, dealercode, dealername, page, size, out totalcount);
                result.IsSuccess = true;
                result.Count = totalcount;
                result.Msg = "";
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }

        /// <summary>
        /// 已发周奖详情统计
        /// </summary>
        /// <param name="num"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dealercode"></param>
        /// <param name="dealername"></param>
        /// <param name="grant"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/TotalWeekPrize")]
        public ResultEntity<WeekPrizeDTO> TotalWeekPrize(int? num, DateTime? start, DateTime? end, string dealercode, string dealername, bool grant = true)
        {
            ResultEntity<WeekPrizeDTO> result = new ResultEntity<WeekPrizeDTO>();
            try
            {
                result.Data = dm.GetWeekPrizeTotal(grant, num, start, end, dealercode, dealername);
                result.IsSuccess = true;
                result.Count = 1;
                result.Msg = "";
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
