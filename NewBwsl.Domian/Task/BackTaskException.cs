using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Task
{
    /// <summary>
    /// 后台任务异常
    /// </summary>
    public class BackTaskException: ApplicationException
    {
        public BackTaskException() { }
        public BackTaskException(string message)  
            : base(message) { }
        public BackTaskException(string message, Exception inner)  
            : base(message, inner) { }
    }
}
