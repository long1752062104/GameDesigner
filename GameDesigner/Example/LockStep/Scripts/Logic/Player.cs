#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
using ECS;
using Net.Share;
using System;
using TrueSync;

namespace LockStep.Client
{
    [Serializable]
    public class Player : Actor, IUpdate
    {
        public ObjectView objectView;
        public FP moveSpeed = 5f;
        internal Operation opt;

        public void Update()
        {
            Net.Vector3 dir = opt.direction;
            if (dir == Net.Vector3.zero)
                objectView.anim.Play("soldierIdle");
            else
            {
                objectView.anim.Play("soldierRun");
                transform.position += new TSVector3(dir.x, dir.y, dir.z) * moveSpeed * LSTime.deltaTime;
            }
        }

        public override void OnDestroy()
        {
            UnityEngine.Object.DestroyImmediate(objectView.gameObject);
        }
    }
}
#endif