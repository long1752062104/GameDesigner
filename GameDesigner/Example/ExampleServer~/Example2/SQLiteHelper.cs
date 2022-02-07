using Net.Event;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

/// <summary>
/// SQLiteHelper 的摘要说明
/// </summary>
public static class SQLiteHelper
{
    private static SQLiteConnection conn = null;
    private static readonly SQLiteCommand cmd = new SQLiteCommand();
    private static SQLiteDataReader sdr;
    private static SQLiteDataAdapter sda = null;

    //数据库连接字符串
    public static string connStr = "Data Source='D:/testDB.db';";

    private static SQLiteConnection Connect
    {
        get
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(connStr); //数据库连接
                conn.Open();
            }
            //conn.Ping();//长时间没有连接后断开连接检查状态
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
                conn = new SQLiteConnection(connStr); //数据库连接
                conn.Open();
            }
            return conn;
        }
    }

    /// <summary>
    /// 初始化SQLite数据库
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="userName"></param>
    /// <param name="pwd"></param>
    /// <returns></returns>
    public static bool Init(string dbName, string ip = "127.0.0.1", string port = "3306", string userName = "root", string pwd = "root")
    {
        connStr = $"Database='{dbName}';Data Source='{ip}';Port={port};User Id='{userName}';Password='{pwd}';charset='utf8';pooling=true";
        if (conn != null)
            conn.Close();
        return Connect.State == ConnectionState.Open;
    }

    /// <summary>
    /// 执行不带参数的增删改SQL语句或存储过程
    /// </summary>
    /// <param name="cmdText">增删改SQL语句或存储过程的字符串</param>
    /// <returns>受影响的函数</returns>
    public static int ExecuteNonQuery(string cmdText)
    {
        try
        {
            cmd.CommandText = cmdText;
            cmd.Connection = Connect;
            int res = cmd.ExecuteNonQuery();
            return res;
        }
        catch (Exception ex)
        {
            NDebug.LogError(cmdText + " 错误:" + ex);
        }
        return -1;
    }

    /// <summary>
    /// 执行不带参数的增删改SQL语句或存储过程
    /// </summary>
    /// <param name="cmdText">增删改SQL语句或存储过程的字符串</param>
    /// <returns>受影响的函数</returns>
    public static int ExecuteNonQuery(string cmdText, List<SQLiteParameter> parameters)
    {
        try
        {
            cmd.CommandText = cmdText;
            cmd.Connection = Connect;
            cmd.Parameters.Clear();
            cmd.Parameters.AddRange(parameters.ToArray());
            int res = cmd.ExecuteNonQuery();
            return res;
        }
        catch (Exception ex)
        {
            NDebug.LogError(cmdText + " 错误:" + ex);
        }
        return -1;
    }

    /// <summary>
    /// 执行不带参数的查询SQL语句或存储过程
    /// </summary>
    /// <param name="cmdText">查询SQL语句或存储过程的字符串</param>
    /// <returns>查询到的DataTable对象</returns>
    public static DataTable ExecuteQuery(string cmdText)
    {
        DataTable dt = new DataTable();
        try
        {
            cmd.CommandText = cmdText;
            cmd.Connection = Connect;
            using (sdr = cmd.ExecuteReader())
            {
                dt.Load(sdr);
            }
        }
        catch (Exception ex)
        {
            NDebug.LogError(cmdText + " 错误:" + ex);
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
        sda = new SQLiteDataAdapter(strSql, Connect);
        try
        {
            sda.Fill(ds);
        }
        catch (Exception ex)
        {
            NDebug.LogError(strSql + " 错误:" + ex);
        }
        return ds;
    }

    /// <summary>
    /// 将数据插入到SQLite数据库
    /// </summary>
    /// <param name="tableName">要插入的数据库表名</param>
    /// <param name="datas">插入的参数</param>
    /// <returns></returns>
    public static int Insert(string tableName, params ColumnData[] datas)
    {
        string cmdText = $"INSERT INTO {tableName} (";
        foreach (var item in datas)
        {
            cmdText += item.name + ",";
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += ") VALUES(";
        foreach (var item in datas)
        {
            if (item.value == null)
            {
                NDebug.LogError($"{item.name}:参数值不能为null");
                return -1;
            }
            if (item.value is string)
                cmdText += $"'{item.value}',";
            else
                cmdText += $"{item.value},";
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += ")";
        return ExecuteNonQuery(cmdText);
    }

    /// <summary>
    /// 插入数据到SQLite数据表
    /// </summary>
    /// <param name="tableName">要插入的数据表名</param>
    /// <param name="values">插入的列表值</param>
    /// <returns></returns>
    public static int Insert(string tableName, params object[] values)
    {
        string cmdText = $"INSERT INTO {tableName} VALUES(";
        foreach (var value in values)
        {
            if (value == null)
            {
                NDebug.LogError($"参数值不能为null");
                return -1;
            }
            if (value is string)
                cmdText += $"'{value}',";
            else
                cmdText += $"{value},";
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += ")";
        return ExecuteNonQuery(cmdText);
    }

    /// <summary>
    /// 更新SQLite数据表
    /// </summary>
    /// <param name="tableName">更新的表名</param>
    /// <param name="cellKey">匹配关键字段修改数据</param>
    /// <param name="datas">修改的参数值</param>
    /// <returns></returns>
    public static int Update(string tableName, ColumnData cellKey, params ColumnData[] datas)
    {
        string cmdText = $"UPDATE {tableName} SET ";
        foreach (var item in datas)
        {
            if (item.value is string)
                cmdText += $"{item.name}='{item.value}',";
            else
                cmdText += $"{item.name}={item.value},";
        }
        cmdText = cmdText.TrimEnd(',');
        if (cellKey.value is string)
            cmdText += $" WHERE {cellKey.name}='{cellKey.value}'";
        else
            cmdText += $" WHERE {cellKey.name}={cellKey.value}";
        return ExecuteNonQuery(cmdText);
    }

    /// <summary>
    /// 删除数据表的某行
    /// </summary>
    /// <param name="tableName">要查的表名</param>
    /// <param name="cellKey">查表的匹配数据</param>
    /// <returns></returns>
    public static int Delete(string tableName, ColumnData cellKey)
    {
        string cmdText = $"DELETE FROM {tableName} ";
        if (cellKey.value is string)
            cmdText += $" WHERE {cellKey.name}='{cellKey.value}'";
        else
            cmdText += $" WHERE {cellKey.name}={cellKey.value}";
        return ExecuteNonQuery(cmdText);
    }

    /// <summary>
    /// 查询数据表
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cellKey">匹配关键列</param>
    /// <param name="ands">多个列匹配</param>
    /// <returns></returns>
    public static DataTable Query(string tableName)
    {
        string cmdText = $"SELECT * FROM {tableName}";
        return ExecuteQuery(cmdText);
    }

    /// <summary>
    /// 查询数据表
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cellKey">匹配关键列</param>
    /// <param name="ands">多个列匹配</param>
    /// <returns></returns>
    public static DataTable Query(string tableName, ColumnData cellKey, params ColumnData[] ands)
    {
        string cmdText = $"SELECT * FROM {tableName} ";
        if (cellKey.value is string)
            cmdText += $" WHERE {cellKey.name}='{cellKey.value}'";
        else
            cmdText += $" WHERE {cellKey.name}={cellKey.value}";
        foreach (var and in ands)
        {
            if (and.value is string)
                cmdText += $" AND {and.name}='{and.value}'";
            else
                cmdText += $" AND {and.name}={and.value}";
        }
        return ExecuteQuery(cmdText);
    }

    /// <summary>
    /// 查询数据表, 并且可以增删改查后提交到数据库
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="table"></param>
    public static void QueryUpdate(string tableName, Action<DataTable> table)
    {
        string sQuery = $"SELECT * FROM {tableName}";
        SQLiteDataAdapter myDA = new SQLiteDataAdapter(sQuery, Connect);
        new SQLiteCommandBuilder(myDA);//需要留着才能增删改查
        DataTable table1 = new DataTable();
        myDA.Fill(table1);
        table(table1);
        myDA.Update(table1);
    }

    /// <summary>
    /// 更新一个列表到SQLite数据库
    /// </summary>
    /// <param name="table"></param>
    public static void Update(DataTable table)
    {
        string sQuery = $"SELECT * FROM {table.TableName}";
        SQLiteDataAdapter myDA = new SQLiteDataAdapter(sQuery, Connect);
        //myDA.RowUpdating += new DataRowUpdatingEventHandler((o, e)=> { e.Status = UpdateStatus.Continue; });
        new SQLiteCommandBuilder(myDA);//需要留着才能增删改查
        myDA.Update(table);
    }
}

/// <summary>
/// 列参数数据
/// </summary>
public class ColumnData
{
    /// <summary>
    /// 列名
    /// </summary>
    public string name;
    /// <summary>
    /// 列值
    /// </summary>
    public object value;

    public ColumnData(string name, object value)
    {
        this.name = name;
        this.value = value;
    }
}