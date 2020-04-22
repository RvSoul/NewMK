using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

using WxPayAPI.lib;
using System.Security.Cryptography;
using System.Linq;
using NewMK.Domian.DomainException;
using NewMK.Domian.Pay.WxException;

namespace WxPayAPI
{
    public class WxPayApi
    {
        /// <summary>
        /// 业务结果成功
        /// </summary>
        public static readonly string RESULE_SUCCESS = "SUCCESS";
        /// <summary>
        /// 业务结果失败
        /// </summary>
        public static readonly string RESULE_FAIL = "FAIL";

        #region 订单查询

        /// <summary>
        /// 支付成功
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_SUCCESS = "SUCCESS";
        /// <summary>
        /// 转入退款
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_REFUND = "REFUND";
        /// <summary>
        /// 未支付
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_NOTPAY = "NOTPAY";
        /// <summary>
        /// 已关闭
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_CLOSED = "CLOSED";
        /// <summary>
        /// 已撤销（付款码支付）
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_REVOKED = "REVOKED";
        /// <summary>
        /// 用户支付中（付款码支付）
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_USERPAYING = "USERPAYING";
        /// <summary>
        /// 支付失败(其他原因，如银行返回失败)
        /// </summary>
        public const string ORDERQUERY_TRADE_STATE_PAYERROR = "PAYERROR";

        /// <summary>
        /// 查询系统中不存在此交易订单号
        /// </summary>
        public const string ORDERQUERY_ERROR_CODE_ORDERNOTEXIST = "ORDERNOTEXIST";

        /// <summary>
        /// 后台系统返回错误
        /// </summary>
        public const string ORDERQUERY_ERROR_CODE_SYSTEMERROR = "SYSTEMERROR";

        #endregion

        #region 订单关闭

        /// <summary>
        /// 错误码：订单已支付
        /// </summary>
        public const string ERROR_ORDERPAID = "ORDERPAID";

        /// <summary>
        /// 错误码：订单已关闭
        /// </summary>
        public const string ERROR_ORDERCLOSED = "ORDERCLOSED";

        #endregion

        #region 订单退款

        /// <summary>
        /// 退款成功
        /// </summary>
        public const string REFUNDQUERY_STATUS_SUCCESS = "SUCCESS";
        /// <summary>
        /// 退款关闭
        /// </summary>
        public const string REFUNDQUERY_STATUS_REFUNDCLOSE = "REFUNDCLOSE";
        /// <summary>
        /// 退款处理中
        /// </summary>
        public const string REFUNDQUERY_STATUS_PROCESSING = "PROCESSING";
        /// <summary>
        /// 退款异常,退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败
        /// </summary>
        public const string REFUNDQUERY_STATUS_CHANGE = "CHANGE";

        #endregion

        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
        * 提交被扫支付API
        * 收银员使用扫码设备读取微信用户刷卡授权码以后，二维码或条码信息传送至商户收银台，
        * 由商户收银台或者商户后台调用该接口发起支付。
        * @param WxPayData inputObj 提交给被扫支付API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回调用结果，其他抛异常
        */
        public static WxPayData Micropay(WxPayData inputObj, int timeOut = 10)
        {
            string url = "https://api.mch.weixin.qq.com/pay/micropay";
            //检测必填参数
            if (!inputObj.IsSet("body"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数body！");
            }
            else if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数total_fee！");
            }
            else if (!inputObj.IsSet("auth_code"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数auth_code！");
            }

            inputObj.SetValue("spbill_create_ip", WxPayConfig.GetConfig().GetIp());//终端ip
            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            Log.Debug("WxPayApi", "MicroPay request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
            Log.Debug("WxPayApi", "MicroPay response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }       


        /**
        *    
        * 查询订单
        * @param WxPayData inputObj 提交给查询订单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回订单查询结果，其他抛异常
        */
        public static WxPayData OrderQuery(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            string outTradeNo = inputObj.OutTradeNo;

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名

            string xml = inputObj.ToXml();
            var start = DateTime.Now;
            log.Info($"调用微信[查询订单]接口，订单号：{outTradeNo}，请求参数：{xml}");
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口提交数据
            log.Info($"调用微信[查询订单]接口，订单号：{outTradeNo}，响应结果：{response}");
            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时
            //将xml格式的数据转化为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);
            //ReportCostTime(url, timeCost, result);//测速上报
            ValidateQueryResult(result);
            return result;
        }

        /// <summary>
        /// 对微信查询接口返回参数进行验证
        /// </summary>
        /// <param name="data"></param>
        private static void ValidateQueryResult(WxPayData data)
        {
            if (!data.ReturnCodeIsSuceess)
            {
                throw new DMException($"调用[微信查询]接口失败，错误信息：{data.ReturnMsg}");
            }
            
            if (data.ResultCodeIsSuceess)
            {
                return;
            }

            var errcode = data.ErrCode;
            var errcodedes = data.ErrCodeDes;

            if (errcode.Equals(ORDERQUERY_ERROR_CODE_ORDERNOTEXIST))
            {
                throw new WxOrderNotExistsException();
            }

            throw new DMException($"调用[微信查询]接口失败，错误码：{errcode}，错误信息：{errcodedes}");
        }


        /**
        * 
        * 撤销订单API接口
        * @param WxPayData inputObj 提交给撤销订单API接口的参数，out_trade_no和transaction_id必填一个
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回API调用结果，其他抛异常
        */
        public static WxPayData Reverse(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/reverse";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("撤销订单API接口中，参数out_trade_no和transaction_id必须填写一个！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            Log.Debug("WxPayApi", "Reverse request : " + xml);

            string response = HttpService.Post(xml, url, true, timeOut);

            Log.Debug("WxPayApi", "Reverse response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }


        /**
        * 
        * 申请退款
        * @param WxPayData inputObj 提交给申请退款API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回接口调用结果，其他抛异常
        */
        public static WxPayData Refund(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("退款申请接口中，out_trade_no、transaction_id至少填一个！");
            }
            else if (!inputObj.IsSet("out_refund_no"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数out_refund_no！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数total_fee！");
            }
            else if (!inputObj.IsSet("refund_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数refund_fee！");
            }
            //else if (!inputObj.IsSet("op_user_id"))
            //{
            //    throw new WxPayException("退款申请接口中，缺少必填参数op_user_id！");
            //}

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WxPayConfig.GetConfig().GetNotifyUrl3());//异步通知url
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string orderNumber = inputObj.OutTradeNo;
            string xml = inputObj.ToXml();
            var start = DateTime.Now;

            log.Info($"调用微信[申请退款]接口参数，Url:{url}，入口参数：{xml}");
            string response = HttpService.Post(xml, url, true, timeOut);//调用HTTP通信接口提交数据到API
            log.Info($"调用微信[申请退款]接口返回结果：{response}");


            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            ValidateRefundResult(result, orderNumber);

            //ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }

        /// <summary>
        /// 对微信查询接口返回参数进行验证
        /// </summary>
        /// <param name="data"></param>
        private static void ValidateRefundResult(WxPayData data,string orderNumber)
        {
            if (!data.ReturnCodeIsSuceess)
            {
                throw new DMException($"订单[{orderNumber}]退款失败，错误信息：{data.ReturnMsg}");
            }

            if (data.ResultCodeIsSuceess)
            {
                return;
            }
            else
            {
                throw new DMException($"订单[{orderNumber}]退款失败，错误码：{data.ErrCode}，错误信息：{data.ErrCodeDes}");
            }
        }

        /**
	    * 
	    * 查询退款
	    * 提交退款申请后，通过该接口查询退款状态。退款有一定延时，
	    * 用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
	    * out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个
	    * @param WxPayData inputObj 提交给查询退款API的参数
	    * @param int timeOut 接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回，其他抛异常
	    */
        public static WxPayData RefundQuery(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/refundquery";

            var outTradeNo = inputObj.OutTradeNo;

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名

            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            log.Info($"调用微信[查询退款]接口，订单号:{outTradeNo}，Url:{url}，请求参数：{xml}");
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
            log.Info($"调用微信[查询退款]接口,订单号:{outTradeNo}，返回结果：{response}");

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);
            ValidateRefundQueryResult(result,outTradeNo);
            //ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }

        /// <summary>
        /// 对微信查询接口返回参数进行验证
        /// </summary>
        /// <param name="data"></param>
        private static void ValidateRefundQueryResult(WxPayData data, string orderNumber)
        {
            if (!data.ReturnCodeIsSuceess)
            {
                throw new DMException($"订单[{orderNumber}]退款查询失败，错误信息：{data.ReturnMsg}");
            }

            if (data.ResultCodeIsSuceess)
            {
                return;
            }
            else
            {
                throw new DMException($"订单[{orderNumber}]退款查询失败，错误码：{data.ErrCode}，错误信息：{data.ErrCodeDes}");
            }
        }

        /**
        * 下载对账单
        * @param WxPayData inputObj 提交给下载对账单API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData DownloadBill(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/downloadbill";
            //检测必填参数
            if (!inputObj.IsSet("bill_date"))
            {
                throw new WxPayException("对账单接口中，缺少必填参数bill_date！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名

            string xml = inputObj.ToXml();

            Log.Debug("WxPayApi", "DownloadBill request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
            Log.Debug("WxPayApi", "DownloadBill result : " + response);

            WxPayData result = new WxPayData();
            //若接口调用失败会返回xml格式的结果
            if (response.Substring(0, 5) == "<xml>")
            {
                result.FromXml(response);
            }
            //接口调用成功则返回非xml格式的数据
            else
                result.SetValue("result", response);

            return result;
        }


        /**
	    * 
	    * 转换短链接
	    * 该接口主要用于扫码原生支付模式一中的二维码链接转成短链接(weixin://wxpay/s/XXXXXX)，
	    * 减小二维码数据量，提升扫描速度和精确度。
	    * @param WxPayData inputObj 提交给转换短连接API的参数
	    * @param int timeOut 接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回，其他抛异常
	    */
        public static WxPayData ShortUrl(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/tools/shorturl";
            //检测必填参数
            if (!inputObj.IsSet("long_url"))
            {
                throw new WxPayException("需要转换的URL，签名用原串，传输需URL encode！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串	
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            Log.Debug("WxPayApi", "ShortUrl request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);
            Log.Debug("WxPayApi", "ShortUrl response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }


        /**
        * 
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public static WxPayData UnifiedOrder(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WxPayConfig.GetConfig().GetNotifyUrl());//异步通知url
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("spbill_create_ip", WxPayConfig.GetConfig().GetIp());//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign());
            string xml = inputObj.ToXml();
            var start = DateTime.Now;

            log.Info($"调用微信下单接口参数，Url:{url}，入口参数：{xml}");
            string response = HttpService.Post(xml, url, false, timeOut);
            log.Info($"调用微信下单接口返回结果：{response}");

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);
            WxPayData result = new WxPayData();
            result.FromXml(response);
            //ReportCostTime(url, timeCost, result);//测速上报
            return result;
        }

        public static WxPayData UnifiedOrder2(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WxPayConfig.GetConfig().GetNotifyUrl2());//异步通知url
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("spbill_create_ip", WxPayConfig.GetConfig().GetIp());//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign());
            string xml = inputObj.ToXml();

            var start = DateTime.Now;            

            log.Info($"调用[微信下单]接口参数，Url:{url}，入口参数：{xml}");
            string response = HttpService.Post(xml, url, false, timeOut);
            log.Info($"调用[微信下单]接口返回结果：{response}");

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            //ReportCostTime(url, timeCost, result);//测速上报
            return result;
        }
        /**
	    * 
	    * 关闭订单
	    * @param WxPayData inputObj 提交给关闭订单API的参数
	    * @param int timeOut 接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回，其他抛异常
	    */
        public static WxPayData CloseOrder(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/closeorder";
            //检测必填参数
            if (string.IsNullOrEmpty(inputObj.OutTradeNo))
            {
                throw new WxPayException("[微信关闭订单]接口中，out_trade_no必填！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串		
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            log.Info($"调用[微信关闭订单]接口参数，Url:{url}，入口参数：{xml}");
            string response = HttpService.Post(xml, url, false, timeOut);
            log.Info($"调用[微信关闭订单]接口返回结果：{response}");

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            ValidateCloseOrderResult(result);
            //ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }

        /// <summary>
        /// 对微信关闭订单接口返回参数进行验证
        /// </summary>
        /// <param name="data"></param>
        private static void ValidateCloseOrderResult(WxPayData data)
        {
            if (!data.ReturnCodeIsSuceess)
            {
                throw new DMException($"调用[关闭订单]接口失败，错误信息：{data.ReturnMsg}");
            }

            if (data.ResultCodeIsSuceess)
            {
                return;
            }

            var errcode = data.ErrCode;
            var errcodedes = data.ErrCodeDes;

            if (errcode.Equals(ERROR_ORDERPAID))
                throw new WxOrderPaidException();
            else if (errcode.Equals(ERROR_ORDERCLOSED))
            {
                throw new WxOrderCloseException();
            }

            throw new DMException($"调用[关闭订单]接口失败，错误码：{errcode}，错误信息：{errcodedes}");
        }


        /**
	    * 
	    * 测速上报
	    * @param string interface_url 接口URL
	    * @param int timeCost 接口耗时
	    * @param WxPayData inputObj参数数组
	    */
        private static void ReportCostTime(string interface_url, int timeCost, WxPayData inputObj)
        {
            //如果不需要进行上报
            if (WxPayConfig.GetConfig().GetReportLevel() == 0)
            {
                return;
            }

            //如果仅失败上报
            if (WxPayConfig.GetConfig().GetReportLevel() == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
             inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
            {
                return;
            }

            //上报逻辑
            WxPayData data = new WxPayData();
            data.SetValue("interface_url", interface_url);
            data.SetValue("execute_time_", timeCost);
            //返回状态码
            if (inputObj.IsSet("return_code"))
            {
                data.SetValue("return_code", inputObj.GetValue("return_code"));
            }
            //返回信息
            if (inputObj.IsSet("return_msg"))
            {
                data.SetValue("return_msg", inputObj.GetValue("return_msg"));
            }
            //业务结果
            if (inputObj.IsSet("result_code"))
            {
                data.SetValue("result_code", inputObj.GetValue("result_code"));
            }
            //错误代码
            if (inputObj.IsSet("err_code"))
            {
                data.SetValue("err_code", inputObj.GetValue("err_code"));
            }
            //错误代码描述
            if (inputObj.IsSet("err_code_des"))
            {
                data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
            }
            //商户订单号
            if (inputObj.IsSet("out_trade_no"))
            {
                data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
            }
            //设备号
            if (inputObj.IsSet("device_info"))
            {
                data.SetValue("device_info", inputObj.GetValue("device_info"));
            }

            try
            {
                Report(data);
            }
            catch (WxPayException ex)
            {
                //不做任何处理
            }
        }


        /**
	    * 
	    * 测速上报接口实现
	    * @param WxPayData inputObj 提交给测速上报接口的参数
	    * @param int timeOut 测速上报接口超时时间
	    * @throws WxPayException
	    * @return 成功时返回测速上报接口返回的结果，其他抛异常
	    */
        public static WxPayData Report(WxPayData inputObj, int timeOut = 1)
        {
            string url = "https://api.mch.weixin.qq.com/payitil/report";
            //检测必填参数
            if (!inputObj.IsSet("interface_url"))
            {
                throw new WxPayException("接口URL，缺少必填参数interface_url！");
            }
            if (!inputObj.IsSet("return_code"))
            {
                throw new WxPayException("返回状态码，缺少必填参数return_code！");
            }
            if (!inputObj.IsSet("result_code"))
            {
                throw new WxPayException("业务结果，缺少必填参数result_code！");
            }
            if (!inputObj.IsSet("user_ip"))
            {
                throw new WxPayException("访问接口IP，缺少必填参数user_ip！");
            }
            if (!inputObj.IsSet("execute_time_"))
            {
                throw new WxPayException("接口耗时，缺少必填参数execute_time_！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("user_ip", WxPayConfig.GetConfig().GetIp());//终端ip
            inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string xml = inputObj.ToXml();

            Log.Info("WxPayApi", "Report request : " + xml);

            string response = HttpService.Post(xml, url, false, timeOut);

            Log.Info("WxPayApi", "Report response : " + response);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            return result;
        }

        /**
        * 根据当前系统时间加随机序列来生成订单号
         * @return 订单号
        */
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", WxPayConfig.GetConfig().GetMchID(), DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }
        public static string GenerateOutTradeNo2()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", "CZ", DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }
        /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
        public static string GenerateNonceStr()
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            return randomGenerator.GetRandomUInt().ToString();
        }
    }
}