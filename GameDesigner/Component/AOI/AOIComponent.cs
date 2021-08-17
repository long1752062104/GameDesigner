#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;
using Net.AOI;
using Grid = Net.AOI.Grid;

namespace Net.Component
{
    public class AOIComponent : SingleCase<AOIComponent>
    {
        public GridManager gridManager = new GridManager();
        public float xPos = -100f;
        public float zPos = -100f;
        public uint xMax = 100;
        public uint zMax = 100;
        public int width = 20;
        public int height = 20;
        public bool EditInit;

        private void Awake()
        {
            gridManager.Init(xPos, zPos, xMax, zMax, width, height);
        }

        private void Update()
        {
            gridManager.UpdateHandler();
        }

        private void OnDrawGizmos()
        {
            if (EditInit)
            {
                EditInit = false;
                gridManager.Init(xPos, zPos, xMax, zMax, width, height);
            }
            Gizmos.color = Color.cyan;
            for (int i = 0; i < gridManager.grids.Count; i++)
            {
                Draw(gridManager.grids[i]);
            }
        }

        private void Draw(Grid grid)
        {
            var pos = grid.rect.center;
            var size = grid.rect.size;
            Gizmos.DrawWireCube(new UnityEngine.Vector3(pos.x, 0, pos.y), new UnityEngine.Vector3(size.x, 0, size.y));
        }
    }
}
#endif