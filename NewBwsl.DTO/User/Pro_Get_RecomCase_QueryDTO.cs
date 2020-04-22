using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    public class Pro_Get_RecomCase_QueryDTO
    {
        /// <summary>
        /// 推荐人编号
        /// </summary>
        public string PDealerCode { get; set; }

        /// <summary>
        /// 推荐人
        /// </summary>
        public string PName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string TelPhone { get; set; }

        /// <summary>
        /// 荣誉级别
        /// </summary>
        public string HonLevel { get; set; }

        public int YkCount { get; set; }       
        public int GkCount { get; set; }      
        public int VIPGkCount { get; set; }
        public int CKCount { get; set; }

        public string DealerCode { get; set; }
         
        public string DName { get; set; }

        /// <summary>
        /// 部门主消
        /// </summary>
        public int Dealer_Zx_Pv { get; set; }

        /// <summary>
        /// 个人主消
        /// </summary>
        public int PDealer_Zx_Pv { get; set; }

    }
}
