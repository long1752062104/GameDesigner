#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using BuildComponent;
using UnityEngine;
namespace Example1 
{
    public class PlayerController : MonoBehaviour
    {
        public bool isLocalPlayer;
        public float moveSpeed = 6f;
        private NetworkAnimation anim;

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<NetworkAnimation>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                return;
            var dir = Transform3Dir(Camera.main.transform, Direction);
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.5f);
                transform.Translate(0, 0, moveSpeed * Time.deltaTime);
                anim.Play("soldierRun");
            }
            else
            {
                anim.Play("soldierIdleRelaxed");
            }
        }

        public static Vector3 Direction
        {
            get { return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); }
        }
        public Vector3 Transform3Dir(Transform t, Vector3 dir)
        {
            var f = Mathf.Deg2Rad * (-t.rotation.eulerAngles.y);
            dir.Normalize();
            var ret = new Vector3(dir.x * Mathf.Cos(f) - dir.z * Mathf.Sin(f), 0, dir.x * Mathf.Sin(f) + dir.z * Mathf.Cos(f));
            return ret;
        }
    }
}
#endif