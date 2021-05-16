#if SERVICE && SQL_SERVER
namespace Net.Component
{
    using Net.Event;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// SQLServer访问数据工具类
    /// </summary>
    public class SqlServerHelper
    {
        // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写
        private static string connetStr = "server=127.0.0.1;port=3306;user=root;password=root; database=mygame;Charset=utf8";
        public static string ConnetStr
        {
            get
            {
                return connetStr;
            }
            set
            {
                connetStr = value;
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection = null;
                }
            }
        }

        private static SqlConnection sqlConnection;
        public static SqlConnection SqlConnection
        {
            get
            {
            JUMP: if (sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(connetStr);
                    try
                    {
                        sqlConnection.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                    }
                    catch (SqlException ex)
                    {
                        sqlConnection.Close();
                        sqlConnection = null;
                        NDebug.LogError("打开数据库失败!" + ex);
                        goto GO;
                    }
                }
                else if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Close();
                    sqlConnection = null;
                    goto JUMP;
                }
            GO: return sqlConnection;
            }
        }

        public static bool ConnectTest()
        {
            return SqlConnection != null;
        }

        private static readonly SqlCommand command = new SqlCommand();

        /// <summary>
        /// 查询数据库
        /// 查询表数据 "select * from student"
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="call"></param>
        public static void Query(string cmdText, Action<DataTable> call)
        {
            lock (SqlConnection)
            {
                command.CommandText = cmdText;
                command.Connection = SqlConnection;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);
                call(dt);
                sqlDataAdapter.Dispose();
                dt.Dispose();
            }
        }

        /// <summary>
        /// 查询数据库
        /// 查询表数据 "select * from student"
        /// </summary>
        /// <param name="cmdText"></param>
        public static DataTable QueryTable(string cmdText)
        {
            lock (SqlConnection)
            {
                command.CommandText = cmdText;
                command.Connection = SqlConnection;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);
                sqlDataAdapter.Dispose();
                return dt;
            }
        }

        /// <summary>
        /// 查询数据
        /// SELECT * FROM tsbm WHERE 编码='cq005'
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static SqlDataReader Query(string cmdText)
        {
            lock (SqlConnection)
            {
                command.CommandText = cmdText;
                command.Connection = SqlConnection;
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// 修改数据库列表的数据
        /// 查询表数据 "select * from student"
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="call"></param>
        public static void Modify(string cmdText, Action<DataTable> call)
        {
            lock (SqlConnection)
            {
                command.CommandText = cmdText;
                command.Connection = SqlConnection;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);
                call(dt);
                // 将DataSet的修改提交至“数据库”
                SqlCommandBuilder mySqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.Update(dt);
                sqlDataAdapter.Dispose();
                dt.Dispose();
                mySqlCommandBuilder.Dispose();
            }
        }

        /// <summary>
        /// 处理数据
        /// "insert into student values (12,'张三',25,'大专')"
        /// </summary>
        /// <param name="cmdText"></param>
        public static int ExecuteNonQuery(string cmdText)
        {
            lock (SqlConnection)
            {
                try
                {
                    command.CommandText = cmdText;
                    command.Connection = SqlConnection;
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    NDebug.LogError(ex);
                    return -1;
                }
            }
        }
    }
}
#endif