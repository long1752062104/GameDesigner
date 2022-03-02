using System;
using System.Data;

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
