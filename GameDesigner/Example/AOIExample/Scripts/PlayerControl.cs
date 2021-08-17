#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace AOIExample
{
    using UnityEngine;

    public class PlayerControl : MonoBehaviour
    {
        public float moveSpeed = 5f;

        void Start()
        {
        }

        public static Vector3 InputDirection
        {
            get { return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); }
        }

        // Update is called once per frame
        void Update()
        {
            var dir1 = Transform3Dir(Camera.main.transform, InputDirection);
            if (dir1 != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir1, Vector3.up), 0.5f);
                transform.Translate(0, 0, moveSpeed * Time.deltaTime);
            }
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