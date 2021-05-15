#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using GameDesigner;
    using UnityEngine;

    public class SkillEffectAction : ActionBehaviour
    {
        public Actor player;

        public override void OnInstantiateSpwan(StateAction action, GameObject spwan)
        {
            var sc = spwan.AddComponent<SkillCollider>();
            var sc1 = spwan.AddComponent<SphereCollider>();
            sc.player = player;
            sc1.radius = 2;
            sc1.isTrigger = true;
        }
    }
}
#endif