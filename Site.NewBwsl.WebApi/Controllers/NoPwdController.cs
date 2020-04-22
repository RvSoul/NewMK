using Infrastructure.Utility;
using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.Domian.Pay.WxException;
using NewMK.DTO;
using NewMK.DTO.Order;
using NewMK.DTO.User;
using Site.NewMK.WebApi.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;
using WxPayAPI;
using DomainEnum = NewMK.Domian.Enum;

namespace Site.NewMK.WebApi.Controllers
{
    [AllowAnonymous]
    public class NoPwdController : ApiControllerBase
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        UserDM dm = new UserDM();
        OrderDM odm = new OrderDM();


        /// <summary>
        /// 根据编号获取用户信息
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserInfo")]
        public ResultEntity<UserDTO> GetUserInfo(string usercode)
        {
            return new ResultEntityUtil<UserDTO>().Success(dm.GetUserInfo(usercode));
        }

        /// <summary>
        /// 验证编号存不存在
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetUserInfoBool")]
        public ResultEntity<bool> GetUserInfoBool(string usercode)
        {
            return new ResultEntityUtil<bool>().Success(dm.GetUserInfoBool(usercode));
        }

        /// <summary>
        /// 微信注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Register")]
        public ResultEntity<string> Register([FromUri]UserDTO model)
        {
            ResultEntity<string> result = new ResultEntity<string>();

            //短信验证暂时屏蔽
            if (model.yzm != null && model.yzm != "" && model.yzm != "undefined")
            {
                string cookyzmtt = HttpRuntime.Cache[model.Phone + "-yzm"] == null ? "" : HttpRuntime.Cache[model.Phone + "-yzm"].ToString();
                if (cookyzmtt != model.yzm)
                {
                    throw new DMException("验证码无效！");
                }
                else
                {
                    HttpRuntime.Cache.Remove(model.Phone + "-yzm");
                }
            }
            else
            {
                throw new DMException("验证码无效！");
            }

            return new ResultEntityUtil<string>().Success(dm.Register(model, null));
        }

        /// <summary>
        /// 代购注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RegisterWeiXin")]
        public ResultEntity<string> RegisterWeiXin([FromUri]UserDTO model)
        {
            return new ResultEntityUtil<string>().Success(dm.Register(model, null));
        }


        /// <summary>
        /// 验证编号是否绑定了微信
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="openid"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BDUserOpenIdYZ")]
        public ResultEntity<bool> BDUserOpenIdYZ(string usercode, string openid, string userPwd)
        {
            return new ResultEntityUtil<bool>().Success(dm.BDUserOpenIdYZ(usercode, userPwd));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="yzm">验证码</param>
        /// <param name="userPwd">密码</param>
        /// <param name="usercode">用户编号</param>
        /// <param name="level">1.登录密码，2.支付密码，3.全部</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UpUserPwd")]
        public ResultEntity<bool> UpUserPwd(string phone, string yzm, string userPwd, string usercode, string level)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();

            //短信验证
            if (yzm != null && yzm != "" && yzm != "undefined")
            {
                string cookyzmtt = HttpRuntime.Cache[phone + "-yzm"] == null ? "" : HttpRuntime.Cache[phone + "-yzm"].ToString();
                if (cookyzmtt != yzm)
                {
                    throw new DMException("验证码无效！");
                }
                else
                {
                    HttpRuntime.Cache.Remove(phone + "-yzm");
                }
            }
            else
            {
                throw new DMException("验证码无效！");
            }
            return new ResultEntityUtil<bool>().Success(dm.UpUserPwd(phone, userPwd, usercode, level));
        }

        /// <summary>
        /// 获取微信授权
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWeiXinPayUrl1")]
        public ResultEntity<string> GetWeiXinPayUrl1(string url)
        {
            JsApiPay jsApiPay = new JsApiPay();
            return new ResultEntityUtil<string>().Success(jsApiPay.GetCode(url));
        }

        /// <summary>
        /// 直接下单支付
        /// </summary> 
        /// <param name="url">重定向的URL（包括code）(不加http://)</param>
        /// <param name="code">code</param>
        /// <param name="ordercode">订单ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWeiXinPayUrl2")]
        public ResultEntity<string> GetWeiXinPayUrl2(string ordercode, string openid, int paytype)
        {
            try
            {
                try
                {
                    ResultEntity<string> result = new ResultEntity<string>();
                    OrdersDTO dto = odm.GetOrderCode(ordercode);
                    if (dto.State != (int)DomainEnum.OrderState.待付款)
                    {
                        throw new DMException($"订单[{dto.OrderNumber}]已支付，请不要重复操作！");
                    }
                    log.Debug($"调用[微信下单]接口的订单信息：{dto.ToJSON()}");
                    JsApiPay jsApiPay = new JsApiPay();
                    jsApiPay.openid = openid;
                    jsApiPay.total_fee = (int)(dto.OtherPayMoney * 100);
                    WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(ordercode);

                    return new ResultEntityUtil<string>().Success(jsApiPay.GetJsApiParameters());
                }
                catch (WxOrderPaidException ope)
                {
                    log.Info(ope.Message);
                    //进行订单已支付的补充处理
                    odm.PaySuccess(ordercode);
                    throw new OrderNoNeedPayException((int)DomainEnum.OrderState.支付已取消, "订单已支付");
                }
                catch (WxOrderCloseException oce)
                {
                    log.Info(oce.Message);
                    //进行订单已取消的补充处理
                    odm.PayCancel(ordercode);
                    throw new OrderNoNeedPayException((int)DomainEnum.OrderState.支付已取消, "订单已取消");
                }
                catch (WxOutTradeNoUsedException ope)
                {
                    try
                    {
                        log.Info(ope.Message);
                        //对订单进行二次确认
                        odm.OrderConfirmByWx(ordercode);
                        return new ResultEntityUtil<string>().Failure("", "商户订单号重复，请重新下单！");
                    }
                    catch (WxOrderNotPayException npe)
                    {
                        /*
                        //处理方式一、关闭微信的原订单，变更当前订单的订单号重新下单
                        log.Info($"调用微信[关闭订单]接口，订单号[{ordercode}]，start");
                        var req = new WxPayData();
                        req.OutTradeNo = ordercode;
                        WxPayApi.CloseOrder(req);
                        log.Info($"调用微信[关闭订单]接口，订单号[{ordercode}]，end");
                        var newOrdercode = odm.ChangeOrderNumber(ordercode);
                        log.Info($"订单号[{ordercode}]变更为订单号[{newOrdercode}]，进入重新调用[微信下单]接口操作");
                        return GetWeiXinPayUrl2(newOrdercode, openid, paytype);
                        */
                        //处理方式二、直接关闭订单
                        odm.PayCancelAll(ordercode);
                        throw new OrderNoNeedPayException((int)DomainEnum.OrderState.支付已取消, $"订单[{ordercode}]已取消");
                    }
                }
            }            
            catch(OrderNoNeedPayException ce)
            {
                log.Info($"订单[{ordercode}],{ce.Message}");
                throw new DMException($"{ce.Message}，请不要重复操作！");
            }
        }


        /// <summary>
        /// 微信充值加积分唤起支付
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="usercode"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWeiXinPayUrl3")]
        public ResultEntity<string> GetWeiXinPayUrl3(double mon, string usercode, string openid)
        {
            double a = mon * 100;
            int b = Convert.ToInt32(a);
            JsApiPay jsApiPay = new JsApiPay();
            jsApiPay.total_fee = b;
            jsApiPay.openid = openid;
            WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult2(usercode);
            return new ResultEntityUtil<string>().Success(jsApiPay.GetJsApiParameters());
        }

        /// <summary>
        /// 生成微信Navitve支付二维码
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetNativePay")]
        public ResultEntity<byte[]> GetNativePay(string ordercode)
        {
            OrdersDTO dto = odm.GetOrderCode(ordercode);
            double mon = Convert.ToDouble(dto.OtherPayMoney);
            double a = mon * 100;
            int b = Convert.ToInt32(a);
            NativePay nativePay = new NativePay();

            //生成扫码支付模式一url
            string str = nativePay.GetPayUrl(ordercode, b);

            //初始化二维码生成工具
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 4;

            //将字符串生成二维码图片
            using (Bitmap image = qrCodeEncoder.Encode(str, Encoding.Default))
            {
                //保存为PNG到内存流  
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);

                    return new ResultEntityUtil<byte[]>().Success(ms.GetBuffer());
                }
            }
        }

        [HttpGet]
        [Route("api/GetNativeCz")]
        public ResultEntity<byte[]> GetNativeCz(string money, string usercode)
        {
            ResultEntity<byte[]> result = new ResultEntity<byte[]>();

            double mon = Convert.ToDouble(money);
            double a = mon * 100;
            int b = Convert.ToInt32(a);
            NativePay nativePay = new NativePay();
            //生成扫码支付模式一url
            string str = nativePay.GetPayUrl2(usercode, b);

            //初始化二维码生成工具
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 4;

            //将字符串生成二维码图片
            using (Bitmap image = qrCodeEncoder.Encode(str, Encoding.Default))
            {
                //保存为PNG到内存流  
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    return new ResultEntityUtil<byte[]>().Success(ms.GetBuffer());
                }
            }
        }
    }
}