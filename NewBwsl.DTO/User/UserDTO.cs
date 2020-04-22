using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    public class Request_User : ModelDTO
    {
        [SelectField("and", "=", "Guid")]
        public Guid? ID { get; set; }

        [SelectField("and", "=", "int")]
        public int? DeLevelID { get; set; }

        [SelectField("and", "in", "string")]
        public string UserCode { get; set; }

        [SelectField("and", "in", "string")]
        public string UserName { get; set; }

        [SelectField("and", "in", "string")]
        public string Phone { get; set; }

        [SelectField("and", "=", "int")]
        public int? UserState { get; set; }

        [SelectField("and", "=", "int")]
        public int? ProvinceId { get; set; }

        [SelectField("and", "=", "int")]
        public int? CityId { get; set; }

        [SelectField("and", "=", "int")]
        public int? CountyId { get; set; }

        [SelectField("and", "in", "string")]
        public string CardCode { get; set; }

        [SelectField("and", "=", "int")]
        public int? IsBalance { get; set; }
        

        public DateTime? stime { get; set; }
        public DateTime? etime { get; set; }

        public string yzm { get; set; }
    }
    public class UserModel
    {
        public System.Guid ID { get; set; }
        public int DeLevelID { get; set; }
        public Nullable<int> HonLevelID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string Phone { get; set; }
        public bool Sex { get; set; }
        public string CardCode { get; set; }
        public System.DateTime AddTime { get; set; }
        public Nullable<System.Guid> PID { get; set; }
        public int UserState { get; set; }
        public string OpenID { get; set; }
        public Nullable<int> Source { get; set; }
        public decimal EleMoney { get; set; }
        public decimal BonusMoney { get; set; }
        public int BonusState { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> CountyId { get; set; }
        public int StageCount { get; set; }
        public string LastIP { get; set; }
        public Nullable<System.DateTime> LastTime { get; set; }
        public int TPoint_Point { get; set; }
        public int RPoint_Point { get; set; }
        public string PayPwd { get; set; }
        public string IsBak { get; set; }
        public decimal PVSurplus { get; set; }
        public string BankNumber { get; set; }
        public string BakAddress { get; set; }
        public string BakName { get; set; }
        public string WeiXinName { get; set; }
        public string WeiXinUrl { get; set; }
        public Nullable<bool> IsVip { get; set; }
        public decimal EleMoneyFrozen { get; set; }
        public string Unionid { get; set; }
        public string BakBranchAddress { get; set; }
        public Nullable<int> VersionNum { get; set; }

    }
    public class UserDTO : UserModel
    {

        public string PCDealerCode { get; set; }
        public string PCDealerName { get; set; }
        public string PPDealerCode { get; set; }
        public string PPDealerName { get; set; }
        public string DeptSelect { get; set; }
        public string DeLevelName { get; set; }
        public string HonLevelName { get; set; }
        public string yzm { get; set; } 

    }
    public class UserMoney
    {
        public Nullable<decimal> EleMoney { get; set; }
        public Nullable<decimal> BonusMoney { get; set; }
        public int BonusState { get; set; }
    }
    public class UserYeJi
    {
        public string DealerName { get; set; }
        public Decimal AchNum { get; set; }
        public int Tag { get; set; }
        public string Msg { get; set; }
    }
    public class UserXiahua
    {
        public string ManageDealerId { get; set; }
        public string ManageDealerName { get; set; }
        public string AreaName { get; set; }
        public int Tag { get; set; }
        public string ErrMessage { get; set; }
    }
    public class UserIsShanXia
    {
        public int result { get; set; }
    }
    public class UserABqu
    {
        public string AreaName { get; set; }
        public int Tag { get; set; }
        public string ErrMessage { get; set; }
    }
    public class UserDianPu
    {
        public Guid CenterId { get; set; }
        public string CenterCode { get; set; }
        public string CenterName { get; set; }
    }
    public class UserBak
    {
        public string BankNumber { get; set; }
        public string BakAddress { get; set; }
        public string BakName { get; set; }
        public string BakBranchAddress { get; set; }
    }
    public class UserRelation
    {
        public Guid Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string AddTime { get; set; }
        public int TPoint { get; set; }
        public int RPoint { get; set; }
        public string HonLevelName { get; set; }
        public Guid TopId { get; set; }
        public string TopUserCode { get; set; }
        public int Layer { get; set; }
        public string AreaName { get; set; }
        public string DeLevelName { get; set; }
        
    }

    public class UserShouYiDTO
    {
        public int StageCount { get; set; }
        public decimal Record { get; set; }
        public decimal Sale { get; set; }
        public decimal ShengQian { get; set; }
        public decimal ServicePrize { get; set; }

        public decimal TotalPrize { get; set; }


    }
}
