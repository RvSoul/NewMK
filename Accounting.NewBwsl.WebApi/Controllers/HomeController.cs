using NewMK.Domian.DM;
using NewMK.DTO.Bonus;
using NewMK.DTO.Order;
using NewMK.DTO.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Accounting.NewMK.WebApi.Controllers
{
    public class HomeController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
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
                    #region MoneyType
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

                    #endregion
                    #region ZMoneyType
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
                    #endregion


                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeTime + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeMarks + "</Data></Cell>");
                    #region State
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
                    #endregion


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

        public ActionResult GetExChangeBonus_CashDTODownload([FromUri]Request_ExChangeBonus_CashDTO dto)
        {
            RecordDM recordDM = new RecordDM();
            List<ExChangeBonus_CashDTO> orderInfo = recordDM.GetExChangeListBonus_CashDownload(dto);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("提现记录{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>用户名称</Data></Cell>");

                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>提现日期</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>银行账号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>开户名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>开户银行</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>开户地址</Data></Cell>");

                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>提现金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>手续费比例</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>手续费</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>实际发放</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>备注</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>状态</Data></Cell>");
                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserName + "</Data></Cell>");

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeTime + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>" + orderInfo[j].BankNumber + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>" + orderInfo[j].BakName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>" + orderInfo[j].BakAddress + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>" + orderInfo[j].BakBranchAddress + "</Data></Cell>");

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeMoney + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].FeeRat + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].FeeAmount + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].CashAmount + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].ChangeMarks + "</Data></Cell>");
                    #region State
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
                    #endregion
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

                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单编号</Data></Cell>");
                //Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>推荐人编号</Data></Cell>");
                //Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>推荐人姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单类型</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单金额</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单PV</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>订单状态</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>是否结算</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>店铺姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>店铺编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>创建时间</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>支付方式</Data></Cell>");

                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");

                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].UserName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNumber + "</Data></Cell>");
                    //Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNum + "</Data></Cell>");
                    //Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderNum + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].OrderTypeName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].MoneyProduct + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].TotalPv + "</Data></Cell>");
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
                    else if (orderInfo[j].State == 10)
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
        /// 奖励积分导出
        /// </summary>
        /// <param name="num"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dealercode"></param>
        /// <param name="dealername"></param>
        /// <returns></returns>
        public ActionResult GetWeekPrizeDownload(int? num, DateTime? start, DateTime? end, string dealercode, string dealername)
        {
            BonusDM bonusDM = new BonusDM();
            List<WeekPrizeDTO> orderInfo = bonusDM.GetWeekPrizeDownload(true, num, start, end, dealercode, dealername);
            if (orderInfo.Count > 0)
            {
                string FileName = string.Format("奖励积分表{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>结算期</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>结算日期</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>编号</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>姓名</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>会员级别</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>经销商</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>销售积分</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>零售积分</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>店补</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>服务费</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>实发奖励积分</Data></Cell>");
                Response.Write("<Cell ss:StyleID='Header'><Data ss:Type='String'>发放时间</Data></Cell>");
                Response.Write("\r\n</Row>");

                for (int j = 0; j < orderInfo.Count; j++)
                {
                    Response.Write("<Row>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].Num + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].Date + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].DealerCode + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].DealerName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].DeLevelName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].HonLevelName + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].Sale + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].Record + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].ServiceCenter + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].ReduceService + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='Number'>" + orderInfo[j].TotalPrize + "</Data></Cell>");
                    Response.Write("<Cell ss:StyleID='border'><Data ss:Type='String'>" + orderInfo[j].GrantDate.ToString() + "</Data></Cell>");
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
