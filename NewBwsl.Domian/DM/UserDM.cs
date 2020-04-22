using EntityFramework.Extensions;

using NewMK.DTO.User;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewMK.DTO.Record;
using NewMK.DTO;
using System.Linq.Expressions;
using NewMK.DTO.Msg;
using Pay;
using NewMK.Domian.DomainException;
using System.Threading;

namespace NewMK.Domian.DM
{
    public class UserDM
    {
        public UserDTO LoginUser(string name, string pwd)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string pp = GetPwd(pwd);
                UserDTO dto = c.User.Where(w => w.UserCode == name && w.UserPwd == pp).Select(
                    x => new UserDTO
                    {
                        ID = x.ID,
                        UserCode = x.UserCode,
                        Phone = x.Phone,
                        UserState = x.UserState,
                        LastTime = x.LastTime,
                        LastIP = x.LastIP

                    }
                    ).FirstOrDefault();

                return dto;
            }
        }
        public void UpLoginTime(string name, string ip)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User u = c.User.Where(w => w.UserCode == name).FirstOrDefault();
                u.LastIP = ip;
                u.LastTime = DateTime.Now;
                c.SaveChanges();
            }
        }
        public List<DeLevelDTO> GetDeLevelList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                List<DeLevel> dto = c.DeLevel.ToList();

                return DeLevelDTO.GetDTOList<DeLevel, DeLevelDTO>(dto);
            }
        }

        public List<HonLevelDTO> GetHonLevelList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                List<HonLevel> dto = c.HonLevel.ToList();

                return HonLevelDTO.GetDTO(dto);
            }
        }

        public UserDTO LoginUserWx(string openid, string unionid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                UserDTO dto = c.User.Where(w => w.OpenID == openid || w.OpenID == unionid).Select(  //&& w.LoginPwd == pwd
                    x => new UserDTO
                    {
                        ID = x.ID,
                        UserCode = x.UserCode,
                        Phone = x.Phone,
                        UserState = x.UserState
                    }
                ).FirstOrDefault();

                return dto;
            }
        }

        public bool UpUser(Guid Id, string Phone, bool Sex, string UserName, string CardCode)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.Where(n => n.ID == Id).FirstOrDefault();
                if (pt.Phone == null || pt.Phone == "")
                {
                    pt.Phone = "";
                }
                User tt = c.User.Where(w => w.Phone == Phone).FirstOrDefault();
                if (tt != null)
                {
                    if (tt.Phone == null || tt.Phone == "")
                    {
                        tt.Phone = "";
                    }
                    //if (pt.Phone != tt.Phone)
                    //{
                    //    throw new DMException("该手机已经绑定，请更换手机！");
                    //}
                }


                pt.Phone = Phone;
                pt.Sex = Sex;
                pt.UserName = UserName;
                if (CardCode != null && CardCode != "")
                {
                    pt.CardCode = CardCode;
                }

                c.SaveChanges();
                return true;
            }
        }

        public bool BDUserPhone(Guid? userid, string name, string phone)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User user = c.User.FirstOrDefault(n => n.ID == userid);
                if (user != null)
                {
                    string pwd = phone.Substring(5, 6);
                    string pp = GetPwd(pwd);

                    user.PayPwd = pp;
                    user.UserPwd = pp;

                    user.UserName = name;
                    user.Phone = phone;
                    c.SaveChanges();

                }
                else
                {
                    throw new DMException("未找到该用户！");
                }
                return true;
            }
        }
        public bool UpWeiXinUser(Guid? userid, WeiXinUserinfo dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User olduser = c.User.Where(n => n.OpenID == dto.openid && n.ID != userid).FirstOrDefault();
                if (olduser != null)
                {
                    throw new DMException("该微信已绑定其他用户！");
                }

                User user = c.User.FirstOrDefault(n => n.ID == userid);
                if (user != null)
                {
                    user.WeiXinName = dto.nickname;
                    user.WeiXinUrl = dto.headimgurl;
                    user.OpenID = dto.openid;
                    user.Unionid = dto.unionid;
                    c.SaveChanges();
                }
                else
                {
                    throw new DMException("未找到该用户！");
                }
                return true;
            }
        }

        public bool UpUserState(Guid userId, int state)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.FirstOrDefault(n => n.ID == userId);

                pt.UserState = state;

                c.SaveChanges();
                return true;
            }
        }

        public bool UpBonusState(Guid userId, int state)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.FirstOrDefault(n => n.ID == userId);
                pt.BonusState = state;

                c.SaveChanges();
                return true;
            }
        }

        public bool UpUserType(UserDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.FirstOrDefault(n => n.ID == dto.ID);
                pt.DeLevelID = dto.DeLevelID;

                c.SaveChanges();
                return true;
            }
        }

        public bool DeWeiXinOpenId(string usercode)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.FirstOrDefault(n => n.UserCode == usercode);
                pt.OpenID = null;
                pt.WeiXinName = null;
                pt.WeiXinUrl = null;
                pt.Unionid = null;

                c.SaveChanges();
                return true;
            }
        }

        public UserDTO GetIduser(Guid? id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    User user = c.User.Where(w => w.ID == id).FirstOrDefault();
                    if (user == null)
                    {
                        throw new DMException("用户不存在！");
                    }
                    UserDTO dto = new UserDTO
                    {
                        ID = user.ID,
                        AddTime = user.AddTime,
                        Phone = user.Phone,
                        Sex = user.Sex,
                        UserName = user.UserName,
                        DeLevelID = user.DeLevelID,
                        OpenID = user.OpenID,
                        Unionid = user.Unionid,
                        UserCode = user.UserCode,
                        DeLevelName = user.DeLevel.Name,
                        HonLevelID = user.HonLevelID,
                        WeiXinName = user.WeiXinName,
                        WeiXinUrl = user.WeiXinUrl,
                        ProvinceId = user.ProvinceId,
                        CityId = user.CityId,
                        CountyId = user.CountyId,
                        // HonLevelName = user.HonLevel.Name
                        EleMoney = user.EleMoney,
                        BonusMoney = user.BonusMoney,
                        //UserState = x.UserState

                    };

                    RecomRelation pc = cc.RecomRelation.Where(w => w.DealerId == dto.ID).FirstOrDefault();
                    if (pc != null)
                    {
                        if (pc.PDealerId != null)
                        {
                            User urp = c.User.Where(t => t.ID == pc.PDealerId).FirstOrDefault();
                            dto.PCDealerCode = urp.UserCode;
                            dto.PCDealerName = urp.UserName;
                        }

                    }

                    PlaceRelation pp = cc.PlaceRelation.Where(w => w.DealerId == dto.ID).FirstOrDefault();
                    if (pp != null)
                    {
                        if (pp.PDealerId != null)
                        {
                            User upp = c.User.Where(t => t.ID == pp.PDealerId).FirstOrDefault();
                            dto.PPDealerCode = upp.UserCode;
                            dto.PPDealerName = upp.UserName;
                            dto.DeptSelect = pp.AreaName;
                        }

                    }

                    return dto;
                }
            }
        }

        public void SignOutLogin(Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                User user = c.User.Where(w => w.ID == userid).FirstOrDefault();
                user.LastTime = Convert.ToDateTime("2012-3-4 05:06:07");
                c.SaveChanges();
            }
        }

        public UserDTO GetUserInfo(string usercode)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    User user = c.User.Where(w => w.UserCode == usercode).FirstOrDefault();
                    if (user == null)
                    {
                        throw new DMException("用户不存在！");
                    }

                    UserDTO dto = new UserDTO
                    {
                        ID = user.ID,
                        AddTime = user.AddTime,
                        Phone = user.Phone,
                        Sex = user.Sex,
                        UserName = user.UserName,
                        DeLevelID = user.DeLevelID,
                        OpenID = user.OpenID,
                        UserCode = user.UserCode,
                        EleMoney = user.EleMoney,
                        BonusMoney = user.BonusMoney,
                        UserState = user.UserState,
                        DeLevelName = user.DeLevel.Name,
                        WeiXinName = user.WeiXinName,
                        WeiXinUrl = user.WeiXinUrl,
                        ProvinceId = user.ProvinceId,
                        CityId = user.CityId,
                        CountyId = user.CountyId

                    };

                    RecomRelation pc = cc.RecomRelation.Where(w => w.DealerId == dto.ID).FirstOrDefault();
                    if (pc != null)
                    {
                        if (pc.PDealerId != null)
                        {
                            User urp = c.User.Where(t => t.ID == pc.PDealerId).FirstOrDefault();
                            dto.PCDealerCode = urp.UserCode;
                            dto.PCDealerName = urp.UserName;
                        }

                    }

                    PlaceRelation pp = cc.PlaceRelation.Where(w => w.DealerId == dto.ID).FirstOrDefault();
                    if (pp != null)
                    {
                        if (pp.PDealerId != null)
                        {
                            User upp = c.User.Where(t => t.ID == pp.PDealerId).FirstOrDefault();
                            dto.PPDealerCode = upp.UserCode;
                            dto.PPDealerName = upp.UserName;
                            dto.DeptSelect = pp.AreaName;
                        }

                    }

                    return dto;
                }
            }
        }

        public UserShouYiDTO GetUserShouYI(Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                UserShouYiDTO dto = new UserShouYiDTO();
                User user = c.User.Where(w => w.ID == userid).FirstOrDefault();
                if (user == null)
                {
                    return dto;
                }
                dto.StageCount = user.StageCount;
                string id = userid.ToString();
                try
                {
                    int max = c.WeekPrize.Max(m => m.BalanceNumber);
                    List<WeekPrize> wp = c.WeekPrize.Where(w => w.DealerId == id && w.BalanceNumber == max).ToList();
                    foreach (var item in wp)
                    {
                        dto.Sale += item.SalePrize;
                        dto.Record += item.RecordPrize;
                        dto.ServicePrize += item.ServicePrize;
                        dto.TotalPrize += item.TotalPrize;
                    }


                    List<OrderProduct> li = c.OrderProduct.Where(w => w.Order.UserID == userid).ToList();
                    double zPrice = 0;
                    foreach (var item in li)
                    {
                        zPrice += Convert.ToDouble(item.Product.Price) * Convert.ToDouble(item.ProductNum);
                    }

                    decimal? sjPrice = c.Order.Where(w => w.UserID == userid).Sum(s => s.MoneyProduct);
                    dto.ShengQian = Convert.ToDecimal(zPrice) - Convert.ToDecimal(sjPrice);
                }
                catch (Exception)
                {
                    dto.Sale = 0;
                    dto.Record = 0;
                    dto.ServicePrize = 0;
                    dto.TotalPrize = 0;
                    dto.ShengQian = 0;
                }

                return dto;

            }
        }

        public bool GetUserInfoBool(string usercode)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    User user = c.User.Where(w => w.UserCode == usercode).FirstOrDefault();
                    if (user == null)
                    {
                        return false;
                    }



                    return true;
                }
            }
        }

        public List<ServiceCenterDTO> GetServiceCenterList(Request_ServiceCenter rdto, out int num)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Expression<Func<ServiceCenter, bool>> expr = AutoAssemble.Splice<ServiceCenter, Request_ServiceCenter>(rdto);
                num = c.ServiceCenter.Where(expr).Count();
                List<ServiceCenterDTO> li = c.ServiceCenter.Where(expr).OrderByDescending(px => px.ApplyDate).Skip((rdto.PageIndex - 1) * rdto.PageSize).Take(rdto.PageSize)
                    .Select(x => new ServiceCenterDTO
                    {
                        ApplyDate = x.ApplyDate,
                        AuditDate = x.AuditDate,
                        AuditResult = x.AuditResult,
                        CityId = x.CityId,
                        CManager = x.CManager,
                        Consignee = x.Consignee,
                        CountyId = x.CountyId,
                        ID = x.ID,
                        IFOrder = x.IFOrder,
                        IsHealthCheckRight = x.IsHealthCheckRight,
                        OldID = x.OldID,
                        OldName = x.OldName,
                        ProvinceId = x.ProvinceId,
                        ReceivingAddress = x.ReceivingAddress,
                        ReceivingCall = x.ReceivingCall,
                        SCType = x.SCType,
                        ServiceCenterName = x.ServiceCenterName,
                        UserID = x.UserID,
                        ZipCode = x.ZipCode,
                        UserCode = x.User.UserCode,
                        ParentDealerCode = x.ParentDealerCode
                    }).ToList();


                return li;
            }
        }

        public bool UpServiceCenter(ServiceCenterDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ServiceCenter model = c.ServiceCenter.Where(w => w.ID == dto.ID).FirstOrDefault();

                // model.UserID = dto.UserID;
                model.ServiceCenterName = dto.ServiceCenterName;
                // model.ApplyDate = dto.ApplyDate;
                // model.AuditDate = dto.AuditDate;
                // model.AuditResult = dto.AuditResult;
                model.SCType = dto.SCType;
                model.CManager = dto.CManager;
                model.ProvinceId = dto.ProvinceId;
                model.CityId = dto.CityId;
                model.CountyId = dto.CountyId;
                model.Consignee = dto.Consignee;
                model.ReceivingCall = dto.ReceivingCall;
                model.ReceivingAddress = dto.ReceivingAddress;
                model.ZipCode = dto.ZipCode;
                model.OldName = dto.OldName;
                model.OldID = dto.OldID;
                model.IFOrder = dto.IFOrder;
                model.IsHealthCheckRight = dto.IsHealthCheckRight;
                model.ParentDealerCode = dto.ParentDealerCode;

                c.SaveChanges();
                return true;
            }
        }

        public bool SHServiceCenter(Guid id, int auditResult)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ServiceCenter model = c.ServiceCenter.Where(w => w.ID == id).FirstOrDefault();

                model.AuditDate = DateTime.Now;
                model.AuditResult = auditResult;

                c.SaveChanges();
                return true;
            }
        }

        public ServiceCenterDTO GetServiceCenter(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                ServiceCenterDTO li = c.ServiceCenter.Where(w => w.ID == id).Select(x => new ServiceCenterDTO
                {
                    ApplyDate = x.ApplyDate,
                    AuditDate = x.AuditDate,
                    AuditResult = x.AuditResult,
                    CityId = x.CityId,
                    CManager = x.CManager,
                    Consignee = x.Consignee,
                    CountyId = x.CountyId,
                    ID = x.ID,
                    IFOrder = x.IFOrder,
                    IsHealthCheckRight = x.IsHealthCheckRight,
                    OldID = x.OldID,
                    OldName = x.OldName,
                    ProvinceId = x.ProvinceId,
                    ReceivingAddress = x.ReceivingAddress,
                    ReceivingCall = x.ReceivingCall,
                    SCType = x.SCType,
                    ServiceCenterName = x.ServiceCenterName,
                    UserID = x.UserID,
                    ZipCode = x.ZipCode,
                    UserCode = x.User.UserCode,
                    ParentDealerCode = x.ParentDealerCode
                }).FirstOrDefault();

                return li;
            }
        }

        public bool AddServiceCenter(ServiceCenterDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                dto.ID = Guid.NewGuid();
                dto.ApplyDate = DateTime.Now;
                dto.AuditResult = 0;
                c.ServiceCenter.Add(GetMapperDTO.SetModel<ServiceCenter, ServiceCenterModel>(dto));

                if (dto.OldID != null)
                {
                    ServiceCenter model = c.ServiceCenter.Where(w => w.ID == Guid.Parse(dto.OldID)).FirstOrDefault();

                    model.AuditDate = DateTime.Now;
                    model.AuditResult = 3;
                }
                c.SaveChanges();
                return true;
            }
        }

        public UserDTO GetPhoneUser(string phone)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                UserDTO dto = c.User.Where(w => w.Phone == phone).Select(
                    x => new UserDTO
                    {
                        ID = x.ID,
                        UserName = x.UserName

                    }
                ).FirstOrDefault();

                return dto;
            }
        }

        public List<UserDTO> GetUserList(Request_User rdto, out int count)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {

                    Expression<Func<User, bool>> expr = AutoAssemble.Splice<User, Request_User>(rdto);

                    if (rdto.stime != null)
                    {
                        expr = expr.And2(w => w.AddTime > rdto.stime);
                    }
                    if (rdto.etime != null)
                    {
                        expr = expr.And2(w => w.AddTime < rdto.etime);
                    }

                    count = c.User.Where(expr).Count();
                    List<UserDTO> dto = c.User.Where(expr).OrderByDescending(px => px.AddTime).Skip((rdto.PageIndex - 1) * rdto.PageSize).Take(rdto.PageSize).Select(
                        x => new UserDTO
                        {
                            ID = x.ID,
                            AddTime = x.AddTime,
                            Phone = x.Phone,
                            Sex = x.Sex,
                            UserName = x.UserName,
                            DeLevelID = x.DeLevelID,
                            OpenID = x.OpenID,
                            UserCode = x.UserCode,
                            EleMoney = x.EleMoney,
                            BonusMoney = x.BonusMoney,
                            UserState = x.UserState,
                            StageCount = x.StageCount,
                            DeLevelName = x.DeLevel.Name,
                            BonusState = x.BonusState,
                            CardCode = x.CardCode,
                            CityId = x.CityId,
                            CountyId = x.CountyId,
                            HonLevelID = x.HonLevelID,
                            ProvinceId = x.ProvinceId,
                            PVSurplus = x.PVSurplus,
                            RPoint_Point = x.RPoint_Point,
                            TPoint_Point = x.TPoint_Point,
                            WeiXinName = x.WeiXinName,
                            WeiXinUrl = x.WeiXinUrl,
                            IsVip = x.IsVip,
                            Source = x.Source
                        }
                    ).ToList();
                    foreach (UserDTO item in dto)
                    {
                        RecomRelation pc = cc.RecomRelation.Where(w => w.DealerId == item.ID).FirstOrDefault();
                        if (pc != null && pc.PDealerId != null)
                        {
                            User urp = c.User.Where(t => t.ID == pc.PDealerId).FirstOrDefault();
                            item.PCDealerCode = urp.UserCode;
                            item.PCDealerName = urp.UserName;

                        }

                        PlaceRelation pp = cc.PlaceRelation.Where(w => w.DealerId == item.ID).FirstOrDefault();
                        if (pp != null && pp.PDealerId != null)
                        {
                            User upp = c.User.Where(t => t.ID == pp.PDealerId).FirstOrDefault();
                            item.PPDealerCode = upp.UserCode;
                            item.PPDealerName = upp.UserName;
                            item.DeptSelect = pp.AreaName;
                        }
                    }

                    return dto;
                }
            }
        }

        public List<UserDTO> GetUserListDownload(string phone, string userName)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<User, bool>> expr = n => true;
                if (phone != null && phone != "")
                {
                    expr = expr.And2(w => w.Phone == phone);
                }
                if (userName != null && userName != "")
                {
                    expr = expr.And2(w => w.UserName == userName);
                }

                List<UserDTO> dto = c.User.Where(expr).OrderByDescending(px => px.AddTime).Select(
                    x => new UserDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        Phone = x.Phone,
                        Sex = x.Sex,
                        UserName = x.UserName,
                        DeLevelID = x.DeLevelID,
                        UserState = x.UserState,
                        DeLevelName = x.DeLevel.Name

                    }
                ).ToList();

                return dto;
            }
        }

        public List<UserAddressDTO> GetUserAddressList(Guid? id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<UserAddressDTO> dto = c.UserAddress.Where(w => w.UserID == id).OrderByDescending(ob => ob.IsDefault).Select(
                    x => new UserAddressDTO
                    {
                        ID = x.ID,
                        IsDefault = x.IsDefault,
                        ProvinceID = x.ProvinceID,
                        ReceivingAddress = x.ReceivingAddress,
                        ReceivingName = x.ReceivingName,
                        ReceivingPhone = x.ReceivingPhone,
                        UserID = x.UserID,
                        AddTime = x.AddTime,
                        ConsigneeCity = x.ConsigneeCity,
                        ConsigneeCounty = x.ConsigneeCounty,
                        ConsigneeProvince = x.ConsigneeProvince
                    }
                ).ToList();

                return dto;
            }
        }

        public bool AddUserAddress(UserAddressDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                if (dto.IsDefault == true)
                {
                    UserAddress tt = c.UserAddress.Where(w => w.UserID == dto.UserID && w.IsDefault == true).FirstOrDefault();
                    if (tt != null)
                    {
                        tt.IsDefault = false;
                    }

                }

                UserAddress model = new UserAddress();
                model.ID = Guid.NewGuid();
                model.UserID = dto.UserID;
                model.IsDefault = dto.IsDefault;
                model.ProvinceID = dto.ProvinceID;
                model.ReceivingAddress = dto.ReceivingAddress;
                model.ReceivingName = dto.ReceivingName;
                model.ReceivingPhone = dto.ReceivingPhone;
                model.AddTime = DateTime.Now;
                model.ConsigneeCity = dto.ConsigneeCity;
                model.ConsigneeCounty = dto.ConsigneeCounty;
                model.ConsigneeProvince = dto.ConsigneeProvince;

                c.UserAddress.Add(model);

                c.SaveChanges();
                return true;
            }
        }

        public bool deUserAddress(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                UserAddress pt = c.UserAddress.FirstOrDefault(n => n.ID == id);

                c.UserAddress.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        public bool UpUserAddressIfDefault(Guid id, Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                c.UserAddress.Where(w => w.UserID == userid).Update(m => new UserAddress { IsDefault = false });

                UserAddress model = c.UserAddress.Where(w => w.ID == id).FirstOrDefault();
                model.IsDefault = true;
                c.SaveChanges();
                return true;
            }
        }

        public bool UpUserPwd(string phone, string userPwd, string usercode, string level)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.FirstOrDefault(n => n.UserCode == usercode && n.Phone == phone);
                if (pt == null)
                {
                    throw new DMException("编号和手机号不匹配！");
                }

                UserPwdCode up = pt.UserPwdCode.FirstOrDefault();

                if (level == "1")
                {
                    pt.UserPwd = GetPwd(userPwd);
                    up.UserPwd = userPwd;
                }
                if (level == "2")
                {
                    pt.PayPwd = GetPwd(userPwd);
                    up.PayPwd = userPwd;
                }
                if (level == "3")
                {
                    pt.UserPwd = GetPwd(userPwd);
                    pt.PayPwd = GetPwd(userPwd);

                    up.UserPwd = userPwd;
                    up.PayPwd = userPwd;
                }


                c.SaveChanges();
                return true;
            }
        }

        public bool UpUserPwdHT(string userPwd, string usercode, string level)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User pt = c.User.FirstOrDefault(n => n.UserCode == usercode);
                if (pt == null)
                {
                    throw new DMException("未找到该用户！");
                }

                UserPwdCode up = pt.UserPwdCode.FirstOrDefault();

                if (level == "1")
                {
                    pt.UserPwd = GetPwd(userPwd);
                    up.UserPwd = userPwd;
                }
                if (level == "2")
                {
                    pt.PayPwd = GetPwd(userPwd);
                    up.PayPwd = userPwd;
                }
                if (level == "3")
                {
                    pt.UserPwd = GetPwd(userPwd);
                    pt.PayPwd = GetPwd(userPwd);

                    up.UserPwd = userPwd;
                    up.PayPwd = userPwd;
                }


                c.SaveChanges();
                return true;
            }
        }

        public string Register(UserDTO model, Guid? AdminUserID)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    Guid userid = Guid.NewGuid();

                    if (string.IsNullOrEmpty(model.Phone))
                    {
                        throw new DMException("手机号不能为空！");
                    }

                    //model.DeLevelID = 1;//默认注册级别
                    model.UserState = 1;//默认启用
                    if (AdminUserID != null)
                    {
                        model.DeLevelID = model.DeLevelID;
                        model.Source = 4;//后台注册
                    }
                    else
                    {
                        if (model.PCDealerCode != null)
                        {
                            if (model.OpenID != null)
                            {
                                model.Source = 3;//链接注册
                                model.DeLevelID = 2;
                            }
                            else
                            {
                                model.DeLevelID = model.DeLevelID;
                                model.Source = 2;//代购注册
                                model.UserState = 0;//代购不启用
                            }
                        }
                        else
                        {
                            model.Source = 1;//自己来注册的
                            model.DeLevelID = 1;
                            throw new DMException("暂时不开放个人注册！");
                        }
                    }
                    if (model.CardCode != null && c.User.FirstOrDefault(w => w.CardCode == model.CardCode) != null)
                    {
                        throw new DMException("该身份证号码已经注册，请重新确认！");
                    }

                    #region 推荐人
                    //if (model.DeLevelID == 3 || model.DeLevelID == 4)
                    //{
                    if (!string.IsNullOrEmpty(model.PCDealerCode))
                    {
                        if (cc.RecomRelation.Where(w => w.DealerId == userid).FirstOrDefault() != null)
                        {
                            throw new DMException("推荐关系已经存在！");
                        }
                        User PCDealer = c.User.Where(w => w.UserCode == model.PCDealerCode).FirstOrDefault();
                        if (PCDealer.UserState == 0)
                        {
                            throw new DMException("推荐人未激活！");
                        }
                        if (PCDealer.UserState == 2)
                        {
                            throw new DMException("推荐人已锁定！");
                        }
                        //if (PCDealer.DeLevelID != (int)Enum.DeLevel.创客)
                        //{
                        //    throw new DMException("推荐人级别不够！");
                        //}

                        RecomRelation rr = new RecomRelation();
                        rr.ID = Guid.NewGuid();
                        rr.DealerId = userid;
                        rr.PDealerId = PCDealer.ID;

                        RecomRelation frr = cc.RecomRelation.Where(w => w.DealerId == PCDealer.ID).FirstOrDefault();
                        if (frr != null)
                        {
                            rr.Layer = frr.Layer + 1;
                        }
                        else
                        {
                            rr.Layer = 2;
                        }
                        cc.RecomRelation.Add(rr);

                        model.UserCode = c.Database.SqlQuery<string>("   Exec    Get_RoundNum 'FX' ").FirstOrDefault();//获取用户编号


                    }
                    else
                    {
                        model.UserCode = c.Database.SqlQuery<string>("   Exec    Get_RoundNum 'YK' ").FirstOrDefault();//获取用户编号
                    }
                    //else
                    //{
                    //    throw new DMException("推荐人不能为空！");
                    //}
                    //}
                    #endregion

                    #region 安置处理
                    if (AdminUserID != null)
                    {
                        if (model.DeLevelID == 4)
                        {
                            if (model.PPDealerCode == "" || model.PPDealerCode == null || model.DeptSelect == "" || model.DeptSelect == null)
                            {
                                throw new DMException("安置信息不全！");
                            }
                        }
                        if (!string.IsNullOrEmpty(model.PPDealerCode) && !string.IsNullOrEmpty(model.DeptSelect))
                        {
                            UserXiahua uxh = cc.Database.SqlQuery<UserXiahua>(" Exec Pro_Get_RegCust_ManageNode @p0,@p1", model.PCDealerCode, model.DeptSelect).FirstOrDefault();
                            //if (cc.PlaceRelation.Where(w => w.DealerId == user.ID).FirstOrDefault() != null)
                            //{
                            //    throw new DMException("安置关系已经存在！");
                            //}
                            User PPDealer = c.User.Where(w => w.UserCode == uxh.ManageDealerId).FirstOrDefault();
                            if (PPDealer.UserState == 0)
                            {
                                throw new DMException("安置人未激活！");
                            }
                            //if (cc.PlaceRelation.Where(w => w.PDealerId == PPDealer.ID && w.AreaName == "A") == null && order.DeptName == "B")
                            //{
                            //    throw new DMException("安置A区为空，请选择A区！");
                            //}

                            //if (cc.PlaceRelation.Where(w => w.PDealerId == PPDealer.ID && w.AreaName == order.DeptName) != null)
                            //{
                            //    throw new DMException("安置节点区域已被占用，请重新选择区域！");
                            //}

                            //if (cc.PlaceRelation.Where(w => w.PDealerId == PPDealer.ID).Count() >= 2)
                            //{
                            //    throw new DMException("安置节点已满，请重新确认安置关系！");
                            //}

                            PlaceRelation rr = new PlaceRelation();
                            rr.ID = Guid.NewGuid();
                            rr.DealerId = userid;
                            rr.PDealerId = PPDealer.ID;

                            PlaceRelation frr = cc.PlaceRelation.Where(w => w.DealerId == PPDealer.ID).FirstOrDefault();
                            if (frr != null)
                            {
                                rr.Layer = frr.Layer + 1;
                            }
                            else
                            {
                                rr.Layer = 2;
                            }
                            rr.AreaName = uxh.AreaName;
                            cc.PlaceRelation.Add(rr);
                        }
                        //else
                        //{
                        //    throw new DMException("安置关系不能为空！");
                        //}
                    }


                    #endregion

                    model.IsVip = false;
                    if (AdminUserID != null)
                    {
                        AdminUser au = c.AdminUser.Where(w => w.ID == AdminUserID).FirstOrDefault();
                        AdminUserOperationLog auol = new AdminUserOperationLog();
                        auol.ID = Guid.NewGuid();
                        auol.AddTime = DateTime.Now;
                        auol.AdminUserCode = au.UserCode;
                        auol.AdminUserID = au.ID;
                        auol.MoneyProduct = null;
                        auol.MoneyTransport = null;
                        auol.OrderMoney = null;
                        auol.NewLevel = null;
                        auol.OldLevel = null;
                        auol.OperationType = 1;
                        auol.UserCode = model.UserCode;
                        auol.OrderNumber = null;
                        auol.OrderTypeName = null;
                        auol.PCDealerCode = model.PCDealerCode;
                        auol.PPDealerCode = model.PPDealerCode;
                        auol.DeptName = model.DeptSelect;
                        c.AdminUserOperationLog.Add(auol);
                        if (model.DeLevelID == 4)
                        {
                            model.IsVip = true;
                        }
                    }

                    //if (c.User.Where(w => w.Phone == model.Phone).FirstOrDefault() == null)
                    //{
                    if (model.OpenID != null && model.OpenID != "")
                    {
                        if (c.User.Where(w => w.OpenID == model.OpenID).FirstOrDefault() == null)
                        {
                            Register(model, c, userid);
                        }
                        else
                        {
                            throw new DMException("该微信号已经注册，请重新确认！");
                        }
                    }
                    else
                    {
                        Register(model, c, userid);
                    }
                    //}
                    //else
                    //{
                    //    throw new DMException("该手机已经注册！");
                    //}


                    c.SaveChanges();

                    cc.SaveChanges();
                    return model.UserCode;
                }
            }
        }

        public bool Bonus_Cash(Guid id, decimal money, string payPwd, decimal sxfbl)
        {
            try
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    if (string.IsNullOrEmpty(payPwd))
                    {
                        throw new DMException("请输入密码！");
                    }
                    string pp = GetPwd(payPwd);
                    User user = c.User.Where(w => w.ID == id && w.PayPwd == pp).FirstOrDefault();
                    if (user == null)
                    {
                        throw new DMException("密码错误！");
                    }
                    if (user.BonusState == 0)
                    {
                        throw new DMException("奖金已锁定！");
                    }
                    ExChange ec = new ExChange();
                    ec.ID = Guid.NewGuid();
                    ec.UserID = user.ID;
                    ec.UserCode = user.UserCode;
                    ec.UserName = user.UserName;
                    ec.ChangeTime = DateTime.Now;
                    ec.ChangeMarks = "积分提现转出！";
                    ec.OrderNum = "";
                    ec.MoneyType = 1;
                    ec.ZMoneyType = 17;
                    ec.BeforeChangeMoney = user.EleMoney;
                    ec.ChangeMoney = 0 - money;
                    ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                    ec.State = 1;
                    ec.FeeRat = sxfbl;
                    ec.FeeAmount = Math.Round(sxfbl * money);
                    ec.CashAmount = money - (ec.FeeAmount);
                    ec.ExChangeType = 2;
                    c.ExChange.Add(ec);


                    user.EleMoney = user.EleMoney - money;
                    if (user.EleMoney < 0)
                    {
                        throw new DMException("用户积分不足！");

                    }
                    else
                    {
                        c.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new DMException(ex.Message);
            }
        }
        public List<DealerRelationDTO> GetGet_DealerRecomBranch(Guid userid)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    User user = c.User.Where(w => w.ID == userid).FirstOrDefault();

                    return cc.Database.SqlQuery<DealerRelationDTO>("   Exec    Get_DealerRecomBranch @p0 ", user.UserCode).ToList();
                }
            }
        }
        public List<Pro_Get_RecomCase_QueryDTO> GetPro_Get_RecomCase_Query(DateTime? time1, DateTime? time2, string code)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    return cc.Database.SqlQuery<Pro_Get_RecomCase_QueryDTO>("   Exec    Pro_Get_RecomCase_Query @p0,@p3,@p2 ", time1, time2, code).ToList();
                }
            }
        }
        public UserBak GetUserBak(Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User user = c.User.Where(w => w.ID == userid).FirstOrDefault();

                UserBak dto = new UserBak();
                dto.BankNumber = user.BankNumber;
                dto.BakAddress = user.BakAddress;
                dto.BakName = user.BakName;
                dto.BakBranchAddress = user.BakBranchAddress;

                return dto;
            }
        }

        public UserMoney IfPayPwd(Guid id, string payPwd)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string pp = GetPwd(payPwd);

                User user = c.User.Where(w => w.ID == id && w.PayPwd == pp).FirstOrDefault();
                if (user == null)
                {
                    throw new DMException("密码错误！");
                }
                UserMoney dto = new UserMoney();
                dto.BonusMoney = user.BonusMoney;
                dto.BonusState = user.BonusState;
                dto.EleMoney = user.EleMoney;
                return dto;
            }
        }

        public int PayType(Guid id, decimal orderMoney)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User user = c.User.Where(w => w.ID == id).FirstOrDefault();
                if (user == null)
                {
                    throw new DMException("用户不存在！");
                }
                if (user.EleMoney <= 0)
                {
                    return -1;
                }
                if (user.EleMoney >= orderMoney)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private static string GetPwd(string payPwd)
        {
            var sha512 = System.Security.Cryptography.SHA512.Create();
            byte[] pwa2 = sha512.ComputeHash(Encoding.Default.GetBytes(payPwd));
            string pp = Encoding.UTF8.GetString(pwa2);
            return pp;
        }

        public bool AddUserBak(UserBak dto, Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User user = c.User.Where(w => w.ID == id).FirstOrDefault();

                user.BakAddress = dto.BakAddress;
                user.BakName = dto.BakName;
                user.BankNumber = dto.BankNumber;
                user.BakBranchAddress = dto.BakBranchAddress;

                c.SaveChanges();
                return true;
            }
        }
        public bool IsServiceCenter(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ServiceCenter sc = c.ServiceCenter.Where(w => w.UserID == id && w.AuditResult == 1).FirstOrDefault();
                if (sc != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public List<MessageTypeDTO> GetMessageTypeList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<MessageType> li = c.MessageType.ToList();

                return MessageTypeDTO.GetDTOList<MessageType, MessageTypeDTO>(li);
            }
        }

        public List<ExChangeDTO> GetCash(Guid id, int PageIndex, int PageSize, out int count)
        {
            List<ExChangeDTO> dtoli = new List<ExChangeDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                //Expression<Func<ExChange, bool>> expr = AutoAssemble.Splice<ExChange, Request_ExChangeDTO>(dto);
                Expression<Func<ExChange, bool>> expr = n => true;
                expr = expr.And2(w => w.UserID == id);
                expr = expr.And2(w => w.ExChangeType == 2);


                count = c.ExChange.Where(expr).Count();
                List<ExChange> li = c.ExChange.Where(expr).OrderByDescending(px => px.ChangeTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                dtoli = GetMapperDTO.GetDTOList<ExChange, ExChangeDTO>(li);
            }

            return dtoli;
        }

        public List<UserDTO> GetUserPlaceList(Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    Expression<Func<User, bool>> expr = n => true;

                    List<PlaceRelation> list = cc.PlaceRelation.Where(w => w.PDealerId == userid).ToList();

                    List<UserDTO> dtoList = new List<UserDTO>();
                    foreach (PlaceRelation item in list)
                    {
                        UserDTO dto = new UserDTO();
                        User user = c.User.Where(w => w.ID == item.DealerId).FirstOrDefault();

                        if (user != null)
                        {
                            dto.ID = user.ID;
                            dto.AddTime = user.AddTime;
                            dto.Phone = user.Phone;
                            dto.Sex = user.Sex;
                            dto.UserName = user.UserName;
                            dto.UserCode = user.UserCode;
                            dto.DeLevelID = user.DeLevelID;
                            dto.UserState = user.UserState;
                            dto.DeLevelName = user.DeLevel.Name;
                            dto.DeptSelect = item.AreaName;
                            if (user.HonLevelID != null)
                            {
                                dto.DeLevelName = user.HonLevel.Name;
                            }

                            dtoList.Add(dto);
                        }
                    }

                    return dtoList;
                }
            }
        }

        public List<UserDTO> GetUserRecomList(Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    Expression<Func<User, bool>> expr = n => true;

                    List<RecomRelation> list = cc.RecomRelation.Where(w => w.PDealerId == userid).ToList();

                    List<UserDTO> dtoList = new List<UserDTO>();
                    foreach (RecomRelation item in list)
                    {
                        UserDTO dto = new UserDTO();
                        User user = c.User.Where(w => w.ID == item.DealerId && w.UserState == 1).FirstOrDefault();
                        if (user != null)
                        {
                            dto.ID = user.ID;
                            dto.AddTime = user.AddTime;
                            dto.Phone = user.Phone;
                            dto.Sex = user.Sex;
                            dto.UserName = user.UserName;
                            dto.UserCode = user.UserCode;
                            dto.DeLevelID = user.DeLevelID;
                            dto.UserState = user.UserState;
                            dto.DeLevelName = user.DeLevel.Name;
                            dto.DeptSelect = "";
                            if (user.HonLevelID != null)
                            {
                                dto.DeLevelName = user.HonLevel.Name;
                            }

                            PlaceRelation pr = new PlaceRelation();
                            Guid? id = user.ID;
                            do
                            {
                                pr = cc.PlaceRelation.Where(w => w.DealerId == id).FirstOrDefault();
                                if (pr != null)
                                {
                                    if (pr.PDealerId == userid)
                                    {
                                        dto.DeptSelect = pr.AreaName;
                                        break;
                                    }
                                    else
                                    {
                                        if (pr.PDealerId != null)
                                        {
                                            id = pr.PDealerId;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            } while (true);



                            dtoList.Add(dto);
                        }

                    }

                    return dtoList;
                }
            }
        }

        public List<MessagesDTO> GetMessageList(string usercode, int PageIndex, int PageSize, out int count)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Expression<Func<Messages, bool>> expr = n => true;
                if (usercode != null && usercode != "")
                {
                    expr = expr.And2(w => w.User.UserCode == usercode);
                }
                expr = expr.And2(w => w.MsgLevel == 1);

                count = c.Messages.Where(expr).Count();
                List<MessagesDTO> dto = c.Messages.Where(expr).OrderByDescending(ob => ob.AddTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(
                    x => new MessagesDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        DataType = x.DataType,
                        MsgLevel = x.MsgLevel,
                        MessageContent = x.MessageContent,
                        ReplyContent = x.ReplyContent,
                        AdminUserCode = x.AdminUser.UserCode,
                        TypeName = x.MessageType.TypeName,
                        Title = x.Title,
                        MessageState = x.MessageState,
                        UserCode = x.User.UserCode,
                        UserID = x.User.ID
                    }
                ).ToList();

                return dto;
            }
        }
        public List<MessagesDTO> GetMessageList(Guid userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<MessagesDTO> dto = c.Messages.Where(w => w.UserID == userid && w.MsgLevel == 1).OrderByDescending(ob => ob.AddTime).Select(
                    x => new MessagesDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        DataType = x.DataType,
                        MsgLevel = x.MsgLevel,
                        MessageContent = x.MessageContent,
                        ReplyContent = x.ReplyContent,
                        AdminUserCode = x.AdminUser.UserCode,
                        TypeName = x.MessageType.TypeName,
                        Title = x.Title,
                        MessageState = x.MessageState,
                        UserCode = x.User.UserCode,
                        UserID = x.User.ID
                    }
                ).ToList();

                return dto;
            }
        }
        public List<MessagesDTO> GetMessageList2(Guid? messagesId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<MessagesDTO> dto = c.Messages.Where(w => w.PID == messagesId).OrderBy(ob => ob.AddTime).Select(
                    x => new MessagesDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        DataType = x.DataType,
                        MsgLevel = x.MsgLevel,
                        MessageContent = x.MessageContent,
                        ReplyContent = x.ReplyContent,
                        AdminUserCode = x.AdminUser.UserCode,
                        TypeName = x.MessageType.TypeName,
                        UserCode = x.User.UserCode,
                        UserID = x.User.ID
                    }
                ).ToList();

                return dto;
            }
        }

        public bool AddMessages(Guid userid, Guid messageTypeID, string messageContent, string title)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Messages model = new Messages();
                model.ID = Guid.NewGuid();
                model.UserID = userid;
                model.AddTime = DateTime.Now;
                model.MessageTypeID = messageTypeID;
                model.MessageContent = messageContent;
                model.DataType = 1;
                model.MsgLevel = 1;
                model.Title = title;
                model.MessageState = 0;
                c.Messages.Add(model);

                c.SaveChanges();
                return true;
            }
        }
        public bool AddMessages2(Guid userid, Guid messageID, string replyContent)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Messages pmodel = c.Messages.Where(w => w.ID == messageID).FirstOrDefault();
                pmodel.MessageState = 0;

                Messages model = new Messages();
                model.ID = Guid.NewGuid();
                model.MessageTypeID = pmodel.MessageTypeID;
                model.UserID = userid;
                model.AddTime = DateTime.Now;
                model.ReplyContent = replyContent;
                model.DataType = 1;
                model.MsgLevel = 2;
                model.PID = messageID;
                c.Messages.Add(model);

                c.SaveChanges();
                return true;
            }
        }

        public bool AddMessages3(Guid userid, Guid messageID, string replyContent)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Messages pmodel = c.Messages.Where(w => w.ID == messageID).FirstOrDefault();
                pmodel.MessageState = 1;

                Messages model = new Messages();
                model.ID = Guid.NewGuid();
                model.MessageTypeID = pmodel.MessageTypeID;
                model.AdminID = userid;
                model.AddTime = DateTime.Now;
                model.ReplyContent = replyContent;
                model.DataType = 2;
                model.MsgLevel = 2;
                model.PID = messageID;
                c.Messages.Add(model);

                c.SaveChanges();
                return true;
            }
        }
        public bool MessageClose(Guid ID)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Messages pmodel = c.Messages.Where(w => w.ID == ID).FirstOrDefault();
                pmodel.MessageState = 2;

                c.SaveChanges();
                return true;
            }
        }
        public bool Bonus_Ele(Guid id, decimal money, string PayPwd)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string pp = GetPwd(PayPwd);
                User user = c.User.Where(w => w.ID == id && w.PayPwd == pp).FirstOrDefault();
                if (user == null)
                {
                    throw new DMException("密码错误！");
                }
                if (user.BonusState == 0)
                {
                    throw new DMException("奖金已锁定！");
                }

                ExChange ec = new ExChange();
                ec.ID = Guid.NewGuid();
                ec.UserID = user.ID;
                ec.UserCode = user.UserCode;
                ec.UserName = user.UserName;
                ec.ChangeTime = DateTime.Now;
                ec.ChangeMarks = "奖励积分转积分转出！";
                ec.OrderNum = "";
                ec.MoneyType = 4;
                ec.ZMoneyType = 15;
                ec.BeforeChangeMoney = user.BonusMoney;
                ec.ChangeMoney = 0 - money;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                ec.State = 0;
                ec.ExChangeType = 5;
                c.ExChange.Add(ec);

                ExChange ecc = new ExChange();
                ecc.ID = Guid.NewGuid();
                ecc.UserID = user.ID;
                ecc.UserCode = user.UserCode;
                ecc.UserName = user.UserName;
                ecc.ChangeTime = DateTime.Now;
                ecc.ChangeMarks = "积分由奖励积分转入！";
                ecc.OrderNum = "";
                ecc.MoneyType = 1;
                ecc.ZMoneyType = 2;
                ecc.BeforeChangeMoney = user.EleMoney;
                ecc.ChangeMoney = money;
                ecc.AfterChangeMoney = ecc.BeforeChangeMoney + ecc.ChangeMoney;
                ecc.State = 0;
                ec.ExChangeType = 5;
                c.ExChange.Add(ecc);


                user.BonusMoney = user.BonusMoney - money;
                user.EleMoney = user.EleMoney + money;
                if (user.BonusMoney < 0)
                {
                    throw new DMException("用户奖励积分不足！");

                }
                else
                {
                    c.SaveChanges();
                }


                return true;
            }
        }

        public bool TransferMoney(Guid id, decimal money, string userCode, string PayPwd, int maxNum = 0)
        {
            if (maxNum > 3)
            {
                throw new DMException("操作失败，请稍后再试！");
            }
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (var tran = c.Database.BeginTransaction())
                {
                    try
                    {
                        #region 条件验证
                        if (money < 0)
                        {
                            throw new DMException("转账金额不规范！");
                        }
                        string pp = GetPwd(PayPwd);
                        User user = c.User.Where(w => w.ID == id && w.PayPwd == pp).FirstOrDefault();
                        if (user == null)
                        {
                            throw new DMException("密码错误！");
                        }
                        User deale = c.User.Where(w => w.UserCode == userCode).FirstOrDefault();
                        DeLevel de = c.DeLevel.Where(w => w.ID == 4).FirstOrDefault();
                        if (deale.UserState != 1)
                        {
                            throw new DMException("转账的用户无效！");
                        }

                        if (user.BonusState == 0)
                        {
                            throw new DMException("奖金已锁定！");
                        }
                        if (deale.BonusState == 0)
                        {
                            throw new DMException("被转账的用户奖金已锁定！");
                        }
                        if (user.EleMoney - money < 0)
                        {
                            throw new DMException("用户积分不足！");
                        }
                        if (deale.DeLevelID < 4)
                        {
                            throw new DMException("只能转账给" + de.Name + "用户！");
                        }
                        if (user.DeLevelID < 4)
                        {
                            throw new DMException("非" + de.Name + "用户不能转账！");
                        }
                        #endregion

                        #region 日志
                        ExChange ec = new ExChange();
                        ec.ID = Guid.NewGuid();
                        ec.UserID = user.ID;
                        ec.UserCode = user.UserCode;
                        ec.UserName = user.UserName;
                        ec.ChangeTime = DateTime.Now;
                        ec.ChangeMarks = "转账给" + deale.UserCode + ":" + deale.UserName + ",转出！";
                        ec.OrderNum = "";
                        ec.MoneyType = 1;
                        ec.ZMoneyType = 6;
                        ec.BeforeChangeMoney = user.EleMoney;
                        ec.ChangeMoney = 0 - money;
                        ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                        ec.State = 0;
                        ec.ExChangeType = 4;
                        c.ExChange.Add(ec);

                        ExChange ecc = new ExChange();
                        ecc.ID = Guid.NewGuid();
                        ecc.UserID = deale.ID;
                        ecc.UserCode = deale.UserCode;
                        ecc.UserName = deale.UserName;
                        ecc.ChangeTime = DateTime.Now;
                        ecc.ChangeMarks = user.UserCode + "" + user.UserName + ",转账转入！";
                        ecc.OrderNum = "";
                        ecc.MoneyType = 1;
                        ecc.ZMoneyType = 4;
                        ecc.BeforeChangeMoney = deale.EleMoney;
                        ecc.ChangeMoney = money;
                        ecc.AfterChangeMoney = ecc.BeforeChangeMoney + ecc.ChangeMoney;
                        ecc.State = 0;
                        ec.ExChangeType = 4;
                        c.ExChange.Add(ecc);

                        c.SaveChanges();
                        #endregion

                        //user.EleMoney = user.EleMoney - money;
                        //deale.EleMoney = deale.EleMoney + money;
                        int a = NewMethod(money, c, user);
                        int aa = NewMethod(0 - money, c, deale);
                        if (a == 0 || aa == 0)
                        {
                            tran.Rollback();
                            //暂停1秒钟
                            Thread.Sleep(1000);
                            maxNum++;
                            TransferMoney(id, money, userCode, PayPwd, maxNum);
                        }
                        else
                        {
                            c.SaveChanges();
                            tran.Commit();
                        }
                        return true;
                    }
                    catch (DMException dme)
                    {
                        throw dme;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }

            }
        }

        private int NewMethod(decimal money, BwslRetailEntities c, User user)
        {
            int a0 = c.Database.ExecuteSqlCommand("UPDATE [dbo].[User] SET [VersionNum] = [VersionNum] + 1  WHERE ID = @p0", user.ID);
            int a = c.Database.ExecuteSqlCommand("UPDATE [dbo].[User] SET [EleMoney] = @p0  WHERE ID = @p1 and VersionNum=@p2", user.EleMoney - money, user.ID, user.VersionNum + 1);
            return a;
        }

        private static void Register(UserModel model, BwslRetailEntities c, Guid userid)
        {

            User user = new User();
            user.ID = userid;
            user.AddTime = DateTime.Now;
            //user.UserState = 1;
            user.UserState = model.UserState;

            user.DeLevelID = model.DeLevelID;
            user.UserCode = model.UserCode;
            user.UserName = model.UserName;
            user.Phone = model.Phone;
            user.Sex = model.Sex;
            user.CardCode = model.CardCode;
            user.OpenID = model.OpenID;
            user.Source = model.Source;
            user.EleMoney = 0;
            user.BonusMoney = 0;
            user.BonusState = 1;
            user.ProvinceId = model.ProvinceId;
            user.CityId = model.CityId;
            user.CountyId = model.CountyId;
            user.StageCount = 0;
            user.TPoint_Point = 0;
            user.RPoint_Point = 0;
            user.IsVip = model.IsVip;
            user.WeiXinName = model.WeiXinName;
            user.WeiXinUrl = model.WeiXinUrl;

            user.VersionNum = 0;
            user.EleMoneyFrozen = 0;

            UserPwdCode up = new UserPwdCode();
            up.ID = Guid.NewGuid();
            up.UserID = userid;
            if (model.UserPwd != "" && model.UserPwd != null)
            {
                string pp = GetPwd(model.UserPwd);
                user.PayPwd = pp;
                user.UserPwd = pp;

                up.PayPwd = model.UserPwd;
                up.UserPwd = model.UserPwd;
            }
            else
            {
                string pwd = model.Phone.Substring(5, 6);
                string pp = GetPwd(pwd);
                user.PayPwd = pp;
                user.UserPwd = pp;

                up.PayPwd = pwd;
                up.UserPwd = pwd;
            }


            //user.HonLevelID = model.HonLevelID;
            user.LastIP = "";
            user.LastTime = Convert.ToDateTime("2019-09-16 19:30:01");
            //user.IsBak = model.IsBak;
            c.UserPwdCode.Add(up);
            c.User.Add(user);
        }

        public bool BDUserOpenId(string usercode, string openid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User olduser = c.User.Where(n => n.Unionid == openid).FirstOrDefault();
                if (olduser != null)
                {
                    throw new DMException("该微信已绑定用户！");
                }
                User pt = c.User.FirstOrDefault(n => n.UserCode == usercode);
                if (pt != null)
                {
                    //pt.OpenID = openid;
                    pt.Unionid = openid;
                    c.SaveChanges();
                    return true;
                }
                else
                {
                    throw new DMException("未找到该用户！");
                }

            }
        }
        public bool BDUserOpenIdYZ(string usercode, string userPwd)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string pp = GetPwd(userPwd);

                User pt = c.User.FirstOrDefault(n => n.UserCode == usercode && n.UserPwd == pp);
                if (pt != null)
                {
                    if (pt.OpenID != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    throw new DMException("用户编号或者密码不正确！");
                }

            }
        }

        public UserYeJi GetPro_Get_BranchAch(string pCDealerCode, string deptSelect)
        {

            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserYeJi>("   Exec    Pro_Get_BranchAch @p0,@p1", pCDealerCode, deptSelect).FirstOrDefault();
            }
        }

        public UserXiahua GetPro_Get_RegCust_ManageNode(string pCDealerCode, string deptSelect)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserXiahua>("   Exec    Pro_Get_RegCust_ManageNode @p0,@p1", pCDealerCode, deptSelect).FirstOrDefault();
            }
        }

        public UserIsShanXia GetPro_PP_PCRelation_Check(string pPdealercode, string pCdealercode)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserIsShanXia>("   Exec    Pro_PP_PCRelation_Check @p0,@p1", pPdealercode, pCdealercode).FirstOrDefault();
            }
        }

        public UserABqu GetPro_Get_AreaNameByManageCode(string pCDealerCode, string pPDealerCode)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserABqu>("   Exec   Pro_Get_AreaNameByManageCode @p0,@p1", pCDealerCode, pPDealerCode).FirstOrDefault();
            }
        }

        public UserDianPu GetPro_Get_StoreNo(string userCode)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserDianPu>("   Exec   Pro_Get_StoreNo @p0 ", userCode).FirstOrDefault();
            }
        }


        public List<UserRelation> GetPro_GetPRelation_Dealers(string userCode, string num)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserRelation>("   Exec   Pro_GetPRelation_Dealers @p0,@p1", userCode, num).ToList();
            }
        }

        public List<UserRelation> GetPro_GetRRelation_Dealers(string userCode, string num)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<UserRelation>("   Exec   Pro_GetRRelation_Dealers @p0,@p1", userCode, num).ToList();
            }
        }
        public CKRelationDTO CKRelation(string code1, string code2)
        {
            CKRelationDTO ck = new CKRelationDTO();

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    User user1 = c.User.FirstOrDefault(w => w.UserCode == code1);
                    if (user1 == null)
                    {
                        throw new DMException("编号" + code1 + "不存在！");
                    }
                    User user2 = c.User.FirstOrDefault(w => w.UserCode == code2);
                    if (user2 == null)
                    {
                        throw new DMException("编号" + code2 + "不存在！");
                    }
                    PlaceRelation pr = cc.PlaceRelation.Where(w => w.DealerId == user2.ID).FirstOrDefault();
                    if (pr != null)
                    {
                        if (pr.PDealerId == user1.ID)
                        {
                            ck.PR = true;
                        }
                        else
                        {
                            ck.PR = false;
                        }
                    }
                    else
                    {
                        ck.PR = false;
                    }

                    Guid? id = user2.ID;
                    do
                    {
                        RecomRelation rr = cc.RecomRelation.Where(w => w.DealerId == id).FirstOrDefault();
                        if (rr != null)
                        {
                            if (rr.PDealerId == user1.ID)
                            {
                                ck.RR = true;
                                break;
                            }
                            else
                            {
                                id = rr.PDealerId;
                            }
                        }
                        else
                        {
                            ck.RR = false;
                            break;
                        }
                    } while (true);

                }
            }
            return ck;
        }
        public string AdminUserUpUser(string UserCode, string PCDealerCode, string PPDealerCode, string DeptSelect, int DeLevelID, Guid? AdminUserID)
        {
            try
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                    {
                        if (DeLevelID == 4)
                        {
                            if (PPDealerCode == "" || PPDealerCode == null || DeptSelect == "" || DeptSelect == null)
                            {
                                throw new DMException("安置信息不全！");
                            }
                        }
                        User user = c.User.FirstOrDefault(w => w.UserCode == UserCode);
                        string OldLevel = user.DeLevel.Name;
                        user.DeLevelID = DeLevelID;
                        #region 安置处理
                        if (AdminUserID != null)
                        {
                            if (!string.IsNullOrEmpty(PPDealerCode) && !string.IsNullOrEmpty(DeptSelect))
                            {
                                #region MyRegion
                                //UserXiahua uxh = cc.Database.SqlQuery<UserXiahua>(" Exec Pro_Get_RegCust_ManageNode @p0,@p1", PCDealerCode, DeptSelect).FirstOrDefault();
                                ////if (cc.PlaceRelation.Where(w => w.DealerId == user.ID).FirstOrDefault() != null)
                                ////{
                                ////    throw new DMException("安置关系已经存在！");
                                ////}
                                //User PPDealer = c.User.Where(w => w.UserCode == uxh.ManageDealerId).FirstOrDefault();
                                //if (PPDealer.UserState == 0)
                                //{
                                //    throw new DMException("安置人未激活！");
                                //}
                                ////if (cc.PlaceRelation.Where(w => w.PDealerId == PPDealer.ID && w.AreaName == "A") == null && order.DeptName == "B")
                                ////{
                                ////    throw new DMException("安置A区为空，请选择A区！");
                                ////}

                                ////if (cc.PlaceRelation.Where(w => w.PDealerId == PPDealer.ID && w.AreaName == order.DeptName) != null)
                                ////{
                                ////    throw new DMException("安置节点区域已被占用，请重新选择区域！");
                                ////}

                                ////if (cc.PlaceRelation.Where(w => w.PDealerId == PPDealer.ID).Count() >= 2)
                                ////{
                                ////    throw new DMException("安置节点已满，请重新确认安置关系！");
                                ////}
                                #endregion

                                #region 非下滑

                                UserABqu uxh = cc.Database.SqlQuery<UserABqu>("   Exec   Pro_Get_AreaNameByManageCode @p0,@p1", PCDealerCode, PPDealerCode).FirstOrDefault();
                                if (uxh.Tag == -1)
                                {
                                    throw new DMException(uxh.ErrMessage);
                                }
                                User PPDealer = c.User.Where(w => w.UserCode == PPDealerCode).FirstOrDefault();
                                if (PPDealer.UserState == 0)
                                {
                                    throw new DMException("安置人未激活！");
                                }
                                #endregion

                                PlaceRelation rr = new PlaceRelation();
                                rr.ID = Guid.NewGuid();
                                rr.DealerId = user.ID;
                                rr.PDealerId = PPDealer.ID;

                                PlaceRelation frr = cc.PlaceRelation.Where(w => w.DealerId == PPDealer.ID).FirstOrDefault();
                                if (frr != null)
                                {
                                    rr.Layer = frr.Layer + 1;
                                }
                                else
                                {
                                    rr.Layer = 2;
                                }
                                rr.AreaName = uxh.AreaName;
                                cc.PlaceRelation.Add(rr);
                            }
                        }


                        #endregion

                        if (AdminUserID != null)
                        {
                            AdminUser au = c.AdminUser.Where(w => w.ID == AdminUserID).FirstOrDefault();
                            AdminUserOperationLog auol = new AdminUserOperationLog();
                            auol.ID = Guid.NewGuid();
                            auol.AddTime = DateTime.Now;
                            auol.AdminUserCode = au.UserCode;
                            auol.AdminUserID = au.ID;
                            auol.MoneyProduct = null;
                            auol.MoneyTransport = null;
                            auol.OrderMoney = null;
                            auol.NewLevel = c.DeLevel.FirstOrDefault(w => w.ID == DeLevelID).Name;
                            auol.OldLevel = OldLevel;
                            auol.OperationType = 1;
                            auol.UserCode = UserCode;
                            auol.OrderNumber = null;
                            auol.OrderTypeName = null;
                            auol.PCDealerCode = PCDealerCode;
                            auol.PPDealerCode = PPDealerCode;
                            auol.DeptName = DeptSelect;
                            auol.UserName = user.UserName;
                            c.AdminUserOperationLog.Add(auol);
                            if (user.DeLevelID == 4)
                            {
                                user.IsVip = true;
                            }
                        }

                        c.SaveChanges();

                        cc.SaveChanges();
                        return UserCode;
                    }
                }
            }
            catch (Exception ex) { throw new DMException(ex.Message); }
        }

    }
}
