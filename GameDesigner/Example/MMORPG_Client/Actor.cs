#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using GameDesigner;
    using UnityEngine;

    public abstract class Actor : MonoBehaviour
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 direction;
        public float moveSpeed = 5f;
        public GameObject skillObj;
        public float hp = 100;
        public bool isDead;
        public int deadID = 7;
        public StateManager stateManager;

        public virtual void OnDead()
        {
            isDead = true;
            stateManager.StatusEntry(deadID);
        }

        public virtual void OnHit(Actor player)
        {
        }
    }
}
#endif