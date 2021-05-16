using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    ///<summary>
    /// 凸多边形碰撞几何体
    ///</summary>
    public class CollisionConvex : CollisionPrimitive
    {
        public TSVector3[] Vertices;
        private TSVector3[] LocalVertices;

        public CollisionConvex(TSVector3[] vertices)
        {
            LocalVertices = vertices;
            Vertices = new TSVector3[vertices.Length];
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            GetVertices();
            BoundingVolum.Update(this);
        }

        void GetVertices()
        {
            for (int i = 0; i < LocalVertices.Length; i++)
            {
                Vertices[i] = Transform * LocalVertices[i];
            }
        }
    }
}
