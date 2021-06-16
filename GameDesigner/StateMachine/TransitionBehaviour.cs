namespace GameDesigner
{
    using System;

    /// <summary>
    /// 连接行为--用户可以继承此类添加组件 2017年12月6日(星期三)
    /// </summary>
    [Serializable]
    public class TransitionBehaviour : IBehaviour
    {
        public int transitionID;
        public Transition transition => state.transitions[transitionID];
        public virtual void OnUpdate(ref bool isEnterNextState) { }
        public TransitionBehaviour InitBehaviour()
        {
            var type = SystemType.GetType(name);
            var runtimeBehaviour = (TransitionBehaviour)Activator.CreateInstance(type);
            runtimeBehaviour.stateMachine = stateMachine;
            runtimeBehaviour.Active = Active;
            runtimeBehaviour.ID = ID;
            runtimeBehaviour.name = name;
            runtimeBehaviour.metadatas = metadatas;
            foreach (var metadata in metadatas)
            {
                var field = type.GetField(metadata.name);
                if (field == null)
                    continue;
                metadata.field = field;
                metadata.target = runtimeBehaviour;
                field.SetValue(runtimeBehaviour, metadata.Read());
            }
            return runtimeBehaviour;
        }
    }
}