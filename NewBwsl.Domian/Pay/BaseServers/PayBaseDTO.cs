using NewMK.Domian.DomainException;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Pay
{
    class PayBaseDTO
    {
        public PayBaseDTO()
        {

        }
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }
        public object GetValue(string key)
        {
            object r = null;
            m_values.TryGetValue(key, out r);
            return r;
        }
        public bool IsSet(string key)
        {
            return GetValue(key) != null;
        }


        public string ToXml()
        {
            if (0 == m_values.Count)
            {
                throw new DMException("XML消息体为空！");
            }
            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new DMException("不能序列化为null的属性!");
                }
                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else
                {
                    throw new DMException("属性数据类型错误！");
                }
            }
            xml += "</xml>";
            return xml;
        }


        public SortedDictionary<string, object> FromXml(string xml, string key)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new DMException("XML字符串不能为空!");
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }
            try
            {
                if (m_values["return_code"].ToString().ToUpper() != "SUCCESS")
                {
                    return m_values;
                }
                CheckSign(key);//验证签名,不通过会抛异常
            }
            catch (DMException ex)
            {
                throw new DMException(ex.Message);
            }

            return m_values;
        }

        public bool CheckSign(string key)
        {
            if (!IsSet("sign"))
            {
                throw new DMException("签名内容为空！");
            }
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                throw new DMException("签名内容为空!");
            }
            string return_sign = GetValue("sign").ToString();
            string cal_sign = MakeSign(key);
            if (cal_sign == return_sign)
            {
                return true;
            }
            throw new DMException("签名验证错误!");
        }


        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new DMException("不能序列化为null的属性！");
                }
                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
        public string MakeSign(string key)
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + key;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }


        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }
    }
}
