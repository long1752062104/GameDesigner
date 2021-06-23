## GDNet
 (Game Designer Network)游戏设计网络框架使用C#开发，支持.NetFramework和Core版本，目前主要用于Unity3D，Form窗体程序和控制台项目开发。扩展性强，支持新协议快速扩展，当前支持tcp，gudp, udx, kcp, enet, web网络协议。简易上手. api注释完整。

## 模块图

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/gdnet.png" width = "620" height = "700" alt="图片名称" align=center />

## 使用

<br>1.创建服务器项目,使用控制台或窗体程序都可以</br>
<br>2.新建一个Service脚本文件, 这个就是你的服务器类</br>
```
internal class Client : NetPlayer//你的客户端类
{
}
internal class Scene : NetScene<Client>//你的游戏场景类
{
}
class Service : TcpServer<Client, Scene>//你的服务器类
{
    protected override bool OnUnClientRequest(Client unClient, RPCModel model)
    {
        Console.WriteLine(model.pars[0]);
        return true;//100%必须理解这个, 返回false则永远在这里被调用, 返回true才被服务器认可
    }
    [Rpc(cmd = NetCmd.SafeCall)]//使用SafeCall指令后, 第一个参数插入客户端对象, 这个客户端对象就是哪个客户端发送,这个参数就是对应那个客户端的对象
    void test(Client client, string str) 
    {
        Console.WriteLine(str);
        SendRT(client, "test", "服务器rpc回调");//服务器回调
    }
}
```
<br>3.mian入口方法写上</br>

```
var server = new Service();//创建服务器对象
server.Log += Console.WriteLine;//打印服务器内部信息
server.Run(6666);//启动6666端口
```

<br>3.创建客户端控制台项目</br>

<br>4.定义一个Test类, 用来测试rpc过程调用</br>

```
class Test 
{
    [Rpc]
    void test(string str) 
    {
        Console.WriteLine(str);
    }
}
```
<br>4.然后在main入口方法写上</br>
```
TcpClient client = new TcpClient();
client.Log += Console.WriteLine;
Test test = new Test();
client.AddRpcHandle(test);
client.Connect("127.0.0.1", 6666).Wait();
client.SendRT("test", "第一次进入服务器的OnUnClientRequest方法");
client.SendRT("test", "客户端rpc请求");
```
到此基本使用完成

## 致谢

谢谢大家对我的支持，如果有其他问题，请加QQ群:825240544讨论
## 支持本项目
您的支持就是我不懈努力的动力。打赏时请一定留下您的称呼
<br>感谢以下人员对gdnet捐款:</br>

<br>1 vsmile ¥ 10</br>
<br>2 南归 ¥ 10</br>
<br>3 王者心，懂么？ ¥ 10</br>

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/GameDesigner/pay.jpg" width = "600" height = "400" alt="图片名称" align=center />