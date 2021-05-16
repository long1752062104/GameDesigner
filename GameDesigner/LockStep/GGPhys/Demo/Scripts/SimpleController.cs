using GGPhysUnity;
using TrueSync;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    private TSTransform sTransform;
    private BRigidBody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<BRigidBody>();
        sTransform = GetComponent<TSTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01)
        {
            if (sTransform != null)
                sTransform.Rotate(new TSVector3(0, Input.GetAxis("Horizontal"), 0) * 5f);
            else
                rigidBody.Rotate(new TSVector3(0, Input.GetAxis("Horizontal"), 0) * 0.05);

        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01)
        {
            if (sTransform != null)
                sTransform.Translate(0, 0, Input.GetAxis("Vertical") * 0.05f);
            else
                rigidBody.AddForce(transform.forward * Input.GetAxis("Vertical") * 15f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        rigidBody.ApplyLinearImpulse(TSVector3.up * 5);
    }
}
