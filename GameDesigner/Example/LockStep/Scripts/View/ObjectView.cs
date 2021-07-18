#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
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
            transform.rotation = actor.transform.rotation;
            transform.position = Vector3.Lerp(transform.position, actor.transform.position, lerpSpeed);
        }
    }
}
#endif