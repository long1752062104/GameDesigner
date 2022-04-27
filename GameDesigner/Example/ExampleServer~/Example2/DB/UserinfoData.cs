using System;
using System.Data;

/// <summary>
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
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
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ID1, id);
}

public void SyncIdCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ID1, (int)id, id);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.ID1)]
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
        if (value == null)
            value = string.Empty;
        account = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "account", value = value, Row = Row });
#elif CLIENT
        AccountCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncAccount
{
    get { return account; }
    set
    {
        if (account == value)
            return;
        if (value == null)
            value = string.Empty;
        account = value;
        AccountCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncIDAccount
{
    get { return account; }
    set
    {
        if (account == value)
            return;
        if (value == null)
            value = string.Empty;
        account = value;
        SyncAccountCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void AccountCall()
{
    if (account == null)
        account = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ACCOUNT, account);
}

public void SyncAccountCall()
{
    if (account == null)
        account = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ACCOUNT, (int)id, account);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.ACCOUNT)]
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
        if (value == null)
            value = string.Empty;
        password = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "password", value = value, Row = Row });
#elif CLIENT
        PasswordCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncPassword
{
    get { return password; }
    set
    {
        if (password == value)
            return;
        if (value == null)
            value = string.Empty;
        password = value;
        PasswordCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncIDPassword
{
    get { return password; }
    set
    {
        if (password == value)
            return;
        if (value == null)
            value = string.Empty;
        password = value;
        SyncPasswordCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void PasswordCall()
{
    if (password == null)
        password = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.PASSWORD, password);
}

public void SyncPasswordCall()
{
    if (password == null)
        password = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.PASSWORD, (int)id, password);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.PASSWORD)]
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
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "moveSpeed", value = value, Row = Row });
#elif CLIENT
        MoveSpeedCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Double SyncMoveSpeed
{
    get { return moveSpeed; }
    set
    {
        if (moveSpeed == value)
            return;
        moveSpeed = value;
        MoveSpeedCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Double SyncIDMoveSpeed
{
    get { return moveSpeed; }
    set
    {
        if (moveSpeed == value)
            return;
        moveSpeed = value;
        SyncMoveSpeedCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void MoveSpeedCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.MOVESPEED, moveSpeed);
}

public void SyncMoveSpeedCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.MOVESPEED, (int)id, moveSpeed);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.MOVESPEED)]
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
        if (value == null)
            value = string.Empty;
        position = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "position", value = value, Row = Row });
#elif CLIENT
        PositionCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncPosition
{
    get { return position; }
    set
    {
        if (position == value)
            return;
        if (value == null)
            value = string.Empty;
        position = value;
        PositionCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncIDPosition
{
    get { return position; }
    set
    {
        if (position == value)
            return;
        if (value == null)
            value = string.Empty;
        position = value;
        SyncPositionCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void PositionCall()
{
    if (position == null)
        position = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.POSITION, position);
}

public void SyncPositionCall()
{
    if (position == null)
        position = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.POSITION, (int)id, position);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.POSITION)]
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
        if (value == null)
            value = string.Empty;
        rotation = value;
#if SERVER
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "rotation", value = value, Row = Row });
#elif CLIENT
        RotationCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncRotation
{
    get { return rotation; }
    set
    {
        if (rotation == value)
            return;
        if (value == null)
            value = string.Empty;
        rotation = value;
        RotationCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.String SyncIDRotation
{
    get { return rotation; }
    set
    {
        if (rotation == value)
            return;
        if (value == null)
            value = string.Empty;
        rotation = value;
        SyncRotationCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void RotationCall()
{
    if (rotation == null)
        rotation = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ROTATION, rotation);
}

public void SyncRotationCall()
{
    if (rotation == null)
        rotation = string.Empty;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.ROTATION, (int)id, rotation);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.ROTATION)]
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
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "health", value = value, Row = Row });
#elif CLIENT
        HealthCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncHealth
{
    get { return health; }
    set
    {
        if (health == value)
            return;
        health = value;
        HealthCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncIDHealth
{
    get { return health; }
    set
    {
        if (health == value)
            return;
        health = value;
        SyncHealthCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void HealthCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.HEALTH, health);
}

public void SyncHealthCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.HEALTH, (int)id, health);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.HEALTH)]
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
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "healthMax", value = value, Row = Row });
#elif CLIENT
        HealthMaxCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncHealthMax
{
    get { return healthMax; }
    set
    {
        if (healthMax == value)
            return;
        healthMax = value;
        HealthMaxCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Int64 SyncIDHealthMax
{
    get { return healthMax; }
    set
    {
        if (healthMax == value)
            return;
        healthMax = value;
        SyncHealthMaxCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void HealthMaxCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.HEALTHMAX, healthMax);
}

public void SyncHealthMaxCall()
{
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.HEALTHMAX, (int)id, healthMax);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.HEALTHMAX)]
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
        Example2DB.I.Update(new DataRowEntity() { state = DataRowState.Modified, key = "buffer", value = value, Row = Row });
#elif CLIENT
        BufferBytesCall();
#endif
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Byte[] SyncBufferBytes
{
    get { return buffer; }
    set
    {
        if (buffer == value)
            return;
        buffer = value;
        BufferBytesCall();
    }
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public System.Byte[] SyncIDBufferBytes
{
    get { return buffer; }
    set
    {
        if (buffer == value)
            return;
        buffer = value;
        SyncBufferBytesCall();
    }
}

/// <summary>
/// 同步变量到MySql数据库
/// </summary>
public void BufferBytesCall()
{
    object array = buffer;
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.BUFFER, array);
}

public void SyncBufferBytesCall()
{
    object array = new object[]{ (int)id, buffer };
    Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)Example2RpcMaskType.BUFFER, array);
}

[Net.Share.Rpc(hash = (ushort)Example2RpcMaskType.BUFFER)]
private void BufferBytesCall(System.Byte[] value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UserinfoData));收集Rpc
{
    BufferBytes = value;
    OnBufferBytes?.Invoke();
}

[Net.Serialize.NonSerialized]
[Newtonsoft_X.Json.JsonIgnore]
[ProtoBuf.ProtoIgnore]
public Action OnBufferBytes;

public UserinfoData(params object[] parms)
{
    if (parms == null)
        return;
    if (parms.Length == 0)
        return;
    var row = Example2DB.I.AddUserinfoNewRow(parms);
    Init(row);
}

public void Delete()
{
    Example2DB.I.UserinfoTable.DeleteRow(Row);
}

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
#if SERVER
public void Update()
{
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "account", account, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "password", password, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "moveSpeed", moveSpeed, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "position", position, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "rotation", rotation, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "health", health, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "healthMax", healthMax, Row));
    Example2DB.I.Update(new DataRowEntity(DataRowState.Modified, "buffer", buffer, Row));
}
public void UpdateDataRow()
{
    Row[1] = account;
    Row[2] = password;
    Row[3] = moveSpeed;
    Row[4] = position;
    Row[5] = rotation;
    Row[6] = health;
    Row[7] = healthMax;
    Row[8] = buffer;
}
#endif
}
