using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 盒体碰撞几何体
    /// </summary>
    public class CollisionBox : CollisionPrimitive
    {

        public TSVector3 HalfSize;
        public TSVector3[] LocalVertices; //顶点本地坐标
        public TSVector3[] Vertices; // 顶点世界坐标
        private bool m_VerticesUpdated = false; //顶点世界坐标更新标识，防止一帧内重复更新

        public CollisionBox(TSVector3 halfSize)
        {
            HalfSize = halfSize;
            LocalVertices = new TSVector3[8];
            Vertices = new TSVector3[8];
            GetLocalVertices();
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            m_VerticesUpdated = false;
            if (BoundingVolum is BoundingBox)
                GetVertices();//更新这个顶点到世界坐标点
            BoundingVolum.Update(this);//更新包围盒的最大最小点
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
                LocalVertices[i] = new TSVector3(Mults[i, 0], Mults[i, 1], Mults[i, 2]).Multiply(HalfSize);
            }
        }

        /// <summary>
        /// 获取顶点世界坐标
        /// </summary>
        public void GetVertices()
        {
            if (m_VerticesUpdated)
                return;

            BoundingVolum.minX = FP.MaxValue;
            BoundingVolum.maxX = FP.MinValue;
            BoundingVolum.minY = FP.MaxValue;
            BoundingVolum.maxY = FP.MinValue;
            BoundingVolum.minZ = FP.MaxValue;
            BoundingVolum.maxZ = FP.MinValue;
            for (int i = 0; i < LocalVertices.Length; i++)
            {
                TSVector3 vertice = Transform * LocalVertices[i];
                Vertices[i] = vertice;
                if (vertice.x > BoundingVolum.maxX) BoundingVolum.maxX = vertice.x;
                if (vertice.x < BoundingVolum.minX) BoundingVolum.minX = vertice.x;
                if (vertice.y > BoundingVolum.maxY) BoundingVolum.maxY = vertice.y;
                if (vertice.y < BoundingVolum.minY) BoundingVolum.minY = vertice.y;
                if (vertice.z > BoundingVolum.maxZ) BoundingVolum.maxZ = vertice.z;
                if (vertice.z < BoundingVolum.minZ) BoundingVolum.minZ = vertice.z;
            }

            m_VerticesUpdated = true;
        }
    }
}
