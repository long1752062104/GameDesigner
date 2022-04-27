using System;
using System.Data;

/// <summary>
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class ConfigData
{
[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public DataRow Row { get; set; }

private System.Int64 id;
public System.Int64 Id
{
    get { return id; }
    set
    {
        if (id == value)
            return;
        id = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "id", value = value, Row = Row });
#elif CLIENT
        IdCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncId
{
    get { return id; }
    set
    {
        if (id == value)
            return;
        id = value;
        IdCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncIDId
{
    get { return id; }
    set
    {
        if (id == value)
            return;
        id = value;
        SyncIdCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void IdCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ID, id);
}

public void SyncIdCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ID, (int)id, id);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.ID)]
private void IdCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
{
    Id = value;
    OnId?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnId;

private System.String name;
public System.String Name
{
    get { return name; }
    set
    {
        if (name == value)
            return;
        if (value == null)
            value = string.Empty;
        name = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "name", value = value, Row = Row });
#elif CLIENT
        NameCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncName
{
    get { return name; }
    set
    {
        if (name == value)
            return;
        if (value == null)
            value = string.Empty;
        name = value;
        NameCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncIDName
{
    get { return name; }
    set
    {
        if (name == value)
            return;
        if (value == null)
            value = string.Empty;
        name = value;
        SyncNameCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void NameCall()
{
    if (name == null)
        name = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.NAME, name);
}

public void SyncNameCall()
{
    if (name == null)
        name = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.NAME, (int)id, name);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.NAME)]
private void NameCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
{
    Name = value;
    OnName?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnName;

private System.Int64 number;
public System.Int64 Number
{
    get { return number; }
    set
    {
        if (number == value)
            return;
        number = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "number", value = value, Row = Row });
#elif CLIENT
        NumberCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncNumber
{
    get { return number; }
    set
    {
        if (number == value)
            return;
        number = value;
        NumberCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncIDNumber
{
    get { return number; }
    set
    {
        if (number == value)
            return;
        number = value;
        SyncNumberCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void NumberCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.NUMBER, number);
}

public void SyncNumberCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.NUMBER, (int)id, number);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.NUMBER)]
private void NumberCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
{
    Number = value;
    OnNumber?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnNumber;

public ConfigData(params object[] parms)
{
    if (parms == null)
        return;
    if (parms.Length == 0)
        return;
    var row = Example2DB.I.AddConfigNewRow(parms);
    Init(row);
}

public void Delete()
{
    Example2DB.I.ConfigTable.DeleteRow(Row);
}

public void Init(DataRow row)
{
    Row = row;
    if (row[0] is System.Int64 id)
        this.id = id;
    if (row[1] is System.String name)
        this.name = name;
    if (row[2] is System.Int64 number)
        this.number = number;
}
#if SERVER
public void Update()
{
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "name", name, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "number", number, Row));
}
public void UpdateDataRow()
{
    Row[1] = name;
    Row[2] = number;
}
#endif
}
