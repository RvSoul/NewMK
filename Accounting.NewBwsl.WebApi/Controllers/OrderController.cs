using Accounting.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Order;
using NewMK.DTO.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Accounting.NewMK.WebApi.Controllers
{
    public class OrderController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        OrderDM dm = new OrderDM();
        /// <summary>
        /// 获取订单类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderTypeList")]
        public ResultEntity<List<OrderTypeDTO>> GetOrderTypeList()
        {
            ResultEntity<List<OrderTypeDTO>> result = new ResultEntity<List<OrderTypeDTO>>();

            try
            {
                int count = 0;
                result.Data = dm.GetOrderTypeList();
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
        /// 获取订单列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrdersList")]
        public ResultEntity<List<OrdersDTO>> GetOrdersList([FromUri]Request_Order dto)
        {
            ResultEntity<List<OrdersDTO>> result = new ResultEntity<List<OrdersDTO>>();

            try
            {
                int count = 0;
                result.Data = dm.GetOrdersList(dto, out count);
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
        /// 订单统计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrdersCount")]
        public ResultEntity<OrdersDTO> GetOrdersCount([FromUri]Request_Order dto)
        {
            ResultEntity<OrdersDTO> result = new ResultEntity<OrdersDTO>();

            try
            {
                int count = 0;
                result.Data = dm.GetOrdersCount(dto);
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
        /// 资金核对
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="sBegin">时间不传时分秒</param>
        /// <param name="sEnd">时间不传时分秒</param>
        /// <param name="Index">'奖励积分'或者'电子币' 或'Prize'：奖励积分/'EleMoney':电币</param>
        /// <param name="IsChenk"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Check_Financial_Index")]
        public ResultEntity<List<Pro_Check_Financial_IndexDTO>> GetPro_Check_Financial_Index(string Code, string sBegin, string sEnd, string Index, string IsChenk, int pageSize, int pageIndex)
        {
            List<Pro_Check_Financial_IndexDTO> li = dm.GetPro_Check_Financial_Index(Code, sBegin, sEnd, Index, IsChenk);

            return new ResultEntityUtil<List<Pro_Check_Financial_IndexDTO>>().Success(li.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), li.Count);

        }


        /// <summary>
        /// 资金核对统计
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="sBegin">时间不传时分秒</param>
        /// <param name="sEnd">时间不传时分秒</param>
        /// <param name="Index">'奖励积分'或者'电子币' 或'Prize'：奖励积分/'EleMoney':电币</param>
        /// <param name="IsChenk"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Check_Financial_IndexCount")]
        public ResultEntity<Pro_Check_Financial_IndexDTO> GetPro_Check_Financial_IndexCount(string Code, string sBegin, string sEnd, string Index, string IsChenk, int pageSize, int pageIndex)
        {
            ResultEntity<Pro_Check_Financial_IndexDTO> result = new ResultEntity<Pro_Check_Financial_IndexDTO>();

            try
            {
                List<Pro_Check_Financial_IndexDTO> li = dm.GetPro_Check_Financial_Index(Code, sBegin, sEnd, Index, IsChenk);

                Pro_Check_Financial_IndexDTO dto = new Pro_Check_Financial_IndexDTO();
                dto.AwardIni = li.Sum(w => w.AwardIni);
                dto.AwardMoney = li.Sum(w => w.AwardMoney);
                dto.AwardMoneyToCash = li.Sum(w => w.AwardMoneyToCash);
                dto.AwardMoneyToEleMoney = li.Sum(w => w.AwardMoneyToEleMoney);
                dto.Compute_QM = li.Sum(w => w.Compute_QM);
                dto.Award_QM = li.Sum(w => w.Award_QM);
                dto.Diff_Award = li.Sum(w => w.Diff_Award);

                dto.EleMoney_QC = li.Sum(w => w.EleMoney_QC);
                dto.EleMoney_OnLineR = li.Sum(w => w.EleMoney_OnLineR);
                dto.EleMoney_OffLine = li.Sum(w => w.EleMoney_OffLine);
                dto.EleMoney_WeChat = li.Sum(w => w.EleMoney_WeChat);
                dto.EleMoney_99Bill = li.Sum(w => w.EleMoney_99Bill);
                dto.EleMoney_AwardTo = li.Sum(w => w.EleMoney_AwardTo);
                dto.EleMoney_DelOrder = li.Sum(w => w.EleMoney_DelOrder);
                dto.EleMoney_OtherExIn = li.Sum(w => w.EleMoney_OtherExIn);
                dto.EleMoney_Award = li.Sum(w => w.EleMoney_Award);
                dto.EleMoney_OldSystemIn = li.Sum(w => w.EleMoney_OldSystemIn);
                dto.EleMoney_PayOrder = li.Sum(w => w.EleMoney_PayOrder);
                dto.EleMoney_OtherExOut = li.Sum(w => w.EleMoney_OtherExOut);
                dto.EleMoney_ToCash = li.Sum(w => w.EleMoney_ToCash);
                dto.EleMoney_QM = li.Sum(w => w.EleMoney_QM);
                dto.Diff_EleMoney = li.Sum(w => w.Diff_EleMoney);

                result.Data = dto;

                result.IsSuccess = true;
                result.Count = li.Count; ;
                result.Msg = "";
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
