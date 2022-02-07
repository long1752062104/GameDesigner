#if SERVER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Data.SQLite;
using Net.Event;
using Net.System;

public class ContextCall
{
    private readonly Action<object> ptr;
    private readonly object obj;
    public ContextCall(Action<object> ptr, object obj = null)
    {
        this.ptr = ptr;
        this.obj = obj;
    }
    internal void Invoke()
    {
        ptr(obj);
    }
}

/// <summary>
/// Example2DB数据库管理类
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class Example2DB
{
     private class ID
     {
         internal string name;
         internal Dictionary<string, object> dic = new Dictionary<string, object>();
         public ID(string name)
         {
             this.name = name;
         }
     }
	public static Example2DB I { get; private set; } = new Example2DB();
private readonly ConcurrentQueue<DBEntity> entityStack = new ConcurrentQueue<DBEntity>();
private readonly ConcurrentQueue<ContextCall> context = new ConcurrentQueue<ContextCall>();
private Dictionary<string, Dictionary<int, ID>> dataTable = new Dictionary<string, Dictionary<int, ID>>();
private ListSafe<DataRow> dataRows = new ListSafe<DataRow>();
public DataTable UserinfoTable;

public void Init(Action<object> onInit)
{
    UserinfoTable = SQLiteHelper.ExecuteQuery($"SELECT * FROM userinfo");
    foreach (DataRow row in UserinfoTable.Rows)
    {
        UserinfoData data = new UserinfoData();
        data.Init(row);
        onInit?.Invoke(data);
    }
}

public void Update(DBEntity dBEntity)//更新的行,列
{
    entityStack.Enqueue(dBEntity);
}

public void Invoke(ContextCall call)//在同一线程调用, 避免多线程调用问题
{
    context.Enqueue(call);
}

public void Invoke(Action call)//在同一线程调用, 避免多线程调用问题
{
    context.Enqueue(new ContextCall((obj=> { call(); })));
}

public void ExecutedContext()//单线程每次轮询调用, 需要自己调用此方法
{
    while (context.TryDequeue(out ContextCall contextCall))
    {
        contextCall.Invoke();
    }
}

public void Executed()//每秒调用一次, 需要自己调用此方法
{
    while (entityStack.TryDequeue(out DBEntity entity))
    {
        UpdateInternal(entity);
    }
        UpdateInternal();
}

private void UpdateInternal(DBEntity entity)//被更改的行,列
{
    try
    {
        if (entity.row == null)
            return;
        if (entity.value == null)
            return;
        if (!dataTable.TryGetValue(entity.table, out var dic))
            dataTable.Add(entity.table, dic = new Dictionary<int, ID>());
        if (!dic.TryGetValue(entity.id, out var id))
            dic.Add(entity.id, id = new ID(entity.idName));
        id.dic[entity.name] = entity.value;
    }
    catch (Exception ex)
    {
       NDebug.LogError($"{ entity.table}更新异常:第{entity.index}列赋值有问题:{entity.value} 详细信息: " + ex);
    }
}

private void UpdateInternal()//往SQLite数据库批量更新
{
   try
   {
     StringBuilder sb = new StringBuilder();
     var parms = new List<SQLiteParameter>();
     int count = 0;
     if (dataRows.Count > 0)
     {
         var dataRows1 = dataRows.GetRemoveRange(0, dataRows.Count);
         if (dataRows1 == null)
             return;
         foreach (DataRow row in dataRows1)
         {
             string cmdText = $"INSERT INTO {row.Table.TableName} (";
             for (int i = 0; i < row.Table.Columns.Count; i++)
             {
                 if (row[i] == null | row[i] == DBNull.Value)
                     continue;
                 cmdText += row.Table.Columns[i].ColumnName + ",";
             }
             cmdText = cmdText.TrimEnd(',');
             cmdText += ") VALUES(";
             for (int i = 0; i < row.Table.Columns.Count; i++)
             {
                 if (row[i] == null | row[i] == DBNull.Value)
                     continue;
                 if (row[i] is string)
                     cmdText += $"'{row[i]}',";
                 else if (row[i] is byte[])
                 {
                     cmdText += $"@buffer{count},";
                     parms.Add(new SQLiteParameter($"@buffer{count}", row[i]));
                 }
                 else
                     cmdText += $"{row[i]},";
             }
             cmdText = cmdText.TrimEnd(',');
             cmdText += "); ";
             sb.Append(cmdText);
             count++;
         }
     }
     foreach (var table1 in dataTable)
     {
         foreach (var id in table1.Value)
         {
             string cmdText = $"UPDATE {table1.Key} SET ";
             foreach (var cell in id.Value.dic)
             {
                 if (cell.Value is string)
                     cmdText += $"{cell.Key}='{cell.Value}',";
                 else if (cell.Value is byte[])
                 {
                     cmdText += $"{cell.Key}=@buffer{count},";
                     parms.Add(new SQLiteParameter($"@buffer{count}", cell.Value));
                 }
                 else
                     cmdText += $"{cell.Key}={cell.Value},";
             }
             cmdText = cmdText.TrimEnd(',');
             cmdText += $" WHERE {id.Value.name}={id.Key};";
             sb.Append(cmdText);
             count++;
         }
     }
     if (sb.Length > 0)
     {
         if (count > 1000)
         {
             Stopwatch stopwatch = Stopwatch.StartNew();
             SQLiteHelper.ExecuteNonQuery(sb.ToString(), parms);
             stopwatch.Stop();
             NDebug.Log($"执行:{count}个账号更新,耗时:{stopwatch.Elapsed}");
         }
         else SQLiteHelper.ExecuteNonQuery(sb.ToString(), parms);
     }
  }
  catch (Exception ex)
   {
      NDebug.LogError("SQL异常: " + ex);
   }
 }
   public DataRow AddUserinfoNewRow(params object[] parms)
   {
       var row = UserinfoTable.Rows.Add(parms);
       dataRows.Add(row);
       return row;
   }
}
#endif
