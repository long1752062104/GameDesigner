#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;

namespace Example2
{

    public class Player : MonoBehaviour
    {
        public GameObject bullet;
        public Transform firePoint;

        internal void Fire()
        {
            var bullet1 = Instantiate(bullet, firePoint.position, firePoint.rotation);
            var rig1 = bullet1.GetComponent<Rigidbody>();
            rig1.AddForce(firePoint.forward * 1000f);
            Destroy(bullet1, 5f);
        }
    }
}
#endif