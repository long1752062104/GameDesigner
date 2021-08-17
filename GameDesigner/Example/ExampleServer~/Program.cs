using ECS;
using Example2;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExampleServer
{
    class TTT : Component 
    {

    }

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //args = new string[] { "Example2" };
            if (args.Length == 0)
                return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args[0] == "Example1") 
            {
                Application.Run(new Form1());
            }
            else if (args[0] == "Example2")
            {
                Task.Run(() => {
                    while (true)
                    {
                        Thread.Sleep(30);
                        Time.time++;
                    }
                });
                Application.Run(new Form2());
            }
            else if (args[0] == "Example3")
            {
                Application.Run(new Form3());
            }
            else if (args[0] == "Example4")
            {
                Application.Run(new Form4());
            }
        }
    }
}
