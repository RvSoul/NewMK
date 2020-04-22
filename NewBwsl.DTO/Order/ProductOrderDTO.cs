using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    public class ProductOrderDTO
    {
        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        public int Count1 { get; set; }
        public decimal je1 { get; set; }
        public decimal pv1 { get; set; }
        public int Count2 { get; set; }
        public decimal je2 { get; set; }
        public decimal pv2 { get; set; }
        public int Count3 { get; set; }
        public decimal je3 { get; set; }
        public decimal pv3 { get; set; }
        public int Count4 { get; set; }
        public decimal je4 { get; set; }
        public decimal pv4 { get; set; }



    }

    public class AddressOrderDTO
    {
        public string ConsigneeProvince { get; set; }

        public int Count1 { get; set; }
        public decimal je1 { get; set; }
        public decimal pv1 { get; set; }
        public int Count2 { get; set; }
        public decimal je2 { get; set; }
        public decimal pv2 { get; set; }
        public int Count3 { get; set; }
        public decimal je3 { get; set; }
        public decimal pv3 { get; set; }
        public int Count4 { get; set; }
        public decimal je4 { get; set; }
        public decimal pv4 { get; set; }

    }
    public class Pro_Query_ItemSaleCase_webDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OrderCode { get; set; }
        public string OrderDateTime { get; set; }
        public Decimal TotalMoney { get; set; }
        public int TotalPV { get; set; }
        public string ReveiveInfo_Privence { get; set; }
        public string ReveiveInfo_City { get; set; }
        public string ReveiveInfo_Area { get; set; }
        public string ReveiveInfo_Address { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string CManager { get; set; }
        public int Count { get; set; }
    }

    public class Pro_Sum_Market_AchievementDTO
    {
        public string Privence { get; set; }
        public int Yk { get; set; }
        public int YkPv { get; set; }
        public int Gk { get; set; }
        public int GkPV { get; set; }       
        public int VIPGk { get; set; }
        public int VIPGkPv { get; set; }
        public int CK { get; set; }
        public int CKPV { get; set; }
        public int Sj { get; set; }
        public int SjPv { get; set; }
        public int Zx { get; set; }
        public int ZxPv { get; set; }
        public int TotalAch { get; set; }
        public int TotalAchPv { get; set; }
    }

}
