using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    public class BoundingBox : BoundingVolum
    {
        /// <summary>
        /// octreeOverlap
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public override byte OverlapNodes(TSVector3 center)
        {
            byte nodeIndex = 0b11111111;

            if (minX > center.x)
            {
                nodeIndex &= 0b10011001;
            }

            if (maxX < center.x)
            {
                nodeIndex &= 0b01100110;
            }

            if (minY > center.y)
            {
                nodeIndex &= 0b11110000;
            }

            if (maxY < center.y)
            {
                nodeIndex &= 0b00001111;
            }

            if (minZ > center.z)
            {
                nodeIndex &= 0b11001100;
            }

            if (maxZ < center.z)
            {
                nodeIndex &= 0b00110011;
            }

            return nodeIndex;
        }

        /// <summary>
        /// 和其他包围盒overlap
        /// </summary>
        /// <param name="volum"></param>
        /// <returns></returns>
        public override bool Overlap(BoundingVolum volum)
        {
            if (maxX < volum.minX || minX > volum.maxX || maxY < volum.minY || minY > volum.maxY || maxZ < volum.minZ || minZ > volum.maxZ)
                return false;
            return true;
        }

        public override void Update(CollisionSphere sphere)
        {
            TSVector3 center = sphere.GetAxis(3);
            FP radius = sphere.Radius;
            minX = center.x - radius;
            maxX = center.x + radius;
            minY = center.y - radius;
            maxY = center.y + radius;
            minZ = center.z - radius;
            maxZ = center.z + radius;
            GetSizeLevel();
        }

        public override void Update(CollisionBox box)
        {
            //GetMinMaxPoint(box.Vertices);//获取包围盒最大和最小点
            GetSizeLevel();
        }

        public override void Update(CollisionCapsule capsule)
        {
            if (capsule.CenterOne.x < capsule.CenterTwo.x)
            {
                minX = capsule.CenterOne.x - capsule.Radius;
                maxX = capsule.CenterTwo.x + capsule.Radius;
            }
            else
            {
                minX = capsule.CenterTwo.x - capsule.Radius;
                maxX = capsule.CenterOne.x + capsule.Radius;
            }

            if (capsule.CenterOne.y < capsule.CenterTwo.y)
            {
                minY = capsule.CenterOne.y - capsule.Radius;
                maxY = capsule.CenterTwo.y + capsule.Radius;
            }
            else
            {
                minY = capsule.CenterTwo.y - capsule.Radius;
                maxY = capsule.CenterOne.y + capsule.Radius;
            }

            if (capsule.CenterOne.z < capsule.CenterTwo.z)
            {
                minZ = capsule.CenterOne.z - capsule.Radius;
                maxZ = capsule.CenterTwo.z + capsule.Radius;
            }
            else
            {
                minZ = capsule.CenterTwo.z - capsule.Radius;
                maxZ = capsule.CenterOne.z + capsule.Radius;
            }
            GetSizeLevel();
        }

        public override void Update(CollisionConvex convex)
        {
            GetMinMaxPoint(convex.Vertices);
            GetSizeLevel();
        }

        public override void Update(CollisionTriangle triangle)
        {
            GetMinMaxPoint(triangle.Vertices);
        }

        /// <summary>
        /// 生成AABB最小最大值
        /// </summary>
        /// <param name="vertices"></param>
        void GetMinMaxPoint(TSVector3[] vertices)
        {
            minX = FP.MaxValue;
            maxX = FP.MinValue;
            minY = FP.MaxValue;
            maxY = FP.MinValue;
            minZ = FP.MaxValue;
            maxZ = FP.MinValue;
            for (int i = 0; i < vertices.Length; ++i)
            {
                TSVector3 vertice = vertices[i];
                if (vertice.x > maxX) maxX = vertice.x;
                if (vertice.x < minX) minX = vertice.x;
                if (vertice.y > maxY) maxY = vertice.y;
                if (vertice.y < minY) minY = vertice.y;
                if (vertice.z > maxZ) maxZ = vertice.z;
                if (vertice.z < minZ) minZ = vertice.z;
            }
        }
    }
}


