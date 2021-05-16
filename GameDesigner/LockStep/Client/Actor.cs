#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
using Net.Share;
using System;
using System.Collections.Generic;
using System.Reflection;
using TrueSync;

namespace LockStep.Client
{
    [Serializable]
    public class Actor
    {
        public string name;
        public UnityEngine.GameObject gameObject;
        internal GameSystem gameSystem;
        public TSTransform transform;
        private List<Component> components = new List<Component>();

        private bool _enabled = true;
        public bool enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (_enabled)
                {
                    OnEnable();
                    if (!gameSystem.updates.Contains(this))
                        gameSystem.updates.Add(this);
                }
                else
                {
                    OnDisable();
                    gameSystem.updates.Remove(this);
                }
            }
        }

        public Actor(GameSystem gameSystem)
        {
            Awake();
            if (enabled) OnEnable();
            this.gameSystem = gameSystem;
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "Start")
                    gameSystem.starts.Add(this);
                if (method.Name == "Update")
                    gameSystem.updates.Add(this);
                if (method.Name == "FixedUpdate")
                    gameSystem.fixedUpdates.Add(this);
                if (method.Name == "LateUpdate")
                    gameSystem.lateUpdates.Add(this);
                if (method.Name == "OnGUI")
                    gameSystem.guis.Add(this);
            }
        }

        public static void Destroy(Actor actor)
        {
            actor.Destroy();
        }

        public void Destroy()
        {
            if (gameSystem.destroys.Contains(this))
                return;
            gameSystem.destroys.Add(this);
        }

        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    return components[i] as T;
            }
            return null;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T t = new T();
            components.Add(t);
            return t;
        }

        public virtual void Awake() { }
        public virtual void OnEnable() { }
        public virtual void Start() { }
        public virtual void FixedUpdate() { }
        public virtual void Update(Operation opt) { }
        public virtual void LateUpdate() { }
        public virtual void OnGUI() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }
}
#endif