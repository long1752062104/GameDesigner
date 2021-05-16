using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 三角形碰撞几何体
    /// </summary>
    public class CollisionTriangle : CollisionPrimitive
    {
        public TSVector3[] Vertices;
        public TSVector3 Normal;
        public TSVector3 EdgeAxis1;
        public TSVector3 EdgeAxis2;
        public TSVector3 EdgeAxis3;

        public CollisionTriangle(TSVector3 a, TSVector3 b, TSVector3 c)
        {
            Vertices = new TSVector3[3] { a, b, c };
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            Vertices[0] = Transform.Transform(Vertices[0]);
            Vertices[1] = Transform.Transform(Vertices[1]);
            Vertices[2] = Transform.Transform(Vertices[2]);
            Normal = TSVector3.Cross(Vertices[1] - Vertices[0], Vertices[2] - Vertices[1]).Normalized;
            EdgeAxis1 = Vertices[1] - Vertices[0];
            EdgeAxis2 = Vertices[2] - Vertices[1];
            EdgeAxis3 = Vertices[0] - Vertices[2];
            BoundingVolum.Update(this);
        }
    }
}

