namespace GameDesigner
{
    using System;
    using UnityEngine;

    /// <summary>
    /// 连接行为--用户可以继承此类添加组件 2017年12月6日(星期三)
    /// </summary>
    [Serializable]
    public class TransitionBehaviour : IBehaviour
    {
        public Transform transform
        {
            get
            {
                return stateManager.transform;
            }
        }

        public int transitionID;
        public Transition transition
        {
            get
            {
                return state.transitions[transitionID];
            }
        }

        public virtual void OnUpdate(ref bool isEnterNextState) { }

        private TransitionBehaviour runtimeBehaviour;
        public TransitionBehaviour RuntimeBehaviour
        {
            get
            {
                if (runtimeBehaviour == null)
                {
                    var type = SystemType.GetType(name);
                    runtimeBehaviour = (TransitionBehaviour)Activator.CreateInstance(type);
                    runtimeBehaviour.stateMachine = stateMachine;
                    runtimeBehaviour.Active = Active;
                    runtimeBehaviour.ID = ID;
                    runtimeBehaviour.name = name;
                    foreach (var metadata in metadatas)
                    {
                        var field = type.GetField(metadata.name);
                        metadata.field = field;
                        metadata.target = runtimeBehaviour; 
                        field.SetValue(runtimeBehaviour, metadata.Read());
                    }
                }
                return runtimeBehaviour;
            }
        }
    }
}