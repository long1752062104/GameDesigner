using GGPhys.Rigid.Constraints;
using GGPhys.Rigid.Forces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;

namespace GGPhys.Rigid
{
    ///<summary>
    /// 刚体引擎类
    ///</summary>
    [System.Serializable]
    public class RigidBodyEngine
    {
        ///<summary>
        /// 所有刚体
        ///</summary>
        public List<RigidBody> Bodies;

        /// <summary>
        /// 作用力发生器
        /// </summary>
        public List<RigidForceArea> ForceAreas;

        /// <summary>
        /// 作用力发生器
        /// </summary>
        public List<RigidForce> Forces;

        ///<summary>
        /// 所有约束列表
        ///<summary>
        public List<RigidConstraint> Constraints;

        /// <summary>
        /// 碰撞约束
        /// </summary>
        public CollisionConstraint Collisions;

        ///<summary>
        /// 碰撞约束求解器
        ///<summary>
        public RigidContactSIResolver SIResolver;

        /// <summary>
        /// 重力发生器
        /// </summary>
        public RigidGravityForce GravityForceArea;

        /// <summary>
        /// 最大使用线程数
        /// </summary>
#pragma warning disable IDE0052 // 删除未读的私有成员
        private readonly int m_MaxThread;
#pragma warning restore IDE0052 // 删除未读的私有成员

        public RigidBodyEngine(FP gravity, TSVector3 gridSize, TSVector3 gridCellSize, TSVector3 gridCenterOffset, int maxThreadCount)
        {
            m_MaxThread = maxThreadCount;

            Bodies = new List<RigidBody>();
            ForceAreas = new List<RigidForceArea>();
            Forces = new List<RigidForce>();
            Constraints = new List<RigidConstraint>();
            SIResolver = new RigidContactSIResolver();

            GravityForceArea = new RigidGravityForce(gravity);
            ForceAreas.Add(GravityForceArea);

            Collisions = new CollisionConstraint(gridSize, gridCellSize, gridCenterOffset);
            Collisions.MaxThread = maxThreadCount;
            Constraints.Add(Collisions);
        }

        /// <summary>
        /// 设置重力
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(float gravity)
        {
            GravityForceArea.SetGravity(gravity);
        }

        ///<summary>
        /// 初始化
        ///<summary>
        public void StartFrame()
        {
            foreach (RigidBody body in Bodies)
            {
                body.ClearAccumulators();
            }
        }

        internal System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public long ApplyForcesTime, GenerateContactsTime, IntegrateTime, PostUpdateTime;

        ///<summary>
        /// 完整执行一次迭代
        ///<summary>
        public void RunPhysics(FP dt)
        {
            sw.Restart();
            ApplyForces(dt);//增加向下的力和角速度 没有这个不会下落， 但碰撞在
            sw.Stop();
            ApplyForcesTime = sw.ElapsedMilliseconds;

            sw.Restart();
            GenerateContacts(); //没有这个碰撞失效
            sw.Stop();
            GenerateContactsTime = sw.ElapsedMilliseconds;

            if (Collisions.CollisionData.Contacts.Count > 0)//没有这个会掉地形
                SIResolver.ResolveContacts(Collisions.CollisionData, dt);

            sw.Restart();
            Integrate(dt);//没有这个碰撞失效
            sw.Stop();
            IntegrateTime = sw.ElapsedMilliseconds;

            sw.Restart();
            PostUpdate();//没有这个移动失效
            sw.Stop();
            PostUpdateTime = sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// 计算所有刚体内参
        /// </summary>
        public void CalculateDerivedData()
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                if (Bodies[i].IsStatic)
                    continue;
                Bodies[i].CalculateDerivedData();
                Bodies[i].UnityBody.CalculateInternals();
            }
        }

        /// <summary>
        /// 运用作用力
        /// </summary>
        /// <param name="dt"></param>
        private void ApplyForces(FP dt)
        {
            for (int i = 0; i < ForceAreas.Count; i++)
            {
                for (int j = 0; j < Bodies.Count; j++)
                {
                    if (Bodies[j].IsStatic)
                        continue;
                    if (!Bodies[j].UseAreaForce)
                        goto J;
                    ForceAreas[i].UpdateForce(Bodies[j], dt);//更新向下的力
                J: Bodies[j].ApplyForceToVelocity(dt);//更新角速度
                }
            }
        }


        ///<summary>
        /// 产生碰撞数据
        ///<summary>
        private void GenerateContacts()
        {
            for (int i = 0; i < Constraints.Count; i++)
            {
                Constraints[i].GenerateContacts();//CollisionConstraint类
            }
        }

        /// <summary>
        /// 刚体逐个迭代
        /// </summary>
        /// <param name="dt"></param>
        private void Integrate(FP dt)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                Bodies[i].Integrate(dt);
            }
        }

        /// <summary>
        /// 循环最后更新
        /// </summary>
        private void PostUpdate()
        {
            CalculateDerivedData();
        }
    }
}