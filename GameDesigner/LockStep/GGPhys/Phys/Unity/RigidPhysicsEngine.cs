using GGPhys.Rigid;
using System;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    public class RigidPhysicsEngine : MonoBehaviour
    {
        public int iterations = 4;
        public float gravity = -9.81f;
        public int maxThreadCount = 32;

        public Vector3 gridSize;
        public Vector3 gridCellSize;
        public Vector3 gridCenterOffset;

        private readonly float belta = 0.22f;
        private readonly float slop = 0.001f;
        private readonly float tolerence = 0.0008f;

        private static List<BRigidBody> m_WaitAddedRigidBodies;

        public static RigidBodyEngine Instance { get; private set; }
        public RigidBodyEngine instance;

        protected virtual void Awake()
        {
            Instance = new RigidBodyEngine(gravity, gridSize, gridCellSize, gridCenterOffset, maxThreadCount);
            Instance.SIResolver.Interations = iterations;
            Instance.SIResolver.Belta = belta;
            Instance.SIResolver.Slop = -slop;
            Instance.SIResolver.Tolerence = tolerence;
            instance = Instance;
            AddWaitRigidBodies();
        }

        private void OnDestroy()
        {
            foreach (RigidBody body in Instance.Bodies)
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
        /// 按顺序把排队刚体加入
        /// </summary>
        void AddWaitRigidBodies()
        {
            if (m_WaitAddedRigidBodies == null) return;
            foreach (BRigidBody rigidBody in m_WaitAddedRigidBodies)
            {
                rigidBody.AddToEngine();
            }
            m_WaitAddedRigidBodies.Clear();
        }

        public static void Simulate(FP step)
        {
            Instance.RunPhysics(step);
        }

        private void OnDrawGizmosSelected()
        {
            DebugExtension.DrawBounds(new Bounds(gridCenterOffset, gridCellSize), Color.green);
            DebugExtension.DrawBounds(new Bounds(gridCenterOffset, gridSize.Multiply(gridCellSize)), Color.green);
        }
    }
}

