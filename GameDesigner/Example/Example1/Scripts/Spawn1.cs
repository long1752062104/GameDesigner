#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Example
{
    using Net.Component;
    using UnityEngine;

    public class Spawn1 : MonoBehaviour
    {
        public GameObject spawnPrefab;
        public int spawnAmount = 1000;
        public float interleave = 1;

        void Start()
        {
            ClientManager.Instance.client.OnConnectedHandle += OnConnected;
        }

        private void OnConnected()
        {
            TransformComponent.Identity = (ClientManager.UID - 10000) * 5000;//避免唯一标识碰撞
            float sqrt = Mathf.Sqrt(spawnAmount);
            float offset = -sqrt / 2 * interleave;
            int spawned = 0;
            for (int spawnX = 0; spawnX < sqrt; ++spawnX)
            {
                for (int spawnZ = 0; spawnZ < sqrt; ++spawnZ)
                {
                    if (spawned < spawnAmount)
                    {
                        GameObject go = Instantiate(spawnPrefab);
                        float x = offset + spawnX * interleave;
                        float z = offset + spawnZ * interleave;
                        go.transform.position = new Vector3(x, 0, z);
                        ++spawned;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            ClientManager.Instance.client.OnConnectedHandle -= OnConnected;
        }
    }
}
#endif