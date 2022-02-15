#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;

namespace LockStep.Client
{
    public class ObjectView : MonoBehaviour
    {
        public Animation anim;
        public Actor actor;
        public float lerpSpeed = 0.25f;

        // Update is called once per frame
        void Update()
        {
            transform.rotation = actor.rigidBody.Rotation;
            transform.position = Vector3.Lerp(transform.position, actor.rigidBody.Position, lerpSpeed);
            transform.hasChanged = false;
        }
    }
}
#endif