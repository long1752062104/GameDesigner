using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;

namespace GGPhys.Rigid.Collisions
{
    public class OctreeNode
    {
        public Vector3d Center;
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

