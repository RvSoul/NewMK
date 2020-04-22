using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace WxPayAPI
{
    public class Refund
    {
        /// <summary>
        /// 申请退款完整业务流程逻辑
        /// </summary>
        /// <param name="transaction_id"></param>
        /// <param name="out_trade_no"></param>
        /// <param name="total_fee"></param>
        /// <param name="refund_fee"></param>
        /// <param name="out_refund_no"></param>
        /// <returns></returns>
        public static WxPayData Run(string out_trade_no, int total_fee, int refund_fee, string out_refund_no)
        {
            WxPayData data = new WxPayData();
            data.OutTradeNo = out_trade_no;
            data.TotalFee = total_fee;//订单总金额
            data.SetValue("refund_fee", refund_fee);//退款金额
            data.SetValue("out_refund_no", out_refund_no);//随机生成商户退款单号
            //data.SetValue("op_user_id", WxPayConfig.GetConfig().GetMchID());//操作员，默认为商户号

            return WxPayApi.Refund(data);
        }

        /// <summary>
        /// AES-256-ECB字符解密
        /// </summary>
        /// <param name="s">要解密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string DecodeAES256ECB(string s, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(s);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// 解密微信支付退款结果通知
        /// </summary>
        /// <param name="s">要解密字符串</param>
        /// <returns></returns>
        public static string DecodeReqInfo(string s)
        {
            string key = FormsAuthentication.HashPasswordForStoringInConfigFile(
                WxPayConfig.GetConfig().GetKey(), "md5").ToLower();
            return DecodeAES256ECB(s, key);
        }
    }
}