using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Example2DB
{
    public ConcurrentDictionary<string, UserinfoData> UserinfoDatas = new ConcurrentDictionary<string, UserinfoData>();
    public ConcurrentDictionary<int, ConfigData> Configs = new ConcurrentDictionary<int, ConfigData>();

    public void OnInit(List<object> data)
    {
        foreach (var item in data)
        {
            if (item is UserinfoData data1)
            {
                UserinfoDatas.TryAdd(data1.Account, data1);
            }
            if (item is ConfigData data2)
            {
                Configs.TryAdd((int)data2.Id, data2);
            }
        }
    }
}