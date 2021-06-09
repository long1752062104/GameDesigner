using Net.Example2;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            DBComponent.Instance.Load().Wait();
            ServiceComponent service = new ServiceComponent();
            service.Log += Console.WriteLine;
            service.AddAdapter(new Net.Adapter.SerializeAdapter2());
            service.Start();
            while (true) { Console.ReadLine(); }
        }
    }
}
