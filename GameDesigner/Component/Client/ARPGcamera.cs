#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using UnityEngine;

    public class ARPGcamera : MonoBehaviour
    {
        public Transform target;
        public float targetHeight = 1.2f;
        public float distance = 4.0f;
        public float maxDistance = 20;
        public float minDistance = 1.0f;
        public float xSpeed = 500.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -10;
        public float yMaxLimit = 70;
        public float zoomRate = 80;
        public float rotationDampening = 3.0f;
        public float x = 20.0f;
        public float y = 0.0f;
        public float aimAngle = 8;
        public KeyCode key = KeyCode.Mouse1;
        protected Quaternion aim;
        protected Quaternion rotation;
        private Vector3 position;

        void LateUpdate()
        {
            if (!target)
                return;

            if (Input.GetKey(key) | key == KeyCode.None)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }

            distance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(distance);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            // Rotate Camera
            rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;

            aim = Quaternion.Euler(y - aimAngle, x, 0);

            //Camera Position
            position = target.position - (rotation * Vector3.forward * distance + new Vector3(0, -targetHeight, 0));
            transform.position = position;
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
#endif