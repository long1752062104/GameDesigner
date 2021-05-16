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
        //[HideInInspector]
        public BlueprintNode _onStart = null;
        public BlueprintNode onStart
        {
            get
            {
                if (_onStart == null)
                {
                    _onStart = BlueprintNode.CreateFunctionBody(blueprint, typeof(BlueprintManager), "Start", "MonoBehaviour", "当mono开始时调用一次");

                }
                return _onStart;
            }
        }

        //[HideInInspector]
        public BlueprintNode _onLateUpdate = null;
        public BlueprintNode onLateUpdate
        {
            get
            {
                if (_onLateUpdate == null)
                {
                    _onLateUpdate = BlueprintNode.CreateFunctionBody(blueprint, typeof(BlueprintManager), "LateUpdate", "MonoBehaviour", "当mono每一帧执行调用");
                }
                return _onLateUpdate;
            }
        }

        public override void CheckUpdate()
        {
            _onStart = onStart;
            _onLateUpdate = onLateUpdate;
        }

        // Use this for initialization
        public void Start()
        {
            if (onStart.runtime)
            {
                onStart.runtime.Invoke();
            }
        }

        // Update is called once per frame
        public void LateUpdate()
        {
            if (onLateUpdate.runtime)
            {
                onLateUpdate.runtime.Invoke();
            }
        }
    }
}