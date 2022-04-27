#if SERVER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SQLite;
using Net.Event;

/// <summary>
/// Example2DB数据库管理类
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class Example2DB
{
	public static Example2DB I { get; private set; } = new Example2DB();
private readonly ConcurrentQueue<Action> context = new ConcurrentQueue<Action>();
private readonly ConcurrentQueue<DataRowEntity> dataRows = new ConcurrentQueue<DataRowEntity>();
private readonly Dictionary<DataRow, DataRowEntity> dataTable = new Dictionary<DataRow, DataRowEntity>();
public DataTableEntity ConfigTable;
public DataTableEntity UserinfoTable;
     private static readonly Stack<SQLiteConnection> conns = new Stack<SQLiteConnection>();
     public static string connStr = @"Data Source='D:\TestProject\Assets\GameDesigner\Example\ExampleServer~\bin\Debug\Data\example2.db'";

     private static SQLiteConnection CheckConn(SQLiteConnection conn)
     {
         if (conn == null)
         {
             conn = new SQLiteConnection(connStr); //数据库连接
             conn.Open();
         }
         if (conn.State != ConnectionState.Open)
         {
             conn.Close();
             conn = new SQLiteConnection(connStr); //数据库连接
             conn.Open();
         }
         return conn;
     }

     public void Init(Action<List<object>> onInit, int connLen = 5)
     {
         while (conns.Count > 0)
         {
             var conn = conns.Pop();
             conn.Close();
         }
         for (int i = 0; i < connLen; i++)
         {
              conns.Push(CheckConn(null));
         }
         List<object> list = new List<object>();
    ConfigTable = ExecuteQuery($"SELECT * FROM config");
    ConfigTable.ColumnChanged += (o, e) =>
     {
         var entity = new DataRowEntity();
         entity.Row = e.Row;
         entity.state = DataRowState.Modified;
         entity.key = e.Column.ColumnName;
         entity.value = e.ProposedValue;
         dataRows.Enqueue(entity);
     };
    ConfigTable.RowDeleting += (o, e) =>
     {
         var entity = new DataRowEntity();
         var primaryKey = e.Row.Table.PrimaryKey[0].ColumnName;
         entity.Row = e.Row;
         entity.state = DataRowState.Deleted;
         entity.key = primaryKey;
         entity.value = e.Row[primaryKey];
         dataRows.Enqueue(entity);
     };
    ConfigTable.TableNewRow += (o, e) =>
     {
         dataRows.Enqueue(new DataRowEntity(DataRowState.Added, e.Row));
     };
    foreach (DataRow row in ConfigTable.Rows)
    {
        ConfigData data = new ConfigData();
        data.Init(row);
        list.Add(data);
    }
    UserinfoTable = ExecuteQuery($"SELECT * FROM userinfo");
    UserinfoTable.ColumnChanged += (o, e) =>
     {
         var entity = new DataRowEntity();
         entity.Row = e.Row;
         entity.state = DataRowState.Modified;
         entity.key = e.Column.ColumnName;
         entity.value = e.ProposedValue;
         dataRows.Enqueue(entity);
     };
    UserinfoTable.RowDeleting += (o, e) =>
     {
         var entity = new DataRowEntity();
         var primaryKey = e.Row.Table.PrimaryKey[0].ColumnName;
         entity.Row = e.Row;
         entity.state = DataRowState.Deleted;
         entity.key = primaryKey;
         entity.value = e.Row[primaryKey];
         dataRows.Enqueue(entity);
     };
    UserinfoTable.TableNewRow += (o, e) =>
     {
         dataRows.Enqueue(new DataRowEntity(DataRowState.Added, e.Row));
     };
    foreach (DataRow row in UserinfoTable.Rows)
    {
        UserinfoData data = new UserinfoData();
        data.Init(row);
        list.Add(data);
    }
         onInit?.Invoke(list);
     }

     public static DataTableEntity ExecuteQuery(string cmdText)
     {
         while (conns.Count == 0)
         {
             Thread.Sleep(1);
         }
         var conn = CheckConn(conns.Pop());
         var dt = new DataTableEntity();
         try
         {
             using (var cmd = new SQLiteCommand())
             {
                 cmd.CommandText = cmdText;
                 cmd.Connection = conn;
                 cmd.Parameters.Clear();
                 using (var sdr = cmd.ExecuteReader())
                 {
                     dt.Load(sdr);
                 }
             }
         }
         catch (Exception ex)
         {
             NDebug.LogError(cmdText + " 错误: " + ex);
         }
         finally
         {
             conns.Push(conn);
         }
         return dt;
     }

     public static async Task<int> ExecuteNonQuery(string cmdText, List<SQLiteParameter> parameters)
     {
         while (conns.Count == 0)
         {
             Thread.Sleep(1);
         }
         var conn = CheckConn(conns.Pop());
         var pars = parameters.ToArray();
         return await Task.Run(() =>
         {
             var count = ExecuteNonQuery(conn, cmdText, pars);
             conns.Push(conn);
             return count;
         });
     }

     public static void ExecuteNonQuery(string cmdText, List<SQLiteParameter> parameters, Action<int> onComplete)
     {
         while (conns.Count == 0)
         {
             Thread.Sleep(1);
         }
         var conn = CheckConn(conns.Pop());
         var pars = parameters.ToArray();
         Task.Run(() =>
         {
             var count = ExecuteNonQuery(conn, cmdText, pars);
             conns.Push(conn);
             onComplete(count);
         });
     }

     private static int ExecuteNonQuery(SQLiteConnection conn, string cmdText, SQLiteParameter[] parameters)
     {
         var transaction = conn.BeginTransaction();
         try
         {
             using (SQLiteCommand cmd = new SQLiteCommand())
             {
                 cmd.Transaction = transaction;
                 cmd.CommandText = cmdText;
                 cmd.Connection = conn;
                 cmd.Parameters.AddRange(parameters);
                 int res = cmd.ExecuteNonQuery();
                 transaction.Commit();
                 return res;
             }
         }
         catch (Exception ex)
         {
             transaction.Rollback();
             NDebug.LogError(cmdText + " 错误: " + ex);
         }
         return -1;
     }

public void Update(DataRowEntity entity)//更新的行,列
{
    dataRows.Enqueue(entity);
}

public void Invoke(Action call)//在同一线程调用, 避免多线程调用问题
{
    context.Enqueue(call);
}

public bool ExecutedContext()//单线程每次轮询调用, 需要自己调用此方法
{
    while (context.TryDequeue(out var contextCall))
        contextCall.Invoke();
    return true;
}

public bool Executed()//每秒调用一次, 需要自己调用此方法
{
    dataTable.Clear();
    while (dataRows.TryDequeue(out var e))
    {
        switch (e.state)
        {
            case DataRowState.Added:
                {
                    dataTable[e.Row] = e;
                }
                break;
            case DataRowState.Deleted:
                {
                    if (!dataTable.TryGetValue(e.Row, out var entity))
                        dataTable.Add(e.Row, entity = e);
                    entity.state = DataRowState.Deleted;
                    entity.key = e.key;
                    entity.value = e.value;
                }
                break;
            case DataRowState.Modified:
                {
                    if (e.state == DataRowState.Detached | e.state == DataRowState.Deleted | e.Row == null) continue;
                    if (!dataTable.TryGetValue(e.Row, out var entity))
                        dataTable.Add(e.Row, entity = e);
                    entity.columns[e.key] = e.value;
                }
                break;
        }
    }
    UpdateInternal();
    return true;
}

private void UpdateInternal()//往mysql数据库批量更新
{
   try
   {
     StringBuilder sb = new StringBuilder();
     var parms = new List<SQLiteParameter>();
     int count = 0, parmsLen = 0;
     foreach (var item in dataTable)
     {
         var row = item.Key;
         switch (item.Value.state)
         {
             case DataRowState.Added:
                 {
                     string cmdText = $"INSERT INTO {row.Table.TableName} (";
                     for (int i = 0; i < row.Table.Columns.Count; i++)
                     {
                         if (row[i] == null | row[i] == DBNull.Value)
                             continue;
                         cmdText += $"`{row.Table.Columns[i].ColumnName}`,";
                     }
                     cmdText = cmdText.TrimEnd(',');
                     cmdText += ") VALUES(";
                     for (int i = 0; i < row.Table.Columns.Count; i++)
                     {
                         if (row[i] == null | row[i] == DBNull.Value)
                             continue;
                         if (row[i] is string | row[i] is DateTime)
                             cmdText += $"'{row[i]}',";
                         else if (row[i] is byte[] buffer)
                         {
                             cmdText += $"@buffer{count},";
                             parms.Add(new SQLiteParameter($"@buffer{count}", buffer));
                             parmsLen += buffer.Length;
                             count++;
                         }
                         else
                             cmdText += $"{row[i]},";
                     }
                     cmdText = cmdText.TrimEnd(',');
                     cmdText += ");";
                     sb.Append(cmdText);
                     count++;
                     row.AcceptChanges();
                 }
                 break;
             case DataRowState.Deleted:
                 {
                     string cmdText = $"DELETE FROM {item.Key.Table.TableName} WHERE `{item.Value.key}` = ";
                     if (item.Value.value is string | item.Value.value is DateTime)
                         cmdText += $"'{item.Value.value}';";
                     else
                         cmdText += $"{item.Value.value};";
                     sb.Append(cmdText);
                 }
                 break;
             case DataRowState.Modified:
                 {
                     if (row.RowState == DataRowState.Detached | row.RowState == DataRowState.Deleted) continue;
                     var prikey = item.Key.Table.PrimaryKey[0];
                     var key = prikey.ColumnName;
                     var value = row[prikey.Ordinal];
                     string cmdText = $"UPDATE {row.Table.TableName} SET ";
                     foreach (var cell in item.Value.columns)
                     {
                         if (cell.Value is string | cell.Value is DateTime)
                             cmdText += $"`{cell.Key}`='{cell.Value}',";
                         else if (cell.Value is byte[] buffer)
                         {
                             cmdText += $"`{cell.Key}`=@buffer{count},";
                             parms.Add(new SQLiteParameter($"@buffer{count}", buffer));
                             parmsLen += buffer.Length;
                             count++;
                         }
                         else
                             cmdText += $"`{cell.Key}`={cell.Value},";
                     }
                     cmdText = cmdText.TrimEnd(',');
                     cmdText += $" WHERE `{key}`={value}; ";
                     sb.Append(cmdText);
                     count++;
                     row.AcceptChanges();
                 }
                 break;
         }
         if (sb.Length + parmsLen >= 2000000)
         {
             ExecuteNonQuery(sb.ToString(), parms, (count1) =>
             {
                 NDebug.Log("sql批量处理完成:" + count1);
             });
             sb.Clear();
             parms.Clear();
             count = 0;
             parmsLen = 0;
         }
     }
     if (sb.Length > 0)
     {
         ExecuteNonQuery(sb.ToString(), parms, (count1) =>
         {
             if (count1 > 2000)
                 NDebug.Log("sql批量处理完成: " + count1);
         });
     }
  }
  catch (Exception ex)
   {
      NDebug.LogError("SQL异常: " + ex);
   }
 }
   public DataRow AddConfigNewRow(params object[] parms)
   {
       var row = ConfigTable.AddRow(parms);
       return row;
   }
   public DataRow AddUserinfoNewRow(params object[] parms)
   {
       var row = UserinfoTable.AddRow(parms);
       return row;
   }
}
#endif
