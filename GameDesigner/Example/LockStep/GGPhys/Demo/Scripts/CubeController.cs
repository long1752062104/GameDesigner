using GGPhysUnity;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float moveSpeed = 6f;
    BRigidBody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<BRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = Direction;// Transform3Dir(Camera.main.transform, Direction);
        if (dir != Vector3.zero)
        {
            //var rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 0.5f);
            //transform.Translate(0, 0, moveSpeed * Time.deltaTime);
            rigidBody.SetAwake(true);
            if (dir.x < 0f)
                dir.x = -1f;
            else if (dir.x > 0f)
                dir.x = 1f;
            if (dir.z < 0f)
                dir.z = -1f;
            else if (dir.z > 0f)
                dir.z = 1f;
            rigidBody.Move(dir * moveSpeed * Time.deltaTime);
            //rigidBody.AddForce(dir * moveSpeed, true);
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
