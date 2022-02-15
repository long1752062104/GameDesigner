#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using ECS;
using GGPhys.Core;
using GGPhysUnity;
using Net.Share;
using System;
using TrueSync;
using UnityEngine;

namespace LockStep.Client
{
    [Serializable]
    public class Player : Actor, IUpdate
    {
        public ObjectView objectView;
        public FP moveSpeed = 6f;
        internal Operation opt;
        
        public void Update()
        {
            Net.Vector3 dir = opt.direction;
            if (dir == Net.Vector3.zero)
                objectView.anim.Play("soldierIdle");
            else
            {
                objectView.anim.Play("soldierRun");
                //rigidBody.transform.position += (Vector3)(new TSVector3(dir.x, dir.y, dir.z) * moveSpeed * LSTime.deltaTime);
                rigidBody.Move((Vector3)(new TSVector3(dir.x, dir.y, dir.z) * moveSpeed * LSTime.deltaTime));
                //rigidBody.AddForce((Vector3)(new TSVector3(dir.x, dir.y, dir.z) * moveSpeed));
            }
        }

        public override void OnDestroy()
        {
            UnityEngine.Object.DestroyImmediate(objectView.gameObject);
        }
    }
}
#endif