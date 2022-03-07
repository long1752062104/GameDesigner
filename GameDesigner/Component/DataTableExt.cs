using System;
using System.Collections.Generic;
using System.Data;

public class DataRowEntity
{
    public string key;
    public object value;
    public Dictionary<string, object> columns = new Dictionary<string, object>();
}

public class DataTableEntity : DataTable
{
    private readonly object syncRoot = new object(); 
    public new DataTableNewRowEventHandler TableNewRow;
    public DataRow AddRow(params object[] pars)
    {
        lock (syncRoot) 
        {
            var row = Rows.Add(pars);
            TableNewRow?.Invoke(this, new DataTableNewRowEventArgs(row));
            return row;
        }
    }

    public void DeleteRow(DataRow dataRow) 
    {
        lock (syncRoot)
        {
            if (dataRow.RowState == DataRowState.Deleted)
                return;
            dataRow.Delete();
        }
    }

    public new DataRow[] Select(string filterExpression) 
    {
        lock (syncRoot)
        {
            var rows = base.Select(filterExpression);
            return rows;
        }
    }

    public DataRow GetRow(int i)
    {
        return Rows[i];
    }
}

public class DBEntity
{
    public string table;
    public DataRow row;
    public int index;
    public object value;
    public int id;
    public string name;
    public string idName;
    public int type;
    public object asynObj;
    public bool isComplete;
    public Action<DataRow[]> action;
}
