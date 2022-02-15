using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using REAL = FixMath.FP;

namespace GGPhysUnity
{
    public class BCollider : MonoBehaviour
    {
        public bool IsTrigger;
        [HideInInspector]
        public uint CollisionLayer;
        [HideInInspector]
        public uint CollisionMask;
        public Vector3 CenterOffset;
        public CollisionPrimitive Primitive;

        public virtual void AddToEngine(BRigidBody bBody) { }

        public virtual Matrix3 CalculateInertiaTensor(REAL mass) { return Matrix3.Identity; }
    }
}



