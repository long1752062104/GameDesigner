using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Rigid;
using GGPhys.Core;
using REAL = FixMath.FP;

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
        public bool canSleep = true;
        public bool sleepOnAwake = false;
        public bool useGravity = true;
        public float mass = 1;
        public float linearDamping = 0.9f;
        public float angularDamping = 0.9f;
        public float friction = 0.1f;
        public float restitution = 0;
        public float sleepEpsilon = 0.12f;
        public float awakeVelocityLimit = 0.1f;
        public CollsionLayer collsionLayer = CollsionLayer.Default;
        [EnumFlags]
        public CollsionLayer collsionMask;
        [EnumFlags]
        public Freeze freezePos;
        [EnumFlags]
        public Freeze freezeRot;

        public Vector3d velocityLimit = new Vector3d(10f, 10f, 10f);

        [HideInInspector]
        public Vector3d CenterOfMassOffset;
        [HideInInspector]
        public Vector3d CenterOfMassOffsetWorld;
        public RigidBody m_body;
        private List<BCollider> m_Colliders;
        private Vector3 m_Position;
        private UnityEngine.Quaternion m_Rotation;

        private RigidBodyCallBack m_CallbackReceiver;
        

        public RigidBody Body { get => m_body; } //物理引擎中的刚体对象
        public Vector3 Position { get => m_Position; } //该物体经过物理计算后的位置
        public UnityEngine.Quaternion Rotation { get => m_Rotation; } //该物体经过物理计算后的旋转



#if UNITY_EDITOR
        [SerializeField]
        [DisplayOnly]
        private Vector3 m_BodyPosition;
        [SerializeField]
        [DisplayOnly]
        private UnityEngine.Quaternion m_BodyRotation;
        [SerializeField]
        [DisplayOnly]
        private Vector3 m_BodyLinearVelocity;
        [SerializeField]
        [DisplayOnly]
        private Vector3 m_BodyAngularVelocity;
        [SerializeField]
        [DisplayOnly]
        private float m_BodyLinearVelocityMagnitude;
#endif

        private void OnEnable()
        {
            if(RigidPhysicsEngine.Instance == null)
            {
                RigidPhysicsEngine.WaitAdd(this);
            }
            else
            {
                AddToEngine();
            }
        }

        private void OnDisable()
        {
            RemoveFromEngine();
        }


        public void AddToEngine()
        {
            if (m_body != null) return;
            REAL finalMass = isStatic ? 0 : mass;
            m_body = RigidPhysicsEngine.Instance.SpawnBody();
            m_Colliders = new List<BCollider>(transform.GetComponents<BCollider>());
            if (m_Colliders == null || m_Colliders.Count == 0) 
                Debug.LogError("missing collider!");
            CalculateCenterOfMassOffset();
            m_body.Position = transform.position.ToVector3d() + CenterOfMassOffsetWorld;
            m_body.Orientation = transform.rotation.ToQuaternion();
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
            m_body.VelocityLimit = velocityLimit;
            m_body.SetMass(finalMass);
            m_body.SetAwake(!isStatic);
            if (sleepOnAwake) m_body.SetAwake(false);
            m_body.SetCanSleep(canSleep);
            m_body.UnityBody = this;

            Matrix3 fInertiaTensor = CalculateInertiaTensor();
            m_body.SetInertiaTensor(fInertiaTensor);
            if (isStatic) m_body.InverseInertiaTensor = Matrix3.Zero;

            m_body.ApplyFreezeRotConstraints();
            m_body.CalculateDerivedData();
            RigidPhysicsEngine.Instance.Bodies.Add(m_body);

            AddCollidersToEngine();

            m_Position = (Body.Position - CenterOfMassOffsetWorld).ToVector3();
            m_Rotation = Body.Orientation.ToQuaternion();

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
        

        protected void Update()
        {
            if (transform.hasChanged) 
            {
                Body.SetAwake();
                Body.Position = transform.position.ToVector3d();
                Body.Orientation = transform.rotation.ToQuaternion();
                m_Position = transform.position;
                m_Rotation = transform.rotation;
            }
            else if (autoSyncTransform)
            {
                transform.position = Position;
                transform.rotation = Rotation;
            }
            transform.hasChanged = false;
#if UNITY_EDITOR
            m_BodyPosition = m_body.Position.ToVector3();
            m_BodyRotation = m_body.Orientation.ToQuaternion();
            m_BodyLinearVelocity = m_body.Velocity.ToVector3();
            m_BodyAngularVelocity = m_body.Rotation.ToVector3();
            m_BodyLinearVelocityMagnitude = m_body.Velocity.Magnitude;

            m_body.UseAreaForce = useGravity;
            m_body.LinearDamping = linearDamping;
            m_body.AngularDamping = angularDamping;
            m_body.FreezeRotation = (byte)freezeRot;
            m_body.Friction = friction;
            m_body.Restitution = restitution;
            m_body.SleepEpsilon = sleepEpsilon;
            m_body.AwakeVelocityLimit = awakeVelocityLimit;
            m_body.VelocityLimit = velocityLimit;

            m_body.IsStatic = isStatic;
#endif
        }

        /// <summary>
        /// 设置回调事件
        /// </summary>
        /// <param name="rigidBodyCallback"></param>
        public void SetCallBackReceiver(RigidBodyCallBack rigidBodyCallback)
        {
            m_CallbackReceiver = rigidBodyCallback;
            if(m_body != null)
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
            CenterOfMassOffsetWorld = Body.Transform.TransformDirection(CenterOfMassOffset);
            m_Position = (Body.Position - CenterOfMassOffsetWorld).ToVector3();
            m_Rotation = Body.Orientation.ToQuaternion();
        }

        /// <summary>
        /// 计算质心偏移向量
        /// </summary>
        void CalculateCenterOfMassOffset()
        {
            if (m_Colliders == null || m_Colliders.Count == 0) return;
            Vector3d sumCenter = Vector3d.Zero;
            foreach (var collider in m_Colliders)
            {
                sumCenter += collider.CenterOffset.ToVector3d();
            }
            CenterOfMassOffset = sumCenter / m_Colliders.Count;

            var mt = new Matrix3();
            mt.SetOrientation(transform.rotation.ToQuaternion());
            CenterOfMassOffsetWorld = mt.Transform(CenterOfMassOffset);
        }

        


        /// <summary>
        /// 计算各刚体的惯性张量
        /// </summary>
        /// <returns></returns>
        Matrix3 CalculateInertiaTensor() 
        { 
            if(m_Colliders == null) return Matrix3.Identity;
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
        public Vector3 Velocity { get => m_body.Velocity.ToVector3(); }

        /// <summary>
        /// 角速度
        /// </summary>
        public Vector3 AngularVelocity { get => m_body.Rotation.ToVector3(); }

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
        public void SetMass(float mass)
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
        public void SetFriction(REAL friction)
        {
            this.friction = friction;
            m_body.Friction = friction;
        }

        /// <summary>
        /// 设置回弹系数
        /// </summary>
        /// <param name="restitution"></param>
        public void SetRestitution(REAL restitution)
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
        public void SetLinearDamping(float damping)
        {
            this.linearDamping = damping;
            m_body.LinearDamping = damping;
        }

        /// <summary>
        /// 设置角速度阻尼系数
        /// </summary>
        /// <param name="damping"></param>
        public void SetAngularDamping(float damping)
        {
            this.angularDamping = damping;
            m_body.AngularDamping = damping;
        }

        /// <summary>
        /// 设置休眠系数
        /// </summary>
        /// <param name="epsilon"></param>
        public void SetSleepEpsilon(float epsilon)
        {
            this.sleepEpsilon = epsilon;
            m_body.SleepEpsilon = epsilon;
        }

        /// <summary>
        /// 设置苏醒最低速度
        /// </summary>
        /// <param name="limit"></param>
        public void SetAwakeVelocityLimit(float limit)
        {
            this.awakeVelocityLimit = limit;
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
        public void AddForce(Vector3 force, bool awakeBody= false)
        {
            m_body.AddForce(force.ToVector3d(), awakeBody);
        }

        public void AddForce(Vector3d force, bool awakeBody = false)
        {
            m_body.AddForce(force, awakeBody);
        }

        /// <summary>
        /// 对刚体的某个点施加作用力，力和点均为世界坐标系
        /// </summary>
        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            m_body.AddForceAtPoint(force.ToVector3d(), point.ToVector3d());
        }

        public void AddForceAtPoint(Vector3d force, Vector3 point)
        {
            m_body.AddForceAtPoint(force, point.ToVector3d());
        }

        /// <summary>
        /// 对刚体施加转矩
        /// </summary>
        /// <param name="torque"></param>
        /// <param name="awakeBody"></param>
        public void AddTorque(Vector3 torque, bool awakeBody = false)
        {
            m_body.AddTorque(torque.ToVector3d(), awakeBody);
        }

        public void AddTorque(Vector3d torque, bool awakeBody = false)
        {
            m_body.AddTorque(torque, awakeBody);
        }

        /// <summary>
        /// 运用线性冲量
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyLinearImpulse(Vector3 impulse, bool awake = false)
        {
            m_body.ApplyLinearImpulse(impulse.ToVector3d(), awake);
        }

        public void ApplyLinearImpulse(Vector3d impulse, bool awake = false)
        {
            m_body.ApplyLinearImpulse(impulse, awake);
        }

        /// <summary>
        /// 运用角冲量
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyAngularImpulse(Vector3 impulse, bool awake = false)
        {
            m_body.ApplyAngularImpulse(impulse.ToVector3d(), awake);
        }

        public void ApplyAngularImpulse(Vector3d impulse, bool awake = false)
        {
            m_body.ApplyAngularImpulse(impulse, awake);
        }

        /// <summary>
        /// 设置刚体位置
        /// </summary>
        /// <param name="velocity"></param>
        public void SetPosition(Vector3 position)
        {
            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Position = position.ToVector3d() + CenterOfMassOffsetWorld;
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_Position = position;
            m_body.Position = position.ToVector3d() + CenterOfMassOffsetWorld;
            m_body.CalculateDerivedData();
        }

        public void SetPosition(Vector3d position)
        {
            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Position = position + CenterOfMassOffsetWorld;
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_Position = position.ToVector3();
            m_body.Position = position + CenterOfMassOffsetWorld;
            m_body.CalculateDerivedData();
        }

        /// <summary>
        /// 设置刚体旋转
        /// </summary>
        /// <param name="velocity"></param>
        public void SetOrientation(UnityEngine.Quaternion orientation)
        {
            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Orientation = orientation.ToQuaternion();
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_Rotation = orientation;
            m_body.Orientation = orientation.ToQuaternion();
            m_body.CalculateDerivedData();
        }

        public void SetOrientation(GGPhys.Core.Quaternion orientation)
        {
            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Orientation = orientation;
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_Rotation = orientation.ToQuaternion();
            m_body.Orientation = orientation;
            m_body.CalculateDerivedData();
        }

        /// <summary>
        /// 设置刚体位置旋转
        /// </summary>
        /// <param name="velocity"></param>
        public void SetPositionAndOrientation(Vector3 position, UnityEngine.Quaternion orientation)
        {
            var mt = new Matrix3();
            mt.SetOrientation(orientation.ToQuaternion());
            CenterOfMassOffsetWorld = mt.Transform(CenterOfMassOffset);

            if (m_body.IsStatic)
            {
                RemoveCollidersFromEngine(false);
                m_body.Position = position.ToVector3d() + CenterOfMassOffsetWorld;
                m_body.Orientation = orientation.ToQuaternion();
                m_body.CalculateDerivedData();
                AddCollidersToEngine();
                return;
            }
            m_Position = position;
            m_Rotation = orientation;
            m_body.Position = position.ToVector3d() + CenterOfMassOffsetWorld;
            m_body.Orientation = orientation.ToQuaternion();
            m_body.CalculateDerivedData();
        }

        public void SetPositionAndOrientation(Vector3d position, GGPhys.Core.Quaternion orientation)
        {
            var mt = new Matrix3();
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
            m_Position = position.ToVector3();
            m_Rotation = orientation.ToQuaternion();
            m_body.Position = position + CenterOfMassOffsetWorld;
            m_body.Orientation = orientation;
            m_body.CalculateDerivedData();
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="delta"></param>
        public void Move(Vector3 delta)
        {
            if (m_body.IsStatic) return;
            m_body.Move(delta.ToVector3d());
        }

        public void Move(Vector3d delta)
        {
            if (m_body.IsStatic) return;
            m_body.Move(delta);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="delta"></param>
        public void Rotate(Vector3 delta)
        {
            if (m_body.IsStatic) return;
            m_body.Rotate(delta.ToVector3d());
        }

        public void Rotate(Vector3d delta)
        {
            if (m_body.IsStatic) return;
            m_body.Rotate(delta);
        }

        /// <summary>
        /// 设置刚体线性速度
        /// </summary>
        /// <param name="velocity"></param>
        public void SetLinearVelocity(Vector3 velocity)
        {
            if (m_body.IsStatic) return;
            m_body.Velocity = velocity.ToVector3d();
        }

        public void SetLinearVelocity(Vector3d velocity)
        {
            if (m_body.IsStatic) return;
            m_body.Velocity = velocity;
        }

        /// <summary>
        /// 设置刚体角速度
        /// </summary>
        /// <param name="velocity"></param>
        public void SetAnguarVolocity(Vector3 velocity)
        {
            if (m_body.IsStatic) return;
            m_body.Rotation = velocity.ToVector3d();
        }

        public void SetAnguarVolocity(Vector3d velocity)
        {
            if (m_body.IsStatic) return;
            m_body.Rotation = velocity;
        }

        /// <summary>
        /// 从物理引擎中移除
        /// </summary>
        void RemoveFromEngine()
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
            }
            if (RigidPhysicsEngine.Instance != null)
            {
                RigidPhysicsEngine.Instance.Bodies.Remove(m_body);
                RigidPhysicsEngine.Instance.RecycleBody(m_body);
                m_body = null;
            }
        }

        void AddCollidersToEngine()
        {
            if (m_Colliders == null || m_Colliders.Count == 0) return;
            foreach (var collider in m_Colliders)
            {
                collider.AddToEngine(this);
            }
        }

        void RemoveCollidersFromEngine(bool clearColliders = true)
        {
            if (RigidPhysicsEngine.Instance == null || RigidPhysicsEngine.Instance.Collisions == null) return;
            foreach (var collider in m_Colliders)
            {
                if(collider is BMeshCollider)
                {
                    var meshCollider = collider as BMeshCollider;
                    foreach (var primitive in meshCollider.Primitives)
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

            if(clearColliders) m_Colliders.Clear();
        }
        #endregion
    }
}

