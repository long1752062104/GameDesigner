using System.Collections.Generic;
using UnityEngine;
using GGPhys.Rigid;

namespace GGPhysUnity
{
    public class RigidPhysicsEngine : MonoBehaviour
    {
        public int iterations = 0;
        public float gravity = -9.81f;
        public int maxThreadCount = 32;

        public Vector3 gridSize;
        public Vector3 gridCellSize;
        public Vector3 gridCenterOffset;

        public float belta = 0.22f;
        public float slop = 0.001f;
        public float tolerence = 0.0008f;

        public float timeStep = 0.01f;

        public int stepCount = 1;

        public bool autoStep = true;

        private static List<BRigidBody> m_WaitAddedRigidBodies;

        public static RigidBodyEngine Instance { get; private set; }

        private void Awake()
        {
            Instance = new RigidBodyEngine(gravity, gridSize.ToVector3d(), gridCellSize.ToVector3d(), gridCenterOffset.ToVector3d(), maxThreadCount);
            Instance.SIResolver.Interations = iterations;
            Instance.SIResolver.Belta = belta;
            Instance.SIResolver.Slop = -slop;
            Instance.SIResolver.Tolerence = tolerence;

            AddWaitRigidBodies();
        }

        private void OnDestroy()
        {
            foreach (var body in Instance.Bodies)
            {
                body.UnityBody = null;
            }
            Instance = null;
        }

        /// <summary>
        /// 排队等待将加入
        /// </summary>
        /// <param name="bBody"></param>
        public static void WaitAdd(BRigidBody bBody)
        {
            if (m_WaitAddedRigidBodies == null)
            {
                m_WaitAddedRigidBodies = new List<BRigidBody>();
            }
            m_WaitAddedRigidBodies.Add(bBody);
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="contactBody">第一个碰撞到的刚体</param>
        /// <param name="contactPoint">碰撞点</param>
        /// <returns></returns>
        //public static bool RayCastStatic(Vector3d start, Vector3d direction, REAL distance, uint layerMask, ref BRigidBody contactBody, ref Vector3d contactPoint)
        //{
        //    RigidBody body = null;
        //    bool result = Instance.Collisions.RayCastStatic(start, direction, distance, layerMask, ref body, ref contactPoint);
        //    contactBody = body.UnityBody;
        //    return result;
        //}


        /// <summary>
        /// 按顺序把排队刚体加入
        /// </summary>
        void AddWaitRigidBodies()
        {
            if (m_WaitAddedRigidBodies == null) return;
            foreach (var rigidBody in m_WaitAddedRigidBodies)
            {
                rigidBody.AddToEngine();
            }
            m_WaitAddedRigidBodies.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            DebugExtension.DrawBounds(new Bounds(gridCenterOffset, gridCellSize), Color.green);
            DebugExtension.DrawBounds(new Bounds(gridCenterOffset, (gridSize.ToVector3d() * gridCellSize.ToVector3d()).ToVector3()), Color.green);
        }
    }
}

