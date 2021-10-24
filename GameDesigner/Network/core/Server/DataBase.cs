namespace Net.Server
{
    using Net.Event;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Data;
    using global::System.IO;
    using global::System.Threading.Tasks;

    /// <summary>
    /// 序列化数据接口, 需要序列化的主类要继承此接口
    /// </summary>
    public interface ISerializableData
    {
        /// <summary>
        /// 用户标识, 记录玩家键值对, 可以以账号来记录或玩家名称来记录等等
        /// </summary>
        string UIDKey { get; set; }
        /// <summary>
        /// 记录数据存储在文件内部的位置索引 (内部自动赋值,不要修改其值)
        /// </summary>
        long StreamPosition { get; set; }
        /// <summary>
        /// 每个玩家的数据在mysql,sqlserver数据库的数据行
        /// </summary>
        [Newtonsoft_X.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        [Serialize.NonSerialized]
        DataRow Row { get; set; }
    }

    /// <summary>
    /// 服务器运行时数据库 19.10.4
    /// 可以重写此类的一些Save和AddPlayer方法转接到mySql数据库
    /// </summary>
    public class DataBase<Player> : DataBase<DataBase<Player>, Player> where Player : ISerializableData
    {
    }

    /// <summary>
    /// 服务器运行时数据库 19.10.4
    /// 可以重写此类的一些Save和AddPlayer方法转接到mySql数据库
    /// </summary>
    public class DataBase<T, Player> where Player : ISerializableData where T : new()
    {
        /// <summary>
        /// 数据库单例
        /// </summary>
        public static T Instance = new T();
        public static T I => Instance;
        /// <summary>
        /// 单个玩家可以存储的数据大小
        /// </summary>
        public int DataSize { get; set; } = 1024;
        /// <summary>
        /// 当前的流位置
        /// </summary>
        public long StreamPosition { get; private set; }

        /// <summary>
        /// 当前程序工作路径, 数据库保存路径
        /// </summary>
        public string rootPath;
        /// <summary>
        /// 玩家数据保存路径
        /// </summary>
        public string DataPath = "/Data/";
        /// <summary>
        /// 所有玩家信息
        /// </summary>
        public ConcurrentDictionary<string, Player> PlayerInfos = new ConcurrentDictionary<string, Player>();

        public ConcurrentQueue<Player> deleteList = new ConcurrentQueue<Player>();
        /// <summary>
        /// 数据表, 接入mysql, sqlserver数据库后可以批量处理数据库
        /// </summary>
        public DataTable Table;

        /// <summary>
        /// 直接读取数据库玩家对象
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Player this[string playerID]
        {
            get { return PlayerInfos[playerID]; }
            set { PlayerInfos[playerID] = value; }
        }

        /// <summary>
        /// 获得所有玩家帐号数据
        /// </summary>
        public List<Player> Players()
        {
            return new List<Player>(PlayerInfos.Values);
        }

        public DataBase() 
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            rootPath = UnityEngine.Application.persistentDataPath;
#else
            rootPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            if (!Directory.Exists(rootPath + DataPath))
                Directory.CreateDirectory(rootPath + DataPath);
        }

        /// <summary>
        /// 加载数据库信息
        /// </summary>
        public Task Load()
        {
            return LoadAsync(null);
        }

        /// <summary>
        /// 加载数据库信息
        /// </summary>
        /// <param name="lastHandle">需要做最后的处理的, Player.playerID必须指定 </param>
        public Task Load(Action<Player> lastHandle)
        {
            return LoadAsync(lastHandle);
        }

        /// <summary>
        /// 异步加载数据库信息
        /// </summary>
        /// <param name="lastHandle">需要做最后的处理的, Player.playerID必须指定 </param>
        /// <returns></returns>
        public virtual Task LoadAsync(Action<Player> lastHandle)
        {
            return Task.Run(() =>
            {
                string path = rootPath + DataPath + "UserData.db";
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    long position = 0;
                    while (position < fileStream.Length) 
                    {
                        try 
                        {
                            byte[] buffer = new byte[DataSize];
                            var count = fileStream.Read(buffer, 0, DataSize);
                            if (count == 0)
                                break;
                            Player player = OnDeserialize(buffer, count);
                            if (player != null)
                            {
                                lastHandle?.Invoke(player);
                                if (!PlayerInfos.TryAdd(player.UIDKey, player))
                                    NDebug.LogError($"有账号冲突:{player.UIDKey}");
                            }
                        }
                        catch (Exception ex)
                        {
                            NDebug.LogError($"文件:{path}异常！详细信息:{ex}");
                        }
                        position += DataSize;
                    }
                    StreamPosition = position;
                }
                OnLoad();
            });
        }

        /// <summary>
        /// 当序列化数据, 即将写入磁盘文件时调用
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual byte[] OnSerialize(Player player)
        {
            string jsonStr = Newtonsoft_X.Json.JsonConvert.SerializeObject(player);
            var bytes = global::System.Text.Encoding.UTF8.GetBytes(jsonStr);
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(bytes.Length));
            list.AddRange(bytes);
            return list.ToArray();
        }

        public virtual Player OnDeserialize(byte[] buffer, int count)
        {
            var count1 = BitConverter.ToInt32(buffer, 0);
            string jsonStr = global::System.Text.Encoding.UTF8.GetString(buffer, 4, count1);
            return Newtonsoft_X.Json.JsonConvert.DeserializeObject<Player>(jsonStr);
        }

        /// <summary>
        /// 当加载持久文件数据时调用, 加载的数据在PlayerInfos属性里面
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// 存储全部玩家数据到文件里
        /// </summary>
        public Task SaveAll()
        {
            return Task.Run(() =>
            {
                string path = rootPath + DataPath + "UserData.db";
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    foreach (Player player in PlayerInfos.Values)
                    {
                        fileStream.Seek(player.StreamPosition, SeekOrigin.Begin);
                        byte[] bytes = OnSerialize(player);
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                }
            });
        }

        /// <summary>
        /// 存储单个玩家的数据到文件里
        /// </summary>
        public virtual void Save(Player player)
        {
            if (string.IsNullOrEmpty(player.UIDKey))
                throw new Exception("UIDKey字段必须赋值，UIDKey是记录玩家账号或唯一标识用!");
            string path = rootPath + DataPath + "UserData.db";
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fileStream.Seek(player.StreamPosition, SeekOrigin.Begin);
                byte[] bytes = OnSerialize(player);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 删除磁盘里面的单个用户的全部数据
        /// </summary>
        public virtual void Delete(Player player)
        {
            deleteList.Enqueue(player);
        }

        /// <summary>
        /// 添加网络玩家到数据库
        /// </summary>
        /// <param name="player"></param>
        public bool AddPlayer(Player player)
        {
            return AddPlayer(player.UIDKey, player);
        }

        /// <summary>
        /// 添加网络玩家到数据库
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="player"></param>
        public bool AddPlayer(string playerID, Player player)
        {
            if (PlayerInfos.TryAdd(playerID, player))
            {
                if (deleteList.TryDequeue(out Player player1)) 
                {
                    player.StreamPosition = player1.StreamPosition;
                    OnAddPlayer(playerID, player);
                    return true;
                }
                player.StreamPosition = StreamPosition;
                StreamPosition += DataSize;
                OnAddPlayer(playerID, player);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加玩家数据并保存到文件
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayerAndSave(Player player)
        {
            AddPlayer(player);
            Save(player);
        }

        /// <summary>
        /// 当添加玩家注册的账号数据到数据库时调用
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="player"></param>
        public virtual void OnAddPlayer(string playerID, Player player)
        {
        }

        /// <summary>
        /// 是否包含玩家ID
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool Contains(string playerID)
        {
            return PlayerInfos.ContainsKey(playerID);
        }

        /// <summary>
        /// 数据库是否已经有这个playerID账号?
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool HasAccout(string playerID)
        {
            return PlayerInfos.ContainsKey(playerID);
        }

        /// <summary>
        /// 尝试移除网络玩家
        /// </summary>
        /// <param name="player"></param>
        public bool Remove(Player player)
        {
            return Remove(player.UIDKey);
        }

        /// <summary>
        /// 尝试移除网络玩家
        /// </summary>
        /// <param name="playerID"></param>
        public bool Remove(string playerID)
        {
            if (PlayerInfos.TryRemove(playerID, out Player player)) 
            {
                Delete(player);
                OnDelete(player);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当从数据库删除playerID的账号时调用
        /// </summary>
        /// <param Player="player"></param>
        public virtual void OnDelete(Player player)
        {
        }
    }
}