using Example2;
using Net.Share;
using Net.System;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExampleServer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private Service server;
        private bool run;

        private void button1_Click(object sender, EventArgs e)
        {
            if (run) 
            {
                button1.Text = "启动";
                server?.Close();
                run = false;
                return;
            }
            int port = int.Parse(textBox2.Text);//设置端口
            server = new Service();//创建服务器对象
            server.Log += str=> {//监听log
                if (listBox1.Items.Count > 2000)
                    listBox1.Items.Clear();
                listBox1.Items.Add(str);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            };
            server.OnlineLimit = 24000;//服务器最大运行2500人连接
            server.LineUp = 24000;
            server.MaxThread = 10; //增加并发线程
            server.RTOMode = RTOMode.Variable;
            server.RTO = 50;
            server.MTU = 1300;
            server.MTPS = 2048;
            server.SetHeartTime(10,200);
            server.OnNetworkDataTraffic += (a, b, c, d, e1, f, g) => {//当统计网络性能,数据传输量
                toolStripStatusLabel1.Text = $"发送数量:{a} 发送字节:{ByteHelper.HumanReadableFilesize(b)} 接收数量:{c} 接收字节:{ByteHelper.HumanReadableFilesize(d)} 发送fps:{f} 接收fps:{g} 解析数量:{e1}";
                label2.Text = "当前在线人数:" + server.OnlinePlayers + " 未知客户端:" + server.UnClientNumber;
            };
            server.AddAdapter(new Net.Adapter.SerializeAdapter3());
            server.AddAdapter(new Net.Adapter.CallSiteRpcAdapter<Player>());
            server.Run((ushort)port);//启动
            run = true;
            button1.Text = "关闭";
            SQLiteHelper.connStr = $"Data Source='{AppDomain.CurrentDomain.BaseDirectory}/Data/example2.db';";
            Example2DB.I.Init(Example2DB.I.OnInit);
            ThreadManager.Invoke(0f, ()=> {//每帧检查调用mysql线程调用中心
                Example2DB.I.ExecutedContext();
                return true;
            }, true);
            ThreadManager.Invoke(1f, ()=> {//每秒检查有没有数据需要往mysql数据库更新
                Example2DB.I.Executed();
                return true;
            }, true);
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(100);
            //        byte num = 0;
            //        foreach (var user in Example2DB.I.UserinfoDatas.Values)
            //        {
            //            //user.Position = RandomHelper.Range(10000, 9999999).ToString();
            //            //user.Health = RandomHelper.Range(10000, 9999999);
            //            //user.MoveSpeed = RandomHelper.Range(10000, 9999999);
            //            //user.Rotation = RandomHelper.Range(10000, 9999999).ToString();
            //            //if (num++ >= 5)
            //            //    break;
            //            //user.BufferBytes = new byte[] { 1,2,3,4,5,6,7, num++, 9 };
            //        }
            //    }
            //});
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server?.Close();
            Process.GetCurrentProcess().Kill();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            var item = listBox1.SelectedItem;
            if (item == null)
                return;
            MessageBox.Show(item.ToString());
        }
    }
}
