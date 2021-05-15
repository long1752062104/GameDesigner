#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.Client
{
    using Net.Component.MMORPG_Client;
    using Net.Share;
    using UnityEngine;

    public class TransformComponent : MonoBehaviour
    {
        internal static int Identity = 0;
        private Net.Vector3 position;
        private Net.Quaternion rotation;
        private Net.Vector3 localScale;
        [DisplayOnly]
        public int identity = -1;//自身id
        private float sendTime;

        private void Awake()
        {
            if (!ClientManager.Instance.control)
                return;
            var sm = FindObjectOfType<SceneManager>();
            if (sm == null)
            {
                Debug.Log("没有找到SceneManager组件！TransformComponent组件无效！");
                Destroy(gameObject);
                return;
            }
            JUMP: identity = Identity++;
            if (sm.transforms.ContainsKey(identity))
                goto JUMP;
            sm.transforms.Add(identity, this);
        }

        // Update is called once per frame
        void Update()
        {
            if (!ClientManager.Instance.control)
                return;
            if (Time.time > sendTime)
            {
                if (transform.position != position | transform.rotation != rotation | transform.localScale != localScale) 
                {
                    position = transform.position;
                    rotation = transform.rotation;
                    localScale = transform.localScale;
                    ClientManager.AddOperation(new Operation(Command.Transform, identity, localScale, position, rotation));
                }
                sendTime = Time.time + (1f / 30f);
            }
        }

        void OnDestroy()
        {
            if (ClientManager.Instance == null)
                return;
            if (!ClientManager.Instance.control)
                return;
            ClientManager.AddOperation(new Operation(Command.Destroy) { index = identity });
        }
    }
}
#endif