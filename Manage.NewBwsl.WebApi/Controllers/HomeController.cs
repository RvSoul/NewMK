using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.DTO.Bonus;
using NewMK.DTO.Order;
using NewMK.DTO.Record;
using NewMK.DTO.User;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Manage.NewMK.WebApi.Controllers
{
    public class HomeController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        UserDM userDM = new UserDM();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult GetUserListDownload(string phone, string userName)
        {
            List<UserDTO> orderInfo = userDM.GetUserListDownload(phone, userName);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("用户列表{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region 格式
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.Charset = "utf-8";
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("Content-Transfer-Encoding", "binary");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(Request.UserAgent))
                {
                    if (!(Request.UserAgent.IndexOf("Firefox") > -1 && Request.UserAgent.IndexOf("Gecko") > -1))
                    {
                        FileName = Server.UrlEncode(FileName);
                    }
                }
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + ".xls");
                Response.Write("<?xml version='1.0'?><?mso-application progid='Excel.Sheet'?>");
                Response.Write(@"<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'  xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>");

                Response.Write(@"<Styles><Style ss:ID='Default' ss:Name='Normal'><Alignment ss:Vertical='Center'/><Borders/><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12'/><Interior/><NumberFormat/><Protection/></Style>");
                // 定义标题样式
                Response.Write(@"<Style ss:ID='Header'><Borders>
       <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12' ss:Bold='1'/></Style>");
                // 定义边框
                Response.Write(@"<Style ss:ID='border'><NumberFormat ss:Format='@'/><Borders>  
      <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders></Style>");
                Response.Write("</Styles>");
                #endregion


                Response.Write("<Worksheet ss:Name='" + "Sheet1" + "'>");
                Response.Write("<Table x:FullColumns='1' x:FullRows='1'>");
                Response.Write("\r\n<Row ss:AutoFitHeight='1'>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>顾客姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>顾客电话</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>顾客类型</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>注册时间</Data></Cell>");
                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Phone + "</Data></Cell>");
                    if (orderInfo[j].DeLevelID == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "普通" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].DeLevelID == 2)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "员工" + "</Data></Cell>");
                    }
                    else
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "会员" + "</Data></Cell>");
                    }

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].AddTime + "</Data></Cell>");
                    Response.Write("</Row>");
                }
                Response.Write("</Table>");
                Response.Write("</Worksheet>");
                Response.Flush();

                Response.Write("</Workbook>");
                Response.End();
            }
            return View("Index");
        }

        public ActionResult GetExChangeListDownload([FromUri]Request_ExChangeDTO dto)
        {
            RecordDM recordDM = new RecordDM();
            List<ExChangeDTO> orderInfo = recordDM.GetExChangeListDownload(dto);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("资金记录{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region 格式
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.Charset = "utf-8";
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("Content-Transfer-Encoding", "binary");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(Request.UserAgent))
                {
                    if (!(Request.UserAgent.IndexOf("Firefox") > -1 && Request.UserAgent.IndexOf("Gecko") > -1))
                    {
                        FileName = Server.UrlEncode(FileName);
                    }
                }
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + ".xls");
                Response.Write("<?xml version='1.0'?><?mso-application progid='Excel.Sheet'?>");
                Response.Write(@"<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'  xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>");

                Response.Write(@"<Styles><Style ss:ID='Default' ss:Name='Normal'><Alignment ss:Vertical='Center'/><Borders/><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12'/><Interior/><NumberFormat/><Protection/></Style>");
                // 定义标题样式
                Response.Write(@"<Style ss:ID='Header'><Borders>
       <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12' ss:Bold='1'/></Style>");
                // 定义边框
                Response.Write(@"<Style ss:ID='border'><NumberFormat ss:Format='@'/><Borders>  
      <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders></Style>");
                Response.Write("</Styles>");
                #endregion


                Response.Write("<Worksheet ss:Name='" + "Sheet1" + "'>");
                Response.Write("<Table x:FullColumns='1' x:FullRows='1'>");
                Response.Write("\r\n<Row ss:AutoFitHeight='1'>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>用户编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>用户名称</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>初期</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>变化</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>期末</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>交易类型</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>详细类型</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>交易时间</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>备注</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>状态</Data></Cell>");
                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNum + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].BeforeChangeMoney + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeMoney + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].AfterChangeMoney + "</Data></Cell>");
                    if (orderInfo[j].MoneyType == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "积分" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].MoneyType == 2)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "微信" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].MoneyType == 3)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "快钱" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].MoneyType == 4)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "奖励积分" + "</Data></Cell>");
                    }

                    if (orderInfo[j].ZMoneyType == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "充值" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 2)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "奖励积分转积分" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 3)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "订单退款" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 4)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "转账转入" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 5)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "下单支付" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 6)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "转账转出" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 7)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "下单支付" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 8)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "退单退款" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 9)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "充值" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 10)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "下单支付" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 11)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "退单退款" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 12)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "充值" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 13)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "奖励积分发放" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 14)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "奖励积分提现" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].ZMoneyType == 15)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "奖励积分转积分" + "</Data></Cell>");
                    }

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeTime + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeMarks + "</Data></Cell>");
                    if (orderInfo[j].State == 0)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "其他" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "已申请" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 2)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "已核实待转账" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 3)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "已转账" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 4)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "已驳回" + "</Data></Cell>");
                    }

                    Response.Write("</Row>");
                }
                Response.Write("</Table>");
                Response.Write("</Worksheet>");
                Response.Flush();

                Response.Write("</Workbook>");
                Response.End();
            }
            return View("Index");
        }

        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult GetOrdersListDownload([FromUri]Request_Order dto)
        {
            OrderDM orderDM = new OrderDM();
            List<OrdersDTO> orderInfo = orderDM.GetOrdersListDownload(dto);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("订单表{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region 格式
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.Charset = "utf-8";
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("Content-Transfer-Encoding", "binary");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(Request.UserAgent))
                {
                    if (!(Request.UserAgent.IndexOf("Firefox") > -1 && Request.UserAgent.IndexOf("Gecko") > -1))
                    {
                        FileName = Server.UrlEncode(FileName);
                    }
                }
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + ".xls");
                Response.Write("<?xml version='1.0'?><?mso-application progid='Excel.Sheet'?>");
                Response.Write(@"<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'  xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>");

                Response.Write(@"<Styles><Style ss:ID='Default' ss:Name='Normal'><Alignment ss:Vertical='Center'/><Borders/><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12'/><Interior/><NumberFormat/><Protection/></Style>");
                // 定义标题样式
                Response.Write(@"<Style ss:ID='Header'><Borders>
       <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12' ss:Bold='1'/></Style>");
                // 定义边框
                Response.Write(@"<Style ss:ID='border'><NumberFormat ss:Format='@'/><Borders>  
      <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders></Style>");
                Response.Write("</Styles>");
                #endregion


                Response.Write("<Worksheet ss:Name='" + "Sheet1" + "'>");
                Response.Write("<Table x:FullColumns='1' x:FullRows='1'>");
                Response.Write("\r\n<Row ss:AutoFitHeight='1'>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>用户编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>用户姓名</Data></Cell>");
                //Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>推荐人编号</Data></Cell>");
                //Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>推荐人姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单类型</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单状态</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>是否结算</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>收货人地址</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>店铺姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>店铺编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>创建时间</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>支付方式</Data></Cell>");

                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNumber + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserName + "</Data></Cell>");
                    //Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNum + "</Data></Cell>");
                    //Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNum + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderTypeName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].MoneyProduct + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderMoney + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].TotalPv + "</Data></Cell>");
                    if (orderInfo[j].State == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "待付款" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 2)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "待确认" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 3)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "待发货" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 4)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "已发货" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 5)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "已退款" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 7)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "支付已取消" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 8)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "支付失败" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State == 9)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "待退款" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].State ==10)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "退款失败" + "</Data></Cell>");
                    }
                    else
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "无效状态" + "</Data></Cell>");
                    }

                    if (orderInfo[j].IsBalance == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "是" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].IsBalance == 0)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "否" + "</Data></Cell>");
                    }

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ConsigneeAddress + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ServiceCenterName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ServiceCenterCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].AddTime + "</Data></Cell>");

                    if (orderInfo[j].PayType == 1)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "积分" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].PayType == 2)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "微信" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].PayType == 3)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "银行卡" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].PayType == 4)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "积分加微信" + "</Data></Cell>");
                    }
                    else if (orderInfo[j].PayType == 5)
                    {
                        Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + "银行卡加微信" + "</Data></Cell>");
                    }

                    Response.Write("</Row>");
                }
                Response.Write("</Table>");
                Response.Write("</Worksheet>");
                Response.Flush();

                Response.Write("</Workbook>");
                Response.End();
            }
            return View("Index");
        }

        /// <summary>
        /// 产品订单导出
        /// </summary>
        /// <param name="dealerCode"></param>
        /// <param name="productCode"></param>
        /// <param name="OrderTypeID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PrivenceName"></param>
        /// <returns></returns>
        public ActionResult GetProductOrderListDownload(string dealerCode, string productCode, string OrderTypeID, DateTime? startTime, DateTime? endTime, int pageSize, int pageIndex, string PrivenceName)
        {
            OrderDM dm = new OrderDM();
            List<ProductOrderDTO> orderInfo = dm.GetProductOrderList(dealerCode, productCode, OrderTypeID, startTime, endTime, PrivenceName);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("产品订单表{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region 格式
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.Charset = "utf-8";
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("Content-Transfer-Encoding", "binary");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(Request.UserAgent))
                {
                    if (!(Request.UserAgent.IndexOf("Firefox") > -1 && Request.UserAgent.IndexOf("Gecko") > -1))
                    {
                        FileName = Server.UrlEncode(FileName);
                    }
                }
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + ".xls");
                Response.Write("<?xml version='1.0'?><?mso-application progid='Excel.Sheet'?>");
                Response.Write(@"<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'  xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>");

                Response.Write(@"<Styles><Style ss:ID='Default' ss:Name='Normal'><Alignment ss:Vertical='Center'/><Borders/><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12'/><Interior/><NumberFormat/><Protection/></Style>");
                // 定义标题样式
                Response.Write(@"<Style ss:ID='Header'><Borders>
       <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12' ss:Bold='1'/></Style>");
                // 定义边框
                Response.Write(@"<Style ss:ID='border'><NumberFormat ss:Format='@'/><Borders>  
      <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders></Style>");
                Response.Write("</Styles>");
                #endregion


                Response.Write("<Worksheet ss:Name='" + "Sheet1" + "'>");
                Response.Write("<Table x:FullColumns='1' x:FullRows='1'>");
                Response.Write("\r\n<Row ss:AutoFitHeight='1'>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>名称</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品数量</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>赠品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>赠品PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>赠品数量</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>礼包金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>礼包PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>礼包数量</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>活动赠品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>活动赠品PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>活动赠品数量</Data></Cell>");

                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ProductCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ProductName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je1.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv1.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count1.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je2.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv2.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count2.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je3.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv3.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count3.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je4.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv4.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count4.ToString() + "</Data></Cell>");
                    Response.Write("</Row>");
                }
                Response.Write("</Table>");
                Response.Write("</Worksheet>");
                Response.Flush();

                Response.Write("</Workbook>");
                Response.End();
            }
            return View("Index");
        }
        
        /// <summary>
        /// 地区订单统计查询
        /// </summary>
        /// <param name="dealerCode"></param>
        /// <param name="productCode"></param>
        /// <param name="OrderTypeID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PrivenceName"></param>
        /// <returns></returns>
        public ActionResult GetAddressOrderListDownload(string dealerCode, string productCode, string OrderTypeID, DateTime? startTime, DateTime? endTime, int pageSize, int pageIndex, string PrivenceName)
        {
            OrderDM dm = new OrderDM();
            List<AddressOrderDTO> orderInfo = dm.GetAddressOrderList(dealerCode, productCode, OrderTypeID, startTime, endTime, PrivenceName);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("地区订单表{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region 格式
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.Charset = "utf-8";
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("Content-Transfer-Encoding", "binary");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(Request.UserAgent))
                {
                    if (!(Request.UserAgent.IndexOf("Firefox") > -1 && Request.UserAgent.IndexOf("Gecko") > -1))
                    {
                        FileName = Server.UrlEncode(FileName);
                    }
                }
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + ".xls");
                Response.Write("<?xml version='1.0'?><?mso-application progid='Excel.Sheet'?>");
                Response.Write(@"<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'  xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>");

                Response.Write(@"<Styles><Style ss:ID='Default' ss:Name='Normal'><Alignment ss:Vertical='Center'/><Borders/><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12'/><Interior/><NumberFormat/><Protection/></Style>");
                // 定义标题样式
                Response.Write(@"<Style ss:ID='Header'><Borders>
       <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12' ss:Bold='1'/></Style>");
                // 定义边框
                Response.Write(@"<Style ss:ID='border'><NumberFormat ss:Format='@'/><Borders>  
      <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders></Style>");
                Response.Write("</Styles>");
                #endregion


                Response.Write("<Worksheet ss:Name='" + "Sheet1" + "'>");
                Response.Write("<Table x:FullColumns='1' x:FullRows='1'>");
                Response.Write("\r\n<Row ss:AutoFitHeight='1'>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>地区</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>产品数量</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>赠品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>赠品PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>赠品数量</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>礼包金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>礼包PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>礼包数量</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>活动赠品金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>活动赠品PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>活动赠品数量</Data></Cell>");

                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ConsigneeProvince + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je1.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv1.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count1.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je2.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv2.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count2.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je3.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv3.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count3.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].je4.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].pv4.ToString() + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Count4.ToString() + "</Data></Cell>");
                    Response.Write("</Row>");
                }
                Response.Write("</Table>");
                Response.Write("</Worksheet>");
                Response.Flush();

                Response.Write("</Workbook>");
                Response.End();
            }
            return View("Index");
        }

        /// <summary>
        /// 推荐业绩查询导出
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult DownloadPro_Get_RecomCase_Query(DateTime? time1, DateTime? time2, string code)
        {
            List<Pro_Get_RecomCase_QueryDTO> orderInfo = userDM.GetPro_Get_RecomCase_Query(time1, time2, code);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("荣誉级别业绩查询{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region 格式
                Response.ClearContent();
                Response.BufferOutput = true;
                Response.Charset = "utf-8";
                Response.ContentType = "application/ms-excel";
                Response.AddHeader("Content-Transfer-Encoding", "binary");
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(Request.UserAgent))
                {
                    if (!(Request.UserAgent.IndexOf("Firefox") > -1 && Request.UserAgent.IndexOf("Gecko") > -1))
                    {
                        FileName = Server.UrlEncode(FileName);
                    }
                }
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + ".xls");
                Response.Write("<?xml version='1.0'?><?mso-application progid='Excel.Sheet'?>");
                Response.Write(@"<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel'  xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40'>");

                Response.Write(@"<Styles><Style ss:ID='Default' ss:Name='Normal'><Alignment ss:Vertical='Center'/><Borders/><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12'/><Interior/><NumberFormat/><Protection/></Style>");
                // 定义标题样式
                Response.Write(@"<Style ss:ID='Header'><Borders>
       <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
       <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders><Font ss:FontName='宋体' x:CharSet='134' ss:Size='12' ss:Bold='1'/></Style>");
                // 定义边框
                Response.Write(@"<Style ss:ID='border'><NumberFormat ss:Format='@'/><Borders>  
      <Border ss:Position='Bottom' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Left' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Right' ss:LineStyle='Continuous' ss:Weight='1'/>  
      <Border ss:Position='Top' ss:LineStyle='Continuous' ss:Weight='1'/>
    </Borders></Style>");
                Response.Write("</Styles>");
                #endregion


                Response.Write("<Worksheet ss:Name='" + "Sheet1" + "'>");
                Response.Write("<Table x:FullColumns='1' x:FullRows='1'>");
                Response.Write("\r\n<Row ss:AutoFitHeight='1'>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>推荐人编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>推荐人</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>联系电话</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>荣誉级别</Data></Cell>");

                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>游客人数</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>顾客人数</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>VIP人数</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>创客人数</Data></Cell>");

                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>部门编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>部门</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>部门主消</Data></Cell>");

                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>个人主消</Data></Cell>");
                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].PDealerCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].PName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].TelPhone + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].HonLevel + "</Data></Cell>");

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].YkCount + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].GkCount + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].VIPGkCount + "</Data></Cell>");                    
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].CKCount + "</Data></Cell>");

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].DealerCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].DName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].Dealer_Zx_Pv + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].PDealer_Zx_Pv + "</Data></Cell>");
                    Response.Write("</Row>");
                }
                Response.Write("</Table>");
                Response.Write("</Worksheet>");
                Response.Flush();

                Response.Write("</Workbook>");
                Response.End();
            }
            return View("Index");
        }

    }
}
