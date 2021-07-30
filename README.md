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
BufferPool.Push(seg);//再次压入
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

## 致谢

谢谢大家对我的支持，如果有其他问题，请加QQ群:825240544讨论
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

<br>不留名的大佬们 微信总资助 ¥ 653</br>

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/pay.jpg" width = "600" height = "400" alt="图片名称" align=center />