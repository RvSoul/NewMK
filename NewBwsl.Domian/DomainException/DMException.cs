using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DomainException
{
    /// <summary>
    /// 业务处理异常
    /// </summary>
    public class DMException : ApplicationException
    {
        public DMException() { }
        public DMException(string message)
            : base(message) { }
        public DMException(string message, Exception inner)
            : base(message, inner) { }
    }
}
