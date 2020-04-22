using NewMK.Domian.DM;
using NewMK.DTO;
using NewMK.DTO.Order;
using NewMK.DTO.User;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Site.NewMK.WebApi.Controllers
{
    public class kuaiqianController : ApiControllerBase
    {
         
        UserDM udm = new UserDM();
        OrderDM odm = new OrderDM();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordercode">订单号(传空表示充值)</param>
        ///  <param name="OrderMoney">金额</param>
        /// <param name="type">PC ,APP</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Recharge99Bill")]
        public ResultEntity<Pay._99BillPay> Recharge99Bill(string ordercode, string OrderMoney, string type)
        {
            UserDTO user = udm.GetIduser(CurrentUserId);

            string OrderNumber = "";
            if (ordercode != "" && ordercode != null)
            {
                OrdersDTO dto = odm.GetOrderCode(ordercode);
                OrderNumber = dto.OrderNumber;
                OrderMoney = dto.OrderMoney.ToString();
            }
            else
            {
                OrderNumber = "CZ" + DateTime.Now.ToString("yyMMddHHmmssfff");
            }
             
            return new ResultEntityUtil<Pay._99BillPay>().Success(new Pay._99BillPay(KuaiQianZhanghao, KuaiQianHuidiao, user.UserCode, type).BuildPayConfig(OrderNumber, Convert.ToDecimal(OrderMoney), KuaiQianMima, KuaiQianZhengshu, type));
             
        }
        private string BuildOrderNumber()
        {
            return string.Format("FNY_{0}", Guid.NewGuid().ToString().ToLower().Replace("-", ""));
        }
    }
}
