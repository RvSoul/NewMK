using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Pay
{
    /// <summary>
    /// 快钱支付回调
    /// </summary>
    public class _99BillReceive
    {
        public string certpath { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="certpath"></param>
        /// <param name="certpassword"></param>
        public _99BillReceive(string certpath )
        {
            this.certpath = certpath;
        }

        /// <summary>
        /// 快钱充值验签
        /// </summary>
        /// <param name="merchantAcctId"></param>
        /// <param name="version"></param>
        /// <param name="language"></param>
        /// <param name="signType"></param>
        /// <param name="payType"></param>
        /// <param name="bankId"></param>
        /// <param name="orderId"></param>
        /// <param name="orderTime"></param>
        /// <param name="orderAmount"></param>
        /// <param name="bindCard"></param>
        /// <param name="bindMobile"></param>
        /// <param name="dealId"></param>
        /// <param name="bankDealId"></param>
        /// <param name="dealTime"></param>
        /// <param name="payAmount"></param>
        /// <param name="fee"></param>
        /// <param name="ext1"></param>
        /// <param name="ext2"></param>
        /// <param name="payResult"></param>
        /// <param name="errCode"></param>
        /// <param name="verifySignature"></param>
        /// <param name="certpath"></param>
        /// <param name="certpassword"></param>
        /// <param name="successaction">验证成功后执行</param>
        /// <param name="failaction">验证失败后执行</param>
        /// <returns></returns>
        public void VerifySignature(string merchantAcctId, string version, string language, string signType, string payType, string bankId, string orderId, string orderTime, string orderAmount,string bindCard,string bindMobile, string dealId, string bankDealId, string dealTime, string payAmount, string fee, string ext1, string ext2, string payResult, string errCode, string verifySignature, 
            Action<string,string,string,string, decimal,decimal> successaction,Action<string,string,string> failaction)
        {
            StringBuilder signnonstr = new StringBuilder();
            signnonstr.Append(string.Format("merchantAcctId={0}", merchantAcctId));
            signnonstr.Append(SomeHelpServer.BuildQueryString("version", version));
            signnonstr.Append(SomeHelpServer.BuildQueryString("language", language));
            signnonstr.Append(SomeHelpServer.BuildQueryString("signType", signType));
            signnonstr.Append(SomeHelpServer.BuildQueryString("payType", payType));
            signnonstr.Append(SomeHelpServer.BuildQueryString("bankId", bankId));
            signnonstr.Append(SomeHelpServer.BuildQueryString("orderId", orderId));
            signnonstr.Append(SomeHelpServer.BuildQueryString("orderTime", orderTime));
            signnonstr.Append(SomeHelpServer.BuildQueryString("orderAmount", orderAmount));
            signnonstr.Append(SomeHelpServer.BuildQueryString("bindCard", bindCard));
            signnonstr.Append(SomeHelpServer.BuildQueryString("bindMobile", bindMobile));
            signnonstr.Append(SomeHelpServer.BuildQueryString("dealId", dealId));
            signnonstr.Append(SomeHelpServer.BuildQueryString("bankDealId", bankDealId));
            signnonstr.Append(SomeHelpServer.BuildQueryString("dealTime", dealTime));
            signnonstr.Append(SomeHelpServer.BuildQueryString("payAmount", payAmount));
            signnonstr.Append(SomeHelpServer.BuildQueryString("fee", fee.ToString()));
            signnonstr.Append(SomeHelpServer.BuildQueryString("ext1", ext1));
            signnonstr.Append(SomeHelpServer.BuildQueryString("ext2", ext2));
            signnonstr.Append(SomeHelpServer.BuildQueryString("payResult", payResult));
            signnonstr.Append(SomeHelpServer.BuildQueryString("errCode", errCode));

            var signresultstr = signnonstr.ToString();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signresultstr);
            X509Certificate2 cert = new X509Certificate2(certpath);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
            rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            f.SetHashAlgorithm("SHA1");
            byte[] result = sha.ComputeHash(bytes);
            byte[] SignatureByte = Convert.FromBase64String(verifySignature);
            if (f.VerifySignature(result, SignatureByte))
            {
                successaction(signresultstr, verifySignature, dealId ,orderId, decimal.Parse(orderAmount) / 100M,decimal.Parse( payAmount) / 100M);
            }
            else
            {
                failaction(signresultstr, verifySignature,"快钱签名验证失败！");
            }
        }

        /// <summary>
        /// POS机验签
        /// </summary>
        /// <param name="processFlag">处理结果</param>
        /// <param name="txnType">交易类型 </param>
        /// <param name="orgTxnType">原始交易类</param>
        /// <param name="amt">交易金额</param>
        /// <param name="externalTraceNo">外部跟踪编 号，一般为 商家订单号 </param>
        /// <param name="orgExternalTraceNo">原始外部跟 踪号 </param>
        /// <param name="terminalOperId">操作员编号</param>
        /// <param name="authCode">授权码 </param>
        /// <param name="terminalId">终端编号 </param>
        /// <param name="rrn">系统参考号 </param>
        /// <param name="txnTime">交易时间 yyyyMMdd HHmmss </param>
        /// <param name="shortPAN">缩略卡号 </param>
        /// <param name="responseCode">交易返回码 </param>
        /// <param name="cardType">卡类型 </param>
        /// <param name="issuerId">发卡机构 </param>
        /// <param name="signature">签名</param>
        /// <param name="successaction">验签通过执行</param>
        /// <param name="failaction">验签失败执行</param>
        public void VerifySignaturePos(string processFlag, string txnType, string orgTxnType, string amt, string externalTraceNo, string orgExternalTraceNo, string terminalOperId, string authCode,string terminalId, string rrn, string txnTime, string shortPAN, string responseCode, string cardType, string issuerId, string signature,
            Action<string,decimal,string,string,DateTime> successaction,Action failaction)
        {
            string val = "";
            val = appendParam(val, processFlag);
            val = appendParam(val, txnType);
            val = appendParam(val, orgTxnType);
            val = appendParam(val, amt);
            val = appendParam(val, externalTraceNo);
            val = appendParam(val, orgExternalTraceNo);
            val = appendParam(val, terminalOperId);
            val = appendParam(val, authCode);
            val = appendParam(val, rrn);
            val = appendParam(val, txnTime);
            val = appendParam(val, shortPAN);
            val = appendParam(val, responseCode);
            val = appendParam(val, cardType);
            val = appendParam(val, issuerId);

            var signresultstr = val.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(signresultstr);
            byte[] SignatureByte = Convert.FromBase64String(signature.Replace(' ', '+'));
            X509Certificate2 cert = new X509Certificate2(certpath);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
            rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(bytes);
            if (f.VerifySignature(result, SignatureByte))
            {
                DateTime dt;
                if(!DateTime.TryParseExact(txnTime, "yyyyMMdd HHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out dt))
                {
                    dt = DateTime.Now;
                }
                successaction(terminalId,decimal.Parse(amt), rrn, string.Format("[{0}]{1}", terminalId,  shortPAN ),dt);
            }
            else
            {
                failaction();
            }
        }

        string appendParam(string returnStr, string paramValue)
        {
            if (!string.IsNullOrEmpty(paramValue) && (paramValue != ""))
            {
                returnStr += paramValue;
            }
            return returnStr;
        }
    }
}
