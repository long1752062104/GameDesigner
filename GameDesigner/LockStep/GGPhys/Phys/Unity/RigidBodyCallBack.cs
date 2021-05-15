using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    public class RigidBodyCallBack : MonoBehaviour
    {
        internal BRigidBody bBody;

        void Awake()
        {
            bBody = gameObject.GetComponent<BRigidBody>();
            if (bBody != null)
            {
                bBody.SetCallBackReceiver(this);
            }
        }

        public virtual void OnBCollisionEnter(BRigidBody otherBody, TSVector3 contactPoint) { }

        public virtual void OnBCollisionStay(BRigidBody otherBody) { }

        public virtual void OnBCollisionExit(BRigidBody otherBody) { }

        public virtual void OnBTriggerEnter(BRigidBody otherBody) { }

        public virtual void OnBTriggerStay(BRigidBody otherBody) { }

        public virtual void OnBTriggerExit(BRigidBody otherBody) { }




    }
}
