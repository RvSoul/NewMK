using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DomainException
{
    /// <summary>
    /// 订单不需要付款异常
    /// </summary>
    public class OrderNoNeedPayException : ApplicationException
    {
        private int status;
        public OrderNoNeedPayException(int status)
        {
            this.status = status;
        }
        public OrderNoNeedPayException(int status, string message)
            : base(message)
        {
            this.status = status;
        }
        public OrderNoNeedPayException(int status, string message, Exception inner)
            : base(message, inner)
        {
            this.status = status;
        }

        public int Status
        {
            get
            {
                return status;
            }
        }
    }
}