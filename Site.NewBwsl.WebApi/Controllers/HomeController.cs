using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.Domian.Pay.WxException;
using NewMK.Domian.ThirdParty;
using NewMK.Domian.ThirdParty.lingkaiDX;
using NewMK.DTO.Order;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;
using DomainEnum = NewMK.Domian.Enum;

namespace Site.NewMK.WebApi.Controllers
{
    public class HomeController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OrderDM orderDM = new OrderDM();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        #region 微信支付

        /// <summary>
        /// 支付结果通知回调处理类
        /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
        /// </summary>
        public void WeiXinPay()
        {
            try
            {
                WxPayData notifyData = GetNotifyData("微信支付结果");

                //检查支付结果中transaction_id是否存在
                string outTradeNo = notifyData.OutTradeNo;
                if (string.IsNullOrEmpty(outTradeNo))
                {
                    FailWX("微信支付回调通知中微信订单号不存在");
                    return;
                }
                var queryData = QueryOrder(outTradeNo);

                ValidatePayCall(notifyData, queryData);
                string orderNumber = notifyData.OutTradeNo;
                Order order = null;
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                    if(order == null)
                    {
                        log.Error($"微信支付回调通知的订单[{orderNumber}]在系统中不存在！");
                        SuccessWX();
                        return;
                    }
                    if(order.State != (int)DomainEnum.OrderState.待付款)
                    {
                        log.Error($"微信支付回调通知的订单[{orderNumber}]在系统中不是待付款状态，不能重复处理！");
                        SuccessWX();
                        return;
                    }
                    if (notifyData.ResultCodeIsSuceess)
                    {
                        orderDM.PaySuccess(c, order);                        
                    }
                    else
                    {
                        orderDM.PayFail(c, order);
                    }
                    c.SaveChanges();
                }

                SuccessWX();
            }
            catch (WxOrderNotExistsException oee)
            {
                log.Info($"微信通知异常，订单不存在！");
                FailWX("微信通知异常，订单不存在！");
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                FailWX("系统处理错误");
            }
        }

        //public void WeiXinPay2(string orderNumber)
        //{
        //    OrderDM dm = new OrderDM();
        //    PayOrderUserDTO order = dm.PayOrder(orderNumber, 3, 2);
        //    if (order.OrderTypeID == 1)
        //    {
        //        NewMethod(order.Phone, order.DeLevelID, order.UserCode, order.UserName);
        //    }
        //}

        /// <summary>
        /// 充值回掉
        /// </summary>
        public void WeiXinPayCZ()
        {
            try
            {
                WxPayData notifyData = GetNotifyData("微信支付结果");

                string outTradeNo = notifyData.OutTradeNo;
                if (string.IsNullOrEmpty(outTradeNo))
                {
                    FailWX("支付结果中微信订单号不存在");
                    return;
                }

                var queryData = QueryOrder(outTradeNo);
                ValidatePayCall(notifyData, queryData);
                if (notifyData.ResultCodeIsSuceess)
                {
                    //TODO，需要在微信积分充值时记录微信积分充值订单，做幂等型处理，防止一笔订单微信重复通知支付成功
                    string orderNumber = notifyData.OutTradeNo;
                    decimal money = Convert.ToDecimal(notifyData.GetValue("total_fee").ToString());
                    string usercode = notifyData.GetValue("attach").ToString();
                    RecordDM dm = new RecordDM();
                    dm.EleMoneyCZ(orderNumber, money, usercode, 1, 9, "微信积分充值！");
                }
                else
                {
                    log.Info($"微信通知充值失败，不做微信积分充值处理，out_trade_no：{notifyData.OutTradeNo}，transaction_id：{notifyData.TransactionId}！");
                }
                SuccessWX();
            }
            catch (WxOrderNotExistsException oee)
            {
                log.Info($"微信通知异常，订单不存在！");
                FailWX("微信通知异常，订单不存在！");
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                FailWX("系统处理错误");
            }
        }

        /// <summary>
        /// 微信退款结果通知回调处理类
        /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
        /// </summary>
        public void WeiXinRefund()
        {
            try
            {
                WxPayData notifyData = GetNotifyData("微信退款通知结果");

                if (!notifyData.ReturnCodeIsSuceess)
                {
                    FailWX("微信退款通知结果失败");
                }

                string reqInfo = Refund.DecodeReqInfo(notifyData.GetValue("req_info").ToString());
                log.Info($"微信退款通知结果的req_info解密后信息：{reqInfo}");
                notifyData = new WxPayData();
                notifyData.FromXmlNoCheckSign(reqInfo);

                //检查支付结果中transaction_id是否存在
                string outTradeNo = notifyData.OutTradeNo;
                if (string.IsNullOrEmpty(outTradeNo))
                {
                    FailWX("微信退款支付回调通知中商户订单号不存在");
                    return;
                }
                //var queryData = QueryFundOrder(outTradeNo);
                //ValidateRefundCall(notifyData, queryData);

                string orderNumber = notifyData.OutTradeNo;

                OrdersDTO order = orderDM.GetOrderCode(orderNumber);
                if (order == null)
                {
                    log.Error($"微信退款回调通知的订单[{orderNumber}]在系统中不存在！");
                    SuccessWX();
                    return;
                }
                if (order.State != (int)DomainEnum.OrderState.待退款)
                {
                    log.Error($"微信退款回调通知的订单[{orderNumber}]在系统中不是待退款状态，不能重复处理！");
                    SuccessWX();
                    return;
                }

                var refundStatus = notifyData.GetValue("refund_status");
                if (refundStatus.Equals(WxPayApi.REFUNDQUERY_STATUS_SUCCESS))
                {
                    orderDM.RefundSuccess(order.OrderNumber, notifyData.GetStringValue("refund_recv_accout"));
                }
                else
                {
                    orderDM.RefundFail(order.OrderNumber);
                }
                SuccessWX();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                FailWX("系统处理错误");
            }
        }

        /// <summary>
        /// 通知结果和查询结果验证下
        /// </summary>
        /// <param name="notifyData"></param>
        /// <param name="queryData"></param>
        private void ValidateRefundCall(WxPayData notifyData, WxPayData queryData)
        {
            if (!notifyData.OutTradeNo.Equals(queryData.OutTradeNo) || notifyData.TotalFee != queryData.TotalFee)
            {
                throw new DMException("微信退款通知异常，通知信息与主动查询信息不一致！");
            }
        }

        /// <summary>
        /// 查询退款
        /// </summary>
        /// <param name="transaction_id"></param>
        /// <returns></returns>
        private WxPayData QueryFundOrder(string outTradeNo)
        {
            WxPayData req = new WxPayData();
            req.OutTradeNo = outTradeNo;
            return WxPayApi.RefundQuery(req);
        }

        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            //接收从微信后台POST过来的数据
            using (System.IO.Stream s = Request.InputStream)
            {
                int count = 0;
                byte[] buffer = new byte[1024];

                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
            }
            log.Error($"{prefix}的通知内容: " + builder.ToString());
            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            data.FromXml(builder.ToString());
            return data;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        private WxPayData QueryOrder(string outTradeNo)
        {
            WxPayData req = new WxPayData();
            req.OutTradeNo = outTradeNo;
            return WxPayApi.OrderQuery(req);
        }
        
        #endregion

        /// <summary>
        /// 快钱H5支付回调
        /// </summary>
        /// <param name="merchantAcctId">人民币网关账号，该账号为11位人民币网关商户编号+01,该值与提交时相同。</param>
        /// <param name="version">网关版本，固定值：v2.0,该值与提交时相同。</param>
        /// <param name="language">语言种类，1代表中文显示，2代表英文显示。默认为1,该值与提交时相同。</param>
        /// <param name="signType">签名类型,该值为4，代表PKI加密方式,该值与提交时相同。</param>
        /// <param name="payType">支付方式，一般为00，代表所有的支付方式。如果是银行直连商户，该值为10,该值与提交时相同。</param>
        /// <param name="bankId">银行代码，如果payType为00，该值为空；如果payType为10,该值与提交时相同。</param>
        /// <param name="orderId">商户订单号，,该值与提交时相同。</param>
        /// <param name="orderTime">订单提交时间，格式：yyyyMMddHHmmss，如：20071117020101,该值与提交时相同。</param>
        /// <param name="orderAmount">订单金额，金额以“分”为单位，商户测试以1分测试即可，切勿以大金额测试,该值与支付时相同。</param>
        /// <param name="bindCard">数字串        可为空        信用卡快捷支付绑定卡信息后返回前六后四位信用卡号        </param>
        /// <param name="bindMobile">数字串        可为空        信用卡快捷支付绑定卡信息后返回前三位后四位手机号码        </param>
        /// <param name="dealId">快钱交易号，商户每一笔交易都会在快钱生成一个交易号。</param>
        /// <param name="bankDealId">银行交易号 ，快钱交易在银行支付时对应的交易号，如果不是通过银行卡支付，则为空</param>
        /// <param name="dealTime">快钱交易时间，快钱对交易进行处理的时间,格式：yyyyMMddHHmmss，如：20071117020101</param>
        /// <param name="payAmount">商户实际支付金额 以分为单位。比方10元，提交时金额应为1000。该金额代表商户快钱账户最终收到的金额。</param>
        /// <param name="fee">费用，快钱收取商户的手续费，单位为分。</param>
        /// <param name="ext1">扩展字段1，该值与提交时相同。(经销商编号)</param>
        /// <param name="ext2">扩展字段2，该值与提交时相同。(备注)</param>
        /// <param name="payResult">处理结果， 10支付成功，11 支付失败，00订单申请成功，01 订单申请失败</param>
        /// <param name="errCode">错误代码 ，请参照《人民币网关接口文档》最后部分的详细解释。</param>
        /// <param name="signMsg">签名字符串</param>
        public void KuaiQian(string merchantAcctId, string version, string language, string signType, string payType, string bankId, string orderId, string orderTime, string orderAmount, string bindCard, string bindMobile, string dealId, string bankDealId, string dealTime, string payAmount, string fee, string ext1, string ext2, string payResult, string errCode, string signMsg)
        {
            string url = "";
            if (orderId.Contains("KQCZ"))
            {
                url = ConfigurationManager.AppSettings["KuaiQianPagePC2"].ToString();
                if (ext2 == "APP")
                {
                    url = ConfigurationManager.AppSettings["KuaiQianPageAPP2"].ToString();
                }
            }
            else
            {
                url = ConfigurationManager.AppSettings["KuaiQianPagePC"].ToString();
                if (ext2 == "APP")
                {
                    url = ConfigurationManager.AppSettings["KuaiQianPageAPP"].ToString();
                }
            }

            try
            {
                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "merchantAcctId", merchantAcctId);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "language", language);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "payType", payType);
                signMsgVal = appendParam(signMsgVal, "bankId", bankId);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
                signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);

                if (bindCard != null && bindCard != "")
                {
                    signMsgVal = appendParam(signMsgVal, "bindCard", bindCard);
                }
                if (bindMobile != null && bindMobile != "")
                {
                    signMsgVal = appendParam(signMsgVal, "bindMobile", bindMobile);
                }


                signMsgVal = appendParam(signMsgVal, "dealId", dealId);
                signMsgVal = appendParam(signMsgVal, "bankDealId", bankDealId);
                signMsgVal = appendParam(signMsgVal, "dealTime", dealTime);
                signMsgVal = appendParam(signMsgVal, "payAmount", payAmount);
                signMsgVal = appendParam(signMsgVal, "fee", fee);
                signMsgVal = appendParam(signMsgVal, "ext1", ext1);
                signMsgVal = appendParam(signMsgVal, "ext2", ext2);
                signMsgVal = appendParam(signMsgVal, "payResult", payResult);
                signMsgVal = appendParam(signMsgVal, "errCode", errCode);

                //UTF-8编码  GB2312编码  用户可以根据自己网站的编码格式来选择加密的编码方式
                //byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(signMsgVal);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
                byte[] SignatureByte = Convert.FromBase64String(signMsg);
                string KuaiQianHuiDiaoZhengshu = ConfigurationManager.AppSettings["KuaiQianHuiDiaoZhengshu"].ToString();
                X509Certificate2 cert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + KuaiQianHuiDiaoZhengshu, "");
                RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
                rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
                RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
                byte[] result;
                f.SetHashAlgorithm("SHA1");
                SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                result = sha.ComputeHash(bytes);


                if (f.VerifySignature(result, SignatureByte))
                {

                    //逻辑处理  写入数据库
                    if (payResult == "10")
                    {
                        int aa = Convert.ToInt32(orderAmount) / 100;

                        //此处做商户逻辑处理
                        string time = orderTime.Substring(0, 4) + "-" + orderTime.Substring(4, 2) + "-" + orderTime.Substring(6, 2) + " " + orderTime.Substring(8, 2) + ":" + orderTime.Substring(10, 2) + ":" + orderTime.Substring(13);
                        // decimal zoom = Convert.ToDecimal(ConfigurationManager.ConnectionStrings["Zoom"].ConnectionString);
                        //rechargeusecase.AppRecharge(dealId, ext1, Convert.ToDecimal(aa) * zoom, Convert.ToDateTime(time), ext2);
                        if (orderId.Contains("KQCZ"))
                        {
                            RecordDM dm = new RecordDM();

                            dm.EleMoneyCZ(orderId, Convert.ToDecimal(orderAmount), ext1, 1, 12, "银行卡积分充值！");
                        }
                        else
                        {
                            OrderDM dm = new OrderDM();
                            OrdersDTO dto = dm.GetOrderCode(orderId);
                            PayOrderUserDTO order = dm.PayOrder(dto.OrderNumber, 3, 3);
                            if (order.OrderTypeID == 1)
                            {
                                SmsUtility.SendWelcomeSMS(order.Phone, order.DeLevelID, order.UserCode, order.UserName);
                            }
                        }


                        Response.Write("<result>1</result>" + "<redirecturl>" + url + "</redirecturl>");
                    }
                    else
                    {
                        Response.Write("<result>2</result>" + "<redirecturl>" + url + "</redirecturl>");
                    }
                }
                else
                {

                    Response.Write("signMsgVal=" + "(" + signMsgVal + ")");
                    Response.Write("</br>" + "signMsg =" + signMsg);
                    Response.Write("</br>" + "错误");
                }
            }
            catch (Exception ex)
            {

                Response.Write("<result>2</result>" + "<redirecturl>" + url + "</redirecturl>");
            }
        }
        String appendParam(String returnStr, String paramId, String paramValue)
        {

            if (returnStr != "")
            {

                if (paramValue != "")
                {

                    returnStr += "&" + paramId + "=" + paramValue;
                }

            }
            else
            {

                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }

            return returnStr;
        }

        /// <summary>
        /// 通知结果和查询结果验证下
        /// </summary>
        /// <param name="notifyData"></param>
        /// <param name="queryData"></param>
        private  void ValidatePayCall(WxPayData notifyData, WxPayData queryData)
        {
            if(!notifyData.OutTradeNo.Equals(queryData.OutTradeNo) || !notifyData.MchId.Equals(queryData.MchId) ||
                !notifyData.Appid.Equals(queryData.Appid) || notifyData.TotalFee != queryData.TotalFee)
            {
                throw new DMException("微信支付通知异常，通知信息与主动查询信息不一致！");
            }
        }
        /// <summary>
        /// 回应微信处理失败
        /// </summary>
        /// <param name="message"></param>
        private void FailWX(string message)
        {
            //若订单查询失败，则立即返回结果给微信支付后台
            WxPayData res = new WxPayData();
            res.SetValue("return_code", "FAIL");
            res.SetValue("return_msg", message);
            //Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
            log.Error("响应微信结果： " + res.ToXml());
            Response.Write(res.ToXml());
            Response.End();
        }

        /// <summary>
        /// 回应微信处理成功
        /// </summary>
        private void SuccessWX()
        {
            WxPayData res = new WxPayData();
            res.SetValue("return_code", "SUCCESS");
            res.SetValue("return_msg", "OK");
            log.Error("响应微信结果： " + res.ToXml());
            Response.Write(res.ToXml());
            Response.End();
        }
    }
}
