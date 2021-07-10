#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using UnityEngine;
namespace Example2
{
    public class PlayerControl : MonoBehaviour
    {
        public Vector3 moveDirection;
        public float moveSpeed = 5f;
        public float jumpSpeed = 500f;
        private Rigidbody rig;
        public PlayerAnimation pa;
        private Player player;
        internal bool jump;
        internal bool fire;
        
        void Start()
        {
            rig = GetComponent<Rigidbody>();
            pa = GetComponent<PlayerAnimation>();
            player = GetComponent<Player>();
        }

        public static Vector3 InputDirection
        {
            get { return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); }
        }

        // Update is called once per frame
        void Update()
        {
            if (player.isDead)
                return;
            var dir = InputDirection;
            if (dir != Vector3.zero)
                moveDirection = dir;
            else if (!InputJoystick.isInputing)
                moveDirection = Vector3.zero;
            var dir1 = Transform3Dir(Camera.main.transform, moveDirection);
            if (Input.GetKeyDown(KeyCode.Space))
                jump = true;
            if (jump & pa.isGround)
            {
                rig.AddForce(transform.up * jumpSpeed);
            }
            jump = false;
            if (dir1 != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir1, Vector3.up), 0.5f);
                transform.Translate(0, 0, moveSpeed * Time.deltaTime);
            }
            if (fire | Input.GetMouseButtonDown(0))
            {
                ClientManager.AddOperation(new Net.Share.Operation(Command.Fire, ClientManager.UID));
                fire = false;
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