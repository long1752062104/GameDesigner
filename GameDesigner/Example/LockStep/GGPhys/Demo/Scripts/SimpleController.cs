using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhysUnity;
using GGPhys.Core;
using REAL = FixMath.FP;

public class SimpleController : MonoBehaviour
{

    private BRigidBody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<BRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01)
        {
            rigidBody.Rotate(new Vector3d(0, Input.GetAxis("Horizontal"), 0) * 0.01);
            
        }
        if(Mathf.Abs(Input.GetAxis("Vertical")) > 0.01)
        {
            rigidBody.Move(transform.forward.ToVector3d() * (REAL)Input.GetAxis("Vertical") * 0.02);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        rigidBody.ApplyLinearImpulse(Vector3d.UnitY * 5);
    }
}
