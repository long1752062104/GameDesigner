using System;

namespace ECS
{
    public class HotfixComponent : Component, IUpdate
    {
        public object[] values;
        public Action OnAwake, OnUpdate, OnDestroyEvt;

        public override void Awake()
        {
            OnAwake?.Invoke();
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        public override void OnDestroy()
        {
            OnDestroyEvt?.Invoke();
            OnAwake = null;
            OnUpdate = null;
            OnDestroyEvt = null;
        }
    }
}
