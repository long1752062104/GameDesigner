using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    /// <summary>
    /// ECS构架可以将此组件从Entity上移除这个组件并丢入对象池，给其他此刻需要此组件的Entity使用，因此可以节省大量的内存， 这也是ECS的特性可以大量重复使用Compoent
    /// </summary>
    public class Component : GObject
    {
#pragma warning disable IDE1006 // 命名样式
        public Entity entity { get; set; }
#pragma warning restore IDE1006 // 命名样式
    }
}
