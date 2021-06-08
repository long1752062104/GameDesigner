using UnityEngine;

namespace GameDesigner
{
    public class NodeRun : MonoBehaviour
    {
        public Node node;

        // Update is called once per frame
        void Update()
        {
            node.Invoke();
        }
    }
}