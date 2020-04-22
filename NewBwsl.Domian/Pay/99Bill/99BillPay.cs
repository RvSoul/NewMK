using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Pay
{
    /// <summary>
    /// 快钱支付
    /// </summary>    
    public class _99BillPay
    {

        /// <summary>
        /// [必填]人民币网关帐号（11位人民币网关商户编号+"01"）
        /// </summary>
        public string merchantAcctId { get; set; }

        /// <summary>
        /// [必填]编码方式，1代表 UTF-8; 2 代表 GBK; 3代表 GB2312
        /// </summary>
        public string inputCharset { get; set; }

        /// <summary>
        /// [必填]服务器接收支付结果的后台地址
        /// </summary>
        public string bgUrl { get; set; }

        /// <summary>
        /// [必填]网关版本，固定值：v2.0
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// [必填]语言种类，1代表中文显示，2代表英文显示
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// [必填]签名类型,该值为4，代表PKI加密方式
        /// </summary>
        public string signType { get; set; }

        /// <summary>
        /// [必填]商户订单号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// [必填]订单金额，金额以“分”为单位
        /// </summary>
        public string orderAmount { get; set; }

        /// <summary>
        /// [必填]订单提交时间，格式：yyyyMMddHHmmss
        /// </summary>
        public string orderTime { get; set; }

        /// <summary>
        /// [可为空]商品名称
        /// </summary>
        public string productName { get; set; }

        /// <summary>
        /// [可为空]商品数量
        /// </summary>
        public string productNum { get; set; }
        /// <summary>
        /// [必填]支付方式，一般为00，代表所有的支付方式
        /// </summary>
        public string payType { get; set; }

        /// <summary>
        /// [必填]signMsg 签名字符串
        /// </summary>
        public string signMsg { get; set; }

        /// <summary>
        /// 快钱请求地址
        /// </summary>
        public string PostUrl { get; set; }


        public string pageUrl { get; set; }

        /// <summary>
        /// 付款人姓名
        /// </summary>
        public string payerName { get; set; }
        /// <summary>
        /// 付款人联系方式类型
        /// </summary>
        public string payerContactType { get; set; }
        /// <summary>
        /// 付款人联系方式
        /// </summary>
        public string payerContact { get; set; }

        /// <summary>
        /// 指定付款人类型
        /// </summary>
        public string payerIdType { get; set; }
        /// <summary>
        /// 银行卡收款的话 写银行卡卡号
        /// </summary>
        public string bankId { get; set; }
        /// <summary>
        /// 指定付款人编号
        /// </summary>
        public string payerId { get; set; }


        /// <summary>
        /// 产品ID
        /// </summary>
        public string productId { get; set; }

        /// <summary>
        /// 产品介绍
        /// </summary>
        public string productDesc { get; set; }


        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string ext1 { get; set; }


        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string ext2 { get; set; }

        /// <summary>
        /// 同一订单禁止重复提交标志，实物购物车填1，虚拟产品用0。1代表只能提交一次，0代表在支付不成功情况下可以再提交
        /// </summary>
        public string redoFlag { get; set; }


        /// <summary>
        /// 快钱合作伙伴的帐户号，即商户编号，可为空。
        /// </summary>
        public string pid { get; set; }
        public string mobileGateway { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="accountcode">收钱账号</param>
        /// <param name="reveiveurl">回调URL</param>
        public _99BillPay(string accountcode, string reveiveurl, string dealercode,string type)
        {
            this.merchantAcctId = accountcode;
            this.inputCharset = "1";
            this.pageUrl = "";
            this.bgUrl = reveiveurl;
            if (type=="PC")
            {
                this.version = "v2.0";
                this.ext2 = "PC";
            }
            else if (type == "APP")
            {
                this.version = "mobile1.0";
                this.mobileGateway = "phone";
                this.ext2 = "APP";
            }
            
            this.language = "1";
            this.signType = "4";
            this.payerName = "";
            this.payerContactType = "1";
            this.payerContact = "2532987@qq.com";
            this.orderId = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.orderAmount = "1";
            this.orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.productName = "在线充值";
            this.productNum = "1";
            this.productId = "";
            this.productDesc = "";
            this.ext1 = dealercode;
            
            this.payType = "00";
            this.redoFlag = "";
            this.pid = "";

            


        }

        /// <summary>
        /// 创建快钱的订单表单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderpay">订单金额</param>
        /// <param name="certpathtt">证书路径"Content/Cert/99bill-rsa.pfx"</param>
        /// <param name="certpassword">证书密码</param>
        /// <returns></returns>
        public _99BillPay BuildPayConfig(string orderId, decimal orderpay, string certpassword,string certpathtt, string type)
        {
            string signMsgVal = "";
            this.orderId = orderId;
            this.orderAmount = Math.Floor(orderpay * 100).ToString();
            signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
            signMsgVal = appendParam(signMsgVal, "pageUrl", pageUrl);
            signMsgVal = appendParam(signMsgVal, "bgUrl", bgUrl);
            signMsgVal = appendParam(signMsgVal, "version", version);
            signMsgVal = appendParam(signMsgVal, "language", language);
            signMsgVal = appendParam(signMsgVal, "signType", signType);
            signMsgVal = appendParam(signMsgVal, "merchantAcctId", merchantAcctId);
            signMsgVal = appendParam(signMsgVal, "payerName", payerName);
            signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
            signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
            signMsgVal = appendParam(signMsgVal, "orderId", this.orderId);
            signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);
            signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
            signMsgVal = appendParam(signMsgVal, "productName", productName);
            signMsgVal = appendParam(signMsgVal, "productNum", productNum);
            signMsgVal = appendParam(signMsgVal, "productId", productId);
            signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
            signMsgVal = appendParam(signMsgVal, "ext1", ext1);
            signMsgVal = appendParam(signMsgVal, "ext2", ext2);
            signMsgVal = appendParam(signMsgVal, "payType", payType);
            signMsgVal = appendParam(signMsgVal, "redoFlag", redoFlag);
            signMsgVal = appendParam(signMsgVal, "pid", pid);
            if (type == "APP")
            {
                signMsgVal = appendParam(signMsgVal, "mobileGateway", mobileGateway);
            } 

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
            string certpath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + certpathtt;

            X509Certificate2 cert = new X509Certificate2(certpath, certpassword, X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            f.SetHashAlgorithm("SHA1");
            byte[] signonstr = sha.ComputeHash(bytes);
            this.signMsg = System.Convert.ToBase64String(f.CreateSignature(signonstr)).ToString();
            return this;
        }
        public string appendParam(string returnStr, string paramId, string paramValue)
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

    }
}
