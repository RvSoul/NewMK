using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.ManageData
{
    public class AreasDTO
    {
        public int ID { get; set; }
        public Nullable<int> PerantId { get; set; }
        public Nullable<int> Level { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
        public string AreaFullName { get; set; }

        public int value { get; set; }
        public string text { get; set; }
        public List<CityList> children { get; set; }
    }

    public class CityList
    {
        public int ID { get; set; }
        public Nullable<int> PerantId { get; set; }
        public Nullable<int> Level { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
        public string AreaFullName { get; set; }
        public int value { get; set; }
        public string text { get; set; }
        public List<CountyList> children { get; set; }
    }
    public class CountyList
    {
        public int ID { get; set; }
        public Nullable<int> PerantId { get; set; }
        public Nullable<int> Level { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
        public string AreaFullName { get; set; }
        public int value { get; set; }
        public string text { get; set; }

    }
}
