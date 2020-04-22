using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers
{
    [Authorize]
    public class RecordController : ApiController
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        RecordDM dm = new RecordDM();

        /// <summary>
        /// 活跃期流水
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetActivePeriodRecordList")]
        public ResultEntity<List<ActivePeriodRecordDTO>> GetActivePeriodRecordList([FromUri]Request_ActivePeriodRecordDTO dto)
        {
            int count = 0; 
            return new ResultEntityUtil<List<ActivePeriodRecordDTO>>().Success(dm.GetActivePeriodRecordList(dto, out count),count);
        }

        /// <summary>
        /// 资金流水
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetExChangeList")]
        public ResultEntity<List<ExChangeDTO>> GetExChangeList([FromUri]Request_ExChangeDTO dto)
        {
            int count = 0;
            return new ResultEntityUtil<List<ExChangeDTO>>().Success(dm.GetExChangeList(dto, out count), count);
        }


        /// <summary>
        /// 充值积分
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="money"></param>
        /// <param name="ChangeMarks"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RechargeEleMoney")]
        public ResultEntity<bool> RechargeEleMoney(string usercode, Decimal money, string ChangeMarks)
        {
            return new ResultEntityUtil<bool>().Success(dm.RechargeEleMoney(usercode, money, ChangeMarks), "充值成功");
        }
    }
}