using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Core;
using GGPhysUnity;

public class CallbackTest : RigidBodyCallBack
{
    public override void OnBCollisionEnter(BRigidBody otherBody, Vector3d contactPoint)
    {
        Debug.Log("CollisionEnter" + "----" + otherBody.name + "----ContactPoint:" + contactPoint.ToVector3());
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
