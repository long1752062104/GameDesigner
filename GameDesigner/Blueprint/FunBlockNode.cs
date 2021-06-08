using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 蓝图功能块节点
    /// </summary>
    [System.Serializable]
    public class FunBlockNode
    {
        public string tooltip = "请编写你的功能块说明!";
        public Rect rect = new Rect(0, 0, 200, 200);
        public List<int> nodes = new List<int>();
        //public List<FunBlockNode> funNodes = new List<FunBlockNode>();
        public Blueprint blueprint;

        static public FunBlockNode CreateFunctionalBlockNodeInstance(Blueprint designer, Rect rect)
        {
            FunBlockNode state = new FunBlockNode();
            state.rect = rect;
            state.blueprint = designer;
            foreach (var node in designer.selectNodes)
            {
                state.nodes.Add(node.ID);
            }
            return state;
        }
    }
}