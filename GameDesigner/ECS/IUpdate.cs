using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    /// <summary>
    /// ecs组件更新接口, 减少组件for循环开销
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// 每帧更新
        /// </summary>
        void Update();
    }
}
