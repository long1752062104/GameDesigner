using System;
using System.Collections.Generic;

namespace Net.AOI
{
    /// <summary>
    /// 九宫格网络同步管理器
    /// </summary>
    [Serializable]
    public class GridManager
    {
        public List<Grid> grids = new List<Grid>();
        public Rect worldSize;
        private readonly List<GridData> handlerList = new List<GridData>();
        /// <summary>
        /// 初始化九宫格
        /// </summary>
        /// <param name="xPos">x开始位置</param>
        /// <param name="zPos">z开始位置</param>
        /// <param name="xMax">x列最大值</param>
        /// <param name="zMax">z列最大值</param>
        /// <param name="width">格子长度</param>
        /// <param name="height">格子高度</param>
        public void Init(float xPos, float zPos, uint xMax, uint zMax, int width, int height)
        {
            grids.Clear();
            worldSize = new Rect(xPos, zPos, width * xMax, height * zMax);
            for (int z = 0; z < zMax; z++)
            {
                float xPos1 = xPos;
                for (int x = 0; x < xMax; x++)
                {
                    var rect = new Rect(xPos1, zPos, width, height);
                    grids.Add(new Grid() { rect = rect });
                    xPos1 += width;
                }
                zPos += height;
            }
            foreach (var item in grids)
            {
                var rect = new Rect(item.rect.x - width, item.rect.y - height, width * 3, height * 3);
                foreach (var item1 in grids)
                {
                    if (rect.Contains(item1.rect.position))
                    {
                        item.grids.Add(item1);
                    }
                }
            }
        }
        /// <summary>
        /// 插入物体到九宫格感兴趣区域
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Insert(IGridBody body)
        {
            foreach (var item in grids)
            {
                if (item.rect.ContainsXZ(body.Position))
                {
                    item.gridBodies.Add(body);
                    body.Grid = item;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取物体的感兴趣九宫格区域
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public Grid TryGetGrid(IGridBody body)
        {
        JMP: if (body.Grid == null)
            {
                if (!worldSize.ContainsXZ(body.Position))
                    goto J;
                foreach(var item in grids)
                {
                    if (item.rect.ContainsXZ(body.Position))
                    {
                        item.gridBodies.Add(body);
                        body.Grid = item;
                        return body.Grid;
                    }
                }
                goto J;
            }
            if (body.Grid.rect.ContainsXZ(body.Position))
                return body.Grid;
            var grids1 = body.Grid.grids;
            for (int i = 0; i < grids1.Count; i++)
            {
                if (grids1[i].rect.ContainsXZ(body.Position))
                {
                    foreach (var item in grids1)
                    {
                        if (!grids1[i].grids.Contains(item))
                        {
                            foreach (var item2 in item.gridBodies)
                            {
                                item2.OnExit(body);
                                body.OnExit(item2);
                            }
                        }
                    }
                    foreach (var item in grids1[i].grids)
                    {
                        if (!grids1.Contains(item))
                        {
                            foreach (var item2 in item.gridBodies)
                            {
                                item2.OnEnter(body);
                                body.OnEnter(item2);
                            }
                        }
                    }
                    //body.Grid.gridBodies.Remove(body);
                    //body.Grid = grids1[i];
                    //body.Grid.gridBodies.Add(body);
                    handlerList.Add(new GridData() { oldGrid = body.Grid, newGrid = grids1[i], body = body });
                    return body.Grid;
                }
            }
            body.Grid = null;
            goto JMP;
#if UNITY_EDITOR
        J: UnityEngine.Debug.Log($"{body.ID}越界了,位置:{body.Position}");
#else
        J: Event.NDebug.LogError($"{body.ID}越界了,位置:{body.Position}");
#endif
            return null;
        }
        /// <summary>
        /// 移除感兴趣物体
        /// </summary>
        /// <param name="body"></param>
        public void Remove(IGridBody body)
        {
            if (body.Grid == null)
                return;
            body.Grid.gridBodies.Remove(body);
        }
        /// <summary>
        /// 更新感兴趣的移除和添加物体
        /// </summary>
        public void UpdateHandler()
        {
            foreach (var item in handlerList) 
            {
                item.oldGrid.gridBodies.Remove(item.body);
                item.newGrid.gridBodies.Add(item.body);
                item.body.Grid = item.newGrid;
            }
            handlerList.Clear();
        }
    }
}
