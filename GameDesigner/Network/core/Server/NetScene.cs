namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using Net.System;
    using Net.Event;
    using Net.Serialize;

    /// <summary>
    /// 网络场景
    /// </summary>
    public class NetScene<Player> where Player : NetPlayer
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 场景容纳人数
        /// </summary>
        public int sceneCapacity;
        private HashSet<Player> _players = new HashSet<Player>();
        /// <summary>
        /// 当前网络场景的玩家
        /// </summary>
        public HashSet<Player> Players
        {
            get { return _players; }
            set { lock (_players) _players = value; }
        }
        /// <summary>
        /// 当前网络场景状态
        /// </summary>
        public NetState state = NetState.Idle;
        /// <summary>
        /// 当前帧
        /// </summary>
        public uint frame;
        /// <summary>
        /// 备用操作, 当玩家被移除后速度比update更新要快而没有地方收集操作指令, 所以在玩家即将被移除时, 可以访问这个变量进行添加操作同步数据
        /// </summary>
        protected ListPool<Operation> operations = new ListPool<Operation>();
        /// <summary>
        /// 玩家操作是以可靠传输进行发送的?
        /// </summary>
        public bool SendOperationReliable { get; set; }
        public Func<OperationList, byte[]> onSerializeOptHandle;
        /// <summary>
        /// 获取场景当前人数
        /// </summary>
        public int SceneNumber
        {
            get { return Players.Count; }
        }

        /// <summary>
        /// 获取场景当前人数
        /// </summary>
        public int CurrNum
        {
            get { return Players.Count; }
        }

        /// <summary>
        /// 获取场景容纳人数
        /// </summary>
        public int Count
        {
            get { return sceneCapacity; }
            set { sceneCapacity = value; }
        }

        /// <summary>
        /// 场景(房间)人数是否已满？
        /// </summary>
        public bool IsFull
        {
            get { return Players.Count >= sceneCapacity; }
        }

        /// <summary>
        /// 操作列表分段值, 当operations.Count的长度大于Split值时, 就会裁剪为多段数据发送 默认为500长度分段
        /// </summary>
        public int Split { get; set; } = 500;

        /// <summary>
        /// 场景事件
        /// </summary>
        public ActionEvent Event = new ActionEvent();

        /// <summary>
        /// 构造网络场景
        /// </summary>
        public NetScene()
        {
            Event.AddEvent(5f, ()=> 
            {
                var clients = Clients;
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i] == null)
                        continue;
                    if (!clients[i].Login | clients[i].isDispose | clients[i].CloseSend)
                    {
                        Clients.Clear();
                        break;
                    }
                    else if (clients[i].Client != null) 
                    {
                        if (!clients[i].Client.Connected)
                        {
                            Clients.Clear();
                            break;
                        }
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// 添加网络主场景并增加主场景最大容纳人数
        /// </summary>
        /// <param name="number">主场景最大容纳人数</param>
        public NetScene(int number)
        {
            sceneCapacity = number;
        }

        /// <summary>
        /// 添加网络场景并增加当前场景人数
        /// </summary>
        /// <param name="client">网络玩家</param>
        /// <param name="number">创建场景容纳人数</param>
        public NetScene(Player client, int number)
        {
            sceneCapacity = number;
            AddPlayer(client);
        }

        /// <summary>
        /// 获取场景内的玩家到一维集合里
        /// </summary>
        /// <returns></returns>
        public List<Player> GetPlayers()
        {
            return Clients;
        }

        private List<Player> players = new List<Player>();
        /// <summary>
        /// 获取场景的所有客户端玩家
        /// </summary>
        public List<Player> Clients
        {
            get
            {
                if (Players.Count != players.Count)
                    players = new List<Player>(Players);
                return players;
            }
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="client"></param>
        public virtual void AddPlayer(Player client)
        {
            client.sceneID = Name;
            client.Scene = this;
            lock (Players)
                Players.Add(client);
            OnEnter(client);
            client.OnEnter();
            players.Clear();
        }

        /// <summary>
        /// 当进入场景的玩家
        /// </summary>
        /// <param name="client"></param>
        public virtual void OnEnter(Player client) { }

        /// <summary>
        /// 当退出场景的玩家
        /// </summary>
        /// <param name="client"></param>
        public virtual void OnExit(Player client) { }

        /// <summary>
        /// 当场景被移除
        /// </summary>
        /// <param name="client"></param>
        public virtual void OnRemove(Player client) { }

        /// <summary>
        /// 当接收到客户端使用Client.AddOperation方法发送的请求时调用
        /// </summary>
        public virtual void OnOperationSync(Player client, OperationList list) 
        { 
            AddOperations(list.operations);
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="collection"></param>
        public virtual void AddPlayerRange(IEnumerable<Player> collection)
        {
            foreach (Player p in collection)
                AddPlayer(p);
        }

        /// <summary>
        /// 网络帧同步, 状态同步更新, 帧时间根据服务器主类的SyncSceneTime属性来调整速率
        /// </summary>
        public virtual void Update(IServerSendHandle<Player> handle, byte cmd = NetCmd.OperationSync)
        {
            var players = Clients;//多线程问题, 心跳 或 未知线程 添加或移除玩家时实时更新了哈希列表
            int playerCount = players.Count;
            if (playerCount <= 0) 
                return;
            frame++;
            for (int i = 0; i < players.Count; i++)
                players[i]?.OnUpdate();//5000个客户端后出现null问题
            int count = operations.Count;//多线程后避免长度增加时,数据还没写入完成就会出现问题, 所以先取出写入完成的数据
            if (count > 0)
            {
                while (count > Split)
                {
                    OnPacket(handle, cmd, Split);
                    count -= Split;
                }
                if (count > 0)
                    OnPacket(handle, cmd, count);
            }
            Event.UpdateEvent();
        }

        /// <summary>
        /// 当封包数据时调用
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="cmd"></param>
        /// <param name="count"></param>
        public virtual void OnPacket(IServerSendHandle<Player> handle, byte cmd, int count)
        {
            Operation[] opts = operations.GetRemoveRange(0, count);
            OperationList list = ObjectPool<OperationList>.Take();
            list.frame = frame;
            list.operations = opts;
            var buffer = onSerializeOptHandle(list);
            handle.Multicast(players, SendOperationReliable, cmd, buffer, false, false);
            ObjectPool<OperationList>.Push(list);
            OnRecovery(opts);
        }

        /// <summary>
        /// 当操作对象即将被回收, 可重写此方法, 用对象池回收复用 注意, 如果回收复用的, 创建也要使用对象池创建
        /// <code>创建代码: var opt = ObjectPool&lt;Operation&gt;.Take();</code>
        /// <code>回收代码: foreach(var opt in opts) ObjectPool&lt;Operation&gt;.Push(opt);</code>
        /// </summary>
        public virtual void OnRecovery(Operation[] opts)
        {
        }

        /// <summary>
        /// 添加操作帧, 等待帧时间同步发送
        /// </summary>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        [Obsolete("此方法不再支持，请使用Send方法代替!", true)]
        public virtual void AddOperation(string func, params object[] pars)
        {
            AddOperation(NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 添加操作帧, 等待帧时间同步发送
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        [Obsolete("此方法不再支持，请使用Send方法代替!", true)]
        public virtual void AddOperation(byte cmd, string func, params object[] pars)
        {
            Operation opt = new Operation(cmd, NetConvert.Serialize(new RPCModel(0, func, pars)));
            AddOperation(opt);
        }

        /// <summary>
        /// 添加操作帧, 等待帧时间同步发送
        /// </summary>
        /// <param name="opt"></param>
        public virtual void AddOperation(Operation opt)
        {
            operations.Add(opt);
        }

        /// <summary>
        /// 添加操作帧, 等待帧时间同步发送
        /// </summary>
        /// <param name="opts"></param>
        public virtual void AddOperations(List<Operation> opts)
        {
            foreach (Operation opt in opts)
                AddOperation(opt);
        }

        /// <summary>
        /// 添加操作帧, 等待帧时间同步发送
        /// </summary>
        /// <param name="opts"></param>
        public virtual void AddOperations(Operation[] opts)
        {
            foreach (Operation opt in opts)
                AddOperation(opt);
        }

        /// <summary>
        /// 场景对象转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}:{Players.Count}/{sceneCapacity}";
        }

        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="player"></param>
        public void Remove(Player player)
        {
            Players.Remove(player);
            OnExit(player);
            player.OnExit();
            player.Scene = null;
            player.sceneID = "";
            players.Clear();
        }

        /// <summary>
        /// 移除所有玩家
        /// </summary>
        public void RemoveAll()
        {
            foreach (var player in Players)
            {
                OnExit(player);
                player.OnExit();
                player.Scene = null;
                player.sceneID = "";
            }
            Players.Clear();
            players.Clear();
        }

        /// <summary>
        /// 移除场景所有玩家操作
        /// </summary>
        public void RemoveOperations()
        {
            operations.Clear();
        }

        ~NetScene() 
        {
            onSerializeOptHandle = null;
        }
    }
}