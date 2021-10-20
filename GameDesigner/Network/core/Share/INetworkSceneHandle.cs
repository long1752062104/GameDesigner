namespace Net.Share
{
    using Net.Server;
    using global::System;

    /// <summary>
    /// 网络场景处理接口
    /// </summary>
    public interface INetworkSceneHandle<Player, Scene> where Player : NetPlayer where Scene : NetScene<Player>
    {
        /// <summary>
        /// 创建网络场景, 退出当前场景,进入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="sceneID">要创建的场景号或场景名称</param>
        /// <returns></returns>
        Scene CreateScene(Player player, string sceneID);

        /// <summary>
        /// 创建网络场景, 退出当前场景并加入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="sceneID">要创建的场景号或场景名称</param>
        /// <param name="scene">创建场景的实体</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        Scene CreateScene(Player player, string sceneID, Scene scene, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 退出当前场景,加入指定的场景 - 成功进入返回场景对象，进入失败返回null
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        Scene JoinScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 进入场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        Scene EnterScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 退出当前场景,切换到指定的场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        Scene SwitchScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 退出场景
        /// </summary>
        /// <param name="player"></param>
        /// <param name="addToMainScene">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        void ExitScene(Player player, bool addToMainScene = true, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 移除服务器场景. 从服务器总场景字典中移除指定的场景: 当你移除指定场景后,如果场景内有其他玩家在内, 则把其他玩家添加到主场景内
        /// </summary>
        /// <param name="sceneID">要移除的场景id</param>
        /// <param name="addToMainScene">允许即将移除的场景内的玩家添加到主场景?</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        bool RemoveScene(string sceneID, bool addToMainScene = true, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 将玩家从当前所在的场景移除掉， 移除之后此客户端将会进入默认主场景
        /// </summary>
        /// <param name="player">要执行的玩家实体</param>
        /// <param name="addToMainScene">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        bool RemoveScenePlayer(Player player, bool addToMainScene = true, Action<Scene> exitCurrentSceneCall = null);

        /// <summary>
        /// 从所有在线玩家字典中删除(移除)玩家实体
        /// </summary>
        /// <param name="player"></param>
        void DeletePlayer(Player player);

        /// <summary>
        /// 从所有在线玩家字典中移除玩家实体
        /// </summary>
        /// <param name="player"></param>
        void RemovePlayer(Player player);

        /// <summary>
        /// 从客户端字典中移除客户端
        /// </summary>
        /// <param name="client"></param>
        void RemoveClient(Player client);
    }
}