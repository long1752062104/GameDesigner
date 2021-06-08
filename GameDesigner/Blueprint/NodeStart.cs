using UnityEngine;

namespace GameDesigner
{
    public class NodeStart : MonoBehaviour
    {
        public Node node;

        void Start()
        {
            node.Invoke();
        }
    }
}
