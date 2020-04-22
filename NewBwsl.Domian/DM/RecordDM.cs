using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewMK.DTO.Record;
using System.Linq.Expressions;
using NewMK.Model.CM;
using NewMK.DTO;
using NewMK.Domian.DomainException;

namespace NewMK.Domian.DM
{
    public class RecordDM
    {
        public List<ExChangeDTO> GetExChangeList(Request_ExChangeDTO dto, out int count)
        {
            List<ExChangeDTO> dtoli = new List<ExChangeDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                Expression<Func<ExChange, bool>> expr = AutoAssemble.Splice<ExChange, Request_ExChangeDTO>(dto);
                if (dto.stime != null)
                {
                    expr = expr.And2(w => w.ChangeTime >= dto.stime);
                }
                if (dto.etime != null)
                {
                    expr = expr.And2(w => w.ChangeTime <= dto.etime);
                }
                if (dto.ExChangeType != null)
                {
                    expr = expr.And2(w => w.ExChangeType == dto.ExChangeType);
                }

                count = c.ExChange.Where(expr).Count();

                List<ExChange> li = c.ExChange.Where(expr).OrderByDescending(px => px.ChangeTime).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();

                dtoli = GetMapperDTO.GetDTOList<ExChange, ExChangeDTO>(li);
            }

            return dtoli;
        }

        public List<ExChangeDTO> GetExChangeListDownload(Request_ExChangeDTO dto)
        {
            List<ExChangeDTO> dtoli = new List<ExChangeDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                Expression<Func<ExChange, bool>> expr = AutoAssemble.Splice<ExChange, Request_ExChangeDTO>(dto);
                if (dto.stime != null)
                {
                    expr = expr.And2(w => w.ChangeTime >= dto.stime);
                }
                if (dto.etime != null)
                {
                    expr = expr.And2(w => w.ChangeTime <= dto.etime);
                }


                List<ExChange> li = c.ExChange.Where(expr).OrderByDescending(px => px.ChangeTime).ToList();

                dtoli = GetMapperDTO.GetDTOList<ExChange, ExChangeDTO>(li);
            }

            return dtoli;
        }

        public List<ExChangeBonus_CashDTO> GetExChangeListBonus_Cash(Request_ExChangeBonus_CashDTO dto, out int count)
        {
            List<ExChangeBonus_CashDTO> dtoli = new List<ExChangeBonus_CashDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                Expression<Func<ExChange, bool>> expr = AutoAssemble.Splice<ExChange, Request_ExChangeBonus_CashDTO>(dto);
                expr = expr.And2(w => w.ExChangeType == 2);
                if (dto.stime != null)
                {
                    expr = expr.And2(w => w.ChangeTime >= dto.stime);
                }
                if (dto.etime != null)
                {
                    expr = expr.And2(w => w.ChangeTime <= dto.etime);
                }
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }

                count = c.ExChange.Where(expr).Count();

                dtoli = c.ExChange.Where(expr).OrderByDescending(px => px.User.BakAddress).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).Select(x => new ExChangeBonus_CashDTO
                {
                    AfterChangeMoney = x.AfterChangeMoney,
                    BeforeChangeMoney = x.BeforeChangeMoney,
                    BakAddress = x.User.BakAddress,
                    BakName = x.User.BakName,
                    BankNumber = x.User.BankNumber,
                    BakBranchAddress = x.User.BakBranchAddress,
                    ChangeMarks = x.ChangeMarks,
                    ChangeMoney = x.ChangeMoney,
                    ChangeTime = x.ChangeTime,
                    ID = x.ID,
                    MoneyType = x.MoneyType,
                    OrderNum = x.OrderNum,
                    State = x.State,
                    UserCode = x.UserCode,
                    UserID = x.UserID,
                    UserName = x.UserName,
                    ZMoneyType = x.ZMoneyType,
                    CashAmount = x.CashAmount,
                    FeeAmount = x.FeeAmount,
                    FeeRat = x.FeeRat
                }).ToList();

            }

            return dtoli;
        }

        public List<ExChangeBonus_CashDTO> GetExChangeListBonus_CashDownload(Request_ExChangeBonus_CashDTO dto)
        {
            List<ExChangeBonus_CashDTO> dtoli = new List<ExChangeBonus_CashDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                Expression<Func<ExChange, bool>> expr = AutoAssemble.Splice<ExChange, Request_ExChangeBonus_CashDTO>(dto);
                expr = expr.And2(w => w.ZMoneyType == 14);
                if (dto.stime != null)
                {
                    expr = expr.And2(w => w.ChangeTime >= dto.stime);
                }
                if (dto.etime != null)
                {
                    expr = expr.And2(w => w.ChangeTime <= dto.etime);
                }
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }

                dtoli = c.ExChange.Where(expr).OrderByDescending(px => px.User.BakAddress).Select(x => new ExChangeBonus_CashDTO
                {
                    AfterChangeMoney = x.AfterChangeMoney,
                    BeforeChangeMoney = x.BeforeChangeMoney,
                    BakAddress = x.User.BakAddress,
                    BakName = x.User.BakName,
                    BankNumber = x.User.BankNumber,
                    BakBranchAddress = x.User.BakBranchAddress,
                    ChangeMarks = x.ChangeMarks,
                    ChangeMoney = x.ChangeMoney,
                    ChangeTime = x.ChangeTime,
                    ID = x.ID,
                    MoneyType = x.MoneyType,
                    OrderNum = x.OrderNum,
                    State = x.State,
                    UserCode = x.UserCode,
                    UserID = x.UserID,
                    UserName = x.UserName,
                    ZMoneyType = x.ZMoneyType,
                    CashAmount = x.CashAmount,
                    FeeAmount = x.FeeAmount,
                    FeeRat = x.FeeRat
                }).ToList();

            }

            return dtoli;
        }

        public ExChangeBonus_CashDTO GetExChangeListBonus_CashCount(Request_ExChangeBonus_CashDTO dto)
        {
            List<ExChangeBonus_CashDTO> dtoli = new List<ExChangeBonus_CashDTO>();
            ExChangeBonus_CashDTO Edto = new ExChangeBonus_CashDTO();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                Expression<Func<ExChange, bool>> expr = AutoAssemble.Splice<ExChange, Request_ExChangeBonus_CashDTO>(dto);
                expr = expr.And2(w => w.ExChangeType == 2);
                if (dto.stime != null)
                {
                    expr = expr.And2(w => w.ChangeTime >= dto.stime);
                }
                if (dto.etime != null)
                {
                    expr = expr.And2(w => w.ChangeTime <= dto.etime);
                }
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }

                dtoli = c.ExChange.Where(expr).OrderByDescending(px => px.User.BakAddress).Select(x => new ExChangeBonus_CashDTO
                {
                    ChangeMoney = x.ChangeMoney,
                    FeeAmount = x.FeeAmount,
                    CashAmount = x.CashAmount


                }).ToList();

                Edto.ChangeMoney = dtoli.Sum(s => s.ChangeMoney);
                Edto.FeeAmount = dtoli.Sum(s => s.FeeAmount);
                Edto.CashAmount = dtoli.Sum(s => s.CashAmount);
            }

            return Edto;
        }

        public List<ActivePeriodRecordDTO> GetActivePeriodRecordList(Request_ActivePeriodRecordDTO dto, out int count)
        {
            List<ActivePeriodRecordDTO> dtoli = new List<ActivePeriodRecordDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {


                Expression<Func<ActivePeriodRecord, bool>> expr = AutoAssemble.Splice<ActivePeriodRecord, Request_ActivePeriodRecordDTO>(dto);
                //if (dto.OrderNumber != null)
                //{
                //    expr = expr.And2(w => w.OrderNumber == dto.OrderNumber);
                //}

                count = c.ActivePeriodRecord.Where(expr).Count();
                List<ActivePeriodRecord> li = c.ActivePeriodRecord.Where(expr).OrderByDescending(px => px.AddTime).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();

                dtoli = ActivePeriodRecordDTO.GetDTO(li);
            }

            return dtoli;
        }

        public bool Bonus_Cash_True(IDList dto, int state)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                foreach (Guid item in dto.ID)
                {
                    ExChange ex = c.ExChange.Where(w => w.ID == item).FirstOrDefault();
                    ex.State = state;

                }
                c.SaveChanges();
            }
            return true;
        }
        public bool Bonus_Cash_false(IDList2 dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                foreach (Guid item in dto.ID)
                {
                    ExChange ex = c.ExChange.Where(w => w.ID == item).FirstOrDefault();
                    ex.State = 4;
                    ex.ChangeMarks = dto.ChangeMarks;
                    User user = ex.User;
                    

                    ExChange ec = new ExChange();
                    ec.ID = Guid.NewGuid();
                    ec.UserID = user.ID;
                    ec.UserCode = user.UserCode;
                    ec.UserName = user.UserName;
                    ec.ChangeTime = DateTime.Now;
                    ec.ChangeMarks = "积分提现退回,原因:" + dto.ChangeMarks + "！";
                    ec.OrderNum = "";
                    ec.MoneyType = 1;
                    ec.ZMoneyType = 20;
                    ec.BeforeChangeMoney = user.EleMoney;
                    ec.ChangeMoney = 0 - ex.ChangeMoney;
                    ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                    ec.State = 4;
                    ec.FeeRat = 0;
                    ec.FeeAmount = 0;
                    ec.CashAmount = 0;
                    ec.ExChangeType = 2;
                    user.EleMoney = user.EleMoney - Convert.ToDecimal(ex.ChangeMoney);

                    c.ExChange.Add(ec);
                }
                c.SaveChanges();
            }
            return true;
        }

        public bool RechargeEleMoney(string usercode, decimal money, string ChangeMarks)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                User user = c.User.Where(w => w.UserCode == usercode).FirstOrDefault();
                ExChange ec = new ExChange();
                ec.ID = Guid.NewGuid();
                ec.UserID = user.ID;
                ec.UserCode = user.UserCode;
                ec.UserName = user.UserName;
                ec.ChangeTime = DateTime.Now;
                ec.ChangeMarks = ChangeMarks;
                ec.OrderNum = "";
                ec.State = 0;
                ec.ExChangeType = 1;

                ec.MoneyType = 1;
                ec.ZMoneyType = 1;

                ec.BeforeChangeMoney = user.EleMoney;
                ec.ChangeMoney = money;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

                user.EleMoney = user.EleMoney + money;//处理余额

                c.ExChange.Add(ec);
                c.SaveChanges();
                return true;
            }
        }

        //public bool EleMoneyCZ(string CZCode, decimal mon, string usercode)
        //{
        //    using (BwslRetailEntities c = new BwslRetailEntities())
        //    {
        //        mon = mon / 100;

        //        User user = c.User.Where(w => w.UserCode == usercode).FirstOrDefault();
        //        ExChange ec = new ExChange();
        //        ec.ID = Guid.NewGuid();
        //        ec.UserID = user.ID;
        //        ec.UserCode = user.UserCode;
        //        ec.UserName = user.UserName;
        //        ec.ChangeTime = DateTime.Now;
        //        ec.ChangeMarks = "微信积分充值！";
        //        ec.OrderNum = CZCode;
        //        ec.State = 0;
        //        ec.ExChangeType = 1;

        //        ec.MoneyType = 2;
        //        ec.ZMoneyType = 9;

        //        ec.BeforeChangeMoney = user.EleMoney;
        //        ec.ChangeMoney = mon;
        //        ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

        //        user.EleMoney = user.EleMoney + mon;//处理余额

        //        c.ExChange.Add(ec);
        //        c.SaveChanges();
        //        return true;
        //    }
        //}
        public bool EleMoneyCZ(string CZCode, decimal mon, string usercode, int MoneyType, int ZMoneyType, string ChangeMarks)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                mon = mon / 100;
                if (c.ExChange.Where(w => w.OrderNum == CZCode && w.ZMoneyType == ZMoneyType).FirstOrDefault() != null)
                {
                    throw new DMException("操作重复！");
                }
                User user = c.User.Where(w => w.UserCode == usercode).FirstOrDefault();
                ExChange ec = new ExChange();
                ec.ID = Guid.NewGuid();
                ec.UserID = user.ID;
                ec.UserCode = user.UserCode;
                ec.UserName = user.UserName;
                ec.ChangeTime = DateTime.Now;
                ec.ChangeMarks = ChangeMarks;
                ec.OrderNum = CZCode;
                ec.State = 0;
                ec.ExChangeType = 1;

                ec.MoneyType = MoneyType;
                ec.ZMoneyType = ZMoneyType;

                ec.BeforeChangeMoney = user.EleMoney;
                ec.ChangeMoney = mon;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

                user.EleMoney = user.EleMoney + mon;//处理余额

                c.ExChange.Add(ec);
                c.SaveChanges();
                return true;
            }
        }

        public List<RatioDTO> GetPro_Tj_Award_Order_AllRat_Web(string begin, string end)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                return c.Database.SqlQuery<RatioDTO>("   Exec    Pro_Tj_Award_Order_AllRat_Web @p0,@p1", begin, end).ToList();
            }
        }

        public List<ServiceCenterPrizeInfoDTO> ServicePrizeInfo(int balancenumber, string dealercode)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<ServiceCenterPrizeInfoDTO>("   Exec    Pro_Get_PrizeDetail  @p0,@p1,@p2", dealercode, balancenumber,"店补").ToList();
            }
        }

        public List<RecordPrizeInfoDTO> RecordPrizeInfo(int balancenumber, string dealercode)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<RecordPrizeInfoDTO>("   Exec    Pro_Get_PrizeDetail  @p0,@p1,@p2", dealercode, balancenumber, "零售积分").ToList();
            }
        }
    }
}
