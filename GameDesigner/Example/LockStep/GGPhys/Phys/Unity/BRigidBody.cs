using GGPhys.Core;
using GGPhys.Rigid;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    public enum CollsionLayer
    {
        Default = 1,
        Layer1 = 2,
        Layer2 = 4,
        Layer3 = 8,
        Layer4 = 16,
        Layer5 = 32
    }

    public enum Freeze
    {
        X = 1,
        Y = 2,
        Z = 4,
    }


    public class BRigidBody : MonoBehaviour
    {
        public bool isStatic = false;
        public bool autoSyncTransform = true;
        public bool canSleep;
        public bool sleepOnAwake = false;
        public bool useGravity = true;
        public FP mass = 1;
        [Header("当线性阻尼为0时, 将无法穿透模型")]
        public FP linearDamping = 0.9f;
        public FP angularDamping = 0.9f;
        public FP friction = 0.1f;
        public FP restitution = 0;
        public FP sleepEpsilon = 0.12f;
        public FP awakeVelocityLimit = 0.1f;
        public CollsionLayer collsionLayer = CollsionLayer.Default;
        [EnumFlags]
        public CollsionLayer collsionMask = (CollsionLayer)(-1);
        [EnumFlags]
        public Freeze freezePos;
        [EnumFlags]
        public Freeze freezeRot;

        [HideInInspector]
        public TSVector3 CenterOfMassOffset;
        [HideInInspector]
        public TSVector3 CenterOfMassOffsetWorld;
        private RigidBody m_body;
        private List<BCollider> m_Colliders;
        private TSVector3 m_Position;
        private TSQuaternion m_Rotation;

        private RigidBodyCallBack m_CallbackReceiver;

        public RigidBody Body { get => m_body; } //物理引擎中的刚体对象
        public TSVector3 Position { get => m_Position; } //该物体经过物理计算后的位置
        public TSQuaternion Rotation { get => m_Rotation; } //该物体经过物理计算后的旋转

#if UNITY_EDITOR
        [SerializeField]
        [DisplayOnly]
        private Vector3 m_BodyPosition;
        [SerializeField]
        [DisplayOnly]
        private Quaternion m_BodyRotation;
        [SerializeField]
        [DisplayOnly]
        private Vector3 m_BodyLinearVelocity;
        [SerializeField]
        [DisplayOnly]
        private Vector3 m_BodyAngularVelocity;
        public bool edit;
#endif

        private void Awake()
        {
            if (RigidPhysicsEngine.Instance == null)
                RigidPhysicsEngine.WaitAdd(this);
            else
                AddToEngine();
        }

        private void OnDestroy()
        {
            RemoveFromEngine();
        }

        public void AddToEngine()
        {
            FP finalMass = isStatic ? 0 : mass;
            m_body = new RigidBody();
            TSTransform transform1 = GetComponent<TSTransform>();
            if (transform1 != null)
                transform1.hasBody = true;
            m_Colliders = new List<BCollider>(transform.GetComponents<BCollider>());
            if (m_Colliders == null || m_Colliders.Count == 0)
                Debug.LogError("missing collider!");
            CalculateCenterOfMassOffset();
            m_body.Position = (TSVector3)transform.position + CenterOfMassOffsetWorld;
            m_body.Orientation = transform.rotation;
            m_body.IsStatic = isStatic;
            m_body.UseAreaForce = useGravity;
            m_body.LinearDamping = linearDamping;
            m_body.AngularDamping = angularDamping;
            m_body.FreezePosition = (byte)freezePos;
            m_body.FreezeRotation = (byte)freezeRot;
            m_body.Friction = friction;
            m_body.Restitution = restitution;
            m_body.SleepEpsilon = sleepEpsilon;
            m_body.AwakeVelocityLimit = awakeVelocityLimit;
            m_body.SetMass(finalMass);
            m_body.SetAwake(!isStatic);
            if (sleepOnAwake) m_body.SetAwake(false);
            m_body.SetCanSleep(canSleep);
            m_body.UnityBody = this;
            m_body.name = name;

            Matrix3 fInertiaTensor = CalculateInertiaTensor();
            m_body.SetInertiaTensor(fInertiaTensor);
            if (isStatic) m_body.InverseInertiaTensor = Matrix3.Zero;

            m_body.ApplyFreezeRotConstraints();
            m_body.CalculateDerivedData();
            RigidPhysicsEngine.Instance.Bodies.Add(m_body);

            AddCollidersToEngine();

            m_Position = Body.Position - CenterOfMassOffsetWorld;
            m_Rotation = Body.Orientation;

            if (m_CallbackReceiver != null)
            {
                m_body.OnCollisionEnterEvent += m_CallbackReceiver.OnBCollisionEnter;
                m_body.OnCollisionStayEvent += m_CallbackReceiver.OnBCollisionStay;
                m_body.OnCollisionExitEvent += m_CallbackReceiver.OnBCollisionExit;
                m_body.OnTriggerEnterEvent += m_CallbackReceiver.OnBTriggerEnter;
                m_body.OnTriggerStayEvent += m_CallbackReceiver.OnBTriggerStay;
                m_body.OnTriggerExitEvent += m_CallbackReceiver.OnBTriggerExit;
            }
        }

        private void Start()
        {
            enabled = autoSyncTransform;
        }

        protected void Update()
        {
            if (autoSyncTransform)
            {
                transform.position = Position;
                transform.rotation = Rotation;
            }
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (m_body == null)
                return;
            m_BodyPosition = m_body.Position;
            m_BodyRotation = m_body.Orientation;
            m_BodyLinearVelocity = m_body.Velocity;
            m_BodyAngularVelocity = m_body.Rotation;

            m_body.UseAreaForce = useGravity;
            m_body.LinearDamping = linearDamping;
            m_body.AngularDamping = angularDamping;
            m_body.FreezePosition = (byte)freezePos;
            m_body.FreezeRotation = (byte)freezeRot;
            m_body.Friction = friction;
            m_body.Restitution = restitution;
            m_body.SleepEpsilon = sleepEpsilon;
            m_body.AwakeVelocityLimit = awakeVelocityLimit;

            if (edit)
            {
                m_body.IsStatic = isStatic;
                m_body.UseAreaForce = useGravity;
                m_body.LinearDamping = linearDamping;
                m_body.AngularDamping = angularDamping;
                m_body.FreezePosition = (byte)freezePos;
                m_body.FreezeRotation = (byte)freezeRot;
                m_body.Friction = friction;
                m_body.Restitution = restitution;
                m_body.SleepEpsilon = sleepEpsilon;
                m_body.AwakeVelocityLimit = awakeVelocityLimit;
                SetPositionAndOrientation(transform.position, transform.rotation);
            }
#endif
        }

        /// <summary>
        /// 设置回调事件
        /// </summary>
        /// <param name="rigidBodyCallback"></param>
        public void SetCallBackReceiver(RigidBodyCallBack rigidBodyCallback)
        {
            m_CallbackReceiver = rigidBodyCallback;
            if (m_body != null)
            {
                m_body.OnCollisionEnterEvent += m_CallbackReceiver.OnBCollisionEnter;
                m_body.OnCollisionStayEvent += m_CallbackReceiver.OnBCollisionStay;
                m_body.OnCollisionExitEvent += m_CallbackReceiver.OnBCollisionExit;
                m_body.OnTriggerEnterEvent += m_CallbackReceiver.OnBTriggerEnter;
                m_body.OnTriggerStayEvent += m_CallbackReceiver.OnBTriggerStay;
                m_body.OnTriggerExitEvent += m_CallbackReceiver.OnBTriggerExit;
            }
        }

        /// <summary>
        /// 计算内部参数
        /// </summary>
        public void CalculateInternals()
        {
            m_Position = Body.Position;
            m_Rotation = Body.Orientation;
        }

        /// <summary>
        /// 计算质心偏移向量
        /// </summary>
        void CalculateCenterOfMassOffset()
        {
            if (m_Colliders == null || m_Colliders.Count == 0) return;
            TSVector3 sumCenter = TSVector3.Zero;
            foreach (BCollider collider in m_Colliders)
            {
                sumCenter += collider.CenterOffset;
            }
            CenterOfMassOffset = sumCenter / m_Colliders.Count;

            Matrix3 mt = new Matrix3();
            mt.SetOrientation(transform.rotation);
            CenterOfMassOffsetWorld = mt.Transform(CenterOfMassOffset);
        }

        /// <summary>
        /// 计算各刚体的惯性张量
        /// </summary>
        /// <returns></returns>
        Matrix3 CalculateInertiaTensor()
        {
            if (m_Colliders == null) return Matrix3.Identity;
            if (m_Colliders.Count == 1)
            {
                return m_Colliders[0].CalculateInertiaTensor(mass);
            }
            else
            {
                return Matrix3.Identity;
            }
        }




        #region RigidBody
        /// <summary>
        /// 线性速度
        /// </summary>
        public Vector3 Velocity { get => m_body.Velocity; }

        /// <summary>
        /// 角速度
        /// </summary>
        public Vector3 AngularVelocity { get => m_body.Rotation; }

        /// <summary>
        /// 设置静态或非静态
        /// </summary>
        public void SetStatic(bool isStatic)
        {
            if (isStatic == this.isStatic) return;
            RemoveFromEngine();
            this.isStatic = isStatic;
            AddToEngine();
        }

        /// <summary>
        /// 设置质量
        /// </summary>
        /// <param name="mass"></param>
        public void SetMass(FP mass)
        {
            if (isStatic) return;

            this.mass = mass;
            m_body.SetMass(mass);

            Matrix3 fInertiaTensor = CalculateInertiaTensor();
            m_body.SetInertiaTensor(fInertiaTensor);
        }

        /// <summary>
        /// 设置旋转约束
        /// </summary>
        /// <param name="freeze"></param>
        public void SetFreezePosition(byte freeze)
        {
            m_body.FreezePosition = freeze;
        }

        /// <summary>
        /// 设置旋转约束
        /// </summary>
        /// <param name="freeze"></param>
        public void SetFreezeRotation(byte freeze)
        {
            m_body.FreezeRotation = freeze;
            Matrix3 fInertiaTensor = CalculateInertiaTensor();
            m_body.SetInertiaTensor(fInertiaTensor);
            m_body.ApplyFreezeRotConstraints();
        }

        /// <summary>
        /// 设置摩擦力
        /// </summary>
        /// <param name="friction"></param>
        public void SetFriction(FP friction)
        {
            this.friction = friction;
            m_body.Friction = friction;
        }

        /// <summary>
        /// 设置回弹系数
        /// </summary>
        /// <param name="restitution"></param>
        public void SetRestitution(FP restitution)
        {
            this.restitution = restitution;
            m_body.Restitution = restitution;
        }

        /// <summary>
        /// 设置是否应用重力
        /// </summary>
        /// <param name="useGravity"></param>
        public void SetUseGravity(bool useGravity)
        {
            this.useGravity = useGravity;
            m_body.UseAreaForce = useGravity;
        }

        /// <summary>
        /// 设置线性阻尼系数
        /// </summary>
        /// <param name="damping"></param>
        public void SetLinearDamping(FP damping)
        {
            linearDamping = damping;
            m_body.LinearDamping = damping;
        }

        /// <summary>
        /// 设置角速度阻尼系数
        /// </summary>
        /// <param name="damping"></param>
        public void SetAngularDamping(FP damping)
        {
            angularDamping = damping;
            m_body.AngularDamping = damping;
        }

        /// <summary>
        /// 设置休眠系数
        /// </summary>
        /// <param name="epsilon"></param>
        public void SetSleepEpsilon(FP epsilon)
        {
            sleepEpsilon = epsilon;
            m_body.SleepEpsilon = epsilon;
        }

        /// <summary>
        /// 设置苏醒最低速度
        /// </summary>
        /// <param name="limit"></param>
        public void SetAwakeVelocityLimit(FP limit)
        {
            awakeVelocityLimit = limit;
            m_body.AwakeVelocityLimit = limit;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        /// <param name="awake"></param>
        public void SetAwake(bool awake)
        {
            m_body.SetAwake(awake);
        }

        /// <summary>
        /// 对刚体施加作用力
        /// </summary>
        /// <param name="force"></param>
        /// <param name="awakeBody"></param>
        public void AddForce(Vector3 force, bool awakeBody = false)
        {
            m_body.AddForce(force, awakeBody);
        }

        public void AddForce(TSVector3 force, bool awakeBody = false)
        {
            m_body.AddForce(force, awakeBody);
        }

        /// <summary>
        /// 对刚体的某个点施加作用力，力和点均为世界坐标系
        /// </summary>
        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            m_body.AddForceAtPoint(force, point);
        }

        public void AddForceAtPoint(TSVector3 force, Vector3 point)
        {
            m_body.AddForceAtPoint(force, point);
        }

        /// <summary>
        /// 运用线性冲量
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyLinearImpulse(Vector3 impulse)
        {
            m_body.ApplyLinearImpulse(impulse);
        }

        public void ApplyLinearImpulse(TSVector3 impulse)
        {
            m_body.ApplyLinearImpulse(impulse);
        }

        /// <summary>
        /// 运用角冲量
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyAngularImpulse(Vector3 impulse)
        {
            m_body.ApplyAngularImpulse(impulse);
        }

        public void ApplyAngularImpulse(TSVector3 impulse)
        {
            m_body.ApplyAngularImpulse(impulse);
        }

        /// <summary>
        /// 设置刚体位置
        /// </summary>
        /// <param name="velocity"></param>
        public void SetPosition(TSVector3 position)
        {
            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Position = position + CenterOfMassOffsetWorld;
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_body.Position = position + CenterOfMassOffsetWorld;
            m_body.CalculateDerivedData();
        }

        /// <summary>
        /// 设置刚体旋转
        /// </summary>
        /// <param name="velocity"></param>
        public void SetOrientation(TSQuaternion orientation)
        {
            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Orientation = orientation;
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_body.Orientation = orientation;
            //m_body.CalculateDerivedData();
        }

        /// <summary>
        /// 设置刚体位置旋转
        /// </summary>
        /// <param name="velocity"></param>
        public void SetPositionAndOrientation(TSVector3 position, TSQuaternion orientation)
        {
            Matrix3 mt = new Matrix3();
            mt.SetOrientation(orientation);
            CenterOfMassOffsetWorld = mt.Transform(CenterOfMassOffset);

            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Position = position + CenterOfMassOffsetWorld;
                m_body.Orientation = orientation;
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_body.Position = position + CenterOfMassOffsetWorld;
            m_body.Orientation = orientation;
            m_body.CalculateDerivedData();
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="delta"></param>
        public void Move(TSVector3 delta)
        {
            if (m_body.IsStatic)
                return;
            m_body.Move(delta);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="delta"></param>
        public void Rotate(Vector3 delta)
        {
            if (m_body.IsStatic) return;
            m_body.Rotate(delta);
        }

        public void Rotate(TSVector3 delta)
        {
            if (m_body.IsStatic) return;
            m_body.Rotate(delta);
        }

        /// <summary>
        /// 设置刚体线性速度
        /// </summary>
        /// <param name="velocity"></param>
        public void SetLinearVelocity(TSVector3 velocity)
        {
            if (m_body.IsStatic) return;
            m_body.Velocity = velocity;
        }

        /// <summary>
        /// 设置刚体角速度
        /// </summary>
        /// <param name="velocity"></param>
        public void SetAnguarVolocity(TSVector3 velocity)
        {
            if (m_body.IsStatic) return;
            m_body.Rotation = velocity;
        }

        /// <summary>
        /// 从物理引擎中移除
        /// </summary>
        public void RemoveFromEngine()
        {
            RemoveCollidersFromEngine();
            if (m_CallbackReceiver != null && m_body != null)
            {
                m_body.OnCollisionEnterEvent -= m_CallbackReceiver.OnBCollisionEnter;
                m_body.OnCollisionStayEvent -= m_CallbackReceiver.OnBCollisionStay;
                m_body.OnCollisionExitEvent -= m_CallbackReceiver.OnBCollisionExit;
                m_body.OnTriggerEnterEvent -= m_CallbackReceiver.OnBTriggerEnter;
                m_body.OnTriggerStayEvent -= m_CallbackReceiver.OnBTriggerStay;
                m_body.OnTriggerExitEvent -= m_CallbackReceiver.OnBTriggerExit;
                m_CallbackReceiver = null;
            }
            if (RigidPhysicsEngine.Instance != null)
            {
                RigidPhysicsEngine.Instance.Bodies.Remove(m_body);
                //m_body.UnityBody = null;
                m_body = null;
            }

        }

        void AddCollidersToEngine()
        {
            if (m_Colliders == null || m_Colliders.Count == 0) return;
            foreach (BCollider collider in m_Colliders)
            {
                collider.AddToEngine(this);
            }
        }

        void RemoveCollidersFromEngine(bool clearColliders = true)
        {
            if (RigidPhysicsEngine.Instance == null || RigidPhysicsEngine.Instance.Collisions == null) return;
            foreach (BCollider collider in m_Colliders)
            {
                if (collider is BMeshCollider)
                {
                    BMeshCollider meshCollider = collider as BMeshCollider;
                    foreach (GGPhys.Rigid.Collisions.CollisionPrimitive primitive in meshCollider.Primitives)
                    {
                        RigidPhysicsEngine.Instance.Collisions.RemovePrimitive(primitive);
                    }
                    meshCollider.Primitives.Clear();
                }
                else
                {
                    RigidPhysicsEngine.Instance.Collisions.RemovePrimitive(collider.Primitive);
                    collider.Primitive = null;
                }
            }

            if (clearColliders) m_Colliders.Clear();
        }
        #endregion
    }
}

