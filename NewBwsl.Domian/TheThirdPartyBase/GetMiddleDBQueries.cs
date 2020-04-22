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
    public class GetMiddleDBQueries
    {
        public readonly string _connectionMiddleString = "data source=125.69.67.83,26159;initial catalog=BwslRetail;user id=sa;password=Fny@1234";

        /// <summary>
        /// 根据订单编号判断是否在中间库订单表中存在
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public bool HasMiddleDBOrderById(string ordercode)
        {

            bool result = false;
            string queryString = "Select count(*) From [Order] Where    OrderNumber = @code";
            using (SqlConnection connection = new SqlConnection(_connectionMiddleString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@code", ordercode));
                connection.Open();
                //  SqlDataReader reader = command.ExecuteReader();
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
        /// 根据订单id集，返回中间库订单列表
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public List<MiddleOrderDTO> HasMiddleDBOrderByIds(List<string> orderIds)
        {

            string ids = string.Empty;
            List<MiddleOrderDTO> dtos = new List<MiddleOrderDTO>();
            SqlHelper helpe = new SqlHelper(_connectionMiddleString);
            foreach (var item in orderIds)
            {
                if (!string.IsNullOrWhiteSpace(ids)) ids += ",";
                ids += "'" + item + "'";
            }
            string queryString = "Select Code From [Order] Where OrderNumber  in (" + ids + ")";
            DataSet data = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionMiddleString))
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
