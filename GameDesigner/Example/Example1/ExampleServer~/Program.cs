using System;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            Service service = new Service();
            service.Log += Console.WriteLine;
            service.AddAdapter(new Net.Adapter.SerializeAdapter2());
            service.Start();
            while (true) { Console.ReadLine(); }
        }
    }
}
