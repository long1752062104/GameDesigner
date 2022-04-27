using System;
using System.Collections.Generic;
using System.Data;

public interface IDataRow 
{
    DataRowState RowState { get; set; }
    void Init(DataRow row);
}

public class DataRowEntity
{
    public DataRowState state;
    public string tableName;
    public IDataRow Row;
    public string columnName;
    public object columnValue;
    public string primaryKey;
    public object primaryValue;
    public Dictionary<string, object> columns = new Dictionary<string, object>();
    
    public DataRowEntity() { }

    public DataRowEntity(string tableName, DataRowState state, string columnName, object columnValue, IDataRow row, string primaryKey, object primaryValue)
    {
        this.tableName = tableName;
        this.state = state; 
        this.Row = row;
        this.columnName = columnName;
        this.columnValue = columnValue;
        this.primaryKey = primaryKey;
        this.primaryValue = primaryValue;
    }
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
            row.AcceptChanges();
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
            dataRow.AcceptChanges();
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
