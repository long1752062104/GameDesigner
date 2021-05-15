#if SERVICE && MYSQL_SERVER
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Net.Component
{
    /// <summary>
    /// MySqlHelper 的摘要说明
    /// </summary>
    public static class MySqlHelper
    {
        private static MySqlConnection conn = null;
        private static MySqlCommand cmd = null;
        private static MySqlDataReader sdr;
        private static MySqlDataAdapter sda = null;

        //数据库连接字符串
        public static string connStr = "Database='poker';Data Source='127.0.0.1';User Id='root';Password='root';charset='utf8';pooling=true";

        static MySqlHelper()
        {
            conn = new MySqlConnection(connStr); //数据库连接
            conn.Open();
        }

        /// <summary>
        /// 执行不带参数的增删改SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">增删改SQL语句或存储过程的字符串</param>
        /// <param name="ct">命令类型</param>
        /// <returns>受影响的函数</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            int res;
            using (cmd = new MySqlCommand(cmdText, conn))
            {
                res = cmd.ExecuteNonQuery();
            }
            return res;
        }

        /// <summary>
        /// 执行带参数的增删改SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">增删改SQL语句或存储过程的字符串</param>
        /// <param name="paras">往存储过程或SQL中赋的参数集合</param>
        /// <param name="ct">命令类型</param>
        /// <returns>受影响的函数</returns>
        public static int ExecuteNonQuery(string cmdText, MySqlParameter[] paras, CommandType ct)
        {
            int res;
            using (cmd = new MySqlCommand(cmdText, conn))
            {
                cmd.CommandType = ct;
                cmd.Parameters.AddRange(paras);
                res = cmd.ExecuteNonQuery();
            }
            return res;
        }

        /// <summary>
        /// 执行不带参数的查询SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">查询SQL语句或存储过程的字符串</param>
        /// <param name="ct">命令类型</param>
        /// <returns>查询到的DataTable对象</returns>
        public static DataTable ExecuteQuery(string cmdText, CommandType ct)
        {
            DataTable dt = new DataTable();
            cmd = new MySqlCommand(cmdText, conn);
            cmd.CommandType = ct;
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dt.Load(sdr);
            }
            return dt;
        }

        /// <summary>
        /// 执行带参数的查询SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">查询SQL语句或存储过程的字符串</param>
        /// <param name="paras">参数集合</param>
        /// <param name="ct">命令类型</param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string cmdText, MySqlParameter[] paras, CommandType ct)
        {
            DataTable dt = new DataTable();
            cmd = new MySqlCommand(cmdText, conn);
            cmd.CommandType = ct;
            cmd.Parameters.AddRange(paras);
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dt.Load(sdr);
            }
            return dt;
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回DataSet.
        /// </summary>
        /// <param name="strSql">一个有效的数据库连接字符串</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public static DataSet ExecuteDataset(string strSql)
        {
            DataSet ds = new DataSet();
            sda = new MySqlDataAdapter(strSql, conn);
            try
            {
                sda.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
    }
}
#endif