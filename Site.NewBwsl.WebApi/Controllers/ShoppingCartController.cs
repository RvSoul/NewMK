using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.ShoppingCart;
using Newtonsoft.Json;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Site.NewMK.WebApi.Controllers
{
    [Authorize]
    public class ShoppingCartController : ApiControllerBase
    {

        ShoppingCartDM dm = new ShoppingCartDM();

        /// <summary>
        /// 获取选中购物车产品集合
        /// </summary>
        /// <param name="gidlist">选中购物数据集合json</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/GetShoppingTT")]
        public ResultEntity<ShoppingCartActivityDTO> GetShoppingTT([FromBody]ShoppingOrder gidlist)
        {
            List<ShoppingCartDTO> objs = JsonConvert.DeserializeObject<List<ShoppingCartDTO>>(gidlist.sclist);
            return new ResultEntityUtil<ShoppingCartActivityDTO>().Success(dm.GetShoppingTT(objs, gidlist.UserID, gidlist.OrderTypeID, null));
        }
        /// <summary>
        /// 获取购物车数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetShopping")]
        public ResultEntity<List<ShoppingCartDTO>> GetShopping()
        {
            var data = dm.GetShopping(CurrentUserId);
            return new ResultEntityUtil<List<ShoppingCartDTO>>().Success(data, data.Count);
        }

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="pid">产品ID</param>
        /// <param name="num">数量</param>
        /// <param name="numtype">1,累加。2，直接输入</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddShopping")]
        public ResultEntity<bool> AddShopping(Guid pid, int num, int numtype)
        {
            return new ResultEntityUtil<bool>().Success(dm.AddShopping(pid, num, CurrentUserId, numtype));
        }
        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deShopping")]
        public ResultEntity<bool> deShopping(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.deShopping(id), "删除成功");
        }
    }
}