namespace Net.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// 序列化数据接口, 需要序列化的主类要继承此接口
    /// </summary>
    public interface ISerializableData
    {
        /// <summary>
        /// 用户标识, 记录玩家键值对, 可以以账号来记录或玩家名称来记录等等
        /// </summary>
        string UIDKey { get; set; }
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
        /// <summary>
        /// 玩家数据保存路径
        /// </summary>
        public static string DataPath = "Data/";
        /// <summary>
        /// 所有玩家信息
        /// </summary>
        public ConcurrentDictionary<string, Player> PlayerInfos = new ConcurrentDictionary<string, Player>();

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
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(baseDirectory + DataPath))
                    Directory.CreateDirectory(baseDirectory + DataPath);
                string[] playerDataPaths = Directory.GetFiles(baseDirectory + DataPath, "PlayerInfo.data", SearchOption.AllDirectories);
                foreach (string path in playerDataPaths)
                {
                    try
                    {
                        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        byte[] buffer = new byte[fileStream.Length];
                        int count = fileStream.Read(buffer, 0, buffer.Length);
                        fileStream.Close();
                        if (count == 0)
                            continue;
                        Player player = OnDeserialize(buffer, count);
                        if (player != null)
                        {
                            lastHandle?.Invoke(player);
                            PlayerInfos.TryAdd(player.UIDKey, player);
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
                OnLoad();
            });
        }

        public virtual Player OnDeserialize(byte[] buffer, int count)
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(buffer, 0, count);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(jsonStr);// Share.NetConvert.Deserialize(buffer, 0, count);
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
                foreach (Player p in PlayerInfos.Values)
                {
                    Save(p).Wait();
                }
            });
        }

        /// <summary>
        /// 存储单个玩家的数据到文件里
        /// </summary>
        public virtual Task Save(Player player)
        {
            if (player.UIDKey == string.Empty)
                throw new Exception("UIDKey字段必须赋值，UIDKey是记录玩家账号或唯一标识用!");
            return Task.Run(() =>
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(path + DataPath))
                    Directory.CreateDirectory(path + DataPath);
                if (!Directory.Exists(path + DataPath + player.UIDKey))
                    Directory.CreateDirectory(path + DataPath + player.UIDKey);
                string path1 = path + DataPath + player.UIDKey + "/PlayerInfo.data";
                FileStream fileStream;
                if (!File.Exists(path1))
                    fileStream = new FileStream(path1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                else
                    fileStream = new FileStream(path1, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
                byte[] bytes = OnSerialize(player);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            });
        }

        /// <summary>
        /// 当序列化数据, 即将写入磁盘文件时调用
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual byte[] OnSerialize(Player player)
        {
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(player);
            return System.Text.Encoding.UTF8.GetBytes(jsonStr);//Share.NetConvert.Serialize(player);
        }

        /// <summary>
        /// 删除磁盘里面的单个用户的全部数据
        /// </summary>
        public virtual Task Delete(Player player)
        {
            return Task.Run(() =>
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(path + DataPath))
                    return;
                if (!Directory.Exists(path + DataPath + player.UIDKey))
                    return;
                string path1 = path + DataPath + player.UIDKey;
                if (Directory.Exists(path1))
                    Directory.Delete(path1, true);
            });
        }

        /// <summary>
        /// 添加网络玩家到数据库
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(Player player)
        {
            AddPlayer(player.UIDKey, player);
        }

        /// <summary>
        /// 添加网络玩家到数据库
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="player"></param>
        public void AddPlayer(string playerID, Player player)
        {
            PlayerInfos.TryAdd(playerID, player);
            OnAddPlayer(playerID, player);
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
        public void Remove(Player player)
        {
            Remove(player.UIDKey);
        }

        /// <summary>
        /// 尝试移除网络玩家
        /// </summary>
        /// <param name="playerID"></param>
        public void Remove(string playerID)
        {
            PlayerInfos.TryRemove(playerID, out Player player);
            Delete(player);
            OnDelete(player);
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