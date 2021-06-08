namespace GGPhys.Rigid.Constraints
{

    ///<summary>
    /// 刚体约束类
    ///<summary>
    public abstract class RigidConstraint
    {
        /// <summary>
        /// 生成碰撞数据
        /// </summary>
        public abstract void GenerateContacts();
    }

}