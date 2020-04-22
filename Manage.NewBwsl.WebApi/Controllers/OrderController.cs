using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.Common;
using NewMK.Domian.DM;
using NewMK.Domian.ThirdParty.lingkaiDX;
using NewMK.DTO;
using NewMK.DTO.Order;
using NewMK.DTO.Record;
using NewMK.DTO.ShoppingCart;
using NewMK.DTO.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Utility;
using WxPayAPI;

namespace Manage.NewMK.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
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
            return new ResultEntityUtil<List<OrderTypeDTO>>().Success(dm.GetOrderTypeList());

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
            int count = 0;
            return new ResultEntityUtil<List<OrdersDTO>>().Success(dm.GetOrdersList(dto, out count), count);

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
            return new ResultEntityUtil<OrdersDTO>().Success(dm.GetOrdersCount(dto));

        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deOrders")]
        public ResultEntity<string> deOrders(Guid id)
        {
            string deorderTime = ConfigurationManager.ConnectionStrings["deOrderTime"].ConnectionString;
            return new ResultEntityUtil<string>().Success("", dm.DeleteOrders(id, deorderTime));
        }

        /// <summary>
        /// 人工退款
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="comment">人工退款备注</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ManualRefund")]
        public ResultEntity<bool> ManualRefund(Guid id, string comment)
        {
            string deorderTime = ConfigurationManager.ConnectionStrings["deOrderTime"].ConnectionString;
            return new ResultEntityUtil<bool>().Success(dm.ManualRefund(id, comment));
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
        /// 获取单日注册人数（原单日订单统计）
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Fl_Dialy_Order_Count")]
        public ResultEntity<List<DataCountDTO>> GetPro_Fl_Dialy_Order_Count(string time1, string time2)
        {
            return new ResultEntityUtil<List<DataCountDTO>>().Success(dm.GetPro_Fl_Dialy_Order_Count(time1, time2));

        }

        /// <summary>
        /// 获取个类型订单数量
        /// </summary>
        /// <param name="dto"></param> 
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderTypeCountList")]
        public ResultEntity<List<OrderTypeCountList>> GetOrderTypeCountList([FromUri]Request_Order dto)
        {
            return new ResultEntityUtil<List<OrderTypeCountList>>().Success(dm.GetOrderTypeCountList(dto));

        }

        /// <summary>
        /// 网体订单
        /// </summary>
        /// <param name="PDealerCode">用户编号</param>
        /// <param name="BusiType">网体类型</param>
        /// <param name="SBegin"></param>
        /// <param name="sEnd"></param>
        /// <param name="OrderArea">地区</param>
        /// <param name="State"></param>
        /// <param name="IsBalance"></param>
        /// <param name="OrderTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_GetSaleTeam_Order")]
        public ResultEntity<List<Pro_GetSaleTeam_OrderDTO>> GetPro_GetSaleTeam_Order(string PDealerCode, string BusiType, string SBegin, string sEnd, string OrderArea, string State, string IsBalance, string OrderTypeId, int pageIndex, int pageSize)
        {
            List<Pro_GetSaleTeam_OrderDTO> li = dm.GetPro_GetSaleTeam_Order(PDealerCode, BusiType, SBegin, sEnd, OrderArea, State, IsBalance, OrderTypeId);

            return new ResultEntityUtil<List<Pro_GetSaleTeam_OrderDTO>>().Success(li.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), li.Count);

        }

        /// <summary>
        /// 产品订单查询
        /// </summary>
        /// <param name="dealerCode"></param>
        /// <param name="productCode"></param>
        /// <param name="OrderTypeID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PrivenceName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductOrderList")]
        public ResultEntity<List<ProductOrderDTO>> GetProductOrderList(string dealerCode, string productCode, string OrderTypeID, DateTime? startTime, DateTime? endTime, int pageSize, int pageIndex, string PrivenceName)
        {
            List<ProductOrderDTO> li = dm.GetProductOrderList(dealerCode, productCode, OrderTypeID, startTime, endTime, PrivenceName);

            return new ResultEntityUtil<List<ProductOrderDTO>>().Success(li.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), li.Count);

        }

        /// <summary>
        /// 产品订单详情
        /// </summary>
        /// <param name="sBegin"></param>
        /// <param name="sEnd"></param>
        /// <param name="ItemCode"></param>
        /// <param name="OrderCategoryName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Query_ItemSaleCase_web")]
        public ResultEntity<List<Pro_Query_ItemSaleCase_webDTO>> GetPro_Query_ItemSaleCase_web(string sBegin, string sEnd, string ItemCode, string OrderCategoryName, int pageIndex, int pageSize)
        {
            List<Pro_Query_ItemSaleCase_webDTO> li = dm.GetPro_Query_ItemSaleCase_web(sBegin, sEnd, ItemCode, OrderCategoryName);

            return new ResultEntityUtil<List<Pro_Query_ItemSaleCase_webDTO>>().Success(li.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), li.Count);

        }
        /// <summary>
        /// 地区订单统计查询
        /// </summary>
        /// <param name="dealerCode"></param>
        /// <param name="productCode"></param>
        /// <param name="OrderTypeID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PrivenceName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAddressOrderList")]
        public ResultEntity<List<AddressOrderDTO>> GetAddressOrderList(string dealerCode, string productCode, string OrderTypeID, DateTime? startTime, DateTime? endTime, int pageSize, int pageIndex, string PrivenceName)
        {
            List<AddressOrderDTO> li = dm.GetAddressOrderList(dealerCode, productCode, OrderTypeID, startTime, endTime, PrivenceName);

            return new ResultEntityUtil<List<AddressOrderDTO>>().Success(li.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), li.Count);

        }

        /// <summary>
        /// 地区业绩
        /// </summary>
        /// <param name="btime">开始时间</param>
        /// <param name="stimg">结束时间</param>
        /// <param name="province">省</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetPro_Sum_Market_Achievement")]
        public ResultEntity<List<Pro_Sum_Market_AchievementDTO>> GetPro_Sum_Market_Achievement(DateTime? btime, DateTime? stimg, string province)
        {
            List<Pro_Sum_Market_AchievementDTO> li = dm.GetPro_Sum_Market_Achievement(btime, stimg, province);
            return new ResultEntityUtil<List<Pro_Sum_Market_AchievementDTO>>().Success(li, li.Count);

        }



        #region 下单


        /// <summary>
        /// 获取选中购物车产品集合
        /// </summary>
        /// <param name="gidlist">选中购物数据集合json</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/GetShoppingTT")]
        public ResultEntity<ShoppingCartActivityDTO> GetShoppingTT([FromBody]ShoppingOrder gidlist)
        {
            ShoppingCartDM scdm = new ShoppingCartDM();
            List<ShoppingCartDTO> objs = JsonConvert.DeserializeObject<List<ShoppingCartDTO>>(gidlist.sclist);

            return new ResultEntityUtil<ShoppingCartActivityDTO>().Success(scdm.GetShoppingTT(objs, gidlist.UserID, gidlist.OrderTypeID, CurrentUserId));

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
            List<ShoppingCartDTO> objs = gidlist.sclist;
            return new ResultEntityUtil<decimal>().Success(dm.CountYf(objs));

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
            Guid DealerId = gidlist.DealerId;
            UserCaheUtil.Validate(DealerId.ToString());
            ShoppingCartDM scdm = new ShoppingCartDM();

            List<ShoppingCartDTO> objs = JsonConvert.DeserializeObject<List<ShoppingCartDTO>>(gidlist.sclist);
            ShoppingCartActivityDTO dto = scdm.GetShoppingTT(objs, gidlist.UserID, gidlist.OrderTypeID, CurrentUserId);

            return new ResultEntityUtil<OrdersModel>().Success(dm.AddOrders(dto, gidlist.UserID, gidlist.DeliverType, gidlist.ConsigneeName, gidlist.ConsigneePhone, gidlist.ConsigneeProvince, gidlist.ConsigneeCity, gidlist.ConsigneeCounty, gidlist.AddressInfo, DealerId,
                 gidlist.OrderTypeID, gidlist.ServiceCenterID, gidlist.DeLevelID, gidlist.PayType, gidlist.PCDealerCode, gidlist.PPDealerCode, gidlist.DeptName, gidlist.MoneyTransport, CurrentUserId));
        }
        #endregion

    }
}
