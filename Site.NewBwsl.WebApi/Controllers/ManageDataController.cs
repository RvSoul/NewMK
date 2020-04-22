using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.ManageData;
using NewMK.DTO.Notice;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Site.NewMK.WebApi.Controllers
{
    [AllowAnonymous]
    public class ManageDataController : ApiControllerBase
    {

        ManageDataDM dm = new ManageDataDM();
        /// <summary>
        /// 获取省市县地址
        /// </summary>
        /// <param name="id">父级ID</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAreaList")]
        public ResultEntity<List<AreasDTO>> GetAreaList(int? id, int level)
        {
            return new ResultEntityUtil<List<AreasDTO>>().Success(dm.GetAreaList(id, level));
        }

        [HttpGet]
        [Route("api/GetNoticeList")]
        public ResultEntity<List<NoticeDTO>> GetNoticeList([FromUri] ModelDTO dto)
        {
            NoticeDM dm = new NoticeDM();
            int pagcount = 0;
            return new ResultEntityUtil<List<NoticeDTO>>().Success(dm.GetNoticeList(dto, out pagcount), pagcount);

        }
    }
}
