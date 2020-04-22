using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    public class DataCountDTO
    {
        public string BusiDate { get; set; }
        public int ReCount { get; set; }
        public int PkiCount { get; set; }
        public int YkiCount { get; set; }
        public int JkiCount { get; set; }
        public int ZkiCount { get; set; }
        public int RegNum { get; set; }
        public int SJNum { get; set; }
        public int ZxNum { get; set; }
        public int FirstJhNum { get; set; }
        public int SecondJhNum { get; set; }
        public Decimal TotalPrice { get; set; }
        public int QgNum { get; set; }

    }
}
