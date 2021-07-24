using Example2;
using Net.Share;
using System;
using System.Diagnostics;
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

        private ServiceComponent server;
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
            server = new ServiceComponent();//创建服务器对象
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
            server.AddAdapter(new Net.Adapter.CallSiteRpcAdapter<PlayerComponent>());
            server.Run((ushort)port);//启动
            run = true;
            button1.Text = "关闭";
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
