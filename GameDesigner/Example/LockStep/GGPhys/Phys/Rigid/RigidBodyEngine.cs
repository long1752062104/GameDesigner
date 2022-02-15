using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid.Constraints;
using GGPhys.Rigid.Forces;
using REAL = FixMath.FP;
using System.Threading.Tasks;

namespace GGPhys.Rigid
{
    ///<summary>
    /// ����������
    ///</summary>
    public class RigidBodyEngine
    {
        ///<summary>
        /// ���и���
        ///</summary>
        public List<RigidBody> Bodies;

        /// <summary>
        /// ������������
        /// </summary>
        public List<RigidForceArea> ForceAreas;

        /// <summary>
        /// ������������
        /// </summary>
        public List<RigidForce> Forces;

        ///<summary>
        /// ����Լ���б�
        ///<summary>
        public List<RigidConstraint> Constraints;

        /// <summary>
        /// ��ײԼ��
        /// </summary>
        public CollisionConstraint Collisions;

        ///<summary>
        /// ��ײԼ�������
        ///<summary>
        public RigidContactSIResolver SIResolver;

        /// <summary>
        /// ����������
        /// </summary>
        public RigidGravityForce GravityForceArea;

        /// <summary>
        /// ��������
        /// </summary>
        private ClassObjectPool<RigidBody> m_BodiesPool;

        /// <summary>
        /// ������ţ����ڶԸ�����б�ʶ
        /// </summary>
        private int m_CurrentBodyIndex = 1;

        /// <summary>
        /// ���ʹ���߳���
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
        /// ��������
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(float gravity)
        {
            GravityForceArea.SetGravity(gravity);
        }

        ///<summary>
        /// ��ʼ��
        ///<summary>
        public void StartFrame()
        {
            foreach (var body in Bodies)
            {
                body.ClearAccumulators();
            }
        }

        ///<summary>
        /// ����ִ��һ�ε���
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
        /// �������и����ڲ�
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
        /// ����һ������
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
        /// ����һ������
        /// </summary>
        /// <param name="body"></param>
        public void RecycleBody(RigidBody body)
        {
            body.Clear();
            m_BodiesPool.Recycle(body);
        }

        /// <summary>
        /// ����������
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
        /// ����Լ������
        ///<summary>
        private void PrepareConstraintData()
        {
            foreach (var gen in Constraints)
            {
                gen.PrepareConstraintData();
            }
        }

        /// <summary>
        /// �����������
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
        /// ѭ��������
        /// </summary>
        private void LateUpdate()
        {
            CalculateDerivedData();
        }
    }
}