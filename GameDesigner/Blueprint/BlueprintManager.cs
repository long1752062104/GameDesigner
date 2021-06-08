using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 行为基类
    /// </summary>
	public abstract class IBlueprint : MonoBehaviour
    {
        public Blueprint _blueprint = null;
        public Blueprint blueprint
        {
            get
            {
                if (_blueprint == null)
                {
                    _blueprint = new GameObject("临时蓝图").AddComponent<Blueprint>();
                    _blueprint.transform.SetParent(transform);
                }
                return _blueprint;
            }
            set { _blueprint = value; }
        }

        /// <summary>
        /// 编辑器检查更新蓝图编辑器
        /// </summary>
        public virtual void CheckUpdate() { }
    }

    /// <summary>
    /// 蓝图管理器,默认蓝图添加此组件
    /// </summary>
    public class BlueprintManager : IBlueprint
    {
        public override void CheckUpdate()
        {

        }

        // Use this for initialization
        public void Start()
        {
            blueprint.nodes[0].runtime?.Invoke();
        }

        // Update is called once per frame
        public void LateUpdate()
        {
            blueprint.nodes[1].runtime?.Invoke();
        }
    }
}