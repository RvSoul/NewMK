
using Infrastructure.Utility;
using NewMK.Domian.DomainException;
using NewMK.Domian.Pay.WxException;
using NewMK.Domian.TheThirdPartyBase;
using NewMK.Domian.ThirdParty;
using NewMK.DTO;
using NewMK.DTO.Activity;
using NewMK.DTO.Order;
using NewMK.DTO.Record;
using NewMK.DTO.ShoppingCart;
using NewMK.DTO.User;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Utility;
using WxPayAPI;

namespace NewMK.Domian.DM
{
    public class OrderDM
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Decimal hyqnum = 150;//活跃期基础数

        /// <summary>
        /// 计算订单产品总价格
        /// </summary>
        /// <param name="ProductList"></param>
        /// <returns></returns>
        public double CountProvince(List<ShoppingCartDTO> ProductList)
        {
            if (ProductList == null)
            {
                return 0;
            }
            double zProvince = 0;
            foreach (ShoppingCartDTO item in ProductList)
            {
                if (item.ShoppingProductType == 1 || item.ShoppingProductType == 3)
                {
                    zProvince += Convert.ToDouble(item.ShoppingProductPrice) * Convert.ToDouble(item.Num);
                }

            }
            return zProvince;
        }
        public decimal CountYf(List<ShoppingCartDTO> ProductList)
        {
            decimal yf = 0;

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                foreach (ShoppingCartDTO item in ProductList)
                {
                    if (item.ShoppingProductType == 1 || item.ShoppingProductType == 3)
                    {
                        Product pt = c.Product.FirstOrDefault(n => n.ID == item.ProductID);
                        //item.ShoppingProductPrice = pt.Price;
                        if (pt.IsBy)
                        {
                            return 0;
                        }
                    }

                }
            }

            double moneyProduct = CountProvince(ProductList); //产品总价
            //计算运费
            if (moneyProduct >= 398)
            {
                yf = 0;
            }
            else
            {
                var s = ReadConfig.GetAppSetting("YF");
                if (string.IsNullOrEmpty(s))
                {
                    yf = 10;
                }
                else
                {
                    yf = decimal.Parse(s);
                }
            }
            return yf;
        }
        public decimal CountYf(List<ShoppingCartDTO> ProductList, Guid? addressid)
        {
            double zj = 0;
            double zzl = 0;

            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                foreach (ShoppingCartDTO item in ProductList)
                {
                    Product pt = c.Product.FirstOrDefault(n => n.ID == item.ProductID);
                    zzl += Convert.ToDouble(item.Num) * Convert.ToDouble(pt.ProductWeight);
                }

                UserAddress ua = c.UserAddress.FirstOrDefault(x => x.ID == addressid);
                FreightRules fr = c.FreightRules.FirstOrDefault(x => x.ProvinceID == ua.ProvinceID);


                if (zzl >= 0 && zzl <= 1)
                {
                    zj = Convert.ToDouble(fr.Price1);
                }
                else if (zzl > 1 && zzl <= 2)
                {
                    zj = Convert.ToDouble(fr.Price2);
                }
                else
                {
                    double sz = Convert.ToDouble(fr.Price3);
                    double szn = Convert.ToDouble(fr.Price4);
                    double zl = zzl - 1;

                    zj += sz;
                    double ys = szn * zl;

                    zj += ys;
                }

            }
            return Convert.ToDecimal(zj);
        }

        public List<OrderTypeDTO> GetOrderTypeList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<OrderType> li = c.OrderType.ToList();

                return OrderTypeDTO.GetDTOList<OrderType, OrderTypeDTO>(li);
            }
        }

        /// <summary>
        /// 删除订单，通过第三方支付将自动发起退款流程
        /// </summary>
        /// <param name="id"></param>
        public string DeleteOrders(Guid id, string deorderTime)
        {
            OrdersDTO order = GetOrderById(id); ;
            if (order == null)
            {
                throw new DMException("订单不存在！");
            }
            if (order.SettleFlag == (int)Enum.SettleFlag.待结算)
            {
                throw new DMException($"订单[{order.OrderNumber}]待安置，无法删除！");
            }
            if (order.State == (int)Enum.OrderState.支付已取消)
            {
                throw new DMException($"订单[{order.OrderNumber}]已删除！");
            }
            if (order.IsBalance != 0)
            {
                throw new DMException($"订单[{order.OrderNumber}]已结算，不能删除！");
            }

            //TODO，退款失败，建议人工处理
            if (order.State == (int)Enum.OrderState.退款失败)
            {
                throw new DMException($"订单[{order.OrderNumber}]退款失败，请人工联系客户进行人工退款处理！");
            }
            if (order.State == (int)Enum.OrderState.待退款)
            {
                throw new DMException($"订单[{order.OrderNumber}]正在退款，无法删除！");
            }
            //TODO，订单已退款，直接提醒
            if (order.State == (int)Enum.OrderState.已退款)
            {
                //ChangeOrderState(Enum.OrderState.支付已取消, id);
                return $"订单[{order.OrderNumber}]已删除！";
            }
            //只有第三方支付参与的情况才会出现待付款
            if (order.State == (int)Enum.OrderState.待付款)
            {
                CancelPayingOrder(id);
                return $"订单[{order.OrderNumber}]删除成功！";
            }
            else if (order.State == (int)Enum.OrderState.支付失败)
            {
                ChangeOrderState(Enum.OrderState.支付已取消, order.ID);
                return $"订单[{order.OrderNumber}]删除成功！";
            }
            //使用了第三方支付直接进入退款流程
            else if (order.OtherPayMoney > 0)
            {
                //调用第三方接口发起退款流程
                if (order.PayType == (int)Enum.PayType.微信 || order.PayType == (int)Enum.PayType.微信加积分)
                {
                    WxRefund(order.OrderNumber);
                    return $"订单[{order.OrderNumber}]已发起退款流程，退款成功后系统将自动删除订单！";
                }
                else
                {
                    //TODO
                    throw new DMException("该通道暂时不支持退款，无法删除订单！");
                }
            }
            else
            {
                //using (var scope = new TransactionScope())
                {
                    CancelOrder(order.ID, Enum.OrderState.已退款);
                    //scope.Complete();
                }
                return $"订单[{order.OrderNumber}]成功！";
            }
        }

        /// <summary>
        /// 自动退款失败情况下，人工退款
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool ManualRefund(Guid id, string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                throw new DMException("人工退款必须填写退款备注！");
            }
            CancelOrder(id, Enum.OrderState.已退款, comment);
            return true;
        }


        /// <summary>
        /// 微信退款
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public void WxRefund(string orderNumber)
        {
            Order order = null;
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                if (order == null)
                {
                    throw new DMException("订单不存在");
                }
                if (order.PayType != (int)Enum.PayType.微信 && order.PayType == (int)Enum.PayType.微信加积分 &&
                    order.OtherPayMoney <= 0)
                {
                    throw new DMException("订单没有使用微信支付，微信退款失败！");
                }
                //退款单号就使用订单号
                if (string.IsNullOrEmpty(order.RefundOrderNumber))
                {
                    order.RefundOrderNumber = order.OrderNumber;
                    c.SaveChanges();
                }
            }

            WxPayData data = Refund.Run(order.OrderNumber, (int)(order.OtherPayMoney * 100),
                (int)(order.OtherPayMoney * 100), order.RefundOrderNumber);
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                order = c.Order.Where(w => w.ID == order.ID).FirstOrDefault();
                order.State = (int)Enum.OrderState.待退款;
                order.RefundStartTime = DateTime.Now;
                order.NextWxSynTime = DateTime.Now.AddMinutes(1);//退款1分钟后自动同步实际的退款状态
                c.SaveChanges();
            }
        }

        /// <summary>
        /// 对订单进行清理处理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deorderTime"></param>
        /// <returns></returns>
        public bool deOrders(Guid id, string deorderTime)
        {
            if (!string.IsNullOrEmpty(deorderTime))
            {
                string[] dot = deorderTime.Split('-');
                string qq = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

                string qqq = DateTime.Now.ToString("yyyy-MM-dd");
                if (qq == qqq)
                {
                    throw new DMException("删除失败，月底不允许删除！");
                }
                foreach (string itemTime in dot)
                {
                    DateTime tt = DateTime.Parse(itemTime);
                    DateTime tt1 = tt.AddMinutes(-20);
                    DateTime tt2 = tt.AddMinutes(20);
                    if (DateTime.Now > tt1 && DateTime.Now < tt2)
                    {
                        throw new DMException("删除失败，该时间段不允许删除，请稍后！");
                    }
                }
            }

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    Order order = c.Order.FirstOrDefault(n => n.ID == id);

                    if ((order.State != (int)Enum.OrderState.待发货 && order.State == (int)Enum.OrderState.订单完成 &&
                                order.State == (int)Enum.OrderState.已退款))
                    {
                        throw new DMException($"订单[{order.OrderNumber}]目前状态为{order.State}，不能删除！");
                    }
                    if (order.IsBalance != 0)
                    {
                        throw new DMException($"订单[{order.OrderNumber}]已结算，不能删除！");
                    }

                    OrderType ordertype = c.OrderType.Where(w => w.ID == order.OrderTypeID).FirstOrDefault();

                    User user = c.User.Where(w => w.ID == order.UserID).FirstOrDefault();//订单所属人
                    User dealer;//订单扣款人
                    if (order.UserID == order.DealerId)
                    {
                        dealer = user;
                    }
                    else
                    {
                        //代购业务
                        dealer = c.User.Where(w => w.ID == order.DealerId).FirstOrDefault();

                    }

                    #region 第三方库验证

                    GetCROrderQueries cr = new GetCROrderQueries();
                    GetMiddleDBQueries mid = new GetMiddleDBQueries();
                    //是否进中间库
                    if (mid.HasMiddleDBOrderById(order.OrderNumber))
                    {
                        //是否进超然
                        if (cr.HasCROrderById3(order.OrderNumber))
                        {
                            //是否清
                            if (!cr.HasCRBCJCByIdDE(order.OrderNumber))
                            {
                                //是否冲红
                                if (!cr.IsChonghong(order.OrderNumber))
                                {
                                    throw new DMException("删除失败，订单未完全冲红！");
                                }
                            }
                        }
                        else
                        {
                            throw new DMException("删除失败，订单正在同步到超然！");
                        }
                    }

                    #endregion

                    return DeOrders(id, c, cc, order, ordertype, user, dealer);
                }
            }
        }

        private bool DeOrders(Guid id, BwslRetailEntities c, BwslRetailRelationEntities cc, Order order, OrderType ordertype, User user, User dealer)
        {
            string userid = order.UserID.ToString();
            if (ordertype.ID == (int)Enum.OrderType.注册单)
            {
                #region 注册单
                user.UserState = 0;
                if (cc.PlaceRelation.Where(w => w.PDealerId == user.ID).FirstOrDefault() != null)
                {
                    throw new DMException("安置节点下有用户存在，不能删除注册单！！");
                }
                if (cc.RecomRelation.Where(w => w.PDealerId == user.ID).FirstOrDefault() != null)
                {
                    throw new DMException("推荐节点下有用户存在，不能删除注册单！！");
                }
                if (c.Order.Count(w => w.ID != order.ID && w.UserID == order.UserID && (w.State == 1 || w.State == 3 || w.State == 4 || w.State == 9 || w.State == 10)) > 0)
                {
                    throw new DMException("还有其他未处理订单，不能删除注册单！！");
                }
                user.WeiXinName = null;
                user.WeiXinUrl = null;
                user.OpenID = null;
                user.Unionid = null;
                user.CardCode = null;
                user.UserState = 3;

                #endregion
            }
            else
            {
                #region 活跃期处理
                if (user.DeLevelID >= (int)Enum.DeLevel.创客 || order.DeLevelID >= (int)Enum.DeLevel.创客)
                {
                    ActivePeriodRecord aprd = c.ActivePeriodRecord.Where(w => w.OrderNumber == order.OrderNumber).FirstOrDefault();
                    int? ap = 0;
                    if (aprd != null)
                    {
                        ap = aprd.StageTwo;
                    }

                    ActivePeriodRecord apr = new ActivePeriodRecord();
                    apr.ID = Guid.NewGuid();
                    apr.UserID = user.ID;
                    apr.UserCode = user.UserCode;
                    apr.UserName = user.UserName;
                    apr.RecordType = order.OrderTypeID;

                    apr.AddTime = DateTime.Now;
                    apr.Remarks = "删单扣除。";
                    apr.OrderNumber = order.OrderNumber;

                    apr.StageOne = user.StageCount;
                    if (order.OrderTypeID == 1)
                    {
                        apr.StageTwo = 0 - 4;
                        apr.PVSurplus = user.PVSurplus;
                    }
                    if (order.OrderTypeID == 2)
                    {
                        apr.StageTwo = 0 - 4;
                        apr.PVSurplus = user.PVSurplus;
                    }
                    if (order.OrderTypeID == 3)
                    {
                        apr.PVSurplus = Convert.ToDecimal(ap * hyqnum + user.PVSurplus - order.MoneyProduct);
                        apr.StageTwo = 0 - ap;
                        if (apr.PVSurplus < 0)
                        {
                            apr.PVSurplus = apr.PVSurplus + apr.PVSurplus;
                            apr.StageTwo = apr.StageTwo - 1;
                        }
                    }

                    apr.StageThree = apr.StageOne + apr.StageTwo;

                    c.ActivePeriodRecord.Add(apr);

                    user.StageCount = Convert.ToInt32(apr.StageThree);
                    user.PVSurplus = apr.PVSurplus;
                }

                #endregion

                #region 升级处理       
                if (order.DeLevelID != null && ordertype.ID == (int)Enum.OrderType.升级单)
                {
                    if (c.Order.Where(w => w.OrderTypeID == (int)Enum.OrderType.升级单 && w.AddTime > order.AddTime).Count() > 0)
                    {
                        throw new DMException("请优先处理最后一次的升级订单！！");
                    }
                    //升级单或者注册
                    if (order.DeLevelID != order.OldDeLevelID)
                    {
                        user.DeLevelID = (int)order.OldDeLevelID;
                    }
                }
                else
                {
                    //自动升级
                    if (cc.RecomRelation.Where(w => w.DealerId == user.ID).FirstOrDefault() != null)
                    {
                        if (user.DeLevelID == (int)Enum.DeLevel.VIP顾客)
                        {
                            Decimal zpv = c.Order.Where(w => w.UserID == user.ID && w.State == 3).Sum(s => s.TotalPv);
                            Decimal? minpv = c.DeLevel.Where(w => w.ID == 3).FirstOrDefault().MinPV;
                            if (zpv < minpv)
                            {
                                user.DeLevelID = (int)order.OldDeLevelID;
                            }
                        }
                    }
                }



                #endregion

                #region 安置处理
                if (order.DeLevelID >= (int)Enum.DeLevel.创客)
                {
                    if (!string.IsNullOrEmpty(order.PPDealerCode) && !string.IsNullOrEmpty(order.DeptName))
                    {
                        PlaceRelation pr = cc.PlaceRelation.Where(w => w.DealerId == user.ID).FirstOrDefault();
                        if (pr != null)
                        {
                            cc.PlaceRelation.Remove(pr);
                            //throw new DMException("安置关系不存在！");
                        }

                    }

                }


                #endregion

            }
            #region 流水      

            ExChange ec = new ExChange();
            ec.ID = Guid.NewGuid();
            ec.UserID = dealer.ID;
            ec.UserCode = dealer.UserCode;
            ec.UserName = dealer.UserName;
            ec.ChangeTime = DateTime.Now;
            ec.ChangeMarks = "删单退回。";
            ec.OrderNum = order.OrderNumber;
            ec.State = 0;
            ec.ExChangeType = 3;

            if (order.PayType == (int)Enum.PayType.积分)//积分支付
            {
                ec.MoneyType = 1;
                ec.ZMoneyType = 3;

                ec.BeforeChangeMoney = dealer.EleMoney;
                ec.ChangeMoney = (decimal)order.UserPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                dealer.EleMoney = dealer.EleMoney + (decimal)order.UserPayMoney;//处理余额 

                if (c.ExChange.Where(w => w.OrderNum == order.OrderNumber && w.ZMoneyType == ec.ZMoneyType).FirstOrDefault() != null)
                {
                    throw new DMException("操作重复！");
                }
            }
            if (order.PayType == (int)Enum.PayType.微信)//微信支付
            {
                ec.MoneyType = 2;
                ec.ZMoneyType = 8;

                ec.BeforeChangeMoney = 0;
                ec.ChangeMoney = (decimal)order.OtherPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

                if (c.ExChange.Where(w => w.OrderNum == order.OrderNumber && w.ZMoneyType == ec.ZMoneyType).FirstOrDefault() != null)
                {
                    throw new DMException("操作重复！");
                }
            }
            if (order.PayType == (int)Enum.PayType.快钱)//快钱支付
            {
                ec.MoneyType = 3;
                ec.ZMoneyType = 11;

                ec.BeforeChangeMoney = 0;
                ec.ChangeMoney = (decimal)order.OtherPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                if (c.ExChange.Where(w => w.OrderNum == order.OrderNumber && w.ZMoneyType == ec.ZMoneyType).FirstOrDefault() != null)
                {
                    throw new DMException("操作重复！");
                }
            }
            if (order.PayType == (int)Enum.PayType.微信加积分)//微信加积分
            {
                //积分支付部分
                ec.MoneyType = 1;
                ec.ZMoneyType = 3;
                ec.BeforeChangeMoney = dealer.EleMoney;
                ec.ChangeMoney = (decimal)order.UserPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                dealer.EleMoney = dealer.EleMoney + (decimal)order.UserPayMoney;//处理余额

                //微信支付部分
                ExChange ec2 = new ExChange();
                ec2.ID = Guid.NewGuid();
                ec2.UserID = dealer.ID;
                ec2.UserCode = dealer.UserCode;
                ec2.UserName = dealer.UserName;
                ec2.ChangeTime = DateTime.Now;
                ec2.ChangeMarks = "删单退回。";
                ec2.OrderNum = order.OrderNumber;
                ec2.State = 0;
                ec2.ExChangeType = 3;

                ec2.MoneyType = 2;
                ec2.ZMoneyType = 8;
                ec2.BeforeChangeMoney = 0;
                ec2.ChangeMoney = (decimal)order.OtherPayMoney;
                ec2.AfterChangeMoney = ec2.BeforeChangeMoney + ec2.ChangeMoney;
                if (c.ExChange.Where(w => w.OrderNum == order.OrderNumber && (w.ZMoneyType == ec2.ZMoneyType || w.ZMoneyType == ec.ZMoneyType)).FirstOrDefault() != null)
                {
                    throw new DMException("操作重复！");
                }
                c.ExChange.Add(ec2);
            }
            if (order.PayType == (int)Enum.PayType.快钱加积分)//快钱加积分
            {
                //积分支付部分
                ec.MoneyType = 1;
                ec.ZMoneyType = 3;
                ec.BeforeChangeMoney = dealer.EleMoney;
                ec.ChangeMoney = (decimal)order.UserPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                dealer.EleMoney = dealer.EleMoney + (decimal)order.UserPayMoney;//处理余额

                //快钱支付部分
                ExChange ec2 = new ExChange();
                ec2.ID = Guid.NewGuid();
                ec2.UserID = dealer.ID;
                ec2.UserCode = dealer.UserCode;
                ec2.UserName = dealer.UserName;
                ec2.ChangeTime = DateTime.Now;
                ec2.ChangeMarks = "删单退回。";
                ec2.OrderNum = order.OrderNumber;
                ec2.State = 0;
                ec2.ExChangeType = 3;

                ec2.MoneyType = 3;
                ec2.ZMoneyType = 11;
                ec2.BeforeChangeMoney = 0;
                ec2.ChangeMoney = (decimal)order.OtherPayMoney;
                ec2.AfterChangeMoney = ec2.BeforeChangeMoney + ec2.ChangeMoney;
                if (c.ExChange.Where(w => w.OrderNum == order.OrderNumber && (w.ZMoneyType == ec2.ZMoneyType || w.ZMoneyType == ec.ZMoneyType)).FirstOrDefault() != null)
                {
                    throw new DMException("操作重复！");
                }
                c.ExChange.Add(ec2);
            }


            c.ExChange.Add(ec);

            #endregion

            #region 删除订单相关活动购买数据记录

            //已够活动产品信息表
            List<ActivityProductPurchased> opappd = c.ActivityProductPurchased.Where(w => w.OrderID == id).ToList();
            foreach (ActivityProductPurchased item in opappd)
            {
                c.ActivityProductPurchased.Remove(item);
            }

            //已赠活动产品信息表
            List<ActivityGiftsPurchased> agpd = c.ActivityGiftsPurchased.Where(w => w.OrderID == id).ToList();
            foreach (ActivityGiftsPurchased item in agpd)
            {
                c.ActivityGiftsPurchased.Remove(item);
            }

            #endregion

            c.SaveChanges();
            cc.SaveChanges();
            if (ordertype.ID == (int)Enum.OrderType.注册单)
            {
                cc.Database.ExecuteSqlCommand("DELETE FROM [dbo].[PlaceRelation] where [DealerId]=@p0 DELETE FROM [dbo].[RecomRelation] where [DealerId]=@p0 ", userid);
            }

            return true;
        }

        public List<OrdersDTO> GetOrdersListDownload(Request_Order dto)
        {
            List<OrdersDTO> li = new List<OrdersDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Expression<Func<Order, bool>> expr = AutoAssemble.Splice<Order, Request_Order>(dto);
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }
                else
                {
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.已退款);
                    //已取消订单做删除处理，不显示
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.支付已取消);
                }
                if (dto.UserCode != null)
                {
                    expr = expr.And2(w => w.User.UserCode == dto.UserCode);
                }
                if (dto.UserName != null)
                {
                    expr = expr.And2(w => w.User.UserName == dto.UserName);
                }
                if (dto.Phone != null)
                {
                    expr = expr.And2(w => w.User.Phone == dto.Phone);
                }
                if (dto.STime != null)
                {
                    expr = expr.And2(w => w.AddTime >= dto.STime);
                }
                if (dto.ETime != null)
                {
                    expr = expr.And2(w => w.AddTime <= dto.ETime);
                }
                if (dto.je1 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct >= dto.je1);
                }
                if (dto.je2 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct < dto.je2);
                }
                if (dto.pv1 != null)
                {
                    expr = expr.And2(w => w.TotalPv >= dto.pv1);
                }
                if (dto.pv2 != null)
                {
                    expr = expr.And2(w => w.TotalPv < dto.pv2);
                }
                if (dto.IsPay != null)
                {
                    if (Convert.ToBoolean(dto.IsPay))
                    {
                        expr = expr.And2(w => w.State == 3 || w.State == 4);
                    }
                    else
                    {
                        expr = expr.And2(w => w.State != 3 && w.State != 4);
                    }
                }
                if (dto.IsDG != null)
                {
                    if (Convert.ToBoolean(dto.IsDG))
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                    else
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                }
                li = c.Order.Where(expr).OrderByDescending(px => px.AddTime).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        ConsigneeAddress = x.ConsigneeAddress,
                        ConsigneeName = x.ConsigneeName,
                        ConsigneePhone = x.ConsigneePhone,
                        LogisticsName = x.LogisticsName,
                        LogisticsNum = x.LogisticsNum,
                        LogisticsState = x.LogisticsState,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        UserID = x.UserID,
                        State = x.State,
                        IsBalance = x.IsBalance,

                        DeLevelName = x.User.DeLevel.Name,
                        UserCode = x.User.UserCode,
                        UserName = x.User.UserName,
                        UserPhone = x.User.Phone,

                        DealerCode = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().UserCode,
                        DealerName = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().UserName,
                        DealerPhone = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().Phone,

                        ServiceCenterCode = x.ServiceCenter.User.UserCode,
                        ServiceCenterName = x.ServiceCenter.ServiceCenterName,

                        UserPayMoney = x.UserPayMoney,
                        OtherPayMoney = x.OtherPayMoney,
                        TotalPv = x.TotalPv,
                        OrderTypeName = x.OrderType.OrderTypeName,
                        PayType = x.PayType,
                        OrderProduct = c.OrderProduct.Where(n => n.OrderID == x.ID).OrderBy(px => px.ShoppingProductType)
                        .Select(z => new OrderProductDTOQD
                        {
                            ID = z.ID,
                            ProductID = z.ProductID,
                            ProductNum = z.ProductNum,
                            ProductPrice = z.ProductPrice,
                            OrderID = z.OrderID,
                            Price = z.Product.Price,
                            ProductName = z.Product.ProductName,
                            ProductCode = z.Product.ProductCode,
                            Unit = z.Product.Unit,
                            ShoppingProductType = z.ShoppingProductType,
                            zt = z.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                        }).ToList()

                    }
                ).ToList();
            }

            return li;
        }

        /// <summary>
        /// 添加订单，积分直接下单并支付，带微信的支付仅下单
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userid"></param>
        /// <param name="deliverType"></param>
        /// <param name="ConsigneeName"></param>
        /// <param name="ConsigneePhone"></param>
        /// <param name="ConsigneeProvince"></param>
        /// <param name="ConsigneeCity"></param>
        /// <param name="ConsigneeCounty"></param>
        /// <param name="AddressInfo"></param>
        /// <param name="DealerId"></param>
        /// <param name="OrderTypeID"></param>
        /// <param name="ServiceCenterID"></param>
        /// <param name="DeLevelID"></param>
        /// <param name="PayType"></param>
        /// <param name="PCDealerCode"></param>
        /// <param name="PPDealerCode"></param>
        /// <param name="DeptName"></param>
        /// <param name="MoneyTransport"></param>
        /// <param name="AdminUserID"></param>
        /// <returns></returns>
        public OrdersModel AddOrders(ShoppingCartActivityDTO dto, Guid userid, int deliverType, string ConsigneeName, string ConsigneePhone, string ConsigneeProvince, string ConsigneeCity, string ConsigneeCounty, string AddressInfo,
           Guid DealerId, int OrderTypeID, Guid? ServiceCenterID, int? DeLevelID, int PayType, string PCDealerCode, string PPDealerCode, string DeptName, decimal? MoneyTransport, Guid? AdminUserID)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                #region 提交信息验证

                if (ServiceCenterID == null)
                {
                    throw new DMException("店铺不能为空！");
                }
                if (ConsigneeProvince == null || ConsigneeProvince == "")
                {
                    throw new DMException("收获省不能为空！");
                }
                if (DeLevelID == (int)Enum.DeLevel.创客 && OrderTypeID != (int)Enum.OrderType.主动消费单 && OrderTypeID != (int)Enum.OrderType.抢购单)
                {
                    if (string.IsNullOrEmpty(PPDealerCode) || string.IsNullOrEmpty(DeptName))
                    {
                        throw new DMException("安置信息不全！");
                    }
                    User PPDealer = c.User.Where(w => w.UserCode == PPDealerCode).FirstOrDefault();
                    if (PPDealer.UserState == 0)
                    {
                        throw new DMException("安置人未激活！");
                    }
                }


                #endregion

                User user = c.User.Where(w => w.ID == userid).FirstOrDefault();//订单所属人
                if (OrderTypeID == (int)Enum.OrderType.主动消费单)
                {
                    DeLevelID = user.DeLevelID;
                }
                if (!DeLevelID.HasValue)
                {
                    throw new DMException("级别不能为空！");
                }
                //是否代购
                bool isAgent = userid != DealerId ? true : false;

                User payUser; //订单付款人
                if (!isAgent)
                {
                    payUser = user;
                }
                else
                {
                    payUser = c.User.Where(w => w.ID == DealerId).FirstOrDefault();
                }

                //新建一个订单ID备用
                Guid orderid = Guid.NewGuid();
                //添加订单
                Order o = new Order();
                o.PCDealerCode = PCDealerCode;
                o.PPDealerCode = PPDealerCode;
                o.DeptName = DeptName;

                o.MoneyProduct = Convert.ToDecimal(CountProvince(dto.sclist)); //产品总价  
                o.MoneyTransport = MoneyTransport;//运费

                var orderMoney = Convert.ToDecimal(o.MoneyProduct + o.MoneyTransport);
                //积分向下取整
                var integralNumber = (int)orderMoney;

                //积分支付验证积分余额
                if (PayType == (int)Enum.PayType.积分 && payUser.EleMoney < integralNumber)
                {
                    throw new DMException("积分不足！");
                }

                #region 订单验证

                //订单总价
                o.OrderMoney = Convert.ToDecimal(o.MoneyProduct);
                if (o.OrderMoney == 0)
                {
                    throw new DMException("订单异常，产品金额为0！");
                }

                //若是注册单，验证注册单是否已经存在。
                //只要用户下了注册单，那怕没有支付也不能下另一个注册单，必须把现有的注册单取消才能下新的注册单。
                if (OrderTypeID == (int)Enum.OrderType.注册单)
                {
                    var registerOrder = FindRegistOrder(c, userid);
                    if (registerOrder != null && !string.IsNullOrEmpty(registerOrder.OrderNumber))
                    {
                        log.Info($"发现注册单[{registerOrder.OrderNumber}]");
                        throw new DMException($"注册单[{registerOrder.OrderNumber}]已存在，请删除后再重新下单！");
                    }
                }
                if (user.Source == null)
                {
                    user.Source = 2;
                }
                o.OrderTypeID = OrderTypeID;
                var firstOrder = FindFirstOrder(c, userid);
                if ((firstOrder == null || string.IsNullOrEmpty(firstOrder.OrderNumber)) && user.AddTime > Convert.ToDateTime("2019-9-17 00:00:00") && user.Source != 4)
                {
                    log.Info($"用户[id:{userid}]无注册单，现在开始创建首单即注册单");
                    o.OrderTypeID = (int)Enum.OrderType.注册单;//第一单默认注册单
                }

                //代购单验证
                if (isAgent)
                {
                    //Decimal? yypv = c.Order.Where(w => (w.State == 3 || w.State == 4) && (w.OrderTypeID == 1 || o.OrderTypeID == 2) && w.UserID == userid).ToList().Sum(s => s.MoneyProduct);

                    if (o.OrderTypeID == (int)Enum.OrderType.注册单 && user.Source != 4)//注册  
                    {
                        if (c.DeLevel.Where(w => w.ID == DeLevelID).FirstOrDefault().MinPV > (o.MoneyProduct))
                        {
                            throw new DMException("订单金额不足！");
                        }
                    }
                    if (o.OrderTypeID == (int)Enum.OrderType.升级单)//  升级
                    {
                        decimal? MinPV1 = c.DeLevel.Where(w => w.ID == DeLevelID).FirstOrDefault().MinPV;//基于只能给优惠顾客下升级单
                        decimal? MinPV2 = c.DeLevel.Where(w => w.ID == user.DeLevelID).FirstOrDefault().MinPV;//基于只能给优惠顾客下升级单
                        if ((MinPV1 - MinPV2) > (o.MoneyProduct))
                        {
                            throw new DMException("订单金额不足！");
                        }
                    }
                }

                #endregion

                //订单号
                string ycode = "";
                do
                {
                    ycode = "SC" + DateTime.Now.ToString("yyMMddHHmmssfff");
                } while (c.Order.FirstOrDefault(n => n.OrderNumber == ycode) != null);

                o.ConsigneeProvince = ConsigneeProvince;
                o.ConsigneeCity = ConsigneeCity;
                o.ConsigneeCounty = ConsigneeCounty;

                if (deliverType != (int)Enum.DeliverType.自提)
                {
                    o.ConsigneeAddress = AddressInfo;
                }
                else
                {
                    o.ConsigneeProvince = "四川省";
                    o.ConsigneeCity = "成都市";
                    o.ConsigneeCounty = "金牛区";
                    o.ConsigneeAddress = "环球广场";
                }

                o.AddTime = DateTime.Now;
                o.ID = orderid;
                o.UserID = userid;
                o.DealerId = DealerId;

                o.ServiceCenterID = ServiceCenterID;
                o.DeLevelID = DeLevelID;
                o.OldDeLevelID = user.DeLevelID;
                o.OrderNumber = ycode;
                o.OrderMoney = orderMoney;
                o.AddTime = DateTime.Now;
                o.State = (int)Enum.OrderState.待付款;
                o.ConsigneeName = ConsigneeName;
                o.ConsigneePhone = ConsigneePhone;
                o.TotalPv = Convert.ToDecimal(CountPV(dto.sclist, c));
                o.IsBalance = 0;
                o.DeliverType = deliverType;
                o.IsAppraise = false;
                //15秒后，待付款订单后台任务自动进行二次确认
                o.NextWxSynTime = DateTime.Now.AddSeconds(15);

                //o.LogisticsName = LogisticsName;
                //o.LogisticsNum = LogisticsNum;
                //o.LogisticsState = LogisticsState;
                //o.IsBak = IsBak;      
                //o.BalanceNumber = BalanceNumber;
                //实际的支付方式。若是混合支付，若全是用积分进行支付就直接进行支付处理
                int actualPayType = PayType;
                o.PayType = PayType;
                if (o.PayType == (int)Enum.PayType.积分)                     //积分支付
                {
                    o.UserPayMoney = integralNumber;      //积分支付金额
                    o.OtherPayMoney = 0;                //第三方支付金额
                }
                if (o.PayType == (int)Enum.PayType.微信 || o.PayType == (int)Enum.PayType.快钱)   //微信快钱支付
                {
                    o.UserPayMoney = 0;                 //积分支付金额
                    o.OtherPayMoney = o.OrderMoney;     //第三方支付金额
                }
                if (o.PayType == (int)Enum.PayType.微信加积分 || o.PayType == (int)Enum.PayType.快钱加积分)   //混合支付
                {
                    if (payUser.EleMoney > o.OrderMoney)
                    {
                        o.UserPayMoney = integralNumber;    //积分支付金额，向下取整
                        o.OtherPayMoney = 0;                //第三方支付金额
                    }
                    else
                    {
                        o.UserPayMoney = payUser.EleMoney;                  //积分支付金额
                        o.OtherPayMoney = o.OrderMoney - o.UserPayMoney;   //第三方支付金额
                    }
                    if (o.OtherPayMoney == 0)
                    {
                        actualPayType = (int)Enum.PayType.积分;
                    }
                }

                //混合支付,冻结积分，返回订单号给客户端，唤起支付
                if (o.OtherPayMoney > 0 && o.UserPayMoney > 0)
                {
                    payUser.EleMoney = payUser.EleMoney - o.UserPayMoney.Value;
                    payUser.EleMoneyFrozen = payUser.EleMoneyFrozen + o.UserPayMoney.Value;
                }

                //1、全部都是使用积分支付，直接扣除用户的积分
                //2、直接进行支付成功处理
                if (actualPayType == (int)Enum.PayType.积分)
                {
                    payUser.EleMoney = payUser.EleMoney - o.UserPayMoney.Value;
                    PaySuccess(c, o);
                }

                //o.UserCouponsPay = UserCouponsPay;    //优惠券优惠金额

                c.Order.Add(o);

                //添加子订单
                foreach (ShoppingCartDTO item in dto.sclist)
                {
                    Product pt = c.Product.FirstOrDefault(n => n.ID == item.ProductID);

                    //验证库存状态
                    if (pt.StockNumber == 0)
                    {
                        throw new DMException($"[{pt.ProductName}]产品缺货，请从购物车删除产品后重新下单");
                    }

                    //验证商品是否下架
                    if (!pt.State)
                    {
                        throw new DMException($"[{pt.ProductName}]产品已下架，请重新选购！");
                    }

                    OrderProduct op = new OrderProduct();
                    op.ID = Guid.NewGuid();
                    op.OrderID = orderid;
                    op.ProductNum = item.Num;
                    op.ProductID = item.ProductID;
                    op.ProductPrice = item.ShoppingProductPrice;
                    op.ShoppingProductType = item.ShoppingProductType;
                    if (c.OrderProduct.Where(w => w.OrderID == orderid && w.ProductID == item.ProductID && w.ShoppingProductType == item.ShoppingProductType) != null)
                    {
                        c.OrderProduct.Add(op);
                    }

                    //删除购物车
                    ShoppingCart sc = c.ShoppingCart.FirstOrDefault(n => n.ID == item.ID);
                    if (sc != null)
                    {
                        c.ShoppingCart.Remove(sc);
                    }
                }
                foreach (ActivityProductPurchasedTT item in dto.applist)
                {
                    ActivityProductPurchased app = new ActivityProductPurchased();
                    app.ID = Guid.NewGuid();
                    app.ActivityID = item.ActivityID;
                    app.ProducID = item.ProducID;
                    app.ProducNum = item.ProducNum;
                    app.UserID = item.UserID;
                    app.OrderID = orderid;
                    c.ActivityProductPurchased.Add(app);

                }
                foreach (ActivityGiftsPurchasedTT item in dto.agplist)
                {
                    ActivityGiftsPurchased agp = new ActivityGiftsPurchased();
                    agp.ID = Guid.NewGuid();
                    agp.ActivityID = item.ActivityID;
                    agp.GiftsID = item.GiftsID;
                    agp.ProducNum = item.ProducNum;
                    agp.UserID = userid;
                    agp.OrderID = orderid;
                    c.ActivityGiftsPurchased.Add(agp);
                }

                if (AdminUserID != null)
                {
                    AdminUser au = c.AdminUser.Where(w => w.ID == AdminUserID).FirstOrDefault();
                    AdminUserOperationLog auol = new AdminUserOperationLog();
                    auol.ID = Guid.NewGuid();
                    auol.AddTime = o.AddTime;
                    auol.AdminUserCode = au.UserCode;
                    auol.AdminUserID = au.ID;
                    auol.MoneyProduct = o.MoneyProduct;
                    auol.MoneyTransport = o.MoneyTransport;
                    auol.OrderMoney = o.OrderMoney;
                    auol.NewLevel = o.DeLevelID.ToString();
                    auol.OldLevel = o.OldDeLevelID.ToString();
                    auol.OperationType = o.OrderTypeID;
                    auol.UserCode = user.UserCode;
                    auol.OrderNumber = o.OrderNumber;
                    auol.OrderTypeName = c.OrderType.FirstOrDefault(w => w.ID == o.OrderTypeID).OrderTypeName;
                    auol.PCDealerCode = o.PCDealerCode;
                    auol.PPDealerCode = o.PPDealerCode;
                    auol.DeptName = o.DeptName;
                    c.AdminUserOperationLog.Add(auol);
                }

                c.SaveChanges();
                OrdersModel order = new OrdersModel
                {
                    OrderNumber = ycode,
                    PayType = actualPayType//返回实际的支付方式，若混合支付仅仅使用积分就只返回积分支付
                };
                return order;
            }
        }

        /// <summary>
        /// 直接取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="manualRefundComment">人工退款备注</param>
        public void CancelOrder(Guid id, Enum.OrderState state, string manualRefundComment = "")
        {
            if (state != Enum.OrderState.支付已取消 && state != Enum.OrderState.已退款)
            {
                throw new DMException("订单必须设置成取消或者已付款状态");
            }
            deOrders(id, null);
            ChangeOrderState(state, id, manualRefundComment);
        }

        /// <summary>
        /// 取消待付款订单
        /// </summary>
        /// <param name="id"></param>
        public bool CancelPayingOrder(Guid id)
        {
            Order order = null;
            {
                try
                {
                    using (BwslRetailEntities c = new BwslRetailEntities())
                    {
                        try
                        {
                            order = c.Order.FirstOrDefault(n => n.ID == id);
                            if (order == null)
                            {
                                throw new DMException($"订单不存在");
                            }
                            if (order.State != (int)Enum.OrderState.待付款)
                            {
                                throw new OrderNoNeedPayException(order.State, $"订单[{order.OrderNumber}]不是待付款状态，不能删除！");
                            }
                            OrderConfirmByWx(c, order);
                        }
                        finally
                        {
                            c.SaveChanges();
                        }
                    }
                }
                catch (OrderNoNeedPayException ce)
                {
                    if (ce.Status != (int)Enum.OrderState.支付已取消)
                    {
                        throw new DMException($"{ce.Message}，无法删除!");
                    }
                    else
                    {
                        log.Info($"订单号[{order.OrderNumber}]，{ce.Message}");
                    }
                }
                catch (WxOrderNotPayException)
                {
                    log.Info($"订单[{order.OrderNumber}]在微信财务通平台没有支付，直接做取消操作!");
                    PayCancelAll(order.OrderNumber);
                }
                catch (WxOrderCloseException)
                {
                    //对订单做取消处理
                    log.Info($"订单[{order.OrderNumber}]在微信财务通平台已关闭，直接做取消操作!");
                    PayCancel(order.OrderNumber);
                }
                catch (WxOrderNotExistsException)
                {
                    log.Info($"订单[{order.OrderNumber}]在微信财务通平台不存在，直接做取消操作!");
                    PayCancel(order.OrderNumber);
                }
            }
            return true;
        }

        /// <summary>
        /// 变更订单状态
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="id"></param>
        /// <param name="manualRefundComment">人工退款备注</param>
        /// <returns></returns>
        public void ChangeOrderState(Enum.OrderState newState, Guid id, string manualRefundComment = "")
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Order order = c.Order.Where(w => w.ID == id).FirstOrDefault();
                if (order == null)
                {
                    throw new DMException("订单不存在！");
                }
                order.State = (int)newState;
                if (newState == Enum.OrderState.已退款)
                {
                    order.ManualRefundComment = manualRefundComment;
                }
                c.SaveChanges();
            }
        }

        /// <summary>
        /// 变更订单的订单号，调用该方式时请和微信已经做了二次确认处理
        /// </summary>
        public string ChangeOrderNumber(string oldOrderNumber)
        {
            //订单号
            string ycode = "";
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Order order = c.Order.Where(w => w.OrderNumber == oldOrderNumber).FirstOrDefault();
                if (order.State != (int)Enum.OrderState.待付款)
                {
                    throw new OrderNoNeedPayException(order.State, $"订单[{order.OrderNumber}]不是待付款状态，不能变更订单号！");
                }

                do
                {
                    ycode = "SC" + DateTime.Now.ToString("yyMMddHHmmssfff");
                } while (c.Order.FirstOrDefault(n => n.OrderNumber == ycode) != null);
                order.OrderNumber = ycode;
                c.SaveChanges();
            }
            return ycode;
        }

        /// <summary>
        /// 修改下一步微信主动同步的时间
        /// </summary>
        /// <param name="dt"></param>
        public void ChangeWxSynTime(DateTime dt, Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Order order = c.Order.Where(w => w.ID == id).FirstOrDefault();
                order.NextWxSynTime = dt;
                c.SaveChanges();
            }
        }

        /// <summary>
        /// 退款失败后的处理
        /// </summary>
        /// <param name="orderNumber"></param>
        public void RefundFail(string orderNumber)
        {
            Order order = null;
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                RefundFail(c, order);
                c.SaveChanges();
            }
        }
        /// <summary>
        /// 退款失败后的处理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public void RefundFail(BwslRetailEntities c, Order order)
        {
            #region 订单验证

            if (order.State != (int)Enum.OrderState.待退款)
            {
                throw new DMException($"订单[{order.OrderNumber}]退款失败操作失败，订单不是带退款状态！");
            }

            #endregion

            order.State = (int)Enum.OrderState.退款失败;
            order.RefundEndTime = DateTime.Now;
        }

        /// <summary>
        /// 退款成功后的处理
        /// </summary>
        /// <param name="orderNumber"></param>
        public void RefundSuccess(string orderNumber, string refundRecvAccout)
        {
            Order order = null;
            //using (var scope = new TransactionScope())
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                    RefundSuccess(c, order, refundRecvAccout);
                    c.SaveChanges();
                }
                deOrders(order.ID, null);
                //scope.Complete();
            }
        }
        /// <summary>
        /// 退款成功后的处理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public void RefundSuccess(BwslRetailEntities c, Order order, string refundRecvAccout)
        {
            #region 订单验证

            if (order.State != (int)Enum.OrderState.待退款)
            {
                throw new DMException($"订单[{order.OrderNumber}]退款操作失败，订单不是待退款状态！");
            }

            #endregion

            order.State = (int)Enum.OrderState.已退款;
            order.RefundEndTime = DateTime.Now;
            order.RefundRecvAccout = refundRecvAccout;
        }


        /// <summary>
        /// 支付成功后的处理
        /// </summary>
        /// <param name="orderNumber"></param>
        public void PaySuccess(string orderNumber)
        {
            Order order = null;
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                PaySuccess(c, order);
                c.SaveChanges();
            }
        }
        /// <summary>
        /// 支付成功后的处理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public void PaySuccess(BwslRetailEntities c, Order order)
        {
            #region 订单验证
            if (order.State != (int)Enum.OrderState.待付款)
            {
                throw new OrderNoNeedPayException(order.State, $"订单[{order.OrderNumber}]已支付！");
            }
            #endregion

            OrderType ordertype = c.OrderType.Where(w => w.ID == order.OrderTypeID).FirstOrDefault();

            User user = c.User.Where(w => w.ID == order.UserID).FirstOrDefault();//订单所属人

            User payUser;//订单扣款人
            if (order.UserID == order.DealerId)
            {
                payUser = user;
            }
            else
            {
                //代购业务
                payUser = c.User.Where(w => w.ID == order.DealerId).FirstOrDefault();
                if (user.UserState == 0)
                {
                    user.UserState = 1;//激活
                }
            }

            if (order.OrderTypeID == (int)Enum.OrderType.升级单)
            {
                user.DeLevelID = (int)order.DeLevelID;
            }
            //做上待结算标志，由后台任务专门处理结算，目前仅处理安置关系
            if (order.DeLevelID >= (int)Enum.DeLevel.创客 && order.OrderTypeID != (int)Enum.OrderType.主动消费单)
            {
                order.SettleFlag = (int)Enum.SettleFlag.待结算;
            }
            order.State = (int)Enum.OrderState.待发货;
            //使用了混合支付，扣除冻结的用户积分
            if (order.OtherPayMoney > 0 && order.UserPayMoney > 0)
            {
                payUser.EleMoneyFrozen = payUser.EleMoneyFrozen - order.UserPayMoney.Value;
            }

            #region 活跃期处理

            if ((user.DeLevelID >= (int)Enum.DeLevel.创客 || order.DeLevelID >= (int)Enum.DeLevel.创客) && order.OrderTypeID != (int)Enum.OrderType.抢购单)
            {
                ActivePeriodRecord apr = new ActivePeriodRecord();
                apr.ID = Guid.NewGuid();
                apr.UserID = user.ID;
                apr.UserCode = user.UserCode;
                apr.UserName = user.UserName;
                apr.RecordType = order.OrderTypeID;

                apr.StageOne = user.StageCount;
                if (order.OrderTypeID == (int)Enum.OrderType.注册单)
                {
                    apr.StageTwo = 4;
                    apr.PVSurplus = user.PVSurplus;
                }
                if (order.OrderTypeID == (int)Enum.OrderType.升级单)
                {
                    apr.StageTwo = 4;
                    apr.PVSurplus = user.PVSurplus;
                }
                if (order.OrderTypeID == (int)Enum.OrderType.主动消费单)
                {
                    apr.StageTwo = (int)Math.Floor((user.PVSurplus + Convert.ToDecimal(order.MoneyProduct)) / hyqnum);
                    apr.PVSurplus = Convert.ToInt32((user.PVSurplus + order.MoneyProduct) % hyqnum);
                }
                apr.StageThree = apr.StageOne + apr.StageTwo;
                apr.AddTime = DateTime.Now;
                apr.Remarks = "下单增加。";
                apr.OrderNumber = order.OrderNumber;

                c.ActivePeriodRecord.Add(apr);

                user.StageCount = Convert.ToInt32(apr.StageThree);
                user.PVSurplus = apr.PVSurplus;
            }

            #endregion

            #region 流水

            ExChange ec = new ExChange();
            ec.ID = Guid.NewGuid();
            ec.UserID = payUser.ID;
            ec.UserCode = payUser.UserCode;
            ec.UserName = payUser.UserName;
            ec.ChangeTime = DateTime.Now;
            ec.ChangeMarks = "下单支付！";
            ec.OrderNum = order.OrderNumber;
            ec.State = 0;
            ec.ExChangeType = 3;

            if (order.PayType == (int)Enum.PayType.积分)//积分支付
            {
                ec.MoneyType = 1;
                ec.ZMoneyType = 5;
                //下单就扣积分
                ec.BeforeChangeMoney = payUser.EleMoney + order.UserPayMoney.Value;
                ec.ChangeMoney = 0 - order.UserPayMoney.Value;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
            }
            if (order.PayType == (int)Enum.PayType.微信)//微信支付
            {
                ec.MoneyType = 2;
                ec.ZMoneyType = 7;

                ec.BeforeChangeMoney = 0;
                ec.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
            }
            if (order.PayType == (int)Enum.PayType.快钱)//快钱支付
            {
                ec.MoneyType = 3;
                ec.ZMoneyType = 10;

                ec.BeforeChangeMoney = 0;
                ec.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
            }
            if (order.PayType == (int)Enum.PayType.微信加积分)//微信加积分
            {
                //积分支付部分
                ec.MoneyType = 1;
                ec.ZMoneyType = 5;
                //下单就冻结积分，支付成功真正扣除积分
                ec.BeforeChangeMoney = payUser.EleMoney + order.UserPayMoney.Value;
                ec.ChangeMoney = 0 - order.UserPayMoney.Value;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

                //微信支付部分
                ExChange ec2 = new ExChange();
                ec2.ID = Guid.NewGuid();
                ec2.UserID = payUser.ID;
                ec2.UserCode = payUser.UserCode;
                ec2.UserName = payUser.UserName;
                ec2.ChangeTime = DateTime.Now;
                ec2.ChangeMarks = "下单支付！";
                ec2.OrderNum = order.OrderNumber;
                ec2.State = 0;
                ec2.ExChangeType = 3;

                ec2.MoneyType = 2;
                ec2.ZMoneyType = 7;
                ec2.BeforeChangeMoney = 0;
                ec2.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                ec2.AfterChangeMoney = ec2.BeforeChangeMoney + ec2.ChangeMoney;

                c.ExChange.Add(ec2);
            }
            if (order.PayType == (int)Enum.PayType.快钱加积分)//快钱加积分
            {
                //积分支付部分
                ec.MoneyType = 1;
                ec.ZMoneyType = 5;
                //下单就冻结积分，支付成功真正扣除积分
                ec.BeforeChangeMoney = payUser.EleMoney + order.UserPayMoney.Value;
                ec.ChangeMoney = 0 - order.UserPayMoney.Value;
                ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

                //快钱支付部分
                ExChange ec2 = new ExChange();
                ec2.ID = Guid.NewGuid();
                ec2.UserID = payUser.ID;
                ec2.UserCode = payUser.UserCode;
                ec2.UserName = payUser.UserName;
                ec2.ChangeTime = DateTime.Now;
                ec2.ChangeMarks = "下单支付！";
                ec2.OrderNum = order.OrderNumber;
                ec2.State = 0;
                ec2.ExChangeType = 3;

                ec2.MoneyType = 3;
                ec2.ZMoneyType = 10;
                ec2.BeforeChangeMoney = 0;
                ec2.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                ec2.AfterChangeMoney = ec2.BeforeChangeMoney + ec2.ChangeMoney;

                c.ExChange.Add(ec2);
            }

            c.ExChange.Add(ec);

            #endregion

            //TODO，建议把要发送的短信内容保存到表里，由后台短信发送任务专门来发送。
            if (order.OrderTypeID == (int)Enum.OrderType.注册单)
            {
                SmsUtility.SendWelcomeSMS(user.Phone, order.OrderTypeID, user.UserCode, user.UserName);
            }
        }

        /// <summary>
        /// 支付失败后的处理
        /// </summary>
        /// <param name="orderNumber"></param>
        public void PayFail(string orderNumber)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Order order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                PayFail(c, order);
                c.SaveChanges();
            }
        }

        /// <summary>
        /// 支付失败后的处理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public void PayFail(BwslRetailEntities c, Order order)
        {
            CloseOrder(c, order, (int)Enum.OrderState.支付失败);
        }

        /// <summary>
        /// 微信端关闭，系统内部订单的支付做取消处理
        /// </summary>
        /// <param name="orderNumber"></param>
        public void PayCancel(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                throw new DMException("订单号不能为空！");
            }
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Order order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                PayCancel(c, order);
                c.SaveChanges();
            }
        }

        /// <summary>
        /// 微信端关闭，系统内部订单进行取消处理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public void PayCancel(BwslRetailEntities c, Order order)
        {
            CloseOrder(c, order, (int)Enum.OrderState.支付已取消);
        }

        /// <summary>
        /// 同时取消系统和微信端的订单
        /// </summary>
        /// <param name="c"></param>
        /// <param name="order"></param>
        public void PayCancelAll(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                throw new DMException("订单号不能为空！");
            }
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Order order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                PayCancelAll(c, order);
                c.SaveChanges();
            }
        }

        /// <summary>
        /// 同时取消系统和微信端的订单
        /// </summary>
        /// <param name="c"></param>
        /// <param name="order"></param>
        public void PayCancelAll(BwslRetailEntities c, Order order)
        {
            var req = new WxPayData();
            req.OutTradeNo = order.OrderNumber;
            WxPayApi.CloseOrder(req);
            //取消系统中的订单
            PayCancel(c, order);
        }

        /// <summary>
        /// 关闭订单，并退还冻结的积分
        /// </summary>
        /// <param name="c"></param>
        /// <param name="order"></param>
        /// <param name="status"></param>
        private void CloseOrder(BwslRetailEntities c, Order order, int status)
        {
            #region 订单验证

            if (order == null)
            {
                throw new DMException($"订单不存在！");
            }

            if (order.State != (int)Enum.OrderState.待付款)
            {
                throw new OrderNoNeedPayException(order.State, $"订单[{order.OrderNumber}]不是待付款状态，不能做关闭处理！");
            }

            #endregion

            User user = c.User.Where(w => w.ID == order.UserID).FirstOrDefault();//订单所属人

            User payUser;//订单扣款人
            if (order.UserID == order.DealerId)
            {
                payUser = user;
            }
            else
            {
                //代购业务
                payUser = c.User.Where(w => w.ID == order.DealerId).FirstOrDefault();
            }

            order.State = status;

            //使用了混合支付，归还冻结的用户积分
            if (order.OtherPayMoney > 0 && order.UserPayMoney > 0)
            {
                payUser.EleMoneyFrozen = payUser.EleMoneyFrozen - order.UserPayMoney.Value;
                payUser.EleMoney = payUser.EleMoney + order.UserPayMoney.Value;
            }
        }

        /// <summary>
        /// 通过微信对订单进行二次确认
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public void OrderConfirmByWx(string orderNumber)
        {
            Order order = null;
            try
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    try
                    {
                        order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                        OrderConfirmByWx(c, order);
                    }
                    finally
                    {
                        c.SaveChanges();
                    }
                }
            }
            //可以不做这两个异常处理，订单支付后台任务下一轮会自动处理
            /*
            catch(OrderPaidException pe)
            {
                //进行订单已支付的补充处理
                PaySuccess(orderNumber);
            }
            catch(OrderCloseException ce)
            {
                //对订单做取消处理
                PayCancel(orderNumber);
            }
            */
            //调用下单接口不会出现这种情况的异常
            catch (WxOrderNotExistsException)
            {
                log.Info($"订单号[{orderNumber}]在微信不存在!");
                //系统自动把订单做关闭处理
                if (!CanAutoCancel(order))
                {
                    throw new WxOrderNotExistsException();
                }
                PayCancel(orderNumber);
            }
        }

        /// <summary>
        /// 通过微信对订单进行二次确认
        /// </summary>
        /// <param name="c"></param>
        /// <param name="order"></param>
        public void OrderConfirmByWx(BwslRetailEntities c, Order order)
        {
            var orderNumber = order.OrderNumber;
            WxPayData req = new WxPayData();
            req.OutTradeNo = orderNumber;
            WxPayData res = WxPayApi.OrderQuery(req);
            switch (res.TradeState)
            {
                case WxPayApi.ORDERQUERY_TRADE_STATE_SUCCESS:
                    PaySuccess(c, order);
                    throw new OrderNoNeedPayException((int)Enum.OrderState.订单完成, $"订单[{orderNumber}]已支付");
                case WxPayApi.ORDERQUERY_TRADE_STATE_REFUND:
                    //不做处理，由退款业务负责处理
                    throw new OrderNoNeedPayException((int)Enum.OrderState.待退款, $"订单[{orderNumber}]正在退款");
                case WxPayApi.ORDERQUERY_TRADE_STATE_NOTPAY:
                    //先调用微信[关闭订单]接口关闭订单，再取消系统中的订单
                    if (!CanAutoCancel(order))
                    {
                        //抛出异常由最终端决定该如何做业务处理
                        throw new WxOrderNotPayException();
                    }
                    PayCancelAll(c, order);
                    throw new OrderNoNeedPayException((int)Enum.OrderState.支付已取消, $"订单[{orderNumber}]已取消");
                case WxPayApi.ORDERQUERY_TRADE_STATE_CLOSED:
                    PayCancel(c, order);
                    throw new OrderNoNeedPayException((int)Enum.OrderState.支付已取消, $"订单[{orderNumber}]已取消");
                case WxPayApi.ORDERQUERY_TRADE_STATE_PAYERROR:
                    PayFail(c, order);
                    throw new OrderNoNeedPayException((int)Enum.OrderState.支付失败, $"订单[{orderNumber}]支付失败");
            }
        }

        /// <summary>
        /// 对订单进行结算
        /// </summary>
        public void BalanceAccount(Guid id)
        {
            string phone = "";
            string userCode = "";
            //using (var scope = new TransactionScope())
            {
                using (BwslRetailEntities c = new BwslRetailEntities())
                {
                    using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                    {
                        try
                        {
                            Order order = c.Order.Where(w => w.ID == id).FirstOrDefault();

                            order.SettleFlag = (int)Enum.SettleFlag.结算完成;
                            order.SettleTime = DateTime.Now;

                            #region 安置处理

                            if (order.DeLevelID >= (int)Enum.DeLevel.创客 && order.OrderTypeID != (int)Enum.OrderType.主动消费单)
                            {
                                if (!string.IsNullOrEmpty(order.PPDealerCode) && !string.IsNullOrEmpty(order.DeptName))
                                {

                                    var dealerUser = c.User.Where(w => w.ID == order.DealerId).FirstOrDefault();
                                    var user = c.User.Where(w => w.ID == order.UserID).FirstOrDefault();//订单所属人
                                    phone = dealerUser.Phone;
                                    userCode = user.UserCode;

                                    #region 下滑
                                    UserXiahua uxh = cc.Database.SqlQuery<UserXiahua>(" Exec Pro_Get_RegCust_ManageNode @p0,@p1", order.PCDealerCode, order.DeptName).FirstOrDefault();
                                    if (cc.PlaceRelation.Where(w => w.DealerId == user.ID).FirstOrDefault() != null)
                                    {
                                        throw new DMException("安置关系已经存在！");
                                    }
                                    User PPDealer = c.User.Where(w => w.UserCode == uxh.ManageDealerId).FirstOrDefault();

                                    #endregion
                                    #region 非下滑

                                    //UserABqu uxh = cc.Database.SqlQuery<UserABqu>("   Exec   Pro_Get_AreaNameByManageCode @p0,@p1", order.PCDealerCode, order.PPDealerCode).FirstOrDefault();
                                    //if (uxh.Tag == -1)
                                    //{
                                    //    order.SettleFlag = (int)Enum.SettleFlag.结算失败;
                                    //    order.SettleTime = DateTime.Now;

                                    //    #region 自动进行订单退款

                                    //    if (order.OtherPayMoney > 0)
                                    //    {
                                    //        //调用第三方接口发起退款流程
                                    //        if (order.PayType == (int)Enum.PayType.微信 || order.PayType == (int)Enum.PayType.微信加积分)
                                    //        {
                                    //            WxRefund(order.OrderNumber);
                                    //            log.Info($"订单[{order.OrderNumber}]已自动发起退款流程，退款成功后系统将自动删除订单！");
                                    //        }
                                    //        else
                                    //        {
                                    //            //TODO
                                    //            throw new DMException("该通道暂时不支持自动发起退款流程！");
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        CancelOrder(order.ID, Enum.OrderState.支付已取消);
                                    //        log.Info($"订单[{order.OrderNumber}]自动取消！");
                                    //    }

                                    //    //TODO，应该把短信记录到短信发送表里，由后台任务专门负责发送短信
                                    //    SmsUtility.SendAutoRefund(phone, userCode);
                                    //    log.Info($"订单[{order.OrderNumber}]自动结束失败，失败原因{uxh.ErrMessage}！");

                                    //    #endregion
                                    //    return;
                                    //}
                                    //User PPDealer = c.User.Where(w => w.UserCode == order.PPDealerCode).FirstOrDefault();

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

                        }
                        finally
                        {
                            c.SaveChanges();
                            cc.SaveChanges();
                            //scope.Complete();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 返回用户的注册单
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Order FindRegistOrder(BwslRetailEntities dbContext, Guid userId)
        {
            //若是注册单，验证是否注册单已经存在
            return dbContext.Order.Where(w => w.UserID == userId && w.State != (int)NewMK.Domian.Enum.OrderState.支付已取消 &&
                   w.State != (int)NewMK.Domian.Enum.OrderState.已退款 && w.OrderTypeID == (int)Enum.OrderType.注册单).FirstOrDefault();
        }

        /// <summary>
        /// 返回用户首单
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Order FindFirstOrder(BwslRetailEntities dbContext, Guid userId)
        {
            //若是注册单，验证是否注册单已经存在
            return dbContext.Order.Where(w => w.UserID == userId && w.State != (int)NewMK.Domian.Enum.OrderState.支付已取消 &&
                    w.State != (int)NewMK.Domian.Enum.OrderState.已退款).FirstOrDefault();
        }

        public List<DataCountDTO> GetPro_Fl_Dialy_Order_Count(string time1, string time2)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                return c.Database.SqlQuery<DataCountDTO>("   Exec    Pro_Fl_Dialy_Order_Count @p0,@p1", time1, time2).ToList();
            }
        }

        public OrderStatrMoney GetOrderStatrMoney(int type, Guid userid)
        {
            OrderStatrMoney model = new OrderStatrMoney();

            List<OrderStatrMoney> li = new List<OrderStatrMoney>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                expr = expr.And2(w => w.User.ID == userid);

                expr = expr.And2(w => w.State == 3 || w.State == 4);

                if (type == 0)
                {
                    expr = expr.And2(w => w.OrderTypeID == 1 || w.OrderTypeID == 2);
                }
                if (type == 1)
                {
                    expr = expr.And2(w => w.OrderTypeID == 1);
                }
                if (type == 2)
                {
                    expr = expr.And2(w => w.OrderTypeID == 2);
                }

                li = c.Order.Where(expr).Select(
                    x => new OrderStatrMoney
                    {
                        MoneyProduct = x.MoneyProduct,
                        TotalPv = x.TotalPv,
                    }
                ).ToList();


                model.MoneyProduct = li.Sum(s => s.MoneyProduct);
                model.TotalPv = li.Sum(s => s.TotalPv);
            }

            return model;
        }

        public bool GetOrderPay(string orderNumber)
        {
            bool li = false;
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                int state = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault().State;
                if (state == 3)
                {
                    li = true;
                }
            }

            return li;
        }


        public double CountPV(List<ShoppingCartDTO> ProductList, BwslRetailEntities c)
        {
            double zPV = 0;
            foreach (ShoppingCartDTO item in ProductList)
            {
                if (item.ShoppingProductType == 1 || item.ShoppingProductType == 3)
                {
                    Decimal dpv = c.Product.Where(w => w.ID == item.ProductID).FirstOrDefault().PV;
                    zPV += Convert.ToDouble(dpv) * Convert.ToDouble(item.Num);
                }


            }
            return zPV;
        }

        public List<OrdersDTO> GetOrdersList(int? strate, Guid? userid, int pageSize, int pageIndex, out int count)
        {
            List<OrdersDTO> li = new List<OrdersDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                expr = expr.And2(w => w.UserID == userid);
                if (strate != null)
                {
                    expr = expr.And2(w => w.State == strate);
                }
                else
                {
                    //已取消订单做删除处理，不显示
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.支付已取消);
                }

                count = c.Order.Where(expr).Count();
                li = c.Order.Where(expr).OrderByDescending(px => px.AddTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        ConsigneeAddress = x.ConsigneeAddress,
                        ConsigneeName = x.ConsigneeName,
                        ConsigneePhone = x.ConsigneePhone,
                        LogisticsName = x.LogisticsName,
                        LogisticsNum = x.LogisticsNum,
                        LogisticsState = x.LogisticsState,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        UserID = x.UserID,
                        State = x.State,
                        PayType = x.PayType,
                        OrderTypeName = x.OrderType.OrderTypeName,
                        OrderProduct = c.OrderProduct.Where(n => n.OrderID == x.ID).OrderBy(px => px.ShoppingProductType)
                        .Select(z => new OrderProductDTOQD
                        {
                            ID = z.ID,
                            ProductID = z.ProductID,
                            ProductNum = z.ProductNum,
                            ProductPrice = z.ProductPrice,
                            OrderID = z.OrderID,
                            Price = z.Product.Price,
                            ProductName = z.Product.ProductName,
                            ProductCode = z.Product.ProductCode,
                            Unit = z.Product.Unit,
                            ShoppingProductType = z.ShoppingProductType,
                            zt = z.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                        }).ToList()

                    }
                ).ToList();
            }

            return li;
        }
        public List<OrdersDTO> GetDealerOrdersList(int? strate, Guid? userid, int pageSize, int pageIndex, out int count)
        {
            List<OrdersDTO> li = new List<OrdersDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                expr = expr.And2(w => w.DealerId == userid && w.DealerId != w.UserID);
                if (strate != null)
                {
                    expr = expr.And2(w => w.State == strate);
                }
                else
                {
                    //已取消订单做删除处理，不显示
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.支付已取消);
                }

                count = c.Order.Where(expr).Count();
                li = c.Order.Where(expr).OrderByDescending(px => px.AddTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        ConsigneeAddress = x.ConsigneeAddress,
                        ConsigneeName = x.ConsigneeName,
                        ConsigneePhone = x.ConsigneePhone,
                        LogisticsName = x.LogisticsName,
                        LogisticsNum = x.LogisticsNum,
                        LogisticsState = x.LogisticsState,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        UserID = x.UserID,
                        State = x.State,

                        OrderProduct = c.OrderProduct.Where(n => n.OrderID == x.ID).OrderBy(px => px.ShoppingProductType)
                        .Select(z => new OrderProductDTOQD
                        {
                            ID = z.ID,
                            ProductID = z.ProductID,
                            ProductNum = z.ProductNum,
                            ProductPrice = z.ProductPrice,
                            OrderID = z.OrderID,
                            Price = z.Product.Price,
                            ProductName = z.Product.ProductName,
                            ProductCode = z.Product.ProductCode,
                            Unit = z.Product.Unit,
                            ShoppingProductType = z.ShoppingProductType,
                            zt = z.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                        }).ToList()

                    }
                ).ToList();
            }

            return li;
        }

        public List<OrdersDTO> GetOrdersList(Request_Order dto, out int count)
        {
            List<OrdersDTO> li = new List<OrdersDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                // System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                //if (dto.OrderNumber != null)
                //{
                //    expr = expr.And2(w => w.OrderNumber == dto.OrderNumber);
                //}

                Expression<Func<Order, bool>> expr = AutoAssemble.Splice<Order, Request_Order>(dto);
                //if (dto.OrderNumber != null)
                //{
                //    expr = expr.And2(w => w.OrderNumber == dto.OrderNumber);
                //}
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }
                else
                {
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.已退款);
                    //已取消订单做删除处理，不显示
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.支付已取消);
                }
                if (dto.UserCode != null)
                {
                    expr = expr.And2(w => w.User.UserCode == dto.UserCode);
                }
                if (dto.UserName != null)
                {
                    expr = expr.And2(w => w.User.UserName == dto.UserName);
                }
                if (dto.Phone != null)
                {
                    expr = expr.And2(w => w.User.Phone == dto.Phone);
                }
                if (dto.STime != null)
                {
                    expr = expr.And2(w => w.AddTime >= dto.STime);
                }
                if (dto.ETime != null)
                {
                    expr = expr.And2(w => w.AddTime <= dto.ETime);
                }

                if (dto.je1 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct >= dto.je1);
                }
                if (dto.je2 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct < dto.je2);
                }
                if (dto.pv1 != null)
                {
                    expr = expr.And2(w => w.TotalPv >= dto.pv1);
                }
                if (dto.pv2 != null)
                {
                    expr = expr.And2(w => w.TotalPv < dto.pv2);
                }
                if (dto.IsPay != null)
                {
                    if (Convert.ToBoolean(dto.IsPay))
                    {
                        expr = expr.And2(w => w.State == 3 || w.State == 4);
                    }
                    else
                    {
                        expr = expr.And2(w => w.State != 3 && w.State != 4);
                    }
                }
                if (dto.IsDG != null)
                {
                    if (Convert.ToBoolean(dto.IsDG))
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                    else
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                }


                count = c.Order.Where(expr).Count();
                li = c.Order.Where(expr).OrderByDescending(px => px.AddTime).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        ConsigneeAddress = x.ConsigneeAddress,
                        ConsigneeName = x.ConsigneeName,
                        ConsigneePhone = x.ConsigneePhone,
                        ConsigneeCity = x.ConsigneeCity,
                        ConsigneeCounty = x.ConsigneeCounty,
                        ConsigneeProvince = x.ConsigneeProvince,
                        LogisticsName = x.LogisticsName,
                        LogisticsNum = x.LogisticsNum,
                        LogisticsState = x.LogisticsState,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        UserID = x.UserID,
                        State = x.State,
                        IsBalance = x.IsBalance,

                        DeLevelName = x.User.DeLevel.Name,
                        UserCode = x.User.UserCode,
                        UserName = x.User.UserName,
                        UserPhone = x.User.Phone,

                        DealerCode = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().UserCode,
                        DealerName = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().UserName,
                        DealerPhone = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().Phone,

                        ServiceCenterCode = x.ServiceCenter.User.UserCode,
                        ServiceCenterName = x.ServiceCenter.ServiceCenterName,

                        UserPayMoney = x.UserPayMoney,
                        OtherPayMoney = x.OtherPayMoney,
                        TotalPv = x.TotalPv,
                        OrderTypeName = x.OrderType.OrderTypeName,
                        PayType = x.PayType,

                        BalanceNumber = x.BalanceNumber,
                        DealerId = x.DealerId,
                        DeLevelID = x.DeLevelID,
                        DeliverType = x.DeliverType,
                        DeptName = x.DeptName,
                        IsAppraise = x.IsAppraise,
                        IsBak = x.IsBak,
                        ManualRefundComment = x.ManualRefundComment,
                        NextWxSynTime = x.NextWxSynTime,
                        OldDeLevelID = x.OldDeLevelID,
                        OrderTypeID = x.OrderTypeID,
                        PCDealerCode = x.PCDealerCode,
                        PPDealerCode = x.PPDealerCode,
                        RefundEndTime = x.RefundEndTime,
                        RefundOrderNumber = x.RefundOrderNumber,
                        RefundRecvAccout = x.RefundRecvAccout,
                        RefundStartTime = x.RefundStartTime,
                        ServiceCenterID = x.ServiceCenterID,
                        SettleFlag = x.SettleFlag,
                        SettleTime = x.SettleTime,
                        UserCouponsPay = x.UserCouponsPay,

                        OrderProduct = c.OrderProduct.Where(n => n.OrderID == x.ID).OrderBy(px => px.ShoppingProductType)
                        .Select(z => new OrderProductDTOQD
                        {
                            ID = z.ID,
                            ProductID = z.ProductID,
                            ProductNum = z.ProductNum,
                            ProductPrice = z.ProductPrice,
                            OrderID = z.OrderID,
                            Price = z.Product.Price,
                            ProductName = z.Product.ProductName,
                            ProductCode = z.Product.ProductCode,
                            Unit = z.Product.Unit,
                            ShoppingProductType = z.ShoppingProductType,
                            zt = z.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                        }).ToList()

                    }
                ).ToList();
            }

            return li;
        }

        public OrdersDTO GetOrdersCount(Request_Order dto)
        {
            OrdersDTO model = new OrdersDTO();

            List<OrdersDTO> li = new List<OrdersDTO>();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Expression<Func<Order, bool>> expr = AutoAssemble.Splice<Order, Request_Order>(dto);
                if (dto.OrderNumber != null)
                {
                    expr = expr.And2(w => w.OrderNumber == dto.OrderNumber);
                }
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }
                else
                {
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.已退款);
                    //已取消订单做删除处理，不显示
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.支付已取消);
                }

                if (dto.UserName != null)
                {
                    expr = expr.And2(w => w.User.UserName == dto.UserName);
                }
                if (dto.Phone != null)
                {
                    expr = expr.And2(w => w.User.Phone == dto.Phone);
                }
                if (dto.STime != null)
                {
                    expr = expr.And2(w => w.AddTime >= dto.STime);
                }
                if (dto.ETime != null)
                {
                    expr = expr.And2(w => w.AddTime <= dto.ETime);
                }
                if (dto.je1 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct >= dto.je1);
                }
                if (dto.je2 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct < dto.je2);
                }
                if (dto.pv1 != null)
                {
                    expr = expr.And2(w => w.TotalPv >= dto.pv1);
                }
                if (dto.pv2 != null)
                {
                    expr = expr.And2(w => w.TotalPv < dto.pv2);
                }
                if (dto.IsPay != null)
                {
                    if (Convert.ToBoolean(dto.IsPay))
                    {
                        expr = expr.And2(w => w.State == 3 || w.State == 4);
                    }
                    else
                    {
                        expr = expr.And2(w => w.State != 3 && w.State != 4);
                    }
                }
                if (dto.IsDG != null)
                {
                    if (Convert.ToBoolean(dto.IsDG))
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                    else
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                }
                li = c.Order.Where(expr).Select(
                    x => new OrdersDTO
                    {
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        TotalPv = x.TotalPv,
                    }
                ).ToList();
                model.OrderMoney = li.Sum(s => s.OrderMoney);
                model.MoneyProduct = li.Sum(s => s.MoneyProduct);
                model.MoneyTransport = li.Sum(s => s.MoneyTransport);
                model.TotalPv = li.Sum(s => s.TotalPv);
            }

            return model;
        }

        public OrdersDTO GetOrderCode(string ordercode)
        {
            OrdersDTO li = new OrdersDTO();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                li = c.Order.Where(w => w.OrderNumber == ordercode).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        OtherPayMoney = x.OtherPayMoney,
                        UserPayMoney = x.UserPayMoney,
                        TotalPv = x.TotalPv,
                        SettleFlag = x.SettleFlag,
                        PayType = x.PayType,
                        IsBalance = x.IsBalance,
                        State = x.State
                    }
                ).FirstOrDefault();
            }

            return li;
        }

        public OrdersDTO GetOrderById(Guid id)
        {
            OrdersDTO li = new OrdersDTO();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                li = c.Order.Where(w => w.ID == id).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        OtherPayMoney = x.OtherPayMoney,
                        UserPayMoney = x.UserPayMoney,
                        TotalPv = x.TotalPv,
                        SettleFlag = x.SettleFlag,
                        PayType = x.PayType,
                        IsBalance = x.IsBalance,
                        State = x.State
                    }
                ).FirstOrDefault();
            }

            return li;
        }

        public OrdersDTO GetOrders(Guid? orderid)
        {
            OrdersDTO li = new OrdersDTO();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {

                li = c.Order.Where(w => w.ID == orderid).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        AddTime = x.AddTime,
                        ConsigneeAddress = x.ConsigneeAddress,
                        ConsigneeName = x.ConsigneeName,
                        ConsigneePhone = x.ConsigneePhone,
                        LogisticsName = x.LogisticsName,
                        LogisticsNum = x.LogisticsNum,
                        LogisticsState = x.LogisticsState,
                        OrderMoney = x.OrderMoney,
                        MoneyProduct = x.MoneyProduct,
                        MoneyTransport = x.MoneyTransport,
                        OrderNumber = x.OrderNumber,
                        UserID = x.UserID,
                        State = x.State,
                        TotalPv = x.TotalPv,
                        UserCode = x.User.UserCode,
                        UserName = x.User.UserName,
                        UserPhone = x.User.Phone,
                        OrderTypeName = x.OrderType.OrderTypeName,
                        DealerCode = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().UserCode,
                        DealerName = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().UserName,
                        DealerPhone = c.User.Where(q => q.ID == x.DealerId).FirstOrDefault().Phone,
                        PayType = x.PayType,

                        OrderProduct = c.OrderProduct.Where(n => n.OrderID == x.ID).OrderBy(px => px.ShoppingProductType)
                        .Select(z => new OrderProductDTOQD
                        {
                            ID = z.ID,
                            ProductID = z.ProductID,
                            ProductNum = z.ProductNum,
                            ProductPrice = z.ProductPrice,
                            OrderID = z.OrderID,
                            Price = z.Product.Price,
                            ProductName = z.Product.ProductName,
                            ProductCode = z.Product.ProductCode,
                            Unit = z.Product.Unit,
                            ShoppingProductType = z.ShoppingProductType,
                            zt = z.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                        }).ToList()

                    }
                ).FirstOrDefault();
            }

            return li;
        }

        [Obsolete]
        public PayOrderUserDTO PayOrder(string orderNumber, int State, int paytype)
        {
            PayOrderUserDTO dto = new PayOrderUserDTO();
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    Order order = c.Order.Where(w => w.OrderNumber == orderNumber).FirstOrDefault();
                    OrderType ordertype = c.OrderType.Where(w => w.ID == order.OrderTypeID).FirstOrDefault();

                    User user = c.User.Where(w => w.ID == order.UserID).FirstOrDefault();//订单所属人
                    dto.DeLevelID = user.DeLevelID;
                    dto.OrderTypeID = order.OrderTypeID;
                    dto.Phone = user.Phone;
                    dto.UserCode = user.UserCode;
                    dto.UserName = user.UserName;

                    User dealer;//订单扣款人
                    if (order.UserID == order.DealerId)
                    {
                        dealer = user;
                    }
                    else
                    {
                        //代购业务
                        dealer = c.User.Where(w => w.ID == order.DealerId).FirstOrDefault();
                        if (user.UserState == 0)
                        {
                            user.UserState = 1;//激活
                        }

                    }
                    if (order.State != (int)Enum.OrderState.待发货)
                    {
                        #region 订单未支付
                        #region 支付方式
                        //if (paytype != 0)
                        //{
                        //order.PayType = paytype;
                        if (order.PayType == (int)Enum.PayType.积分)                     //积分支付
                        {
                            order.UserPayMoney = order.OrderMoney;      //余额支付金额
                            order.OtherPayMoney = 0;                //第三方支付金额

                            if ((dealer.EleMoney - (decimal)order.UserPayMoney) < 0)
                            {
                                throw new DMException("积分不足！");
                            }

                        }
                        if (order.PayType == (int)Enum.PayType.微信 || order.PayType == (int)Enum.PayType.快钱)   //微信快钱支付
                        {
                            order.UserPayMoney = 0;                 //余额支付金额
                            order.OtherPayMoney = order.OrderMoney;     //第三方支付金额
                        }

                        if (order.PayType == (int)Enum.PayType.微信加积分 || order.PayType == (int)Enum.PayType.快钱加积分)   //混合支付
                        {
                            order.UserPayMoney = dealer.EleMoney;                   //余额支付金额
                            order.OtherPayMoney = order.OrderMoney - dealer.EleMoney;   //第三方支付金额

                            if ((dealer.EleMoney - (decimal)order.UserPayMoney) < 0)
                            {
                                throw new DMException("积分不足！");
                            }
                        }
                        //}
                        order.State = State;
                        #endregion


                        #region 活跃期处理
                        if (user.DeLevelID >= (int)Enum.DeLevel.创客 || order.DeLevelID >= (int)Enum.DeLevel.创客)
                        {
                            ActivePeriodRecord apr = new ActivePeriodRecord();
                            apr.ID = Guid.NewGuid();
                            apr.UserID = user.ID;
                            apr.UserCode = user.UserCode;
                            apr.UserName = user.UserName;
                            apr.RecordType = order.OrderTypeID;

                            apr.StageOne = user.StageCount;
                            if (order.OrderTypeID == (int)Enum.OrderType.注册单)
                            {
                                apr.StageTwo = 4;
                                apr.PVSurplus = user.PVSurplus;
                            }
                            if (order.OrderTypeID == (int)Enum.OrderType.升级单)
                            {
                                apr.StageTwo = 4;
                                apr.PVSurplus = user.PVSurplus;
                            }
                            if (order.OrderTypeID == (int)Enum.OrderType.主动消费单)
                            {
                                apr.StageTwo = (int)Math.Floor((user.PVSurplus + Convert.ToDecimal(order.MoneyProduct)) / hyqnum);
                                apr.PVSurplus = Convert.ToInt32((user.PVSurplus + order.MoneyProduct) % hyqnum);
                            }


                            apr.StageThree = apr.StageOne + apr.StageTwo;

                            apr.AddTime = DateTime.Now;
                            apr.Remarks = "下单增加。";
                            apr.OrderNumber = order.OrderNumber;

                            c.ActivePeriodRecord.Add(apr);

                            user.StageCount = Convert.ToInt32(apr.StageThree);
                            user.PVSurplus = apr.PVSurplus;
                        }

                        #endregion

                        #region 升级处理       
                        if (order.OrderTypeID == 2)//order.DeLevelID != null && 
                        {
                            if (order.DeLevelID == null)
                            {
                                throw new DMException("升级单级别不能为空！");
                            }
                            else
                            {
                                user.DeLevelID = (int)order.DeLevelID;
                            }
                        }
                        #endregion

                        #region 安置处理
                        if (order.DeLevelID >= 4 && order.OrderTypeID != 3)
                        {
                            if (!string.IsNullOrEmpty(order.PPDealerCode) && !string.IsNullOrEmpty(order.DeptName))
                            {
                                if (cc.PlaceRelation.Where(w => w.DealerId == user.ID).FirstOrDefault() != null)
                                {
                                    if (order.OrderTypeID != 2)
                                    {
                                        throw new DMException("安置关系已经存在！");
                                    }
                                }
                                else
                                {



                                    #region 下滑
                                    UserXiahua uxh = cc.Database.SqlQuery<UserXiahua>(" Exec Pro_Get_RegCust_ManageNode @p0,@p1", order.PCDealerCode, order.DeptName).FirstOrDefault();
                                    if (cc.PlaceRelation.Where(w => w.DealerId == user.ID).FirstOrDefault() != null)
                                    {
                                        throw new DMException("安置关系已经存在！");
                                    }
                                    User PPDealer = c.User.Where(w => w.UserCode == uxh.ManageDealerId).FirstOrDefault();

                                    #endregion
                                    #region 非下滑

                                    //UserABqu uxh = cc.Database.SqlQuery<UserABqu>("   Exec   Pro_Get_AreaNameByManageCode @p0,@p1", order.PCDealerCode, order.PPDealerCode).FirstOrDefault();
                                    //if (uxh.Tag == -1)
                                    //{
                                    //    throw new DMException(uxh.ErrMessage);
                                    //}

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
                            //else
                            //{
                            //    throw new DMException("安置关系不能为空！");
                            //}
                        }


                        #endregion

                        #region 流水
                        ExChange ec = new ExChange();
                        ec.ID = Guid.NewGuid();
                        ec.UserID = dealer.ID;
                        ec.UserCode = dealer.UserCode;
                        ec.UserName = dealer.UserName;
                        ec.ChangeTime = DateTime.Now;
                        ec.ChangeMarks = "下单支付！";
                        ec.OrderNum = order.OrderNumber;
                        ec.State = 0;
                        ec.ExChangeType = 3;

                        if (order.PayType == 1)//积分支付
                        {
                            ec.MoneyType = 1;
                            ec.ZMoneyType = 5;

                            ec.BeforeChangeMoney = dealer.EleMoney;
                            ec.ChangeMoney = 0 - (decimal)order.UserPayMoney;
                            ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;

                            dealer.EleMoney = dealer.EleMoney - (decimal)order.UserPayMoney;//处理余额
                        }
                        if (order.PayType == 2)//微信支付
                        {
                            ec.MoneyType = 2;
                            ec.ZMoneyType = 7;

                            ec.BeforeChangeMoney = 0;
                            ec.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                            ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                        }
                        if (order.PayType == 3)//快钱支付
                        {
                            ec.MoneyType = 3;
                            ec.ZMoneyType = 10;

                            ec.BeforeChangeMoney = 0;
                            ec.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                            ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                        }
                        if (order.PayType == 4)//微信加积分
                        {
                            //积分支付部分
                            ec.MoneyType = 1;
                            ec.ZMoneyType = 5;
                            ec.BeforeChangeMoney = dealer.EleMoney;
                            ec.ChangeMoney = 0 - (decimal)order.UserPayMoney;
                            ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                            dealer.EleMoney = dealer.EleMoney - (decimal)order.UserPayMoney;//处理余额

                            //微信支付部分
                            ExChange ec2 = new ExChange();
                            ec2.ID = Guid.NewGuid();
                            ec2.UserID = dealer.ID;
                            ec2.UserCode = dealer.UserCode;
                            ec2.UserName = dealer.UserName;
                            ec2.ChangeTime = DateTime.Now;
                            ec2.ChangeMarks = "下单支付！";
                            ec2.OrderNum = order.OrderNumber;
                            ec2.State = 0;
                            ec2.ExChangeType = 3;

                            ec2.MoneyType = 2;
                            ec2.ZMoneyType = 7;
                            ec2.BeforeChangeMoney = 0;
                            ec2.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                            ec2.AfterChangeMoney = ec2.BeforeChangeMoney + ec2.ChangeMoney;

                            c.ExChange.Add(ec2);
                        }
                        if (order.PayType == 5)//快钱加积分
                        {
                            //积分支付部分
                            ec.MoneyType = 1;
                            ec.ZMoneyType = 5;
                            ec.BeforeChangeMoney = dealer.EleMoney;
                            ec.ChangeMoney = 0 - (decimal)order.UserPayMoney;
                            ec.AfterChangeMoney = ec.BeforeChangeMoney + ec.ChangeMoney;
                            dealer.EleMoney = dealer.EleMoney - (decimal)order.UserPayMoney;//处理余额

                            //快钱支付部分
                            ExChange ec2 = new ExChange();
                            ec2.ID = Guid.NewGuid();
                            ec2.UserID = dealer.ID;
                            ec2.UserCode = dealer.UserCode;
                            ec2.UserName = dealer.UserName;
                            ec2.ChangeTime = DateTime.Now;
                            ec2.ChangeMarks = "下单支付！";
                            ec2.OrderNum = order.OrderNumber;
                            ec2.State = 0;
                            ec2.ExChangeType = 3;

                            ec2.MoneyType = 3;
                            ec2.ZMoneyType = 10;
                            ec2.BeforeChangeMoney = 0;
                            ec2.ChangeMoney = 0 - (decimal)order.OtherPayMoney;
                            ec2.AfterChangeMoney = ec2.BeforeChangeMoney + ec2.ChangeMoney;

                            c.ExChange.Add(ec2);
                        }

                        c.ExChange.Add(ec);
                        #endregion

                        #endregion
                    }
                    else
                    {
                        if (State == 5 || State == 2)
                        {
                            order.State = State;
                        }
                    }

                    c.SaveChanges();
                    cc.SaveChanges();
                }
            }
            return dto;
        }

        public string GetLoginDxInfo(int type, string code, string username)
        {
            if (type == 4)
            {
                return "尊敬的：" + username + "，您好，福能源新零售商城欢迎您！您的编号是：" + code + "，初始密码是注册手机号码后六位，请您尽快修改密码。祝您购物愉快，事业顺利。客服电话：4006050503。";
            }
            else
            {
                return "尊敬的：" + username + "，您好，福能源新零售商城欢迎您！您的编号是：" + code + "，初始密码是注册手机号码后六位，请您尽快修改密码。祝您购物愉快，事业顺利。客服电话：4006050503。";
            }
        }

        public List<OrderTypeCountList> GetOrderTypeCountList(Request_Order dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                OrderTypeCountList data = new OrderTypeCountList();
                List<OrderTypeCountList> li = new List<OrderTypeCountList>();
                Expression<Func<Order, bool>> expr = AutoAssemble.Splice<Order, Request_Order>(dto);
                if (dto.State != null)
                {
                    expr = expr.And2(w => w.State == dto.State);
                }
                else
                {
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.已退款);
                    //已取消订单做删除处理，不显示
                    expr = expr.And2(w => w.State != (int)Enum.OrderState.支付已取消);
                }
                if (dto.UserCode != null)
                {
                    expr = expr.And2(w => w.User.UserCode == dto.UserCode);
                }
                if (dto.UserName != null)
                {
                    expr = expr.And2(w => w.User.UserName == dto.UserName);
                }
                if (dto.Phone != null)
                {
                    expr = expr.And2(w => w.User.Phone == dto.Phone);
                }
                if (dto.STime != null)
                {
                    expr = expr.And2(w => w.AddTime >= dto.STime);
                }
                if (dto.ETime != null)
                {
                    expr = expr.And2(w => w.AddTime <= dto.ETime);
                }
                if (dto.je1 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct >= dto.je1);
                }
                if (dto.je2 != null)
                {
                    expr = expr.And2(w => w.MoneyProduct < dto.je2);
                }
                if (dto.pv1 != null)
                {
                    expr = expr.And2(w => w.TotalPv >= dto.pv1);
                }
                if (dto.pv2 != null)
                {
                    expr = expr.And2(w => w.TotalPv < dto.pv2);
                }
                if (dto.IsPay != null)
                {
                    if (Convert.ToBoolean(dto.IsPay))
                    {
                        expr = expr.And2(w => w.State == 3 || w.State == 4);
                    }
                    else
                    {
                        expr = expr.And2(w => w.State != 3 && w.State != 4);
                    }
                }
                if (dto.IsDG != null)
                {
                    if (Convert.ToBoolean(dto.IsDG))
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                    else
                    {
                        expr = expr.And2(w => w.UserID != w.DealerId);
                    }
                }


                li = c.Order.Where(expr).OrderBy(o => o.OrderTypeID).GroupBy(g => new { g.OrderType.OrderTypeName, g.OrderTypeID })
                    .Select(x => new OrderTypeCountList
                    {
                        CountTT = x.Count(),
                        OrderTypeName = x.Key.OrderTypeName,
                        MoneyProduct = x.Sum(i => i.MoneyProduct),
                        MoneyTransport = x.Sum(i => i.MoneyTransport),
                        OrderMoney = x.Sum(i => i.OrderMoney),
                        TotalPv = x.Sum(i => i.TotalPv)
                    }).ToList();

                return li;
            }
        }


        public List<Pro_GetSaleTeam_OrderDTO> GetPro_GetSaleTeam_Order(string PDealerCode, string BusiType, string SBegin, string sEnd, string OrderArea, string State, string IsBalance, string OrderTypeId)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<Pro_GetSaleTeam_OrderDTO>("   Exec    Pro_GetSaleTeam_Order  @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", PDealerCode, BusiType, SBegin, sEnd, OrderArea, State, IsBalance, OrderTypeId).ToList();
            }
        }

        public List<ProductOrderDTO> GetProductOrderList(string dealerCode, string productCode, string OrderTypeID, DateTime? startTime, DateTime? endTime, string PrivenceName)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = string.Format(@"              

                SELECT (( ROW_NUMBER() over( order by ProductCode desc))-1)/10+1 as p, ProductCode,ProductName	 
		                , sum(je1) je1,sum(pv1) pv1, sum(je2) je2,sum(pv2) pv2,sum(je3) je3,sum(pv3) pv3,sum(Count1) Count1,sum(Count2) Count2,sum(Count3) Count3,sum(je4) je4,sum(pv4) pv4,sum(Count4) Count4
                from (
	                SELECT    ProductCode,ProductName	 
		                , je1	,pv1	,0 je2	,0 pv2	,0 je3	,0 pv3	, Count1,0 Count2,0 Count3,0 je4,0 pv4,0 Count4
	                 FROM ( select pt.ProductCode ProductCode,pt.ProductName ProductName,sum(so.ProductPrice*so.ProductNum) je1,sum(pt.PV) pv1,sum(so.ProductNum) Count1 
			                FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='1' and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by pt.ProductCode,pt.ProductName 
	                ) a

	                Union All
	                SELECT     ProductCode,ProductName	
		                ,0 je1	,0 pv1, je2	, pv2,0 je3	,0 pv3	,0 Count1, Count2,0 Count3,0 je4,0 pv4,0 Count4
	                 FROM ( select pt.ProductCode ,pt.ProductName ,sum(so.ProductPrice*so.ProductNum) je2,sum(pt.PV) pv2,sum(so.ProductNum) Count2 
			                FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='2' and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by pt.ProductCode,pt.ProductName 
	                ) b

	                Union All
	                SELECT   ProductCode,ProductName		
		                ,0 je1,0 pv1,  0 je2,0 pv2,  je3, pv3,0 Count1,0 Count2, Count3,0 je4,0 pv4,0 Count4
	                 FROM ( select pt.ProductCode ,pt.ProductName ,sum(so.ProductPrice*so.ProductNum) je3,sum(pt.PV) pv3,sum(so.ProductNum) Count3 
			               FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='3'  and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by pt.ProductCode,pt.ProductName 
	                ) c

					Union All
	                SELECT   ProductCode,ProductName		
		                ,0 je1	,0 pv1,0 je2	,0 pv2,0 je3,0 pv3	,0 Count1,0 Count2,0 Count3, je4, pv4, Count4
	                 FROM ( select pt.ProductCode ,pt.ProductName ,sum(so.ProductPrice*so.ProductNum) je4,sum(pt.PV) pv4,sum(so.ProductNum) Count4 
			                FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='4' and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by pt.ProductCode,pt.ProductName 
	                ) d
                )d group by  ProductCode, ProductName
                    
                ", !string.IsNullOrEmpty(dealerCode) ? " d.UserCode='" + dealerCode + "'" : " 1=1 "
                , !string.IsNullOrEmpty(productCode) ? " pt.ProductCode like '%" + productCode + "%'" : " 1=1 "
                , startTime.HasValue ? (" o.AddTime >= '" + ((DateTime)startTime).ToString("yyyy-MM-dd") + " 00:00:00'") : "1=1"
                , endTime.HasValue ? (" o.AddTime <= '" + ((DateTime)endTime).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1"
                 , !string.IsNullOrEmpty(OrderTypeID) ? " oc.Id='" + OrderTypeID + "'" : " 1=1 "
                 , !string.IsNullOrEmpty(PrivenceName) ? " o.ConsigneeProvince like '%" + PrivenceName + "%'" : " 1=1 "
                );
                return c.Database.SqlQuery<ProductOrderDTO>(sql).ToList();
            }
        }
        public List<AddressOrderDTO> GetAddressOrderList(string dealerCode, string productCode, string OrderTypeID, DateTime? startTime, DateTime? endTime, string PrivenceName)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = string.Format(@"
              

                SELECT (( ROW_NUMBER() over( order by ConsigneeProvince desc))-1)/10+1 as p, ConsigneeProvince	 
		                , sum(je1) je1,sum(pv1) pv1, sum(je2) je2,sum(pv2) pv2,sum(je3) je3,sum(pv3) pv3,sum(Count1) Count1,sum(Count2) Count2,sum(Count3) Count3,sum(je4) je4,sum(pv4) pv4,sum(Count4) Count4
                from (
	                SELECT    ConsigneeProvince	 
		                , je1	,pv1	,0 je2	,0 pv2	,0 je3	,0 pv3	, Count1,0 Count2,0 Count3,0 je4,0 pv4,0 Count4
	                 FROM ( select o.ConsigneeProvince ConsigneeProvince  ,sum(so.ProductPrice*so.ProductNum) je1,sum(pt.PV) pv1,sum(so.ProductNum) Count1 
			                FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='1' and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by o.ConsigneeProvince  
	                ) a

	                Union All
	                SELECT     ConsigneeProvince	
		                ,0 je1	,0 pv1, je2	, pv2,0 je3	,0 pv3	,0 Count1, Count2,0 Count3,0 je4,0 pv4,0 Count4
	                 FROM ( select o.ConsigneeProvince   ,sum(so.ProductPrice*so.ProductNum) je2,sum(pt.PV) pv2,sum(so.ProductNum) Count2 
			                FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='2' and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by o.ConsigneeProvince  
	                ) b

	                Union All
	                SELECT   ConsigneeProvince		
		                ,0 je1,0 pv1,  0 je2,0 pv2,  je3, pv3,0 Count1,0 Count2, Count3,0 je4,0 pv4,0 Count4
	                 FROM ( select o.ConsigneeProvince   ,sum(so.ProductPrice*so.ProductNum) je3,sum(pt.PV) pv3,sum(so.ProductNum) Count3 
			               FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='3'  and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by o.ConsigneeProvince  
	                ) c

					Union All
	                SELECT   ConsigneeProvince		
		                ,0 je1	,0 pv1,0 je2	,0 pv2,0 je3,0 pv3	,0 Count1,0 Count2,0 Count3, je4, pv4, Count4
	                 FROM ( select o.ConsigneeProvince   ,sum(so.ProductPrice*so.ProductNum) je4,sum(pt.PV) pv4,sum(so.ProductNum) Count4 
			                FROM [OrderProduct] so							
			                inner join [dbo].[Order] o on so.OrderID= o.Id  
							inner join [dbo].[User] d on d.ID=o.UserID
			                inner join [dbo].OrderType oc on oc.Id=o.OrderTypeID
			                inner join [dbo].[Product] pt on pt.Id=so.[ProductID]
			                where so.ShoppingProductType='4' and  {0} and {1} and {2} and {3} and {4} and {5} and o.State!=7 and o.State!=5 
			                group by o.ConsigneeProvince  
	                ) d
                )d group by  ConsigneeProvince
                    
                ", !string.IsNullOrEmpty(dealerCode) ? " d.UserCode='" + dealerCode + "'" : " 1=1 "
                , !string.IsNullOrEmpty(productCode) ? " pt.ProductCode like '%" + productCode + "%'" : " 1=1 "
                , startTime.HasValue ? (" o.AddTime >= '" + ((DateTime)startTime).ToString("yyyy-MM-dd") + " 00:00:00'") : "1=1"
                , endTime.HasValue ? (" o.AddTime <= '" + ((DateTime)endTime).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1"
                 , !string.IsNullOrEmpty(OrderTypeID) ? " oc.Id='" + OrderTypeID + "'" : " 1=1 "
                 , !string.IsNullOrEmpty(PrivenceName) ? " o.ConsigneeProvince like '%" + PrivenceName + "%'" : " 1=1 "
                );
                return c.Database.SqlQuery<AddressOrderDTO>(sql).ToList();
            }
        }

        public List<Pro_Query_ItemSaleCase_webDTO> GetPro_Query_ItemSaleCase_web(string sBegin, string sEnd, string itemCode, string orderCategoryName)
        {
            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                return cc.Database.SqlQuery<Pro_Query_ItemSaleCase_webDTO>(" Exec Pro_Query_ItemSaleCase_web '" + sBegin + "','" + sEnd + "','" + itemCode + "','" + orderCategoryName + "'").ToList();
            }
        }

        public List<Pro_Sum_Market_AchievementDTO> GetPro_Sum_Market_Achievement(DateTime? btime, DateTime? stimg, string province)
        {
            using (BwslRetailEntities cc = new BwslRetailEntities())
            {
                return cc.Database.SqlQuery<Pro_Sum_Market_AchievementDTO>(" exec Pro_Sum_Market_Achievement '" + btime + "','" + stimg + "' ,'" + province + "'").ToList();
            }
        }
        public List<Pro_Check_Financial_IndexDTO> GetPro_Check_Financial_Index(string Code, string sBegin, string sEnd, string Index, string IsChenk)
        {
            using (BwslRetailEntities cc = new BwslRetailEntities())
            {
                return cc.Database.SqlQuery<Pro_Check_Financial_IndexDTO>(" exec Pro_Check_Financial_Index '" + Code + "','" + sBegin + "' ,'" + sEnd + "','" + Index + "','" + IsChenk + "'").ToList();
            }
        }
        /// <summary>
        /// 订单是否可以自动取消
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private bool CanAutoCancel(Order order)
        {
            if (order == null)
            {
                return false;
            }
            var orderAutoCancelSecond = 300;
            if (ReadConfig.GetAppSetting("OrderAutCancel") != null)
            {
                orderAutoCancelSecond = int.Parse(ReadConfig.GetAppSetting("OrderAutCancel"));
            }
            TimeSpan ts = (DateTime.Now - order.AddTime);
            if (ts.TotalSeconds > orderAutoCancelSecond)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取用户TopN的待付款订单
        /// </summary>
        /// <param name="topN"></param>
        /// <returns></returns>
        public List<OrdersDTO> FindNoPay(int topN)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                expr = expr.And2(w => w.State == (int)Enum.OrderState.待付款);
                expr = expr.And2(w => w.NextWxSynTime < DateTime.Now);
                return c.Order.Where(expr).OrderBy(px => px.UserID).ThenBy(px => px.AddTime).Take(topN).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        OrderNumber = x.OrderNumber,
                        AddTime = x.AddTime,
                        OtherPayMoney = x.OtherPayMoney
                    }
                ).ToList();
            }
        }

        /// <summary>
        /// 获取用户TopN的待退款订单
        /// </summary>
        /// <param name="topN"></param>
        /// <returns></returns>
        public List<OrdersDTO> FindRefundDoing(int topN)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                expr = expr.And2(w => w.State == (int)Enum.OrderState.待退款);
                expr = expr.And2(w => w.NextWxSynTime < DateTime.Now);
                return c.Order.Where(expr).OrderBy(px => px.UserID).ThenBy(px => px.AddTime).Take(topN).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        OrderNumber = x.OrderNumber,
                        AddTime = x.AddTime,
                        OtherPayMoney = x.OtherPayMoney,
                        RefundOrderNumber = x.RefundOrderNumber
                    }
                ).ToList();
            }
        }


        /// <summary>
        /// 获取用户TopN的待结算订单
        /// </summary>
        /// <param name="topN"></param>
        /// <returns></returns>
        public List<OrdersDTO> FindBalanceAccount(int topN)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                System.Linq.Expressions.Expression<Func<Order, bool>> expr = n => true;
                expr = expr.And2(w => w.SettleFlag == (int)Enum.SettleFlag.待结算);

                return c.Order.Where(expr).OrderBy(px => px.UserID).ThenBy(px => px.AddTime).Take(topN).Select(
                    x => new OrdersDTO
                    {
                        ID = x.ID,
                        OrderNumber = x.OrderNumber,
                        AddTime = x.AddTime,
                        UserPayMoney = x.UserPayMoney,
                        OtherPayMoney = x.OtherPayMoney,
                        RefundOrderNumber = x.RefundOrderNumber,
                        State = x.State
                    }
                ).ToList();
            }
        }
    }
}