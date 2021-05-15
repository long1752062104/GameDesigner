using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 蓝图功能块节点
    /// </summary>
    [System.Serializable]
    public class FunBlockNode : MonoBehaviour
    {
        public string tooltip = "请编写你的功能块说明!";
        public Rect rect = new Rect(0, 0, 200, 200);
        public List<BlueprintNode> nodes = new List<BlueprintNode>();
        public List<FunBlockNode> funNodes = new List<FunBlockNode>();

        static public FunBlockNode CreateFunctionalBlockNodeInstance(Blueprint designer, Rect rect)
        {
            FunBlockNode state = new GameObject("FunBlockNode" + designer.functionalBlocks.Count).AddComponent<FunBlockNode>();
            state.transform.SetParent(designer.transform);
            state.rect = rect;
            state.nodes.AddRange(designer.selectNodes);
            foreach (FunBlockNode n in designer.functionalBlocks)
            {
                if (n.rect.Contains(state.rect.position))
                {
                    n.funNodes.Add(state);
                }
            }
            state.funNodes.Add(state);
            return state;
        }
    }
}