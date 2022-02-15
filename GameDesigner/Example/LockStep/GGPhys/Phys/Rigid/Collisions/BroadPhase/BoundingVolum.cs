using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;
using GGPhysUnity;

namespace GGPhys.Rigid.Collisions
{
    public class BoundingVolum
    {
        public REAL minX;
        public REAL maxX;
        public REAL minY;
        public REAL maxY;
        public REAL minZ;
        public REAL maxZ;
        public int sizeLevel = -1;
        private Vector3d gridSize;
        private int gridGroupScale;
        private int gridLimit1;
        private int gridLimit2;

        public virtual void Init(CollisionPrimitive primitive) 
        {
            gridSize = RigidPhysicsEngine.Instance.Collisions.GridCellSize;
            gridGroupScale = RigidPhysicsEngine.Instance.Collisions.GridGroupScale;
            gridLimit1 = gridGroupScale * gridGroupScale * gridGroupScale;
            gridLimit2 = gridLimit1 * gridLimit1 * gridLimit1;
        }
        public virtual void Update(CollisionSphere sphere) { }
        public virtual void Update(CollisionBox box) { }
        public virtual void Update(CollisionCapsule capsule) { }
        public virtual void Update(CollisionConvex convex) { }
        public virtual void Update(CollisionTriangle triangle) { }
        public virtual byte OverlapNodes(Vector3d center) { return 0b00000000; }
        public virtual bool Overlap(BoundingVolum volum) { return false; }

        /// <summary>
        /// 设置所属网格层级
        /// </summary>
        public void GetSizeLevel()
        {
            var deltaX = (maxX - minX) / gridSize.x;
            var deltaY = (maxY - minY) / gridSize.y;
            var deltaZ = (maxZ - minZ) / gridSize.z;
            var volum = deltaX * deltaY * deltaZ;
            if(volum > gridLimit2)
            {
                sizeLevel = 0;
            }
            else if(volum > gridLimit1)
            {
                sizeLevel = 1;
            }
            else
            {
                sizeLevel = 2;
            }
        }

    }
}

