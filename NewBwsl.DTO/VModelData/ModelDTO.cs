using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO
{
    public class ModelDTO
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
    }

    public class IDList
    {
        public List<Guid> ID { get; set; }
         
    }

    public class IDList2
    {
        public List<Guid> ID { get; set; }

        public string ChangeMarks { get; set; }
    }
}
