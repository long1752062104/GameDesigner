using GGPhysUnity;
using TrueSync;
using UnityEngine;

public class CallbackTest : RigidBodyCallBack
{
    public override void OnBCollisionEnter(BRigidBody otherBody, TSVector3 contactPoint)
    {
        Debug.Log("CollisionEnter" + "----" + otherBody.name + "----ContactPoint:" + contactPoint);
    }

    public override void OnBCollisionStay(BRigidBody otherBody)
    {
        Debug.Log("CollisionStay" + "----" + otherBody.name);
    }

    public override void OnBCollisionExit(BRigidBody otherBody)
    {
        Debug.Log("CollisionExit" + "----" + otherBody.name);
    }

    public override void OnBTriggerEnter(BRigidBody otherBody)
    {
        Debug.Log("TriggerEnter" + "----" + otherBody.name);
    }

    public override void OnBTriggerStay(BRigidBody otherBody)
    {
        Debug.Log("TriggerStay" + "----" + otherBody.name);
    }

    public override void OnBTriggerExit(BRigidBody otherBody)
    {
        Debug.Log("TriggerExit" + "----" + otherBody.name);
    }
}
