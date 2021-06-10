#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Example
{
    using UnityEngine;

    public class CubeMovement : MonoBehaviour
    {
        public float speed = 1;
        public float movementDistance = 20;
        bool moving;
        Vector3 start;
        Vector3 destination;

        void Start()
        {
            start = transform.position;
        }

        void Update()
        {
            if (moving)
            {
                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    moving = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                }
            }
            else
            {
                Vector2 circlePos = Random.insideUnitCircle;
                Vector3 dir = new Vector3(circlePos.x, 0, circlePos.y);
                Vector3 dest = transform.position + dir * movementDistance;
                if (Vector3.Distance(start, dest) <= movementDistance)
                {
                    destination = dest;
                    moving = true;
                }
            }
        }
    }
}
#endif