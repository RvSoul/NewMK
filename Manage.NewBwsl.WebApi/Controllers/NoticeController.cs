using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Notice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers
{
    /// <summary>
    /// 公告处理类
    /// </summary>
    public class NoticeController : ApiControllerBase
    {
        NoticeDM dm = new NoticeDM();

        #region 公告类型操作
        /// <summary>
        /// 获取公告类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetNoticeType")]
        public ResultEntity<List<NoticeTypeDTO>> GetNoticeType()
        {
            return new ResultEntityUtil<List<NoticeTypeDTO>>().Success(dm.GetNoticeType());

        }

        /// <summary>
        /// 添加公告类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddNoticeType")]
        public ResultEntity<bool> AddNoticeType(string name, int? px)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddNoticeType(name, px));

        }

        /// <summary>
        /// 查询单个公告类型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetNoticeTypeFirst")]
        public ResultEntity<NoticeTypeDTO> GetNoticeTypeFirst(Guid id)
        {
            return new ResultEntityUtil<NoticeTypeDTO>().Success(dm.GetNoticeTypeFirst(id));

        }

        /// <summary>
        /// 修改公告类型名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="BgColor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpNoticeType")]
        public ResultEntity<bool> UpNoticeType(Guid id, string name, int? px)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpNoticeType(id, name, px));

        }

        /// <summary>
        /// 删除公告类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deNoticeType")]
        public ResultEntity<bool> deNoticeType(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deNoticeType(id));

        }
        #endregion

        #region 公告操作


        /// <summary>
        /// 公告查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetNoticeList")]
        public ResultEntity<List<NoticeDTO>> GetNoticeList([FromUri] Request_NoticeDTO dto)
        {
            int pagcount = 0;
            return new ResultEntityUtil<List<NoticeDTO>>().Success(dm.GetNoticeList(dto, out pagcount), pagcount);
        }

        /// <summary>
        /// 根据公告ID查公告详细
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetNotice")]
        public ResultEntity<NoticeDTO> GetNotice(Guid? id)
        {
            return new ResultEntityUtil<NoticeDTO>().Success(dm.GetNotice(id));
        }

        /// <summary>
        /// 公告添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AddNotice")]
        public ResultEntity<bool> AddNotice([FromBody] NoticeModel pageDataList)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddNotice(pageDataList, CurrentUserId));

        }

        [HttpPost]
        [Route("api/UpNotice")]
        public ResultEntity<bool> UpNotice([FromBody] NoticeModel pageDataList)
        {
            return new ResultEntityUtil<bool>().Success(dm.UpNotice(pageDataList, CurrentUserId));
        }
        [HttpGet]
        [Route("api/deNotice")]
        public ResultEntity<bool> deNotice(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deNotice(id));
        }


        #endregion
    }
}
