#if SERVER
using System;
using System.Collections.Concurrent;
using System.Data;
using Net.Event;

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
/// 此类由SQLiteDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// SQLiteDataBuild工具提供Rpc自动同步到SQLite数据库的功能, 提供数据库注释功能
/// SQLiteDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class Example2DB
{
    public static Example2DB I { get; private set; } = new Example2DB();
    private readonly ConcurrentQueue<DBEntity> entityStack = new ConcurrentQueue<DBEntity>();
    private readonly ConcurrentQueue<ContextCall> context = new ConcurrentQueue<ContextCall>();
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

    public void Update(string table, DataRow row, int index, object value)//更新的行,列
    {
        entityStack.Enqueue(new DBEntity() { table = table, row = row, index = index, value = value });
    }

    public void Invoke(ContextCall call)//在同一线程调用, 避免多线程调用问题
    {
        context.Enqueue(call);
    }

    public void Invoke(Action call)//在同一线程调用, 避免多线程调用问题
    {
        context.Enqueue(new ContextCall((obj => { call(); })));
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
        UpdateInternal(UserinfoTable);
    }

    private void UpdateInternal(DBEntity entity)//被更改的行,列
    {
        try
        {
            if (entity.row == null)
                return;
            if (entity.value == null)
                return;
            entity.row[entity.index] = entity.value;
        }
        catch (Exception ex)
        {
            NDebug.LogError($"{ entity.table}更新异常:第{entity.index}列赋值有问题:{entity.value} 详细信息: " + ex);
            entity.row.ClearErrors();
        }
    }

    private void UpdateInternal(DataTable table)//往SQLite数据库批量更新
    {
        try
        {
            if (table == null)
                return;
            if (table.GetChanges() == null)
                return;
            SQLiteHelper.Update(table);
            table.AcceptChanges();
        }
        catch (Exception ex)
        {
            NDebug.LogError("表" + table.TableName + "异常: " + ex);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i].HasErrors)
                {
                    table.Rows[i].RejectChanges();
                    table.Rows[i].ClearErrors();
                }
            }
        }
    }
}
#endif