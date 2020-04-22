using NewMK.DTO.TheThirdPartyBaseDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.TheThirdPartyBase
{
    public class GetCROrderQueries
    {
        public readonly string _connectionChaoRanString = "data source=125.69.67.83,59753;initial catalog=CR_YPPF10;user id=sa;password=Fny@1234";
        public readonly bool isOK = false;


        /// <summary>
        /// 根据订单编号判断是否在超然销售订单中存在
        /// </summary>
        /// <returns></returns>
        public bool HasCROrderById(string ordercode)
        {
            bool result = false;
            string queryString = "Select count(*) From gxddhz Where webdjbh= @OrderCode And Is_Zx<> '清'";
            using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@OrderCode", ordercode));
                connection.Open();
                // SqlDataReader reader = command.ExecuteReader();
                try
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return result;
        }

        public bool HasCROrderById3(string ordercode)
        {
            bool result = false;
            string queryString = "Select count(*) From gxddhz Where webdjbh= @OrderCode";
            using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@OrderCode", ordercode));
                connection.Open();
                // SqlDataReader reader = command.ExecuteReader();
                try
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return result;
        }

        /// <summary>
        /// =清，表示订单关闭
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public bool HasCROrderById2(string ordercode)
        {
            bool result = false;
            string queryString = "Select count(*) From gxddhz Where webdjbh= @OrderCode And Is_Zx= '清'";
            using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@OrderCode", ordercode));
                connection.Open();
                // SqlDataReader reader = command.ExecuteReader();
                try
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return result;
        }
        public List<MiddleOrderDTO> GetCROrderByIds(List<string> orderIds)
        {
            List<MiddleOrderDTO> dtos = new List<MiddleOrderDTO>();
            SqlHelper helpe = new SqlHelper(_connectionChaoRanString);
            string ids = string.Empty;
            foreach (var item in orderIds)
            {
                if (!string.IsNullOrWhiteSpace(ids)) ids += ",";
                ids += "'" + item + "'";
            }
            string queryString = "Select webdjbh as Code From gxddhz Where webdjbh in (" + ids + ") And Is_Zx<> '清' ";
            DataSet data = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, connection))
                    {
                        //申明sqldataadapter，通过cmd来实例化它，这个是数据设备器，可以直接往datatable,dataset中写入。
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(data);   //利用Fill来填充。

                        dtos = helpe.DataSetToIList<MiddleOrderDTO>(data, 0);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dtos;
        }
        /// <summary>
        /// 根据订单编号判断是否在超然系统中已做波次决策
        /// </summary>
        /// <returns></returns>
        public bool HasCRBCJCById(string orderId)
        {
            bool result = false;
            string queryString = @"select count(*) from (Select M.Orderid From gxddhz M, bcjhmx D --已完成波次决策
Where M.djbh = D.xgdjbh
  And M.webdjbh  =@Id
Union All
Select M.Orderid From gxddhz M, cr_dj_BACB03 D --正在进行波次决策
Where M.djbh = D.xgdjbh
  And M.webdjbh  = @Id) c";
            using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@Id", orderId));
                connection.Open();
                // SqlDataReader reader = command.ExecuteReader();
                try
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return result;
        }

        /// <summary>
        /// =1允许删
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool HasCRBCJCByIdDE(string orderId)
        {
            bool result = false;
            string queryString = @"
Select Count(*) iCount
From
(
   Select webdjbh,Sum(iCount_All) iCount_All,Sum(iCount_Close) iCount_Close
   From
       (
           Select webdjbh, Count(*) iCount_All, 0 iCount_Close
           From gxddhz m,gxddmx d
           Where M.djbh = d.djbh
             And M.webdjbh = @Id
		   Group By webdjbh
           Union All
           Select webdjbh,0 iCount_All,Count(*) iCount_Close
           From gxddhz m,gxddmx d
           Where M.djbh = d.djbh
             And M.webdjbh = @Id
             And M.Is_zx = '清'
             And D.is_zx = '清'
		   Group By webdjbh
)  Detail
	   Group By webdjbh
) T Where webdjbh = @Id And iCount_All = iCount_Close
";
            using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@Id", orderId));
                connection.Open();
                // SqlDataReader reader = command.ExecuteReader();
                try
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 1)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return result;
        }

        /// <summary>
        /// =0允许删除
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool IsChonghong(string orderId)
        {
            bool result = false;
            string queryString = @"Select count(*) From(Select Spid,Sum(Shl) Shl
From
(
   Select E.Spid,D.Shl 
   From Gxddhz M,
        Gxddmx D,
   	    Spzl   E
   Where M.Djbh    = D.Djbh 
     And M.Is_Zx   <> '清'
     And M.WebDjbh = @Id
     And D.SpId    = E.SpId
   Union All
   Select D.Spid,D.Shl
   From gxywhz M,
        Gxywmx D,
   	    Spzl   E
   Where M.Djbh   = D.Djbh
     And M.Zhy    Like '%冲红单'
     And M.djbs   = 'XHC'
     And M.WebDjbh = @Id
     And D.SpId   = E.Spid 
   Union All
   Select D.Spid,D.Shl
   From gxkphz M,
        gxkpmx D,
		Spzl   E
   Where M.Djbh  = D.Djbh
     And M.djbs  = 'XZB'
	 And M.Is_Zx   <> '清'
     And D.Spid  = E.Spid
	 And M.WebDjbh = @Id
   Union All
   Select D.Spid,D.Shl
   From gxywhz M,
        Gxywmx D,
   	    Spzl   E
   Where M.Djbh   =     D.Djbh
     And M.djbs   =    'XZC'
	 And M.Zhy    Like '%冲红单'
     And M.WebDjbh = @Id
     And D.SpId   = E.Spid 
) T
Group By Spid
Having Sum(Shl) <> 0) TT";
            using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@Id", orderId));
                connection.Open();
                // SqlDataReader reader = command.ExecuteReader();
                try
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            return result;
        }

        public List<MiddleOrderDTO> GetCRBCJCByIds(List<string> orderIds)
        {

            List<MiddleOrderDTO> dtos = new List<MiddleOrderDTO>();
            SqlHelper helpe = new SqlHelper(_connectionChaoRanString);
            string ids = string.Empty;
            foreach (var item in orderIds)
            {
                if (!string.IsNullOrWhiteSpace(ids)) ids += ",";
                ids += "'" + item + "'";
            }
            string queryString = @"Select M.Webdjbh as Code From gxddhz M,      bcjhmx D Where M.djbh = D.xgdjbh  And  M.Webdjbh in( " + ids + ")  Union All Select M.Webdjbh as Code  From gxddhz M,      cr_dj_BACB03 D Where M.djbh = D.xgdjbh   And M.Webdjbh in ( " + ids + ")";
            DataSet data = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionChaoRanString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, connection))
                    {
                        //申明sqldataadapter，通过cmd来实例化它，这个是数据设备器，可以直接往datatable,dataset中写入。
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                        adapter.Fill(data);   //利用Fill来填充。

                        dtos = helpe.DataSetToIList<MiddleOrderDTO>(data, 0);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dtos;
        }
    }
}
