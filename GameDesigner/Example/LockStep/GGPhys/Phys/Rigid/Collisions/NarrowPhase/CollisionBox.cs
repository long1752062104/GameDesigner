using System;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 盒体碰撞几何体
    /// </summary>
    public class CollisionBox : CollisionPrimitive
    {

        public Vector3d HalfSize;
        public Vector3d[] LocalVertices; //顶点本地坐标
        public Vector3d[] Vertices; // 顶点世界坐标
        private bool m_VerticesUpdated = false; //顶点世界坐标更新标识，防止一帧内重复更新

        public CollisionBox(Vector3d halfSize)
        {
            HalfSize = halfSize;
            LocalVertices = new Vector3d[8];
            Vertices = new Vector3d[8];
            GetLocalVertices();
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            m_VerticesUpdated = false;
            if (BoundingVolum is BoundingBox) GetVertices();
            BoundingVolum.Update(this);
        }

        /// <summary>
        /// 获取顶点本地坐标
        /// </summary>
        void GetLocalVertices()
        {
            int[,] Mults = new int[,]
            {
                {1,1,1},{-1,1,1},{-1,1,-1},{1,1,-1},
                {1,-1,1},{-1,-1,1},{-1,-1,-1},{1,-1,-1}
            };
            for (int i = 0; i < 8; i++)
            {
                LocalVertices[i] = new Vector3d(Mults[i, 0], Mults[i, 1], Mults[i, 2]) * HalfSize;
            }
        }

        /// <summary>
        /// 获取顶点世界坐标
        /// </summary>
        public void GetVertices()
        {
            if (m_VerticesUpdated) return;
            for (int i = 0; i < LocalVertices.Length; i++)
            {
                Vertices[i] = Transform * LocalVertices[i];
            }
            m_VerticesUpdated = true;
        }
    }
}
