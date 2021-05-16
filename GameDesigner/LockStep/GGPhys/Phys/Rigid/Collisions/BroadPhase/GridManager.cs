using FixMath;
using System;
using System.Collections.Generic;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 网格管理类
    /// </summary>
    public class GridManager
    {
        public int SizeX; // x尺寸
        public int SizeY; // y尺寸
        public int SizeZ; // z尺寸
        public int GridSizeGroupScale; //三层级格子大小比例
        public Grid[,,] Grids; // 格子数组
        public Grid[,,] MidGrids; // 中等格子数组
        public Grid[,,] LargeGrids; // 大格子数组
        public TSVector3 GridSize; // 每个格子尺寸
        public TSVector3 StartPosition; // 整体网格起始位置
        public DMapList<Grid> ContactGrids; // 潜在碰撞格子
        public DMapList<Grid> MidContactGrids; // 潜在mid碰撞格子
        public DMapList<Grid> LargeContactGrids; // 潜在large碰撞格子
        public int StaticHashOrder = -1; // 静态几何体编码序号

        private TSVector3 MidGridSize;
        private TSVector3 LargeGridSize;

        private int LargeSizeX;
        private int LargeSizeY;
        private int LargeSizeZ;
        private int MidSizeX;
        private int MidSizeY;
        private int MidSizeZ;


        public GridManager(int sizeX, int sizeY, int sizeZ, TSVector3 gridSize, TSVector3 centerOffset, int groupScale)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
            GridSize = gridSize;
            GridSizeGroupScale = groupScale;
            MidGridSize = gridSize * GridSizeGroupScale;
            LargeGridSize = MidGridSize * GridSizeGroupScale;
            StartPosition = new TSVector3(-sizeX * 0.5f, -sizeY * 0.5f, -sizeZ * 0.5f).Multiply(gridSize) + centerOffset;
            ContactGrids = new DMapList<Grid>();
            MidContactGrids = new DMapList<Grid>();
            LargeContactGrids = new DMapList<Grid>();
            Grid.PrimitiveNodePool = new ClassObjectPool<LinkedNode<CollisionPrimitive>>(1000);
        }

        ~GridManager()
        {
            Grid.PrimitiveNodePool.Destroy();
        }

        /// <summary>
        /// 格子初始化
        /// </summary>
        public void InitGrids()
        {
            int largeSizeScale = GridSizeGroupScale * GridSizeGroupScale;

            if (SizeX % largeSizeScale != 0)
            {
                throw new NotSupportedException("SizeX Must be a multiple of 16!");
            }

            if (SizeY % largeSizeScale != 0)
            {
                throw new NotSupportedException("SizeY Must be a multiple of 16!");
            }

            if (SizeZ % largeSizeScale != 0)
            {
                throw new NotSupportedException("SizeZ Must be a multiple of 16!");
            }

            LargeSizeX = SizeX / largeSizeScale;
            LargeSizeY = SizeY / largeSizeScale;
            LargeSizeZ = SizeZ / largeSizeScale;
            MidSizeX = LargeSizeX * GridSizeGroupScale;
            MidSizeY = LargeSizeY * GridSizeGroupScale;
            MidSizeZ = LargeSizeZ * GridSizeGroupScale;

            Grids = new Grid[SizeX, SizeY, SizeZ];
            LargeGrids = new Grid[LargeSizeX, LargeSizeY, LargeSizeZ];
            MidGrids = new Grid[MidSizeX, MidSizeY, MidSizeZ];


            for (int i = 0; i < LargeSizeX; i++)
            {
                for (int j = 0; j < LargeSizeY; j++)
                {
                    for (int k = 0; k < LargeSizeZ; k++)
                    {
                        Grid grid = new Grid();
                        LargeGrids[i, j, k] = grid;
                        CreateSubGrids(i * GridSizeGroupScale, j * GridSizeGroupScale, k * GridSizeGroupScale, 1, grid);
                    }
                }
            }
        }

        /// <summary>
        /// 创建子格子
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="startZ"></param>
        /// <param name="level">当前所属层级</param>
        /// <param name="parent"></param>
        void CreateSubGrids(int startX, int startY, int startZ, int level, Grid parent)
        {
            int endX = startX + GridSizeGroupScale;
            int endY = startY + GridSizeGroupScale;
            int endZ = startZ + GridSizeGroupScale;
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    for (int k = startZ; k < endZ; k++)
                    {
                        Grid grid = new Grid();
                        grid.ParentGrid = parent;
                        if (level == 1)
                        {
                            MidGrids[i, j, k] = grid;
                            CreateSubGrids(i * GridSizeGroupScale, j * GridSizeGroupScale, k * GridSizeGroupScale, 2, grid);
                        }
                        if (level == 2)
                        {
                            Grids[i, j, k] = grid;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成全部潜在碰撞
        /// </summary>
        /// <param name="primitives"></param>
        /// <param name="data"></param>
        public void GeneratePotentialContacts(CollisionData data)
        {
            int size = ContactGrids.Size();
            for (int i = 0; i < size; i++)
            {
                AddPotentialContact(ContactGrids.Pop(), data);
            }

            int midSize = MidContactGrids.Size();
            for (int i = 0; i < midSize; i++)
            {
                AddPotentialContact(MidContactGrids.Pop(), data);
            }

            int largeSize = LargeContactGrids.Size();
            for (int i = 0; i < largeSize; i++)
            {
                AddPotentialContact(LargeContactGrids.Pop(), data);
            }
        }

        /// <summary>
        /// 在一个格子中生成潜在碰撞
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="data"></param>
        void AddPotentialContact(Grid grid, CollisionData data)
        {
            while (grid.HeadPrimitive != null)
            {
                LinkedNode<CollisionPrimitive> node1 = grid.HeadPrimitive;
                LinkedNode<CollisionPrimitive> node2 = grid.HeadPrimitive.next;
                LinkedNode<CollisionPrimitive> node3 = grid.HeadStaticPrimitive;
                LinkedNode<CollisionPrimitive> node4 = grid.ParentGrid?.HeadPrimitive;
                LinkedNode<CollisionPrimitive> node5 = (grid.ParentGrid != null && grid.ParentGrid.ParentGrid != null) ? grid.ParentGrid.ParentGrid.HeadPrimitive : null;
                while (node2 != null)
                {
                    data.AddPotentialContact(node1.obj, node2.obj);
                    node2 = node2.next;
                }
                while (node3 != null)
                {
                    data.AddPotentialContact(node1.obj, node3.obj);
                    node3 = node3.next;
                }
                while (node4 != null)
                {
                    data.AddPotentialContact(node1.obj, node4.obj);
                    node4 = node4.next;
                }
                while (node5 != null)
                {
                    data.AddPotentialContact(node1.obj, node5.obj);
                    node5 = node5.next;
                }
                grid.HeadPrimitive = node1.next;
                grid.ClearNode(node1);
            }
        }

        /// <summary>
        /// 把所有几何体加入网格
        /// </summary>
        public void AddPrimitives(List<CollisionPrimitive> primitives)
        {
            for (int i = 0; i < primitives.Count; i++)
            {
                CollisionPrimitive primitive = primitives[i];
                primitive.HashOrder = i;
                AddPrimitive(primitive);
            }
        }

        /// <summary>
        /// 把几何体加入网格
        /// </summary>
        /// <param name="primitive"></param>
        public void AddPrimitive(CollisionPrimitive primitive)
        {
            AddPrimitive(primitive, primitive.BoundingVolum.sizeLevel);
        }

        /// <summary>
        /// 把几何体按层级加入网格
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="level"></param>
        void AddPrimitive(CollisionPrimitive primitive, int level)
        {
            if (level == 0)
            {
                int startX = GetIndexX(primitive.BoundingVolum.minX, LargeGridSize);
                int endX = GetIndexX(primitive.BoundingVolum.maxX, LargeGridSize);
                int startY = GetIndexY(primitive.BoundingVolum.minY, LargeGridSize);
                int endY = GetIndexY(primitive.BoundingVolum.maxY, LargeGridSize);
                int startZ = GetIndexZ(primitive.BoundingVolum.minZ, LargeGridSize);
                int endZ = GetIndexZ(primitive.BoundingVolum.maxZ, LargeGridSize);
                if (startX < 0 || startY < 0 || startZ < 0 || endX >= LargeSizeX || endY >= LargeSizeY || endZ >= LargeSizeZ) return;
                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        for (int k = startZ; k <= endZ; k++)
                        {
                            Grid grid = LargeGrids[i, j, k];
                            grid.AddPrimitive(primitive);
                            LargeContactGrids.Insert(grid);
                        }
                    }
                }
            }
            if (level == 1)
            {
                int startX = GetIndexX(primitive.BoundingVolum.minX, MidGridSize);
                int endX = GetIndexX(primitive.BoundingVolum.maxX, MidGridSize);
                int startY = GetIndexY(primitive.BoundingVolum.minY, MidGridSize);
                int endY = GetIndexY(primitive.BoundingVolum.maxY, MidGridSize);
                int startZ = GetIndexZ(primitive.BoundingVolum.minZ, MidGridSize);
                int endZ = GetIndexZ(primitive.BoundingVolum.maxZ, MidGridSize);
                if (startX < 0 || startY < 0 || startZ < 0 || endX >= MidSizeX || endY >= MidSizeY || endZ >= MidSizeZ) return;
                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        for (int k = startZ; k <= endZ; k++)
                        {
                            Grid grid = MidGrids[i, j, k];
                            grid.AddPrimitive(primitive);
                            MidContactGrids.Insert(grid);
                        }
                    }
                }
            }
            if (level == 2)
            {
                int startX = GetIndexX(primitive.BoundingVolum.minX, GridSize);
                int endX = GetIndexX(primitive.BoundingVolum.maxX, GridSize);
                int startY = GetIndexY(primitive.BoundingVolum.minY, GridSize);
                int endY = GetIndexY(primitive.BoundingVolum.maxY, GridSize);
                int startZ = GetIndexZ(primitive.BoundingVolum.minZ, GridSize);
                int endZ = GetIndexZ(primitive.BoundingVolum.maxZ, GridSize);
                if (startX < 0 || startY < 0 || startZ < 0 || endX >= SizeX || endY >= SizeY || endZ >= SizeZ) return;
                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        for (int k = startZ; k <= endZ; k++)
                        {
                            Grid grid = Grids[i, j, k];
                            grid.AddPrimitive(primitive); // 60k GC
                            ContactGrids.Insert(grid);// 30k GC
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 把静态几何体加入网格
        /// </summary>
        /// <param name="primitive"></param>
        public void AddStaticPrimitive(CollisionPrimitive primitive)
        {
            primitive.HashOrder = StaticHashOrder;
            StaticHashOrder -= 1;
            int startX = GetIndexX(primitive.BoundingVolum.minX, GridSize);
            int endX = GetIndexX(primitive.BoundingVolum.maxX, GridSize);
            int startY = GetIndexY(primitive.BoundingVolum.minY, GridSize);
            int endY = GetIndexY(primitive.BoundingVolum.maxY, GridSize);
            int startZ = GetIndexZ(primitive.BoundingVolum.minZ, GridSize);
            int endZ = GetIndexZ(primitive.BoundingVolum.maxZ, GridSize);
            if (startX < 0 || startY < 0 || startZ < 0 || endX >= SizeX || endY >= SizeY || endZ >= SizeZ) return;
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    for (int k = startZ; k <= endZ; k++)
                    {
                        Grid grid = Grids[i, j, k];
                        grid.AddStaticPrimitive(primitive);
                    }
                }
            }

            startX = GetIndexX(primitive.BoundingVolum.minX, MidGridSize);
            endX = GetIndexX(primitive.BoundingVolum.maxX, MidGridSize);
            startY = GetIndexY(primitive.BoundingVolum.minY, MidGridSize);
            endY = GetIndexY(primitive.BoundingVolum.maxY, MidGridSize);
            startZ = GetIndexZ(primitive.BoundingVolum.minZ, MidGridSize);
            endZ = GetIndexZ(primitive.BoundingVolum.maxZ, MidGridSize);
            if (startX < 0 || startY < 0 || startZ < 0 || endX >= MidSizeX || endY >= MidSizeY || endZ >= MidSizeZ) return;
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    for (int k = startZ; k <= endZ; k++)
                    {
                        Grid grid = MidGrids[i, j, k];
                        grid.AddStaticPrimitive(primitive);
                    }
                }
            }

            startX = GetIndexX(primitive.BoundingVolum.minX, LargeGridSize);
            endX = GetIndexX(primitive.BoundingVolum.maxX, LargeGridSize);
            startY = GetIndexY(primitive.BoundingVolum.minY, LargeGridSize);
            endY = GetIndexY(primitive.BoundingVolum.maxY, LargeGridSize);
            startZ = GetIndexZ(primitive.BoundingVolum.minZ, LargeGridSize);
            endZ = GetIndexZ(primitive.BoundingVolum.maxZ, LargeGridSize);
            if (startX < 0 || startY < 0 || startZ < 0 || endX >= LargeSizeX || endY >= LargeSizeY || endZ >= LargeSizeZ) return;
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    for (int k = startZ; k <= endZ; k++)
                    {
                        Grid grid = LargeGrids[i, j, k];
                        grid.AddStaticPrimitive(primitive);
                    }
                }
            }
        }

        /// <summary>
        /// 把静态几何体移除网格
        /// </summary>
        /// <param name="primitive"></param>
        public void RemoveStaticPrimitive(CollisionPrimitive primitive)
        {
            int startX = GetIndexX(primitive.BoundingVolum.minX, GridSize);
            int endX = GetIndexX(primitive.BoundingVolum.maxX, GridSize);
            int startY = GetIndexY(primitive.BoundingVolum.minY, GridSize);
            int endY = GetIndexY(primitive.BoundingVolum.maxY, GridSize);
            int startZ = GetIndexZ(primitive.BoundingVolum.minZ, GridSize);
            int endZ = GetIndexZ(primitive.BoundingVolum.maxZ, GridSize);
            if (startX < 0 || startY < 0 || startZ < 0 || endX >= SizeX || endY >= SizeY || endZ >= SizeZ) return;
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    for (int k = startZ; k <= endZ; k++)
                    {
                        Grid grid = Grids[i, j, k];
                        grid.RemoveStaticPrimitive(primitive);
                    }
                }
            }

            startX = GetIndexX(primitive.BoundingVolum.minX, MidGridSize);
            endX = GetIndexX(primitive.BoundingVolum.maxX, MidGridSize);
            startY = GetIndexY(primitive.BoundingVolum.minY, MidGridSize);
            endY = GetIndexY(primitive.BoundingVolum.maxY, MidGridSize);
            startZ = GetIndexZ(primitive.BoundingVolum.minZ, MidGridSize);
            endZ = GetIndexZ(primitive.BoundingVolum.maxZ, MidGridSize);
            if (startX < 0 || startY < 0 || startZ < 0 || endX >= MidSizeX || endY >= MidSizeY || endZ >= MidSizeZ) return;
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    for (int k = startZ; k <= endZ; k++)
                    {
                        Grid grid = MidGrids[i, j, k];
                        grid.RemoveStaticPrimitive(primitive);
                    }
                }
            }

            startX = GetIndexX(primitive.BoundingVolum.minX, LargeGridSize);
            endX = GetIndexX(primitive.BoundingVolum.maxX, LargeGridSize);
            startY = GetIndexY(primitive.BoundingVolum.minY, LargeGridSize);
            endY = GetIndexY(primitive.BoundingVolum.maxY, LargeGridSize);
            startZ = GetIndexZ(primitive.BoundingVolum.minZ, LargeGridSize);
            endZ = GetIndexZ(primitive.BoundingVolum.maxZ, LargeGridSize);
            if (startX < 0 || startY < 0 || startZ < 0 || endX >= LargeSizeX || endY >= LargeSizeY || endZ >= LargeSizeZ) return;
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    for (int k = startZ; k <= endZ; k++)
                    {
                        Grid grid = LargeGrids[i, j, k];
                        grid.RemoveStaticPrimitive(primitive);
                    }
                }
            }
        }

        /// <summary>
        /// 沿射线方向延申，获取射线当前所在格子
        /// </summary>
        /// <param name="contactBody">第一个碰撞到的刚体</param>
        /// <param name="contactPoint">碰撞点</param>
        /// <returns></returns>
        public Grid NextRayGrid(CollisionRay ray)
        {
            return null;
        }

        /// <summary>
        /// 根据位置获取x方向序号
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        int GetIndexX(FP x, TSVector3 gridSize)
        {
            return F64.FloorToInt((x - StartPosition.x) / gridSize.x);
        }

        /// <summary>
        /// 根据位置获取y方向序号
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        int GetIndexY(FP y, TSVector3 gridSize)
        {
            return F64.FloorToInt((y - StartPosition.y) / gridSize.y);
        }

        /// <summary>
        /// 根据位置获取z方向序号
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        int GetIndexZ(FP z, TSVector3 gridSize)
        {
            return F64.FloorToInt((z - StartPosition.z) / gridSize.z);
        }
    }
}

