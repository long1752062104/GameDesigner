#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using Net.Component.Client;
    using UnityEngine;

    public class SkillCollider : MonoBehaviour
    {
        public Actor player;
        public float damage = 10;

        private void Start()
        {
            Destroy(gameObject, 0.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!ClientManager.Instance.control)
                return;
            var player1 = other.GetComponent<Actor>();
            if (player == player1 | player1 == null)
                return;
            player1.hp -= damage;
            player1.OnHit(player);
        }
    }

}
#endif