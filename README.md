## GDNet
 (Game Designer Network)游戏设计网络框架使用C#开发，支持.NetFramework和Core版本，目前主要用于Unity3D，Form窗体程序和控制台项目开发。扩展性强，支持新协议快速扩展，当前支持tcp，gudp, udx, kcp, enet, web网络协议。简易上手. api注释完整。

## 模块图

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/gdnet.png" width = "620" height = "700" alt="图片名称" align=center />

## 使用

<br>下载GameDesigner, 解压之前要进入GameDesigner目录的第二层GameDesigner文件夹拖入unity的Assets资源目录</br>
<br>然后打开BuildSettings->ProjectSettings->OtherSettings->设置 ApiCompatibilityLevel* = .NET 4.x 和 AllowUnsafeCode勾上</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/gdnetsetting.png" width = "645" height = "239" alt="图片名称" align=center />

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
        //你也可以理解为返回true则是输入的账号密码正确, 返回false则是账号或密码错误
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
<br>3.main入口方法写上</br>

```
var server = new Service();//创建服务器对象
server.Log += Console.WriteLine;//打印服务器内部信息
server.Run(6666);//启动6666端口
```

<br>4.创建客户端控制台项目</br>

<br>5.定义一个Test类, 用来测试rpc过程调用</br>

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
<br>6.然后在main入口方法写上</br>
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

## 对象池
gdnet提供BufferPool二进制数据对象池和ObjectPool类对象池, 在网络代码内部采用了BufferPool对象池, 使得网络可以高速读写处理数据, 而不是每次要创建一个byte[]来处理!


```
var seg = BufferPool.Take(65535);//申请65535字节的内存片
seg.WriteValue(123);//写入4字节的值
BufferPool.Push(seg);//压入内存片,等待下次复用
var seg1 = BufferPool.Take(65535);//这次的申请内存片,实际是从BufferPool中弹出seg对象,在这个过程中不会再创建byte[65535]
seg1.WriteValue(456);
BufferPool.Push(seg1);//再次压入
```

## 极速序列化
gdnet内部实现了极速序列化, 速度远超出protobuff 5-10倍, 在案例1测试中就采用了极速序列化适配器, 可以同步1万个cube, 如果用protobuff的话,只能同步2500个cube
内部的序列化已经有三个版本, 一个是之前的NetConvertOld字符串序列化,这个版本性能是非常糟糕的,性能远不及Newtonsoft.Json, 而第二版本的序列化NetConvertBinary二进制序列化则超越protobuff的性能, 体积也和protobuff一样, 为什么比protobuff快? protobuff内部实现还是使用的反射field.GetValue这种方法,而NetConvertBinary则是采用了dynamic动态语法实现的,在获取值和写值时比反射field.GetValue要快5倍. 这个NetConvertBinary版本已经超越protobuff了,为什么还要开发极速序列化NetConvertFast2? 主要还是为了框架的高性能处理.
NetConvertFast2极速序列化的使用:
1.要生成绑定类型, 在unity中有生成绑定类型工具, 也可以在这里生成:[绑定类型工具](https://gitee.com/leng_yue/fast2-build-tool)

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/fast2build.png" width = "414" height = "229" alt="图片名称" align=center />

```
public class Test //序列化的类型
{
    public int num;
    public string str;
}

static void Main(string[] args)
{
    NetConvertFast2.AddSerializeType3<Test>();//绑定Test为可序列化类型
    var seg = NetConvertFast2.SerializeObject(new Test());//序列化Test类
    var obj = NetConvertFast2.DeserializeObject<Test>(seg);//反序列化Test类
}
```
你也可以调用此api直接生成绑定类型

```
Fast2BuildToolMethod.Build(typeof(Test), AppDomain.CurrentDomain.BaseDirectory);//生成test绑定类型
Fast2BuildToolMethod.BuildArray(typeof(Test), AppDomain.CurrentDomain.BaseDirectory);//生成test数组绑定类型
Fast2BuildToolMethod.BuildGeneric(typeof(Test), AppDomain.CurrentDomain.BaseDirectory);//生成test泛型绑定类型
```
新版本中增加了运行时动态编译绑定类型

```
Fast2BuildMethod.DynamicBuild(BindingEntry.GetBindTypes());//动态编译指定的类型列表
```
详细信息请打开案例: GameDesigner\Example\SerializeTest\Scenes\example3.unity 查看

## ECS模块
ECS模块类似unity的gameObject->component模式, 在ecs中gameObject=entity, component=component, system类执行, ecs跟gameObject模式基本流程是一样的, 只是ecs中的组件可以复用, 而gameObject的component则不能复用, 在创建上万个对象时, gameObject就得重新new出来对象和组件, 而ecs调用Destroy时是把entity或component压入对象池, 等待下一次复用.实际上对象没有被释放,所以性能高于gameObject的原因


```
//ecs时间组件
public class TimerComponent : Component, IUpdate //继承IUpdate接口后就会每帧调用Update方法
{
    private DateTime dateTime;
    public override void Awake()
    {
        dateTime = DateTime.Now.AddSeconds(5);//在初始化时,把当前时间推到5秒后
    }
    public void Update()
    {
        if (DateTime.Now >= dateTime)//当5秒时间到, 则删除这个时间组件, 实际上是压入对象池
        {
            Destroy(this);
        }
    }
    public override void OnDestroy()//当销毁, 实际是压入对象池前调用一次
    {
    }
}

static void Main(string[] args)
{
    var entity = GSystem.Instance.Create<Entity>();//创建实体对象,这会在对象池中查询,如果对象池没有对象,则会new, 有则弹出entity
    entity.AddComponent<TimerComponent>();//添加时间组件,也是从对象池查询,没有则new, 有则弹出TimerComponent对象
    while (true)
    {
        Thread.Sleep(30);
        GSystem.Instance.Run();//每帧执行ecs系统
    }
}
```

## MVC模块
mvc模块:模型,控制,视图分离, mvc模块适应于帧同步游戏, model定义了对象字段,属性,事件, controller执行业务逻辑, view显示结果
在帧同步中, mvc是分离的, 各自处理各自的, 做到可以不相关的地步, 比如view卡住, controller还是一直执行, 互不影响!

热更新FieldCollection组件使用:当在热更新项目中, 字段无需使用Find各种查找, 使用FieldCollection组件即可自动帮你处理完成字段收集引用, 一键生成即可写你的功能代码
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/hotfixFC.png" width = "1179" height = "685" alt="图片名称" align=center />

## ILRuntime热更新
网络协议传输类型必须在主工程定义! 那什么热更网络协议类型? 热更新网络协议类型必须新下载主工程apk替换旧的apk, 重新安装新的apk, 由于主工程的apk大小不是很大, 所有的资源都在ab文件里面! 所以是可以这样更新的
注意: 协议定义在热更新项目中将无法反序列化! 必须定义在主工程!

热更新案例文档:[案例2热更新](https://docs.qq.com/doc/DS3FXbERiUXZnWHVx)

## UNet&Mirror设计模式
变量同步案例:Assets/GameDesigner/Example/Example1/Scenes/SyncVarDemo.unity
可以同步C#基础单元结构体: byte, sbyte, short, ushort, int, uint, float, long, ulong, DateTime, decimal, string类型
同步自定义结构体: 纯包含基元类型, 不需要任何额外处理
同步自定义结构体,包含类型字段: 需要重写Equals额外处理字段对等
同步类型:需要重写Equals额外处理字段对等

与场景内的玩家进行变量同步: 原理是检查字段值有没有改变, 改变了就会往服务器发送, 服务器转发给场景内的所有客户端, 以identiy值取到对应的对象, 进行变量设置, 达到变量同步效果!
客户端与服务器进行变量同步: 原理是检查字段值改变后, 发送字段的id和值到服务器, 服务器检查NetPlayer的变量管理列表取出对应的对象, 进行变量设置, 达到p2p变量同步效果

[SyncVar]//在字段定义这个特性, 则为玩家之间变量
[SyncVar(authorize = false)]//这是你实例化的网络物体, 其他玩家不能改变你的对象变量, 即使改变了也不会发生同步给其他玩家, 只能由自己控制变量变化后才会同步给其他玩家
[SyncVar(id = 1)]//这是p2p 客户端只与服务器的netplayer之间变量同步, 开发者要保证id必须是唯一的 详情请看案例1的Example1.Client类定义

## 百万级别RPC小数据测试
这里我们测试了100万次从客户端到服务器的请求并响应, 所需要的时间是4.67秒


```
class Program
{
    static Stopwatch stopwatch;

    static void Main(string[] args)
    {
        NDebug.BindLogAll(Console.WriteLine);

        BufferStreamShare.Size = 1024 * 1024 * 100;//服务器每个客户端可以缓存的数据大小

        //此处是服务器部分, 可以复制到另外一个控制台项目
        var server = new TcpServer<NetPlayer,NetScene<NetPlayer>>();
        server.LimitQueueCount = 10000000;//测试小数据的快速性能, 可以设置这里, 默认限制在65536
        server.PackageLength = 1000000;//小数据包封包合包大小, 一次性能运送的小数据包数量
        server.StackBufferSize = 1024 * 1024 * 50;//接收缓存数据包的最大值, 如果超出则被丢弃
        server.StackNumberMax = 1000000;//允许叠包数据次数, 超出则被丢弃
        server.AddAdapter(new Net.Adapter.SerializeAdapter3());//采用极速序列化进行序列化rpc数据模型
        server.AddAdapter(new Net.Adapter.CallSiteRpcAdapter<NetPlayer>());//采用极速调用rpc方法适配器
        server.Run();

        //此处是客户端部分, 可以复制到另外一个控制台项目
        var client = new TcpClient();
        client.LimitQueueCount = 10000000;
        client.PackageLength = 1000000;
        client.StackBufferSize = 1024 * 1024 * 50;
        client.StackNumberMax = 1000000;
        client.AddAdapter(new Net.Adapter.SerializeAdapter3());
        client.AddAdapter(new Net.Adapter.CallSiteRpcAdapter());
        client.AddRpcHandle(new Program());
        client.Connect().Wait();

        stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 1000000; i++)
        {
            client.SendRT(NetCmd.Local, 1, i);
        }

        Console.ReadLine();
    }

    [Rpc(mask = 1)]
    void test(int i)
    {
        if (i % 10000 == 0)
            Console.WriteLine(i);
        if (i >= 999999)
        {
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
```


## 常见问题总汇
这里是开发者遇到的问题, 我都会在这里详细写出来, 这样大家遇到的问题都可以先在这里查看

## 致谢

谢谢大家对我的支持，如果有其他问题，请加QQ群:825240544讨论

## TapTap游戏
<br>1.[士兵召唤](https://www.taptap.com/app/221847)</br>
<br>2.[我是一只鱼](https://www.taptap.com/app/220242)</br>
<br>3.[山海经吞食天地](https://www.taptap.com/app/207447)</br>
<br>4.[捍卫星球](https://www.taptap.com/app/170490)</br>

## 支持本项目
您的支持就是我不懈努力的动力。打赏时请一定留下您的称呼
<br>感谢以下人员对gdnet捐款:</br>

<br>1 vsmile ¥ 10</br>
<br>2 南归 ¥ 10</br>
<br>3 王者心，懂么？ ¥ 10</br>
<br>4 郭少 ¥ 5000</br>
<br>5 思念天边的你 ¥ 52</br>
<br>6 娟子 ¥ 1000</br>
<br>7 Slarvens ¥ 30</br>
<br>8 达西莉莉 ¥ 200</br>
<br>9 扬神无敌 ¥ 100</br>
<br>10 29.8°C ¥ 30</br>

<br>不留名的大佬们 微信总资助 ¥ 653</br>

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/pay.jpg" width = "600" height = "400" alt="图片名称" align=center />