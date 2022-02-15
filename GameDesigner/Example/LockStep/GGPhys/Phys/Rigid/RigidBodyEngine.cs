using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid.Constraints;
using GGPhys.Rigid.Forces;
using REAL = FixMath.FP;
using System.Threading.Tasks;

namespace GGPhys.Rigid
{
    ///<summary>
    /// 刚体引擎类
    ///</summary>
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
        /// 刚体对象池
        /// </summary>
        private ClassObjectPool<RigidBody> m_BodiesPool;

        /// <summary>
        /// 刚体序号，用于对刚体进行标识
        /// </summary>
        private int m_CurrentBodyIndex = 1;

        /// <summary>
        /// 最大使用线程数
        /// </summary>
        private int m_MaxThread;

        public RigidBodyEngine(REAL gravity, Vector3d gridSize, Vector3d gridCellSize, Vector3d gridCenterOffset, int maxThreadCount)
        {
            m_MaxThread = maxThreadCount;

            Bodies = new List<RigidBody>();
            ForceAreas = new List<RigidForceArea>();
            Forces = new List<RigidForce>();
            Constraints = new List<RigidConstraint>();
            SIResolver = new RigidContactSIResolver();
            m_BodiesPool = new ClassObjectPool<RigidBody>(2000);

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
            foreach (var body in Bodies)
            {
                body.ClearAccumulators();
            }
        }

        ///<summary>
        /// 完整执行一次迭代
        ///<summary>
        public void RunPhysics(REAL dt)
        {
            ApplyForces(dt);

            PrepareConstraintData();

            SIResolver.Resolve(Collisions.CollisionData, dt); ;

            Integrate(dt);

            LateUpdate();
        }

        /// <summary>
        /// 计算所有刚体内参
        /// </summary>
        public void CalculateDerivedData()
        {
            Task[] tasks = new Task[m_MaxThread];
            int rowNum = Bodies.Count / m_MaxThread + 1;
            for (int k = 0; k < m_MaxThread; k++)
            {
                int start = rowNum * k;
                int end = rowNum * (k + 1);
                tasks[k] = Task.Factory.StartNew(() =>
                {
                    for (int i = start; (i < Bodies.Count && i < end); i++)
                    {
                        var body = Bodies[i];
                        body.CalculateDerivedData();
                        body.UnityBody.CalculateInternals();
                    }
                });
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 生产一个刚体
        /// </summary>
        /// <returns></returns>
        public RigidBody SpawnBody()
        {
            RigidBody body = m_BodiesPool.Spawn();
            body.Active = true;
            body.ID = m_CurrentBodyIndex;
            m_CurrentBodyIndex += 1;
            return body;
        }

        /// <summary>
        /// 回收一个刚体
        /// </summary>
        /// <param name="body"></param>
        public void RecycleBody(RigidBody body)
        {
            body.Clear();
            m_BodiesPool.Recycle(body);
        }

        /// <summary>
        /// 运用作用力
        /// </summary>
        /// <param name="dt"></param>
        private void ApplyForces(REAL dt)
        {
            foreach (var f in ForceAreas)
            {
                foreach (var b in Bodies)
                {
                    if (b.IsStatic) continue;
                    if (!b.UseAreaForce) continue;
                    f.UpdateForce(b, dt);
                }
            }

            foreach (var b in Bodies)
            {
                if (b.IsStatic) continue;
                b.ApplyForceToVelocity(dt);
            }
        }


        ///<summary>
        /// 产生约束数据
        ///<summary>
        private void PrepareConstraintData()
        {
            foreach (var gen in Constraints)
            {
                gen.PrepareConstraintData();
            }
        }

        /// <summary>
        /// 刚体逐个迭代
        /// </summary>
        /// <param name="dt"></param>
        private void Integrate(REAL dt)
        {
            foreach (var b in Bodies)
            {
                b.Integrate(dt);
            }
        }

        /// <summary>
        /// 循环最后更新
        /// </summary>
        private void LateUpdate()
        {
            CalculateDerivedData();
        }
    }
}