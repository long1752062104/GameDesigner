using System.Collections.Generic;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    public class Octree
    {
        public TSVector3 Center;
        public TSVector3 Size;
        public int Level;
        public OctreeNode RootNode;
        public int[,] Mults;
        public DMapList<OctreeNode> ContactNodes;


        public void Init(TSVector3 center, TSVector3 size, int level)
        {
            Center = center;
            Size = size;
            Level = level;
            Mults = new int[,]
            {
                {1,1,1},{-1,1,1},{-1,1,-1},{1,1,-1},
                {1,-1,1},{-1,-1,1},{-1,-1,-1},{1,-1,-1}
            };
            ContactNodes = new DMapList<OctreeNode>();
            RootNode = CreateNode(center, size);
        }


        OctreeNode CreateNode(TSVector3 center, TSVector3 size, int level = 0)
        {
            bool isLeaf = level == Level;
            OctreeNode node = new OctreeNode();
            node.IsLeaf = isLeaf;
            node.Center = center;
            if (isLeaf) return node;
            int nextLevel = level + 1;
            for (int i = 0; i < 8; i++)
            {
                TSVector3 multsVector = new TSVector3(Mults[i, 0], Mults[i, 1], Mults[i, 2]);
                TSVector3 nextCenter = center + multsVector * TSVector3.one * size * 0.25;
                node.ChildNodes[i] = CreateNode(nextCenter, size * 0.5, nextLevel);
            }
            return node;
        }

        public void GeneratePotentialContacts(List<CollisionPrimitive> primitives, CollisionData data)
        {
            for (int i = 0; i < primitives.Count; i++)
            {
                CollisionPrimitive primitive = primitives[i];
                primitive.HashOrder = i;
                AddPrimitive(primitive);
            }

            int size = ContactNodes.Size();
            for (int i = 0; i < size; i++)
            {
                AddPotentialContact(ContactNodes.Pop(), data);
            }
        }

        void AddPrimitive(CollisionPrimitive primitive)
        {
            AddPrimitiveToNode(primitive, RootNode);
        }

        void AddPrimitiveToNode(CollisionPrimitive primitive, OctreeNode node)
        {
            if (node.IsLeaf)
            {
                if (!node.Primitives.Contains(primitive))
                    node.Primitives.Add(primitive);
                if (node.Primitives.Count > 1)
                    ContactNodes.Insert(node);
                return;
            }
            byte nodeIndex = primitive.BoundingVolum.OverlapNodes(node.Center);
            if ((nodeIndex & 0b10000000) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[0]);
            if ((nodeIndex & 0b01000000) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[1]);
            if ((nodeIndex & 0b00100000) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[2]);
            if ((nodeIndex & 0b00010000) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[3]);
            if ((nodeIndex & 0b00001000) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[4]);
            if ((nodeIndex & 0b00000100) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[5]);
            if ((nodeIndex & 0b00000010) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[6]);
            if ((nodeIndex & 0b00000001) != 0) AddPrimitiveToNode(primitive, node.ChildNodes[7]);
        }

        void AddPotentialContact(OctreeNode node, CollisionData data)
        {
            for (int i = 0; i < node.Primitives.Count; i++)
            {
                CollisionPrimitive primitive1 = node.Primitives[i];
                for (int j = i + 1; j < node.Primitives.Count; j++)
                {
                    CollisionPrimitive primitive2 = node.Primitives[j];
                    data.AddPotentialContact(primitive1, primitive2);
                }
            }
            node.Primitives.Clear();
        }
    }
}


