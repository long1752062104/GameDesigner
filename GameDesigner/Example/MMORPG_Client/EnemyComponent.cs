#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using Net.Component;
    using Net.Component.Client;
    using Net.Share;
    using UnityEngine;

    public class EnemyComponent : Actor
    {
        public float walkSpeed = 3f;
        private float rotoTime;
        public Actor player;
        public float pursuit = 10f;
        public float attackR = 2f;

        private void Update()
        {
            if (ClientManager.Instance.control)
                ClientManager.AddOperation(new Operation(Command.EnemySync, name, transform.position, transform.rotation) { health = hp });
            else
            {
                transform.rotation = rotation;
                transform.position = Vector3.Lerp(transform.position, position, 0.5f);
            }
        }

        public override void OnHit(Actor player)
        {
            if (this.player != null)
                return;
            this.player = player;
        }

        public override void OnDead()
        {
            isDead = true;
            stateManager.StatusEntry(deadID);
        }
    }
}
#endif