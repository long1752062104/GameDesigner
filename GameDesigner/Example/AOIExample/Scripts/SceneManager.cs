#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace AOIExample 
{
    using Net.Component;
    using Net.Share;
    using UnityEngine;

    public class SceneManager : Net.Component.SceneManager
    {
        public GameObject player;

        public override void Start()
        {
            FindObjectOfType<ClientManager>().client.AddStateHandler(NetworkState.Connected, Connected);
            base.Start();
        }
        void Connected() 
        {
            NetworkTransformBase.Identity = ClientManager.UID;
            var player1 = Instantiate(player, new Vector3(Random.Range(-20, 20), 1, Random.Range(-20, 20)), Quaternion.identity);
            player1.AddComponent<PlayerControl>();
            player1.name = ClientManager.Identify;
            player1.GetComponent<AOIBody>().IsLocal = true;
            player1.GetComponent<PlayerControl>().moveSpeed = 20f;
            FindObjectOfType<ARPGcamera>().target = player1.transform;
        }
    }
}
#endif