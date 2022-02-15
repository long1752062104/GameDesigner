using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Rigid;

namespace GGPhysUnity
{
    public class RigidBodyCallBack : MonoBehaviour
    {
        private BRigidBody bBody;
        void Awake()
        {
            bBody = gameObject.GetComponent<BRigidBody>();
            if (bBody != null)
            {
                bBody.SetCallBackReceiver(this);
            }
        }

        public virtual void OnBCollisionEnter(BRigidBody otherBody, GGPhys.Core.Vector3d contactPoint) { }

        public virtual void OnBCollisionStay(BRigidBody otherBody) { }

        public virtual void OnBCollisionExit(BRigidBody otherBody) { }

        public virtual void OnBTriggerEnter(BRigidBody otherBody) { }

        public virtual void OnBTriggerStay(BRigidBody otherBody) { }

        public virtual void OnBTriggerExit(BRigidBody otherBody) { }




    }
}
