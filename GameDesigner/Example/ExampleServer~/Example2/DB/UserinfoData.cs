using System;
using System.Data;

/// <summary>
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到SQLite数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class UserinfoData
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
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "id", value = value, index = 0, row = Row });
#elif CLIENT
        IdCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void IdCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 1, id);
}

[Net.Share.Rpc(mask = 1)]
private void IdCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    Id = value;
    OnId?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnId;

private System.String account;
public System.String Account
{
    get { return account; }
    set
    {
        if (account == value)
            return;
        account = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "account", value = value, index = 1, row = Row });
#elif CLIENT
        AccountCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void AccountCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 2, account);
}

[Net.Share.Rpc(mask = 2)]
private void AccountCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    Account = value;
    OnAccount?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnAccount;

private System.String password;
public System.String Password
{
    get { return password; }
    set
    {
        if (password == value)
            return;
        password = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "password", value = value, index = 2, row = Row });
#elif CLIENT
        PasswordCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void PasswordCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 3, password);
}

[Net.Share.Rpc(mask = 3)]
private void PasswordCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    Password = value;
    OnPassword?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnPassword;

private System.Double moveSpeed;
public System.Double MoveSpeed
{
    get { return moveSpeed; }
    set
    {
        if (moveSpeed == value)
            return;
        moveSpeed = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "moveSpeed", value = value, index = 3, row = Row });
#elif CLIENT
        MoveSpeedCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void MoveSpeedCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 4, moveSpeed);
}

[Net.Share.Rpc(mask = 4)]
private void MoveSpeedCall(System.Double value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    MoveSpeed = value;
    OnMoveSpeed?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnMoveSpeed;

private System.String position;
public System.String Position
{
    get { return position; }
    set
    {
        if (position == value)
            return;
        position = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "position", value = value, index = 4, row = Row });
#elif CLIENT
        PositionCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void PositionCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 5, position);
}

[Net.Share.Rpc(mask = 5)]
private void PositionCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    Position = value;
    OnPosition?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnPosition;

private System.String rotation;
public System.String Rotation
{
    get { return rotation; }
    set
    {
        if (rotation == value)
            return;
        rotation = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "rotation", value = value, index = 5, row = Row });
#elif CLIENT
        RotationCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void RotationCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 6, rotation);
}

[Net.Share.Rpc(mask = 6)]
private void RotationCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    Rotation = value;
    OnRotation?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnRotation;

private System.Int64 health;
public System.Int64 Health
{
    get { return health; }
    set
    {
        if (health == value)
            return;
        health = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "health", value = value, index = 6, row = Row });
#elif CLIENT
        HealthCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void HealthCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 7, health);
}

[Net.Share.Rpc(mask = 7)]
private void HealthCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    Health = value;
    OnHealth?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnHealth;

private System.Int64 healthMax;
public System.Int64 HealthMax
{
    get { return healthMax; }
    set
    {
        if (healthMax == value)
            return;
        healthMax = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "healthMax", value = value, index = 7, row = Row });
#elif CLIENT
        HealthMaxCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void HealthMaxCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 8, healthMax);
}

[Net.Share.Rpc(mask = 8)]
private void HealthMaxCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    HealthMax = value;
    OnHealthMax?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnHealthMax;

private System.Byte[] buffer;
[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Byte[] BufferBytes
{
    get { return buffer; }
    set
    {
        if (buffer == value)
            return;
        buffer = value;
#if SERVER
        Example2DB.I.Update(new DBEntity() { table = "Userinfo", idName = "id", id = (int)id, name = "buffer", value = value, index = 8, row = Row });
#elif CLIENT
        BufferBytesCall();
#endif
    }
}

/// <summary>
/// 同步变量到SQLite数据库
/// </summary>
public void BufferBytesCall()
{
    object array = buffer;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, 9, array);
}

[Net.Share.Rpc(mask = 9)]
private void BufferBytesCall(System.Byte[] value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    BufferBytes = value;
    OnBufferBytes?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnBufferBytes;

public void Init(DataRow row)
{
    Row = row;
    if (row[0] is System.Int64 id)
        this.id = id;
    if (row[1] is System.String account)
        this.account = account;
    if (row[2] is System.String password)
        this.password = password;
    if (row[3] is System.Double moveSpeed)
        this.moveSpeed = moveSpeed;
    if (row[4] is System.String position)
        this.position = position;
    if (row[5] is System.String rotation)
        this.rotation = rotation;
    if (row[6] is System.Int64 health)
        this.health = health;
    if (row[7] is System.Int64 healthMax)
        this.healthMax = healthMax;
    if (row[8] is System.Byte[] buffer)
        this.buffer = buffer;
}
}
