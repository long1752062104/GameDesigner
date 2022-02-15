﻿using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    public class Octree
    {
        public Vector3d Center;
        public Vector3d Size;
        public int Level;
        public OctreeNode RootNode;
        public int[,] Mults;
        public DMapList<OctreeNode> ContactNodes;


        public void Init(Vector3d center, Vector3d size, int level)
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


        OctreeNode CreateNode(Vector3d center, Vector3d size, int level = 0)
        {
            var isLeaf = level == Level;
            var node = new OctreeNode();
            node.IsLeaf = isLeaf;
            node.Center = center;
            if (isLeaf) return node;
            var nextLevel = level + 1;
            for (int i = 0; i < 8; i++)
            {
                var multsVector = new Vector3d(Mults[i, 0], Mults[i, 1], Mults[i, 2]);
                var nextCenter = center + multsVector * Vector3d.One * size * 0.25;
                node.ChildNodes[i] = CreateNode(nextCenter, size * 0.5, nextLevel);
            }
            return node;
        }

        public void GeneratePotentialContacts(List<CollisionPrimitive> primitives, CollisionData data)
        {
            for (int i = 0; i < primitives.Count; i++)
            {
                var primitive = primitives[i];
                primitive.HashOrder = i;
                AddPrimitive(primitive);
            }

            var size = ContactNodes.Size();
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
                if(!node.Primitives.Contains(primitive))
                    node.Primitives.Add(primitive);
                if(node.Primitives.Count > 1)
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
                var primitive1 = node.Primitives[i];
                for (int j = i + 1; j < node.Primitives.Count; j++)
                {
                    var primitive2 = node.Primitives[j];
                    data.AddPotentialContact(primitive1, primitive2);
                }
            }
            node.Primitives.Clear();
        }
    }
}


