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

    public void OnInit(object data)
    {
        if (data is UserinfoData data1)
        {
            UserinfoDatas.TryAdd(data1.Account, data1);
        }
    }
}