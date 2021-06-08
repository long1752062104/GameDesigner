using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    public class BoundingSphere : BoundingVolum
    {
        private TSVector3 center;
        private FP radius;

        /// <summary>
        /// octree overlap
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

        public override void Init(CollisionPrimitive primitive)
        {
            base.Init(primitive);
            switch (primitive)
            {
                case CollisionSphere sphere:
                    radius = sphere.Radius;
                    break;
                case CollisionBox box:
                    radius = box.HalfSize.Magnitude;
                    break;
                default:
                    break;
            }
        }

        public override void Update(CollisionSphere sphere)
        {
            center = sphere.GetAxis(3);
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
            center = box.GetAxis(3);
            minX = center.x - radius;
            maxX = center.x + radius;
            minY = center.y - radius;
            maxY = center.y + radius;
            minZ = center.z - radius;
            maxZ = center.z + radius;
            GetSizeLevel();
        }
    }
}

