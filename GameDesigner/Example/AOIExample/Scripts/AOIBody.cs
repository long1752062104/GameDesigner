#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace AOIExample
{
    using Net.AOI;
    using Net.Component;
    using UnityEngine;
    using Grid = Net.AOI.Grid;

    public class AOIBody : MonoBehaviour, IGridBody
    {
        public int ID { get; set; }
        public Net.Vector3 Position { get; set; }
        public Grid Grid { get; set; }

        public bool IsLocal;

        // Start is called before the first frame update
        void Start()
        {
            Position = transform.position;
            AOIComponent.I.gridManager.Insert(this);
        }

        // Update is called once per frame
        void Update()
        {
            Position = transform.position;
            AOIComponent.I.gridManager.TryGetGrid(this);
        }

        void OnDrawGizmos() 
        {
            if (!IsLocal)
                return;
            if (Grid == null)
                return; 
            Gizmos.color = Color.green;
            for (int i = 0; i < Grid.grids.Count; i++)
            {
                Draw(Grid.grids[i]);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (IsLocal)
                return;
            if (!Application.isPlaying)
                return;
            if (AOIComponent.I.gridManager == null)
                return;
            var grid = AOIComponent.I.gridManager.TryGetGrid(this);
            if (grid == null)
                return;
            Gizmos.color = Color.green;
            for (int i = 0; i < grid.grids.Count; i++)
            {
                Draw(grid.grids[i]);
            } 
        }

        private void Draw(Grid grid)
        {
            var pos = grid.rect.center;
            var size = grid.rect.size;
            Gizmos.DrawCube(new Vector3(pos.x, 0, pos.y), new Vector3(size.x, 0.1f, size.y));
        }

        public void OnEnter(IGridBody body)
        {
            if (IsLocal)
                return;
            GetComponent<MeshRenderer>().enabled = true;
        }

        public void OnExit(IGridBody body)
        {
            if (IsLocal)
                return;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
#endif