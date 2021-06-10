using System;
using System.Windows.Forms;

namespace ExampleServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
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
                Application.Run(new Form2());
            }
            else if (args[0] == "Example3")
            {
                Application.Run(new Form3());
            }
        }
    }
}
