using System.Collections.Generic;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    public class OctreeNode
    {
        public TSVector3 Center;
        public bool IsLeaf;
        public OctreeNode[] ChildNodes;
        public List<CollisionPrimitive> Primitives;

        public OctreeNode()
        {
            ChildNodes = new OctreeNode[8];
            Primitives = new List<CollisionPrimitive>();
        }
    }
}

