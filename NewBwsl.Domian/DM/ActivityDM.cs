
using NewMK.DTO.Activity;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DM
{
    public class ActivityDM
    {
        #region 活动操作
        public List<ActivityDTO> GetActivity(out int count)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                count = c.Activity.ToList().Count;
                List<ActivityDTO> userDto = c.Activity.OrderByDescending(ob => ob.StartTime).Select(
                    x => new ActivityDTO
                    {
                        ID = x.ID,
                        ActivityName = x.ActivityName,
                        EndTime = x.EndTime,
                        StartTime = x.StartTime,
                        ActivityExplain = x.ActivityExplain,
                        ActivityForm = x.ActivityForm,
                        ActivityObjectForm = x.ActivityObjectForm,
                        BasicsMoney = x.BasicsMoney,
                        IfEnable = x.IfEnable,
                        DiscountMoney = x.DiscountMoney,
                        IfAll = x.IfAll,
                        BuyNumber = x.BuyNumber,
                        IfDoubleGive = x.IfDoubleGive
                    }
                ).ToList();

                return userDto;
            }
        }


        public bool AddActivity(ActivityTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Activity pt = new Activity();
                pt.ID = Guid.NewGuid();
                pt.ActivityName = dto.ActivityName;
                pt.EndTime = dto.EndTime;
                pt.StartTime = dto.StartTime;
                pt.ActivityExplain = dto.ActivityExplain;
                pt.ActivityForm = dto.ActivityForm;
                pt.ActivityObjectForm = dto.ActivityObjectForm;
                pt.BasicsMoney = dto.BasicsMoney;
                pt.IfEnable = dto.IfEnable;
                pt.IfAll = dto.IfAll;
                pt.DiscountMoney = dto.DiscountMoney;
                pt.BuyNumber = dto.BuyNumber;
                pt.IfDoubleGive = dto.IfDoubleGive;

                c.Activity.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityDTO GetActivityFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDTO userDto = c.Activity.Where(n => n.ID == id).Select(
                    x => new ActivityDTO
                    {
                        ID = x.ID,
                        ActivityName = x.ActivityName,
                        EndTime = x.EndTime,
                        StartTime = x.StartTime,
                        ActivityExplain = x.ActivityExplain,
                        ActivityForm = x.ActivityForm,
                        ActivityObjectForm = x.ActivityObjectForm,
                        BasicsMoney = x.BasicsMoney,
                        IfEnable = x.IfEnable,
                        DiscountMoney = x.DiscountMoney,
                        IfAll = x.IfAll,
                        BuyNumber = x.BuyNumber,
                        IfDoubleGive = x.IfDoubleGive
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivity(ActivityTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Activity pt = c.Activity.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityName = dto.ActivityName;
                pt.EndTime = dto.EndTime;
                pt.StartTime = dto.StartTime;
                pt.ActivityExplain = dto.ActivityExplain;
                pt.ActivityForm = dto.ActivityForm;
                pt.ActivityObjectForm = dto.ActivityObjectForm;
                pt.BasicsMoney = dto.BasicsMoney;
                pt.IfEnable = dto.IfEnable;
                pt.IfAll = dto.IfAll;
                pt.DiscountMoney = dto.DiscountMoney;
                pt.BuyNumber = dto.BuyNumber;
                pt.IfDoubleGive = dto.IfDoubleGive;

                c.SaveChanges();
                return true;
            }
        }



        public bool deActivity(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Activity pt = c.Activity.FirstOrDefault(n => n.ID == id);

                c.Activity.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #region 活动产品操作
        public List<ActivityProductDTO> GetActivityProduct(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityProductDTO> userDto = c.ActivityProduct.Where(w => w.ActivityID == id).Select(
                    x => new ActivityProductDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        NumBase = x.NumBase,
                        NumMax = x.NumMax,
                        NumMaxAll = x.NumMaxAll,
                        ProductID = x.ProductID,
                        ProductCode = x.Product.ProductCode,
                        ProductName = x.Product.ProductName,
                        Discount = x.Discount
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddActivityProduct(ActivityProductTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityProduct pt = new ActivityProduct();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.NumBase = dto.NumBase;
                pt.NumMax = dto.NumMax;
                pt.NumMaxAll = dto.NumMaxAll;
                pt.ProductID = dto.ProductID;
                pt.Discount = dto.Discount;

                c.ActivityProduct.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityProductDTO GetActivityProductFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityProductDTO userDto = c.ActivityProduct.Where(n => n.ID == id).Select(
                    x => new ActivityProductDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        NumBase = x.NumBase,
                        NumMax = x.NumMax,
                        NumMaxAll = x.NumMaxAll,
                        ProductID = x.ProductID,
                        ProductCode = x.Product.ProductCode,
                        ProductName = x.Product.ProductName,
                        Discount = x.Discount
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityProduct(ActivityProductTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityProduct pt = c.ActivityProduct.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.NumBase = dto.NumBase;
                pt.NumMax = dto.NumMax;
                pt.NumMaxAll = dto.NumMaxAll;
                pt.ProductID = dto.ProductID;
                pt.Discount = dto.Discount;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityProduct(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityProduct pt = c.ActivityProduct.FirstOrDefault(n => n.ID == id);

                c.ActivityProduct.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        #endregion

        #region 活动赠品操作

        public List<ActivityGiftsDTO> GetActivityGifts(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityGiftsDTO> userDto = c.ActivityGifts.Where(w => w.ActivityID == id).Select(
                    x => new ActivityGiftsDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        NumBase = x.NumBase,
                        NumMax = x.NumMax,
                        NumMaxAll = x.NumMaxAll,
                        GiftID = x.GiftID,
                        ProductCode = x.Product.ProductCode,
                        ProductName = x.Product.ProductName,
                        MoneyMin = x.MoneyMin,
                        MoneyMax = x.MoneyMax
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddActivityGifts(ActivityGiftsTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityGifts pt = new ActivityGifts();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.NumBase = dto.NumBase;
                pt.NumMax = dto.NumMax;
                pt.NumMaxAll = dto.NumMaxAll;
                pt.GiftID = dto.GiftID;
                pt.MoneyMin = dto.MoneyMin;
                pt.MoneyMax = dto.MoneyMax;

                c.ActivityGifts.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityGiftsDTO GetActivityGiftsFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityGiftsDTO userDto = c.ActivityGifts.Where(n => n.ID == id).Select(
                    x => new ActivityGiftsDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        NumBase = x.NumBase,
                        NumMax = x.NumMax,
                        NumMaxAll = x.NumMaxAll,
                        GiftID = x.GiftID,
                        ProductCode = x.Product.ProductCode,
                        ProductName = x.Product.ProductName,
                        MoneyMin = x.MoneyMin,
                        MoneyMax = x.MoneyMax
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityGifts(ActivityGiftsTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityGifts pt = c.ActivityGifts.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.NumBase = dto.NumBase;
                pt.NumMax = dto.NumMax;
                pt.NumMaxAll = dto.NumMaxAll;
                pt.GiftID = dto.GiftID;
                pt.MoneyMin = dto.MoneyMin;
                pt.MoneyMax = dto.MoneyMax;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityGifts(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityGifts pt = c.ActivityGifts.FirstOrDefault(n => n.ID == id);

                c.ActivityGifts.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #region 折上折类型2


        #region 折上折主数据
        public List<ActivityDiscountTwoDTO> GetActivityDiscountTwo(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityDiscountTwoDTO> userDto = c.ActivityDiscountTwo.Where(w => w.ActivityID == id).Select(
                    x => new ActivityDiscountTwoDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        ProductID = x.ProductID,
                        ProductCode = x.Product.ProductCode,
                        ProductName = x.Product.ProductName
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddActivityDiscountTwo(ActivityDiscountTwoTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountTwo pt = new ActivityDiscountTwo();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.ProductID = dto.ProductID;

                c.ActivityDiscountTwo.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityDiscountTwoDTO GetActivityDiscountTwoFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountTwoDTO userDto = c.ActivityDiscountTwo.Where(n => n.ID == id).Select(
                    x => new ActivityDiscountTwoDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        ProductID = x.ProductID,
                        ProductCode = x.Product.ProductCode,
                        ProductName = x.Product.ProductName
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityDiscountTwo(ActivityDiscountTwoTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountTwo pt = c.ActivityDiscountTwo.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.ProductID = dto.ProductID;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityDiscountTwo(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountTwo pt = c.ActivityDiscountTwo.FirstOrDefault(n => n.ID == id);

                c.ActivityDiscountTwo.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #region 折上折数值
        public List<DiscountTwoInfoDTO> GetDiscountTwoInfo(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<DiscountTwoInfoDTO> userDto = c.DiscountTwoInfo.Where(w => w.DiscountTwoID == id).Select(
                    x => new DiscountTwoInfoDTO
                    {
                        ID = x.ID,
                        Discount = x.Discount,
                        DiscountTwoID = x.DiscountTwoID,
                        Num = x.Num
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddDiscountTwoInfo(DiscountTwoInfoTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                DiscountTwoInfo pt = new DiscountTwoInfo();
                pt.ID = Guid.NewGuid();
                pt.DiscountTwoID = dto.DiscountTwoID;
                pt.Num = dto.Num;
                pt.Discount = dto.Discount;

                c.DiscountTwoInfo.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public DiscountTwoInfoDTO GetDiscountTwoInfoFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                DiscountTwoInfoDTO userDto = c.DiscountTwoInfo.Where(n => n.ID == id).Select(
                    x => new DiscountTwoInfoDTO
                    {
                        ID = x.ID,
                        Discount = x.Discount,
                        DiscountTwoID = x.DiscountTwoID,
                        Num = x.Num
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpDiscountTwoInfo(DiscountTwoInfoTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                DiscountTwoInfo pt = c.DiscountTwoInfo.FirstOrDefault(n => n.ID == dto.ID);

                pt.DiscountTwoID = dto.DiscountTwoID;
                pt.Num = dto.Num;
                pt.Discount = dto.Discount;

                c.SaveChanges();
                return true;
            }
        }

        public bool deDiscountTwoInfo(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                DiscountTwoInfo pt = c.DiscountTwoInfo.FirstOrDefault(n => n.ID == id);

                c.DiscountTwoInfo.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }


        #endregion


        #endregion

        #region 折上折类型1
        public List<ActivityDiscountOneInfoDTO> GetActivityDiscountOneInfo(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityDiscountOneInfoDTO> userDto = c.ActivityDiscountOneInfo.Where(w => w.ActivityID == id).Select(
                    x => new ActivityDiscountOneInfoDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        Discount = x.Discount,
                        Num = x.Num
                    }
                ).ToList();

                return userDto;
            }
        }
        public bool AddActivityDiscountOneInfo(ActivityDiscountOneInfoTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountOneInfo pt = new ActivityDiscountOneInfo();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.Num = dto.Num;
                pt.Discount = dto.Discount;

                c.ActivityDiscountOneInfo.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityDiscountOneInfoDTO GetActivityDiscountOneInfoFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountOneInfoDTO userDto = c.ActivityDiscountOneInfo.Where(n => n.ID == id).Select(
                    x => new ActivityDiscountOneInfoDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        Discount = x.Discount,
                        Num = x.Num
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityDiscountOneInfo(ActivityDiscountOneInfoTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountOneInfo pt = c.ActivityDiscountOneInfo.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.Num = dto.Num;
                pt.Discount = dto.Discount;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityDiscountOneInfo(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityDiscountOneInfo pt = c.ActivityDiscountOneInfo.FirstOrDefault(n => n.ID == id);

                c.ActivityDiscountOneInfo.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }


        #endregion

        #region 活动用户列表
        public List<ActivityUserDTO> GetActivityUser(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityUserDTO> userDto = c.ActivityUser.Where(w => w.ActivityID == id).Select(
                    x => new ActivityUserDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        UserID = x.UserID,
                        Phone = x.User.Phone,
                        UserName = x.User.UserName,
                        DeLevelID = x.User.DeLevelID
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddActivityUser(ActivityUserTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUser pt = new ActivityUser();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.UserID = dto.UserID;

                c.ActivityUser.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityUserDTO GetActivityUserFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUserDTO userDto = c.ActivityUser.Where(n => n.ID == id).Select(
                    x => new ActivityUserDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        UserID = x.UserID,
                        Phone = x.User.Phone,
                        UserName = x.User.UserName,
                        DeLevelID = x.User.DeLevelID
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityUser(ActivityUserTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUser pt = c.ActivityUser.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.UserID = dto.UserID;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityUser(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUser pt = c.ActivityUser.FirstOrDefault(n => n.ID == id);

                c.ActivityUser.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        #endregion

        #region 活动用户类型列表

        public List<ActivityUserTypeDTO> GetActivityUserType(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityUserTypeDTO> userDto = c.ActivityUserType.Where(w => w.ActivityID == id).Select(
                    x => new ActivityUserTypeDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        DeLevelID = x.DeLevelID,
                        DeLevelName = c.DeLevel.Where(ww => ww.ID == x.DeLevelID).FirstOrDefault().Name
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddActivityUserType(ActivityUserTypeTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUserType pt = new ActivityUserType();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.DeLevelID = dto.DeLevelID;

                c.ActivityUserType.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityUserTypeDTO GetActivityUserTypeFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUserTypeDTO userDto = c.ActivityUserType.Where(n => n.ID == id).Select(
                    x => new ActivityUserTypeDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        DeLevelID = x.DeLevelID,
                        DeLevelName = c.DeLevel.Where(ww => ww.ID == x.DeLevelID).FirstOrDefault().Name
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityUserType(ActivityUserTypeTT dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUserType pt = c.ActivityUserType.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.DeLevelID = dto.DeLevelID;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityUserType(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityUserType pt = c.ActivityUserType.FirstOrDefault(n => n.ID == id);

                c.ActivityUserType.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        #endregion

        #region 活动订单类型列表

        public List<ActivityOrderTypeDTO> GetActivityOrderType(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ActivityOrderTypeDTO> userDto = c.ActivityOrderType.Where(w => w.ActivityID == id).Select(
                    x => new ActivityOrderTypeDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        OrderTypeID = x.OrderTypeID,
                        OrderTypeName = x.OrderType.OrderTypeName
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddActivityOrderType(ActivityOrderTypeModel dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityOrderType pt = new ActivityOrderType();
                pt.ID = Guid.NewGuid();
                pt.ActivityID = dto.ActivityID;
                pt.OrderTypeID = dto.OrderTypeID;

                c.ActivityOrderType.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ActivityOrderTypeDTO GetActivityOrderTypeFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityOrderTypeDTO userDto = c.ActivityOrderType.Where(n => n.ID == id).Select(
                    x => new ActivityOrderTypeDTO
                    {
                        ID = x.ID,
                        ActivityID = x.ActivityID,
                        OrderTypeID = x.OrderTypeID,
                        OrderTypeName = x.OrderType.OrderTypeName
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpActivityOrderType(ActivityOrderTypeModel dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityOrderType pt = c.ActivityOrderType.FirstOrDefault(n => n.ID == dto.ID);

                pt.ActivityID = dto.ActivityID;
                pt.OrderTypeID = dto.OrderTypeID;

                c.SaveChanges();
                return true;
            }
        }

        public bool deActivityOrderType(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ActivityOrderType pt = c.ActivityOrderType.FirstOrDefault(n => n.ID == id);

                c.ActivityOrderType.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        #endregion
    }
}
