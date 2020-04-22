using Infrastructure.Utility;
using NewMK.Domian.Common;
using NewMK.Domian.DM;
using NewMK.Domian.ThirdParty.lingkaiDX;
using NewMK.DTO;
using NewMK.DTO.Order;
using NewMK.DTO.ShoppingCart;
using Newtonsoft.Json;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Utility;

namespace Site.NewMK.WebApi.Controllers
{
    [Authorize]
    public class OrderController : ApiControllerBase
    {

        OrderDM dm = new OrderDM();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderTypeList")]
        public ResultEntity<List<OrderTypeDTO>> GetOrderTypeList()
        {
            return new ResultEntityUtil<List<OrderTypeDTO>>().Success(dm.GetOrderTypeList());

        }

        /// <summary>
        /// 获取运费
        /// </summary>
        /// <param name="gidlist">购物车实体对象，填充ID，产品ID，数量</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/CountYf")]
        public ResultEntity<decimal> CountYf([FromBody]ShoppingCartActivityDTO gidlist)
        {
            log.Info($"入口请求参数:{gidlist.ToJSON()}");
            return new ResultEntityUtil<decimal>().Success(dm.CountYf(gidlist.sclist));
        }

        /// <summary>
        /// 提交订单 
        /// </summary>
        /// <param name="gidlist"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AddOrders")]
        public ResultEntity<OrdersModel> AddOrders([FromBody]ShoppingOrder gidlist)
        {
            log.Info($"AddOrders接口入口请求参数:{gidlist.ToJSON()}");
            UserCaheUtil.Validate(CurrentUserId.ToString());
            ShoppingCartDM scdm = new ShoppingCartDM();
            List<ShoppingCartDTO> objs = JsonConvert.DeserializeObject<List<ShoppingCartDTO>>(gidlist.sclist);
            ShoppingCartActivityDTO dto = scdm.GetShoppingTT(objs, gidlist.UserID, gidlist.OrderTypeID, null);
            log.Info($"AddOrders接口实际使用请求参数:{objs.ToJSON()}");
            return new ResultEntityUtil<OrdersModel>().Success(dm.AddOrders(dto, gidlist.UserID, gidlist.DeliverType, gidlist.ConsigneeName, gidlist.ConsigneePhone, gidlist.ConsigneeProvince, gidlist.ConsigneeCity, gidlist.ConsigneeCounty, gidlist.AddressInfo, CurrentUserId,
                 gidlist.OrderTypeID, gidlist.ServiceCenterID, gidlist.DeLevelID, gidlist.PayType, gidlist.PCDealerCode, gidlist.PPDealerCode, gidlist.DeptName, gidlist.MoneyTransport, null));
        }

        [HttpGet]
        [Route("api/GetOrdersList")]
        public ResultEntity<List<OrdersDTO>> GetOrdersList(int pageSize, int pageIndex, int? strate)
        {
            int count = 0;
            return new ResultEntityUtil<List<OrdersDTO>>().Success(dm.GetOrdersList(strate, CurrentUserId, pageSize, pageIndex, out count), count);

        }

        [HttpGet]
        [Route("api/GetDealerOrdersList")]
        public ResultEntity<List<OrdersDTO>> GetDealerOrdersList(int pageSize, int pageIndex, int? strate)
        {
            int count = 0;
            return new ResultEntityUtil<List<OrdersDTO>>().Success(dm.GetDealerOrdersList(strate, CurrentUserId, pageSize, pageIndex, out count), count);

        }

        [HttpGet]
        [Route("api/GetOrders")]
        public ResultEntity<OrdersDTO> GetOrders(Guid? orderid)
        {
            return new ResultEntityUtil<OrdersDTO>().Success(dm.GetOrders(orderid));

        }

        /// <summary>
        /// PC扫描支付获取订单支付结果
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderPay")]
        public ResultEntity<bool> GetOrderPay(string orderNumber)
        {
            return new ResultEntityUtil<bool>().Success(dm.GetOrderPay(orderNumber));

        }


        [HttpGet]
        [Route("api/deOrders")]
        public ResultEntity<bool> deOrders(Guid id)
        {
            return new ResultEntityUtil<bool>().Success(dm.CancelPayingOrder(id));

        }

        /// <summary>
        /// 物流运输详情
        /// </summary>
        /// <param name="bianma"></param>
        /// <param name="danhao"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetLogisticsInfo")]
        public ResultEntity<string> GetLogisticsInfo(string bianma, string danhao, string phone)
        {

            //参数 
            String param = "{\"com\":\"" + bianma + "\",\"num\":\"" + danhao + "\",\"from\":\"\",\"phone\":\"" + phone + "\",\"to\":\"0\"}";
            String key = "qNabMmpS4612";
            String customer = "ED239E1A2C410E53DB95E7115DB76500";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] InBytes = Encoding.GetEncoding("UTF-8").GetBytes(param + key + customer);
            byte[] OutBytes = md5.ComputeHash(InBytes);
            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            String sign = OutString.ToUpper();
            //http://poll.kuaidi100.com/poll/query.do?customer=ED239E1A2C410E53DB95E7115DB76500&sign=7C7AED34A07901A5DF33DBA9AB9E3DCC&param={"com":"shunfeng","num":"708329128335","from":"","phone":"13319093662","to":"","resultv2":0}
            string url = "http://poll.kuaidi100.com/poll/query.do?customer=" + customer + "&sign=" + sign + "&param=" + param;


            return new ResultEntityUtil<string>().Success(HttpHelper.GetHttpInfo(url));

        }

        /// <summary>
        /// 获取注册单和升级单的产品金额和PV
        /// </summary>
        /// <param name="type">0.升级和注册，1.注册，2.升级</param>
        /// <param name="userid">用户编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderStatrMoney")]
        public ResultEntity<OrderStatrMoney> GetOrderStatrMoney(int type, Guid userid)
        {
            return new ResultEntityUtil<OrderStatrMoney>().Success(dm.GetOrderStatrMoney(type, userid));


        }
    }
}