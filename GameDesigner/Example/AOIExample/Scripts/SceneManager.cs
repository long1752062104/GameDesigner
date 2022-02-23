#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace AOIExample 
{
    using Net.Component;
    using Net.Share;
    using Net.UnityComponent;
    using UnityEngine;

    public class SceneManager : NetworkSceneManager
    {
        public GameObject player;

        public void Start()
        {
            FindObjectOfType<ClientManager>().client.AddStateHandler(NetworkState.Connected, Connected);
        }
        void Connected() 
        {
            var player1 = Instantiate(player, new Vector3(Random.Range(-20, 20), 1, Random.Range(-20, 20)), Quaternion.identity);
            player1.AddComponent<PlayerControl>();
            player1.name = ClientManager.Identify;
            player1.GetComponent<AOIBody>().IsLocal = true;
            player1.GetComponent<PlayerControl>().moveSpeed = 20f;
            FindObjectOfType<ARPGcamera>().target = player1.transform;
        }
        public override void OnNetworkObjectCreate(Operation opt, NetworkObject identity)
        {
            var rigidbody = identity.GetComponent<Rigidbody>();
            Destroy(rigidbody);
        }
    }
}
#endif