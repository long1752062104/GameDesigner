using GGPhys.Core;
using GGPhys.Rigid;
using GGPhys.Rigid.Collisions;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    [RequireComponent(typeof(TSTransform))]
    public class BCollider : MonoBehaviour
    {
        public new TSTransform transform;
        public bool IsTrigger;
        [HideInInspector]
        public uint CollisionLayer;
        [HideInInspector]
        public uint CollisionMask;
        public TSVector3 CenterOffset;
        public CollisionPrimitive Primitive;
        public bool showGizmos = true;

        public RigidBody Body { get { if (Primitive != null) return Primitive.Body; return null; } }

        public virtual void AddToEngine(BRigidBody bBody) { }

        public virtual Matrix3 CalculateInertiaTensor(FP mass) { return Matrix3.Identity; }

        public virtual void Awake()
        {
            transform = GetComponent<TSTransform>();
        }
    }
}



