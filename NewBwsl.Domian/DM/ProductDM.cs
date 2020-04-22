
using NewMK.DTO.Product;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewMK.DTO.Order;
using System.Linq.Expressions;
using NewMK.DTO;
using NewMK.DTO.Activity;
using NewMK.Domian.DomainException;
using NewMK.DTO.ManageData;

namespace NewMK.Domian.DM
{
    public class ProductDM
    {
        #region 后台

        #region 订单产品配置     
        public bool AddOrderConfig(OrderConfigDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                dto.ID = Guid.NewGuid();

                c.OrderConfig.Add(GetMapperDTO.SetModel<OrderConfig, OrderConfigDTO>(dto));
                c.SaveChanges();
                return true;
            }
        }

        public List<OrderConfigDTO> GetOrderConfigList(Request_OrderConfigDTO dto, out int pagcount)
        {
            Expression<Func<OrderConfig, bool>> expr = AutoAssemble.Splice<OrderConfig, Request_OrderConfigDTO>(dto);
            if (dto.ProductCode != null && dto.ProductCode != "")
            {
                expr = expr.And2(w => w.Product.ProductCode == dto.ProductCode);
            }
            if (dto.ProductName != null && dto.ProductName != "")
            {
                expr = expr.And2(w => w.Product.ProductName == dto.ProductName);
            }
            //if (dto.LevelList != null && dto.LevelList != "")
            //{
            //    expr = expr.And2(w => w.LevelList.Contains(dto.LevelList));
            //}

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                pagcount = c.OrderConfig.Where(expr).ToList().Count;
                List<OrderConfig> li = c.OrderConfig.Where(expr).OrderByDescending(px => px.Product.PX).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).ToList();

                return OrderConfigDTO.GetOrderConfigDTOList(li);
            }
        }

        public bool deOrderConfig(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                OrderConfig pt = c.OrderConfig.FirstOrDefault(n => n.ID == id);

                c.OrderConfig.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }
        #endregion

        #region 产品类型操作
        public List<ProductTypeDTO> GetProductType()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ProductTypeDTO> userDto = c.ProductType.OrderByDescending(p => p.Px).Select(
                    x => new ProductTypeDTO
                    {
                        ID = x.ID,
                        TypeName = x.TypeName,
                        Px = x.Px
                    }
                ).ToList();

                return userDto;
            }
        }


        public bool AddProductType(string name, int? px)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductType pt = new ProductType();
                pt.ID = Guid.NewGuid();
                pt.TypeName = name;
                pt.Px = px;

                c.ProductType.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public ProductTypeDTO GetProductTypeFirst(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductTypeDTO userDto = c.ProductType.Where(n => n.ID == id).Select(
                    x => new ProductTypeDTO
                    {
                        ID = x.ID,
                        TypeName = x.TypeName,
                        Px = x.Px
                    }
                ).FirstOrDefault();

                return userDto;
            }
        }

        public bool UpProductType(Guid id, string name, int? px)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductType pt = c.ProductType.FirstOrDefault(n => n.ID == id);

                pt.TypeName = name;
                pt.Px = px;

                c.SaveChanges();
                return true;
            }
        }



        public bool deProductType(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductType pt = c.ProductType.FirstOrDefault(n => n.ID == id);

                c.ProductType.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #region 产品操作

        public List<OrderConfigProductDTO> GetProductList(Request_OrderConfigDTO dto, out int pagcount)
        {
            Expression<Func<OrderConfig, bool>> expr = AutoAssemble.Splice<OrderConfig, Request_OrderConfigDTO>(dto);
            if (dto.ProductCode != null && dto.ProductCode != "")
            {
                expr = expr.And2(w => w.Product.ProductCode == dto.ProductCode);
            }
            if (dto.ProductName != null && dto.ProductName != "")
            {
                expr = expr.And2(w => w.Product.ProductName == dto.ProductName);
            }
            if (dto.KeyWord != null && dto.KeyWord != "")
            {
                expr = expr.And2(w => w.Product.KeyWord == dto.KeyWord);
            }
            if (dto.IsStarProduct != null)
            {
                expr = expr.And2(w => w.Product.IsStarProduct == dto.IsStarProduct);
            }
            if (dto.ProductTypeID != null)
            {
                expr = expr.And2(w => w.Product.ProductTypeID == dto.ProductTypeID);
            }

            expr = expr.And2(w => w.Product.State == true);

            //if (dto.LevelList != null && dto.LevelList != "")
            //{
            //    expr = expr.And2(w => w.LevelList.Contains(dto.LevelList));
            //}

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                pagcount = c.OrderConfig.Where(expr).ToList().Count;
                List<OrderConfigProductDTO> li = c.OrderConfig.Where(expr).OrderByDescending(px => px.Product.PX).Skip((dto.PageIndex - 1) * dto.PageSize).Take(dto.PageSize).Select(
                    x => new OrderConfigProductDTO
                    {
                        ID = x.Product.ID,
                        AddTime = x.Product.AddTime,
                        CrProductId = x.Product.CrProductId,
                        ETime = x.Product.ETime,
                        IfCount = x.IfCount,
                        IsBy = x.Product.IsBy,
                        IsRequired = x.IsRequired,
                        IsStarProduct = x.IsRequired,
                        KeyWord = x.Product.KeyWord,
                        LevelList = x.LevelList,
                        Mulriple = x.Product.Mulriple,
                        OldLevelList = x.OldLevelList,
                        ETime1 = x.ETime1,
                        STime1 = x.STime1,
                        OrderTypeID = x.OrderTypeID,
                        Price = x.Product.Price,
                        PriceMin = x.PriceMin,
                        ProductCode = x.Product.ProductCode,
                        ProductID = x.ProductID,
                        ProductName = x.Product.ProductName,
                        ProductNature = x.Product.ProductNature,
                        //ProductNum = x.Product.ProductNature,
                        ProductTypeID = x.Product.ProductTypeID,
                        ProductTypeName = x.Product.ProductType.TypeName,
                        ProductWeight = x.Product.ProductWeight,
                        PV = x.Product.PV,
                        PX = x.Product.PX,
                        SalesVolume = x.Product.SalesVolume,
                        State = x.Product.State,
                        STime = x.Product.STime,
                        StockNumber = x.Product.StockNumber,
                        Unit = x.Product.Unit,
                        zt = x.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                    ,
                    }
                    ).ToList();

                foreach (OrderConfigProductDTO item in li)
                {
                    item.ActivityList = GetProductActivity(item.ID, dto.OrderTypeID, c);
                }

                return li;
            }



            //    return dto.OrderByDescending(o => o.PX).ToList();

        }

        public List<ProductDTO> GetProductListHD(Request_ProductDTO rDto, out int pagcount)
        {
            //System.Linq.Expressions.Expression<Func<Product, bool>> expr = n => true;
            Expression<Func<Product, bool>> expr = AutoAssemble.Splice<Product, Request_ProductDTO>(rDto);


            if (rDto.IsStarProduct != null)
            {
                expr = expr.And2(w => w.IsStarProduct == rDto.IsStarProduct);
            }

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                pagcount = c.Product.Where(expr).ToList().Count;
                List<ProductDTO> dto = c.Product.Where(expr).OrderByDescending(o => o.PX).Skip((rDto.PageIndex - 1) * rDto.PageSize).Take(rDto.PageSize).Select(
                    x => new ProductDTO
                    {
                        ID = x.ID,
                        ProductTypeID = x.ProductTypeID,
                        ProductTypeName = x.ProductType.TypeName,
                        AddTime = x.AddTime,
                        ProductNature = x.ProductNature,
                        KeyWord = x.KeyWord,
                        Price = x.Price,
                        ProductName = x.ProductName,
                        ProductWeight = x.ProductWeight,
                        PV = x.PV,
                        SalesVolume = x.SalesVolume,
                        State = x.State,
                        Unit = x.Unit,
                        CrProductId = x.CrProductId,
                        ProductCode = x.ProductCode,
                        PX = x.PX,
                        Mulriple = x.Mulriple,
                        IsBy = x.IsBy,
                        zt = x.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url,
                        ETime = x.ETime,
                        STime = x.STime,
                        StockNumber = x.StockNumber,
                        IsStarProduct = x.IsStarProduct
                    }
                ).ToList();


                return dto.OrderByDescending(o => o.PX).ToList();
            }
        }

        public OrderConfigProductDTO GetProduct(Guid? ID, int? OrderTypeID)
        {

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                OrderConfigProductDTO dto = c.OrderConfig.Where(w => w.ProductID == ID && w.OrderTypeID == OrderTypeID).Select(
                    x => new OrderConfigProductDTO
                    {
                        ID = x.Product.ID,
                        AddTime = x.Product.AddTime,
                        CrProductId = x.Product.CrProductId,
                        ETime = x.Product.ETime,
                        IfCount = x.IfCount,
                        IsBy = x.Product.IsBy,
                        ETime1 = x.ETime1,
                        STime1 = x.STime1,
                        IsRequired = x.IsRequired,
                        IsStarProduct = x.IsRequired,
                        KeyWord = x.Product.KeyWord,
                        LevelList = x.LevelList,
                        Mulriple = x.Product.Mulriple,
                        OldLevelList = x.OldLevelList,
                        OrderTypeID = x.OrderTypeID,
                        Price = x.Product.Price,
                        PriceMin = x.PriceMin,
                        ProductCode = x.Product.ProductCode,
                        ProductID = x.ProductID,
                        ProductName = x.Product.ProductName,
                        ProductNature = x.Product.ProductNature,
                        //ProductNum = x.Product.ProductNature,
                        ProductTypeID = x.Product.ProductTypeID,
                        ProductTypeName = x.Product.ProductType.TypeName,
                        ProductWeight = x.Product.ProductWeight,
                        PV = x.Product.PV,
                        PX = x.Product.PX,
                        SalesVolume = x.Product.SalesVolume,
                        State = x.Product.State,
                        STime = x.Product.STime,
                        StockNumber = x.Product.StockNumber,
                        Unit = x.Product.Unit,
                        zt = x.Product.ProductImage.Where(w => w.ImgType == 1).FirstOrDefault().Url
                    ,
                    }
                    ).FirstOrDefault();

                return dto;
            }
        }

        public ProductDTO GetProductHD(Guid? ID)
        {

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductDTO dto = c.Product.Where(w => w.ID == ID).Select(
                    x => new ProductDTO
                    {
                        ID = x.ID,
                        ProductTypeID = x.ProductTypeID,
                        ProductTypeName = x.ProductType.TypeName,
                        AddTime = x.AddTime,
                        ProductNature = x.ProductNature,
                        KeyWord = x.KeyWord,
                        Price = x.Price,
                        ProductName = x.ProductName,
                        ProductWeight = x.ProductWeight,
                        PV = x.PV,
                        SalesVolume = x.SalesVolume,
                        State = x.State,
                        Unit = x.Unit,
                        CrProductId = x.CrProductId,
                        ProductCode = x.ProductCode,
                        Mulriple = x.Mulriple,
                        IsBy = x.IsBy,
                        PX = x.PX,
                        ETime = x.ETime,
                        STime = x.STime,
                        StockNumber = x.StockNumber,
                        IsStarProduct = x.IsStarProduct
                    }
                ).FirstOrDefault();

                return dto;
            }
        }
        public List<ActivityDTO> GetProductActivity(Guid? ID, int OrderTypeID)
        {

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                return GetProductActivity(ID, OrderTypeID, c);
            }
        }

        private static List<ActivityDTO> GetProductActivity(Guid? ID, int? OrderTypeID, BwslRetailEntities c)
        {
            System.Linq.Expressions.Expression<Func<Activity, bool>> expr = n => true;
            if (OrderTypeID != null)
            {
                expr = expr.And2(w => w.ActivityOrderType.Any(a => a.OrderTypeID == OrderTypeID));
            }

            expr = expr.And2(w => w.StartTime <= DateTime.Now && w.EndTime >= DateTime.Now && w.IfEnable);
            expr = expr.And2(w => w.ActivityObjectForm == 2);
            expr = expr.And2(w => w.ActivityProduct.Any(a => a.ProductID == ID));
            List<Activity> li = c.Set<Activity>().Where(expr).ToList();

            List<ActivityDTO> list = ActivityDTO.GetActivityDTOList(li);
            foreach (ActivityDTO item in list)
            {
                foreach (ActivityGiftsDTO items in item.ActivityGifts)
                {
                    items.ProductImg = c.ProductImage.Where(w => w.ProductID == items.GiftID && w.ImgType == 1).FirstOrDefault().Url;
                }
            }


            return list;
        }

        public bool AddProduct(Product dt, Guid userId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Product pt = new Product();
                pt.ID = Guid.NewGuid();
                pt.AddTime = DateTime.Now;
                pt.ProductNature = dt.ProductNature;
                pt.KeyWord = dt.KeyWord;
                pt.Price = dt.Price;
                pt.ProductName = dt.ProductName;
                pt.ProductTypeID = dt.ProductTypeID;
                pt.ProductWeight = dt.ProductWeight;
                pt.PV = dt.PV;
                pt.SalesVolume = dt.SalesVolume;
                pt.State = dt.State;
                pt.Unit = dt.Unit;
                pt.CrProductId = dt.CrProductId;
                pt.ProductCode = dt.ProductCode;
                pt.PX = dt.PX;
                pt.Mulriple = dt.Mulriple;
                pt.IsBy = dt.IsBy;
                pt.ETime = dt.ETime;
                pt.STime = dt.STime;
                pt.StockNumber = dt.StockNumber;
                pt.IsStarProduct = dt.IsStarProduct;

                c.Product.Add(pt);
                c.SaveChanges();
                return true;
            }
        }
        public bool UpProduct(Product dt, Guid userId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Product pt = c.Product.Where(w => w.ID == dt.ID).FirstOrDefault();
                int oldStockNumber = pt.StockNumber;

                pt.ProductNature = dt.ProductNature;
                pt.KeyWord = dt.KeyWord;
                pt.Price = dt.Price;
                pt.ProductName = dt.ProductName;
                pt.ProductTypeID = dt.ProductTypeID;
                pt.ProductWeight = dt.ProductWeight;
                pt.PV = dt.PV;
                pt.SalesVolume = dt.SalesVolume;
                pt.State = dt.State;
                pt.Unit = dt.Unit;
                pt.CrProductId = dt.CrProductId;
                pt.ProductCode = dt.ProductCode;
                pt.PX = dt.PX;
                pt.Mulriple = dt.Mulriple;
                pt.IsBy = dt.IsBy;
                pt.ETime = dt.ETime;
                pt.STime = dt.STime;
                pt.StockNumber = dt.StockNumber;
                pt.IsStarProduct = dt.IsStarProduct;

                c.SaveChanges();
                return true;
            }

        }

        /// <summary>
        /// 产品出入库
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="stockNum"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool InOutProduct(Guid productId, int stockNum, string message, Guid userId)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Product pt = c.Product.Where(w => w.ID == productId).FirstOrDefault();
                int num = pt.StockNumber + stockNum;

                if (num < 0)
                {
                    throw new DMException("出入库失败，库存不能小于0");
                }

                pt.StockNumber = num;

                ProductStockHistory productStockHistory = new ProductStockHistory();
                productStockHistory.ID = Guid.NewGuid();
                productStockHistory.ProductID = productId;
                productStockHistory.StockNumber = stockNum;
                productStockHistory.OperateDate = DateTime.Now;
                productStockHistory.OperateUserId = userId;
                productStockHistory.Comment = message;
                c.ProductStockHistory.Add(productStockHistory);
                c.SaveChanges();
                return true;
            }
        }

        public bool SaveProductImg(List<SaveProductImgDTO> dto, Guid productID)
        {
            List<ProductImage> li = new List<ProductImage>();

            ImageHelper img = new ImageHelper();


            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                //查询
                li = c.ProductImage.Where(n => n.ProductID == productID).ToList();
                //删除
                bool ifnull = true;
                for (int i = li.Count - 1; i >= 0; i--)
                {
                    ifnull = true;
                    if (dto != null)
                    {
                        foreach (var items in dto)
                        {
                            if (li.ElementAt(i).ID == items.ID)
                            {
                                ifnull = false;
                            }
                        }
                    }
                    if (ifnull)
                    {
                        c.ProductImage.Remove(li.ElementAt(i));
                    }
                }
                //修改添加
                if (dto != null)
                {
                    foreach (var item in dto)
                    {
                        ifnull = true;
                        for (int i = 0; i < li.Count; i++)
                        {
                            if (item.ID == li.ElementAt(i).ID)
                            {
                                if (item.Url != null & item.Url != "")
                                {
                                    li.ElementAt(i).Url = item.Url;
                                }

                                li.ElementAt(i).PX = item.PX;
                                ifnull = false;
                            }
                        }
                        if (ifnull)
                        {
                            ProductImage pt = new ProductImage();
                            pt.ID = Guid.NewGuid();
                            pt.ImgType = item.ImgType;
                            pt.Url = item.Url;
                            pt.ProductID = item.ProductID;
                            pt.PX = item.PX;
                            c.ProductImage.Add(pt);
                        }
                    }
                }


                c.SaveChanges();
                return true;
            }
        }
        public List<ProductImgDTO> GetProductImg(Guid id)
        {
            System.Linq.Expressions.Expression<Func<ProductImage, bool>> expr = n => true;

            if (id != null)
            {
                expr = expr.And2(n => n.ProductID == id);
            }

            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ProductImgDTO> dto = c.ProductImage.Where(expr).OrderByDescending(px => px.PX).Select(
                    x => new ProductImgDTO
                    {
                        ID = x.ID,
                        PX = x.PX,
                        ProductID = x.ProductID,
                        Url = x.Url,
                        ImgType = x.ImgType

                    }
                ).ToList();

                return dto;
            }
        }

        #endregion

        #region 产品属性操作

        public List<ProductAttributeDTO> GetProductAttribute(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<ProductAttributeDTO> userDto = c.ProductAttribute.Where(n => n.ProductID == id).OrderByDescending(by => by.PX).Select(
                    x => new ProductAttributeDTO
                    {
                        ID = x.ID,
                        PX = x.PX,
                        ProductID = x.ProductID,
                        AttributeValue = x.AttributeValue,
                        AttributeKey = x.AttributeKey
                    }
                ).ToList();

                return userDto;
            }
        }

        public bool AddAttribute(ProductAttributeTT dt)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductAttribute pt = new ProductAttribute();
                pt.ID = Guid.NewGuid();
                pt.AttributeKey = dt.AttributeKey;
                pt.AttributeValue = dt.AttributeValue;
                pt.ProductID = dt.ProductID;
                pt.PX = dt.PX;

                c.ProductAttribute.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public bool deProductAttribute(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                ProductAttribute pt = c.ProductAttribute.FirstOrDefault(n => n.ID == id);

                c.ProductAttribute.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }


        #endregion

        #region 产品，礼包赠品
        public List<GiftProductDTO> GetGiftProduct(Guid productID)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                List<GiftProductDTO> userDto = c.GiftProduct.Where(n => n.ProductID == productID).Select(
                    x => new GiftProductDTO
                    {
                        ID = x.ID,
                        GiftID = x.GiftID,
                        GiftNum = x.GiftNum,
                        ProductID = x.ProductID,
                        ProductCode = c.Product.Where(w => w.ID == x.GiftID).FirstOrDefault().ProductCode,//. x.Product1.ProductCode,
                        ProductName = c.Product.Where(w => w.ID == x.GiftID).FirstOrDefault().ProductName,//x.Product1.ProductName,
                        DeLevelID = x.DeLevelID,
                        GiftPrice = x.GiftPrice
                    }
                ).ToList();

                return userDto;
            }
        }
        public bool AddGiftProduct(GiftProductTT rDto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                GiftProduct pt = new GiftProduct();
                pt.ID = Guid.NewGuid();
                pt.GiftID = rDto.GiftID;
                pt.GiftNum = rDto.GiftNum;
                pt.ProductID = rDto.ProductID;
                pt.DeLevelID = rDto.DeLevelID;
                pt.GiftPrice = rDto.GiftPrice;

                c.GiftProduct.Add(pt);
                c.SaveChanges();
                return true;
            }
        }

        public bool deGiftProduct(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                GiftProduct pt = c.GiftProduct.FirstOrDefault(n => n.ID == id);

                c.GiftProduct.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #region  首页图片管理     

        public List<IndexImagesManageDTO> GetIndexImagesManage(int? ImgType, int? DelevelID)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                Expression<Func<IndexImagesManage, bool>> expr = n => true;
                if (ImgType != null)
                {
                    expr = expr.And2(w => w.ImgType == ImgType);
                }
                if (DelevelID != null)
                {
                    expr = expr.And2(w => w.ImgDeLevel.Any(ay => ay.DeLevelID == DelevelID));
                }

                List<IndexImagesManageDTO> dto = c.IndexImagesManage.Where(expr).OrderByDescending(ob => ob.PX).Select(
                    x => new IndexImagesManageDTO
                    {
                        ID = x.ID,
                        ImgType = x.ImgType,
                        ImgURL = x.ImgURL,
                        Name = x.Name,
                        ProductID = x.ProductID,
                        ProductTypeID = x.ProductTypeID,
                        PX = x.PX,
                        ImgDeLevel = x.ImgDeLevel.Select(
                            n => new ImgDeLevelDTO { DeLevelName = n.DeLevel.Name, DeLevelID = n.DeLevelID, ID = n.ID, ImgID = n.ImgID }
                            ).ToList()
                    }
                    ).ToList();

                return dto;//IndexImagesManageDTO.GetIndexImagesManageDTOList(dto);
            }
        }

        public bool AddIndexImagesManage(IndexImagesManageDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                IndexImagesManage pt = new IndexImagesManage();
                pt = GetMapperDTO.SetModel<IndexImagesManage, IndexImagesManageDTO>(dto);
                pt.ID = Guid.NewGuid();

                c.IndexImagesManage.Add(pt);

                string[] ImgDeLevel = dto.AddImgDeLevel.Split('-'); ;


                foreach (string item in ImgDeLevel)
                {
                    ImgDeLevel idl = new ImgDeLevel();
                    idl.DeLevelID = Convert.ToInt32(item);
                    idl.ID = Guid.NewGuid();
                    idl.ImgID = pt.ID;

                    c.ImgDeLevel.Add(idl);
                }

                c.SaveChanges();
                return true;
            }
        }


        public bool UpIndexImagesManage(IndexImagesManageDTO dto)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                IndexImagesManage pt = c.IndexImagesManage.FirstOrDefault(n => n.ID == dto.ID);
                pt = GetMapperDTO.SetModel<IndexImagesManage, IndexImagesManageDTO>(dto);

                List<ImgDeLevel> li = c.ImgDeLevel.Where(w => w.ImgID == pt.ID).ToList();
                for (int i = li.Count - 1; i >= 0; i--)
                {
                    c.ImgDeLevel.Remove(li.ElementAt(i));
                }

                string[] ImgDeLevel = dto.AddImgDeLevel.Split('-');

                foreach (string item in ImgDeLevel)
                {
                    ImgDeLevel idl = new ImgDeLevel();
                    idl.DeLevelID = Convert.ToInt32(item);
                    idl.ID = Guid.NewGuid();
                    idl.ImgID = pt.ID;

                    c.ImgDeLevel.Add(idl);
                }

                c.SaveChanges();
                return true;
            }
        }

        public bool deIndexImagesManage(Guid id)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                IndexImagesManage pt = c.IndexImagesManage.FirstOrDefault(n => n.ID == id);

                List<ImgDeLevel> li = c.ImgDeLevel.Where(w => w.ImgID == pt.ID).ToList();
                for (int i = li.Count - 1; i >= 0; i--)
                {
                    c.ImgDeLevel.Remove(li.ElementAt(i));
                }

                c.IndexImagesManage.Remove(pt);

                c.SaveChanges();
                return true;
            }
        }

        #endregion

        #endregion

        #region 前台


        #endregion
    }
}
