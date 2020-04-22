using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.TheThirdPartyBase
{
    public class SqlHelper
    {
        public readonly string _connectionString;

        public SqlHelper(string connectionString)
        {
            this._connectionString = connectionString;
        }
        /// <summary>
        /// 执行insert,update,delete命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns>返回受影响的行数</returns>
        public int ExcuteSQLReturnInt(string sql, SqlParameter[] pars)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == System.Data.ConnectionState.Closed || conn.State == System.Data.ConnectionState.Broken)
                    {
                        conn.Open();
                    }
                    if (pars != null && pars.Length > 0)
                    {
                        foreach (SqlParameter p in pars)
                        {
                            cmd.Parameters.Add(p);
                        }
                    }

                    int count = cmd.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 执行一个查询，返回结果集的首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = commandType;
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            conn.Open();
            //cmd.ExecuteScalar()+执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
            object result = cmd.ExecuteScalar();
            conn.Close();
            return result;
        }
        /// <summary>
        /// 执行一个查询，返回查询的结果集。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandtype"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandtype, SqlParameter[] parameters)
        {
            DataTable data = new DataTable();  //实例化datatable,用于装载查询的结果集
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = commandtype;
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);   //将参数添加到sql语句中。
                        }
                    }
                    //申明sqldataadapter，通过cmd来实例化它，这个是数据设备器，可以直接往datatable,dataset中写入。
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(data);   //利用Fill来填充。
                }
            }
            return data;
        }
        public List<T> DataSetToIList<T>(DataSet p_DataSet, int p_TableIndex)
        {

            if (p_DataSet == null || p_DataSet.Tables.Count <= 0)
                return null;
            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return null;
            if (p_TableIndex < 0)
                p_TableIndex = 0;

            DataTable p_Data = p_DataSet.Tables[p_TableIndex];
            // 返回值初始化 
            List<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < p_Data.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (pi.Name.Equals(p_Data.Columns[i].ColumnName))
                        {
                            // 数据库NULL值单独处理 
                            if (p_Data.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, p_Data.Rows[j][i], null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }

    }
}
