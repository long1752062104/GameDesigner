using TrueSync;

namespace GGPhys.Rigid.Forces
{

    /// <summary>
    /// 刚体作用力类
    /// </summary>
    public abstract class RigidForce
    {
        /// <summary>
        /// 为刚体生成力
        /// </summary>
        public abstract void UpdateForce(FP dt);

    }
}
