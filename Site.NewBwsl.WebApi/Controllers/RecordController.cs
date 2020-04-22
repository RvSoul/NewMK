using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Record;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Site.NewMK.WebApi.Controllers
{
    public class RecordController : ApiControllerBase
    {
         
        RecordDM dm = new RecordDM();

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
            dto.UserID = CurrentUserId;
            return new ResultEntityUtil<List<ExChangeDTO>>().Success(dm.GetExChangeList(dto, out count), count);
             
        }
    }
}
