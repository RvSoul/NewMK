
using NewMK.Domian.DomainException;
using NewMK.DTO.Activity;
using NewMK.DTO.Product;
using NewMK.DTO.ShoppingCart;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DM
{
    public class ShoppingCartDM
    {
        /// <summary>
        /// 计算订单产品总价格
        /// </summary>
        /// <param name="ProductList"></param>
        /// <returns></returns>
        public decimal CountProvince(List<ShoppingCartDTO> ProductList)
        {
            decimal zProvince = 0;
            foreach (ShoppingCartDTO item in ProductList)
            {

                zProvince += Convert.ToDecimal(item.ShoppingProductPrice) * Convert.ToDecimal(item.Num);
            }
            return zProvince;
        }

        public ShoppingCartActivityDTO GetShoppingTT(List<ShoppingCartDTO> dto, Guid userid, int OrderTypeID, Guid? adminUserID)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                if (adminUserID == null)
                {
                    //验证加价购数量
                    List<OrderConfig> minP = c.OrderConfig.Where(w => w.PriceMin > 0).ToList();
                    List<OrderConfig> icP = c.OrderConfig.Where(w => w.IfCount == true).ToList();

                    int mPc = 0;//加价购产品数量
                    int iPc = 0;//活动产品数量
                    foreach (ShoppingCartDTO item in dto)
                    {
                        if (item.ShoppingProductType == 1 || item.ShoppingProductType == 3)
                        {
                            foreach (OrderConfig items in minP)
                            {
                                if (items.ProductID == item.ProductID)
                                {
                                    mPc += item.Num;
                                }
                            }
                            foreach (OrderConfig items in icP)
                            {
                                if (items.ProductID == item.ProductID)
                                {
                                    iPc += item.Num;
                                }
                            }
                        }
                    }
                    if (mPc > iPc)
                    {
                        throw new DMException("加价购产品数量不能大于活动产品数量！");
                    }
                }

                User user = c.User.Where(w => w.ID == userid).FirstOrDefault();
                List<ActivityProductPurchasedTT> applist = new List<ActivityProductPurchasedTT>();//已够活动产品信息表
                List<ActivityGiftsPurchasedTT> agplist = new List<ActivityGiftsPurchasedTT>();//已赠活动产品信息表

                List<ShoppingCartDTO> dto2 = new List<ShoppingCartDTO>();//记录产品的赠品
                //完善购物车产品价格,添加赠品
                foreach (ShoppingCartDTO item in dto)
                {
                    Product pt = c.Product.Where(w => w.ID == item.ProductID).FirstOrDefault();
                    //item.ShoppingProductType = pt.ProductNature;
                    if (!pt.State)
                    {
                        throw new DMException($"[{pt.ProductName}]产品已下架，请重新选购！");
                    }

                    if (item.ShoppingProductType == 1 || item.ShoppingProductType == 3)
                    {
                        item.ShoppingProductPrice = pt.Price;
                        item.ShoppingProductPV = pt.PV;
                        if (item.ShoppingProductType == 1)
                        {
                            List<GiftProduct> gplist = c.GiftProduct.Where(w => w.ProductID == item.ProductID && w.DeLevelID.Contains(user.DeLevelID.ToString())).ToList();

                            foreach (GiftProduct gp in gplist)
                            {
                                ShoppingCartDTO sc = new ShoppingCartDTO();
                                sc.ID = Guid.NewGuid();
                                sc.ProductID = gp.GiftID;
                                sc.Num = gp.GiftNum * item.Num;
                                sc.UserID = userid;
                                sc.AddTime = DateTime.Now;
                                sc.ShoppingProductType = 2;
                                sc.ShoppingProductPrice = 0;
                                dto2.Add(sc);
                            }
                        }

                    }
                }
                foreach (ShoppingCartDTO item in dto2)
                {
                    dto.Add(item);
                }

                //获取满足要求的活动列表
                System.Linq.Expressions.Expression<Func<Activity, bool>> expr = n => true;
                expr = expr.And2(w => w.StartTime <= DateTime.Now && w.EndTime >= DateTime.Now && w.IfEnable);
                expr = expr.And2(w => w.ActivityOrderType.Any(a => a.OrderTypeID == OrderTypeID));
                List<Activity> at = c.Set<Activity>().Where(expr).ToList();
                foreach (Activity activity in at)
                {
                    bool usersatisfy = false;
                    //活动对象是用户
                    if (activity.ActivityObjectForm == 1)
                    {
                        if (c.Set<ActivityUser>().Where(w => w.ActivityID == activity.ID && w.UserID == userid).Count() > 0)
                        {
                            usersatisfy = true;
                        }
                    }
                    //活动对象是用户类型
                    if (activity.ActivityObjectForm == 2 && usersatisfy == false)
                    {
                        if (c.Set<ActivityUserType>().Where(w => w.ActivityID == activity.ID && w.DeLevelID == user.DeLevelID).Count() > 0)
                        {
                            usersatisfy = true;
                        }
                    }
                    //该用户满足活动
                    if (usersatisfy)
                    {

                        switch (activity.ActivityForm)
                        {
                            #region 1买赠
                            case 1:
                                if (true)
                                {
                                    List<ActivityProduct> activityProductList = c.ActivityProduct.Where(w => w.ActivityID == activity.ID).ToList();//活动产品
                                    List<ActivityGifts> aglist = c.ActivityGifts.Where(w => w.ActivityID == activity.ID).ToList();//活动赠品

                                    //非全必买
                                    #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动
                                    bool ifall = false;//是否满足活动
                                    int baseNum = 2147483647;//最小基数 
                                    bool ifpt = true;//判断活动产品购买赠送限制
                                    if (activityProductList.Count() == 0)
                                    {
                                        //没有必买产品，逻辑为满足活动
                                        ifall = true;
                                    }
                                    foreach (ActivityProduct activityProduct in activityProductList)
                                    {
                                        if (!ifpt)
                                        {
                                            break;//已经判断出活动产品库存不够，优化
                                        }
                                        foreach (ShoppingCartDTO item in dto)
                                        {
                                            if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                            {
                                                ifall = true;
                                                if ((item.Num / activityProduct.NumBase) < baseNum)
                                                {
                                                    baseNum = item.Num / activityProduct.NumBase;
                                                }
                                                //判断所有用户最大购买限制                                                
                                                if (activityProduct.NumMaxAll != null)
                                                {
                                                    //所有用户已购该产品数量
                                                    int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                                //判断该最大购买限制  
                                                if (activityProduct.NumMax != null)
                                                {
                                                    //该用户已购数量
                                                    int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMax - nm) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 判断活动赠品库存还够不够满足活动
                                    foreach (ActivityGifts activityGifts in aglist)
                                    {
                                        //判断所有用户最大赠送限制                                                
                                        if (activityGifts.NumMaxAll != null)
                                        {
                                            //所有用户已赠该产品数量
                                            int nmall = c.ActivityGiftsPurchased.Where(w => w.ActivityID == activity.ID && w.GiftsID == activityGifts.GiftID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                            if ((activityGifts.NumMaxAll - nmall) <= 0)
                                            {
                                                ifpt = false;
                                            }
                                        }
                                        //判断该最大赠送限制  
                                        if (activityGifts.NumMax != null)
                                        {
                                            //该用户已赠数量
                                            int nm = c.ActivityGiftsPurchased.Where(w => w.ActivityID == activity.ID && w.GiftsID == activityGifts.GiftID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                            if ((activityGifts.NumMax - nm) <= 0)
                                            {
                                                ifpt = false;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 满足活动产品必买限制，活动产品赠品库存都够,最小基数大于0之后，添加活动产品购买记录和赠送记录，并将赠品加入购物车
                                    if (ifall && ifpt && baseNum > 0)
                                    {
                                        foreach (ActivityProduct item in activityProductList)
                                        {
                                            foreach (ShoppingCartDTO items in dto)
                                            {
                                                if (item.ProductID == items.ProductID && items.ShoppingProductType != 2 && items.ShoppingProductType != 4)
                                                {
                                                    //添加活动产品购买记录
                                                    ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                    app.ID = Guid.NewGuid();
                                                    app.ActivityID = activity.ID;
                                                    app.ProducID = item.ProductID;
                                                    if (Convert.ToBoolean(activity.IfDoubleGive))
                                                    {
                                                        app.ProducNum = item.NumBase * baseNum;
                                                    }
                                                    else
                                                    {
                                                        app.ProducNum = item.NumBase;
                                                    }

                                                    app.UserID = userid;
                                                    applist.Add(app);
                                                }
                                            }
                                        }
                                        //购物车加赠品                                       
                                        foreach (ActivityGifts item in aglist)
                                        {
                                            //添加活动赠品赠送记录
                                            ActivityGiftsPurchasedTT agp = new ActivityGiftsPurchasedTT();
                                            agp.ID = Guid.NewGuid();
                                            agp.ActivityID = activity.ID;
                                            agp.GiftsID = item.GiftID;

                                            if (Convert.ToBoolean(activity.IfDoubleGive))
                                            {
                                                agp.ProducNum = item.NumBase * baseNum;
                                            }
                                            else
                                            {
                                                agp.ProducNum = item.NumBase;
                                            }
                                            agp.UserID = userid;
                                            agplist.Add(agp);
                                            //购物车加赠品
                                            ShoppingCartDTO sc = new ShoppingCartDTO();
                                            sc.ID = Guid.NewGuid();
                                            sc.ProductID = item.GiftID;

                                            List<ActivityGiftsPurchased> agpList = c.ActivityGiftsPurchased.Where(w => w.ActivityID == activity.ID && w.GiftsID == item.GiftID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList();
                                            int nm = agpList.Sum(s => s.ProducNum);//该用户已赠数量


                                            if (Convert.ToBoolean(activity.IfDoubleGive))
                                            {
                                                sc.Num = item.NumBase * baseNum;
                                            }
                                            else
                                            {
                                                sc.Num = item.NumBase;
                                            }

                                            if ((item.NumMax - nm) < sc.Num)
                                            {
                                                sc.Num = (int)item.NumMax - nm;
                                            }
                                            sc.ShoppingProductPrice = 0;
                                            sc.UserID = userid;
                                            sc.AddTime = DateTime.Now;
                                            sc.ShoppingProductType = 4;
                                            sc.ShoppingProductPrice = 0;
                                            dto.Add(sc);
                                        }
                                    }
                                    #endregion

                                }

                                continue;
                            #endregion                            

                            #region 2折扣 
                            case 2:
                                if (true)
                                {
                                    List<ActivityProduct> activityProductList = c.ActivityProduct.Where(w => w.ActivityID == activity.ID).ToList();//活动产品

                                    //非全必买
                                    #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动
                                    bool ifall = false;//是否满足活动

                                    bool ifpt = true;//判断活动产品购买赠送限制
                                    if (activityProductList.Count() == 0)
                                    {
                                        //没有必买产品，逻辑为满足活动
                                        ifall = true;
                                    }
                                    foreach (ActivityProduct activityProduct in activityProductList)
                                    {
                                        if (!ifpt)
                                        {
                                            break;//已经判断出活动产品库存不够，优化
                                        }
                                        foreach (ShoppingCartDTO item in dto)
                                        {
                                            if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                            {
                                                ifall = true;

                                                //判断所有用户最大购买限制                                                
                                                if (activityProduct.NumMaxAll != null)
                                                {
                                                    //所有用户已购该产品数量
                                                    int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                                //判断该最大购买限制  
                                                if (activityProduct.NumMax != null)
                                                {
                                                    //该用户已购数量
                                                    int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMax - nm) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 满足活动产品必买限制，活动产品赠品库存都够,最小基数大于0之后，添加活动产品购买记录和赠送记录，并将赠品加入购物车
                                    if (ifall && ifpt)
                                    {
                                        foreach (ActivityProduct item in activityProductList)
                                        {
                                            foreach (ShoppingCartDTO items in dto)
                                            {
                                                if (item.ProductID == items.ProductID && items.ShoppingProductType != 2 && items.ShoppingProductType != 4)
                                                {
                                                    //添加活动产品购买记录
                                                    ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                    app.ID = Guid.NewGuid();
                                                    app.ActivityID = activity.ID;
                                                    app.ProducID = item.ProductID;
                                                    app.ProducNum = items.Num;
                                                    app.UserID = userid;
                                                    applist.Add(app);
                                                    //修改购物车产品价格
                                                    items.ShoppingProductPrice = items.ShoppingProductPrice * Convert.ToDecimal(item.Discount);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                }
                                continue;
                            #endregion

                            #region 3折上折1
                            case 3:

                                continue;
                            #endregion

                            #region 4折上折2
                            case 4:

                                continue;
                            #endregion

                            #region 5满减
                            case 5:
                                if (true)
                                {
                                    List<ActivityProduct> activityProductList = c.ActivityProduct.Where(w => w.ActivityID == activity.ID).ToList();//活动产品
                                    List<ActivityGifts> aglist = c.ActivityGifts.Where(w => w.ActivityID == activity.ID).ToList();//活动赠品

                                    //非全必买
                                    #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动
                                    bool ifall = false;//是否满足活动

                                    bool ifpt = true;//判断活动产品购买赠送限制
                                    if (activityProductList.Count() == 0)
                                    {
                                        //没有必买产品，逻辑为满足活动
                                        ifall = true;
                                    }
                                    foreach (ActivityProduct activityProduct in activityProductList)
                                    {
                                        if (!ifpt)
                                        {
                                            break;//已经判断出活动产品库存不够，优化
                                        }
                                        foreach (ShoppingCartDTO item in dto)
                                        {
                                            if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                            {
                                                ifall = true;

                                                //判断所有用户最大购买限制                                                
                                                if (activityProduct.NumMaxAll != null)
                                                {
                                                    //所有用户已购该产品数量
                                                    int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                                //判断该最大购买限制  
                                                if (activityProduct.NumMax != null)
                                                {
                                                    //该用户已购数量
                                                    int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMax - nm) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 判断产品总价够不够
                                    bool ifmon = false;
                                    if (CountProvince(dto) >= activity.BasicsMoney)
                                    {
                                        ifmon = true;
                                    }
                                    #endregion

                                    #region 满足活动产品必买限制，活动产品赠品库存都够,最小基数大于0之后，添加活动产品购买记录和赠送记录，并将赠品加入购物车
                                    if (ifmon && ifall && ifpt)
                                    {
                                        //购物车产品均摊优惠金额
                                        int pnum = dto.Where(w => w.ShoppingProductType != 2 && w.ShoppingProductType != 4).ToList().Sum(s => s.Num);
                                        decimal? jtmom = activity.DiscountMoney / pnum;
                                        foreach (ShoppingCartDTO item in dto)
                                        {
                                            //不减还是直接写0，待确定
                                            if (item.ShoppingProductPrice - jtmom > 0)
                                            {
                                                item.ShoppingProductPrice = item.ShoppingProductPrice - jtmom;
                                            }
                                            else
                                            {
                                                item.ShoppingProductPrice = 0;
                                            }

                                        }

                                        foreach (ActivityProduct item in activityProductList)
                                        {
                                            foreach (ShoppingCartDTO items in dto)
                                            {
                                                if (item.ProductID == items.ProductID && items.ShoppingProductType != 2 && items.ShoppingProductType != 4)
                                                {
                                                    //添加活动产品购买记录
                                                    ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                    app.ID = Guid.NewGuid();
                                                    app.ActivityID = activity.ID;
                                                    app.ProducID = item.ProductID;
                                                    app.ProducNum = item.NumBase;
                                                    app.UserID = userid;
                                                    applist.Add(app);
                                                }
                                            }
                                        }

                                    }
                                    #endregion

                                }
                                continue;
                            #endregion

                            #region 6满赠
                            case 6:
                                if (true)
                                {
                                    List<ActivityProduct> activityProductList = c.ActivityProduct.Where(w => w.ActivityID == activity.ID).ToList();//活动产品
                                    List<ActivityGifts> aglist = c.ActivityGifts.Where(w => w.ActivityID == activity.ID).ToList();//活动赠品

                                    //非全必买
                                    #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动
                                    bool ifall = false;//是否满足活动
                                    int baseNum = 2147483647;//最小基数 
                                    bool ifpt = true;//判断活动产品购买赠送限制
                                    if (activityProductList.Count() == 0)
                                    {
                                        //没有必买产品，逻辑为满足活动
                                        ifall = true;
                                    }
                                    foreach (ActivityProduct activityProduct in activityProductList)
                                    {
                                        if (!ifpt)
                                        {
                                            break;//已经判断出活动产品库存不够，优化
                                        }
                                        foreach (ShoppingCartDTO item in dto)
                                        {
                                            if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                            {
                                                ifall = true;
                                                if ((item.Num / activityProduct.NumBase) < baseNum)
                                                {
                                                    baseNum = item.Num / activityProduct.NumBase;
                                                }
                                                //判断所有用户最大购买限制                                                
                                                if (activityProduct.NumMaxAll != null)
                                                {
                                                    //所有用户已购该产品数量
                                                    int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                                //判断该最大购买限制  
                                                if (activityProduct.NumMax != null)
                                                {
                                                    //该用户已购数量
                                                    int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMax - nm) < item.Num)
                                                    {
                                                        ifpt = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 判断活动赠品库存还够不够满足活动
                                    foreach (ActivityGifts activityGifts in aglist)
                                    {
                                        //判断所有用户最大赠送限制                                                
                                        if (activityGifts.NumMaxAll != null)
                                        {
                                            //所有用户已购该产品数量
                                            int nmall = c.ActivityGiftsPurchased.Where(w => w.ActivityID == activity.ID && w.GiftsID == activityGifts.GiftID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                            if ((activityGifts.NumMaxAll - nmall) <= 0)
                                            {
                                                ifpt = false;
                                            }
                                        }
                                        //判断该最大赠送限制  
                                        if (activityGifts.NumMax != null)
                                        {
                                            //该用户已购数量
                                            int nm = c.ActivityGiftsPurchased.Where(w => w.ActivityID == activity.ID && w.GiftsID == activityGifts.GiftID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                            if ((activityGifts.NumMax - nm) <= 0)
                                            {
                                                ifpt = false;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 判断产品总价够不够
                                    //bool ifmon = false;
                                    //if (CountProvince(dto) >= activity.BasicsMoney)
                                    //{
                                    //    ifmon = true;
                                    //}
                                    #endregion

                                    #region 满足活动产品必买限制，活动产品赠品库存都够,最小基数大于0之后，添加活动产品购买记录和赠送记录，并将赠品加入购物车
                                    if (ifall && ifpt && baseNum > 0)//ifmon && 
                                    {
                                        bool ifmon = false;
                                        //购物车加赠品                                       
                                        foreach (ActivityGifts item in aglist)
                                        {
                                            decimal zj = CountProvince(dto);
                                            if (zj >= item.MoneyMin && zj < item.MoneyMax)
                                            {
                                                //添加活动赠品赠送记录
                                                ActivityGiftsPurchasedTT agp = new ActivityGiftsPurchasedTT();
                                                agp.ID = Guid.NewGuid();
                                                agp.ActivityID = activity.ID;
                                                agp.GiftsID = item.GiftID;
                                                if (Convert.ToBoolean(activity.IfDoubleGive))
                                                {
                                                    agp.ProducNum = item.NumBase * baseNum;// 
                                                }
                                                else
                                                {
                                                    agp.ProducNum = item.NumBase;// * baseNum
                                                }

                                                agp.UserID = userid;
                                                agplist.Add(agp);
                                                ShoppingCartDTO sc = new ShoppingCartDTO();
                                                sc.ID = Guid.NewGuid();
                                                sc.ProductID = item.GiftID;

                                                if (Convert.ToBoolean(activity.IfDoubleGive))
                                                {
                                                    sc.Num = item.NumBase * baseNum;// 
                                                }
                                                else
                                                {
                                                    sc.Num = item.NumBase;// * baseNum
                                                }

                                                sc.UserID = userid;
                                                sc.AddTime = DateTime.Now;
                                                sc.ShoppingProductType = 4;
                                                sc.ShoppingProductPrice = 0;
                                                dto.Add(sc);
                                                ifmon = true;
                                            }
                                        }

                                        if (ifmon)
                                        {
                                            foreach (ActivityProduct item in activityProductList)
                                            {
                                                foreach (ShoppingCartDTO items in dto)
                                                {
                                                    if (item.ProductID == items.ProductID && items.ShoppingProductType != 2 && items.ShoppingProductType != 4)
                                                    {
                                                        //添加活动产品购买记录
                                                        ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                        app.ID = Guid.NewGuid();
                                                        app.ActivityID = activity.ID;
                                                        app.ProducID = item.ProductID;

                                                        if (Convert.ToBoolean(activity.IfDoubleGive))
                                                        {
                                                            app.ProducNum = item.NumBase * baseNum;// 
                                                        }
                                                        else
                                                        {
                                                            app.ProducNum = item.NumBase;// * baseNum
                                                        }
                                                        app.UserID = userid;
                                                        applist.Add(app);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                }

                                continue;
                            #endregion

                            #region 7买1赠N
                            case 7:
                                if (true)
                                {
                                    List<ActivityProduct> activityProductList = c.ActivityProduct.Where(w => w.ActivityID == activity.ID).ToList();//活动产品

                                    if (activity.IfAll)
                                    {
                                        //全必买
                                        #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动
                                        bool ifall = true;//是否满足活动

                                        bool ifpt = true;//判断活动产品购买赠送限制
                                        foreach (ActivityProduct activityProduct in activityProductList)
                                        {
                                            bool ifProduct = false;//是否包含所有产品
                                            if (!ifpt)
                                            {
                                                break;//已经判断出活动产品库存不够，优化
                                            }
                                            foreach (ShoppingCartDTO item in dto)
                                            {
                                                if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                                {
                                                    ifProduct = true;

                                                    //判断所有用户最大购买限制                                                
                                                    if (activityProduct.NumMaxAll != null)
                                                    {
                                                        //所有用户已购该产品数量
                                                        int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                        if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                        {
                                                            ifpt = false;
                                                            break;
                                                        }
                                                    }
                                                    //判断该最大购买限制  
                                                    if (activityProduct.NumMax != null)
                                                    {
                                                        //该用户已购数量
                                                        int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                        if ((activityProduct.NumMax - nm) < item.Num)
                                                        {
                                                            ifpt = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            //判断购物车产品是不是包含所有活动产品
                                            if (!ifProduct)
                                            {
                                                ifall = false;
                                                break;
                                            }
                                        }
                                        #endregion

                                        #region 满足活动产品必买限制，活动产品赠品库存都够,最小基数大于0之后，添加活动产品购买记录和赠送记录，并将赠品加入购物车
                                        if (ifall && ifpt)
                                        {
                                            List<ShoppingCartDTO> li = new List<ShoppingCartDTO>();
                                            foreach (ActivityProduct item in activityProductList)
                                            {
                                                foreach (ShoppingCartDTO items in dto)
                                                {
                                                    if (item.ProductID == items.ProductID && items.ShoppingProductType != 2 && items.ShoppingProductType != 4)
                                                    {
                                                        //添加活动产品购买记录
                                                        ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                        app.ID = Guid.NewGuid();
                                                        app.ActivityID = activity.ID;
                                                        app.ProducID = item.ProductID;
                                                        app.ProducNum = items.Num;
                                                        app.UserID = userid;
                                                        applist.Add(app);

                                                        //添加活动赠品赠送记录
                                                        ActivityGiftsPurchasedTT agp = new ActivityGiftsPurchasedTT();
                                                        agp.ID = Guid.NewGuid();
                                                        agp.ActivityID = activity.ID;
                                                        agp.GiftsID = item.ProductID;
                                                        agp.ProducNum = item.NumBase;// * baseNum
                                                        agp.UserID = userid;
                                                        agplist.Add(agp);
                                                        //购物车加赠品（来源于活动产品表）
                                                        ShoppingCartDTO sc = new ShoppingCartDTO();
                                                        sc.ID = Guid.NewGuid();
                                                        sc.ProductID = item.ProductID;
                                                        sc.Num = item.NumBase * items.Num;
                                                        sc.ShoppingProductPrice = 0;
                                                        sc.UserID = userid;
                                                        sc.AddTime = DateTime.Now;
                                                        sc.ShoppingProductType = 4;
                                                        sc.ShoppingProductPrice = 0;
                                                        li.Add(sc);
                                                    }
                                                }
                                            }

                                            dto.AddRange(li);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        //非全必买
                                        #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动
                                        bool ifall = false;//是否满足活动

                                        bool ifpt = true;//判断活动产品购买赠送限制
                                        if (activityProductList.Count() == 0)
                                        {
                                            //没有必买产品，逻辑为满足活动
                                            ifall = true;
                                        }
                                        foreach (ActivityProduct activityProduct in activityProductList)
                                        {
                                            if (!ifpt)
                                            {
                                                break;//已经判断出活动产品库存不够，优化
                                            }
                                            foreach (ShoppingCartDTO item in dto)
                                            {
                                                if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                                {
                                                    ifall = true;

                                                    //判断所有用户最大购买限制                                                
                                                    if (activityProduct.NumMaxAll != null)
                                                    {
                                                        //所有用户已购该产品数量
                                                        int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                        if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                        {
                                                            ifpt = false;
                                                            break;
                                                        }
                                                    }
                                                    //判断该最大购买限制  
                                                    if (activityProduct.NumMax != null)
                                                    {
                                                        //该用户已购数量
                                                        int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                        if ((activityProduct.NumMax - nm) < item.Num)
                                                        {
                                                            ifpt = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 满足活动产品必买限制，活动产品赠品库存都够,最小基数大于0之后，添加活动产品购买记录和赠送记录，并将赠品加入购物车
                                        if (ifall && ifpt)
                                        {
                                            List<ShoppingCartDTO> li = new List<ShoppingCartDTO>();
                                            foreach (ActivityProduct item in activityProductList)
                                            {
                                                foreach (ShoppingCartDTO items in dto)
                                                {
                                                    if (item.ProductID == items.ProductID && items.ShoppingProductType != 2 && items.ShoppingProductType != 4)
                                                    {
                                                        //添加活动产品购买记录
                                                        ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                        app.ID = Guid.NewGuid();
                                                        app.ActivityID = activity.ID;
                                                        app.ProducID = item.ProductID;
                                                        app.ProducNum = items.Num;
                                                        app.UserID = userid;
                                                        applist.Add(app);
                                                        //添加活动赠品赠送记录
                                                        ActivityGiftsPurchasedTT agp = new ActivityGiftsPurchasedTT();
                                                        agp.ID = Guid.NewGuid();
                                                        agp.ActivityID = activity.ID;
                                                        agp.GiftsID = item.ProductID;
                                                        agp.ProducNum = item.NumBase;// * baseNum
                                                        agp.UserID = userid;
                                                        agplist.Add(agp);
                                                        //购物车加赠品（来源于活动产品表）
                                                        ShoppingCartDTO sc = new ShoppingCartDTO();
                                                        sc.ID = Guid.NewGuid();
                                                        sc.ProductID = item.ProductID;
                                                        sc.Num = item.NumBase * items.Num;
                                                        sc.ShoppingProductPrice = 0;
                                                        sc.UserID = userid;
                                                        sc.AddTime = DateTime.Now;
                                                        sc.ShoppingProductType = 4;
                                                        sc.ShoppingProductPrice = 0;
                                                        li.Add(sc);
                                                    }
                                                }
                                            }
                                            dto.AddRange(li);
                                        }
                                        #endregion
                                    }
                                }
                                continue;
                            #endregion

                            #region 8限购
                            case 8:
                                if (true)
                                {
                                    List<ActivityProduct> activityProductList = c.ActivityProduct.Where(w => w.ActivityID == activity.ID).ToList();//活动产品
                                    List<ActivityGifts> aglist = c.ActivityGifts.Where(w => w.ActivityID == activity.ID).ToList();//活动赠品

                                    #region 判断购车车选择产品是否满足活动，以及活动产品库存还够不够满足活动

                                    foreach (ActivityProduct activityProduct in activityProductList)
                                    {

                                        foreach (ShoppingCartDTO item in dto)
                                        {
                                            if (activityProduct.ProductID == item.ProductID && item.ShoppingProductType != 2 && item.ShoppingProductType != 4)
                                            {

                                                //判断所有用户最大购买限制                                                
                                                if (activityProduct.NumMaxAll != null)
                                                {
                                                    //所有用户已购该产品数量
                                                    int nmall = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMaxAll - nmall) < item.Num)
                                                    {
                                                        throw new DMException("产品：" + activityProduct.Product.ProductName + "已达最大购买限制！");
                                                    }
                                                }
                                                //判断该最大购买限制  
                                                if (activityProduct.NumMax != null)
                                                {
                                                    //该用户已购数量
                                                    int nm = c.ActivityProductPurchased.Where(w => w.ActivityID == activity.ID && w.ProducID == activityProduct.ProductID && w.UserID == userid && (w.Order.State == 1 || w.Order.State == 3 || w.Order.State == 4 || w.Order.State == 9 || w.Order.State == 10)).ToList().Sum(s => s.ProducNum);
                                                    if ((activityProduct.NumMax - nm) < item.Num)
                                                    {
                                                        throw new DMException("产品：" + activityProduct.Product.ProductName + "已达最大购买限制！");
                                                    }
                                                }
                                                //添加活动产品购买记录
                                                ActivityProductPurchasedTT app = new ActivityProductPurchasedTT();
                                                app.ID = Guid.NewGuid();
                                                app.ActivityID = activity.ID;
                                                app.ProducID = item.ProductID;
                                                app.ProducNum = item.Num;
                                                app.UserID = userid;
                                                applist.Add(app);

                                            }
                                        }
                                    }
                                    #endregion

                                }

                                continue;
                            #endregion                            

                            default:
                                break;
                        }
                    }
                }
                //完善购物车产品信息（不包括价格）
                foreach (ShoppingCartDTO item in dto)
                {
                    Product pt = c.Product.Where(w => w.ID == item.ProductID).FirstOrDefault();

                    if (c.ProductImage.Where(w => w.ImgType == 1 && w.ProductID == item.ProductID).FirstOrDefault() != null)
                    {
                        item.zt = c.ProductImage.Where(w => w.ImgType == 1 && w.ProductID == item.ProductID).FirstOrDefault().Url;
                    }
                    item.Unit = pt.Unit;
                    item.ProductName = pt.ProductName;
                    item.IsBy = pt.IsBy;
                }
                ShoppingCartActivityDTO acadto = new ShoppingCartActivityDTO();
                acadto.sclist = dto;
                acadto.applist = applist;
                acadto.agplist = agplist;

                return acadto;
            }
        }
        public List<ShoppingCartDTO> GetShopping(Guid? userid)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ShoppingCartDTO> dto = c.ShoppingCart.Where(w => w.UserID == userid).Select(
                    x => new ShoppingCartDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        IsXz = false,
                        Num = x.Num,
                        ProductID = x.ProductID,
                        UserID = x.UserID,
                        ShoppingProductPrice = x.Product.Price,
                        ProductName = x.Product.ProductName,
                        Unit = x.Product.Unit,
                        ShoppingProductType = x.ShoppingProductType,
                        ShoppingProductPV = x.Product.PV,
                        IsBy = x.Product.IsBy,
                        zt = x.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                    }
                ).ToList();
                foreach (ShoppingCartDTO item in dto)
                {
                    OrderConfig of = c.OrderConfig.Where(w => w.ProductID == item.ProductID && w.OrderTypeID == (int)Enum.OrderType.主动消费单).FirstOrDefault();
                    if (of != null)
                    {
                        item.IsYx = true;
                        item.PriceMin = of.PriceMin;
                        item.IfCount = of.IfCount;
                        //public Nullable<decimal> PriceMin { get; set; }
                        //public Nullable<bool> IfCount { get; set; }
                    }
                    else
                    {
                        item.IsYx = false;
                    }
                }


                return dto;
            }
        }

        public bool AddShopping(Guid pid, int num, Guid userid, int numtype)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Product pt = c.Product.Where(w => w.ID == pid).FirstOrDefault();
                ShoppingCart yygwc = c.ShoppingCart.Where(w => w.ProductID == pid && w.UserID == userid).FirstOrDefault();
                if (yygwc == null)
                {
                    ValidateStockNumber(pt, num);

                    ShoppingCart sc = new ShoppingCart();
                    sc.ID = Guid.NewGuid();
                    sc.ProductID = pid;
                    sc.Num = num;
                    sc.UserID = userid;
                    sc.AddTime = DateTime.Now;
                    sc.ShoppingProductType = pt.ProductNature;

                    c.ShoppingCart.Add(sc);
                }
                else
                {
                    if (numtype == 1)
                    {
                        yygwc.Num = num + yygwc.Num;
                    }
                    else if (numtype == 2)
                    {
                        yygwc.Num = num;
                    }
                    ValidateStockNumber(pt, yygwc.Num);

                    if (yygwc.Num == 0)
                    {
                        c.ShoppingCart.Remove(yygwc);
                    }
                }

                c.SaveChanges();
                return true;
            }
        }

        public bool deShopping(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ShoppingCart pt = c.ShoppingCart.FirstOrDefault(n => n.ID == id);

                c.ShoppingCart.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #region Private

        /// <summary>
        ///  验证产品的库存是否可以满足采购需求，若缺货仅作提示
        /// </summary>
        /// <returns></returns>
        private void ValidateStockNumber(Product pt, int buyNumber)
        {
            if (buyNumber <= 0)
            {
                return;
            }

            //验证商品是否缺货
            if (pt.StockNumber == 0)
            {
                throw new DMException($"[{pt.ProductName}]商品缺货，无法加入购物车！");
            }
        }

        #endregion

    }
}
