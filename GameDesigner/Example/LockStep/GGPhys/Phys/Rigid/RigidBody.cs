using System;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;
using GGPhysUnity;

namespace GGPhys.Rigid
{

    public delegate void OnCollisionEnterCallBack(BRigidBody otherBody, Vector3d contactPoint);
    public delegate void OnCollisionStayCallBack(BRigidBody otherBody);
    public delegate void OnCollisionExitCallBack(BRigidBody otherBody);

    public delegate void OnTriggerEnterCallBack(BRigidBody otherBody);
    public delegate void OnTriggerStayCallBack(BRigidBody otherBody);
    public delegate void OnTriggerExitCallBack(BRigidBody otherBody);

    ///<summary>
    /// 刚体类
    ///</summary>
    public class RigidBody
    {

        public event OnCollisionEnterCallBack OnCollisionEnterEvent; // 碰撞产生回调
        public event OnCollisionStayCallBack OnCollisionStayEvent; // 碰撞停留回调
        public event OnCollisionExitCallBack OnCollisionExitEvent; // 碰撞结束回调

        public event OnTriggerEnterCallBack OnTriggerEnterEvent; // 触发产生回调
        public event OnTriggerStayCallBack OnTriggerStayEvent; // 触发停留回调
        public event OnTriggerExitCallBack OnTriggerExitEvent; // 触发结束回调

        /// <summary>
        /// Unity中的RigidBody
        /// </summary>
        public BRigidBody UnityBody;

        /// <summary>
        /// 是否处于激活状态
        /// </summary>
        public bool Active;

        /// <summary>
        /// 刚体标识
        /// </summary>
        public int ID;

        /// <summary>
        /// 是否是静态刚体
        /// </summary>
        public bool IsStatic = false;

        /// <summary>
        /// 是否受场力(重力等)影响
        /// </summary>
        public bool UseAreaForce = true;

        /// <summary>
        /// 锁定位置，包含X、Y、Z
        /// </summary>
        public byte FreezePosition = 0x00;
        /// <summary>
        /// 锁定旋转，包含轴X、Y、Z
        /// </summary>
        public byte FreezeRotation = 0x00;

        /// <summary>
        /// 摩擦力
        /// </summary>
        public REAL Friction = 0.1;

        /// <summary>
        /// 回弹系数
        /// </summary>
        public REAL Restitution = 0;

        /// <summary>
        /// 休眠系数
        /// </summary>
        public REAL SleepEpsilon = 0.12;

        /// <summary>
        /// 唤醒速度限制
        /// </summary>
        public REAL AwakeVelocityLimit = 0.1;

        ///<summary>
        /// 质量的倒数
        ///</summary>
        public REAL InverseMass;

        ///<summary>
        /// 是否为有穷质量
        ///</summary>
        public bool HasFiniteMass => InverseMass != 0;

        ///<summary>
        /// 是否为无穷质量
        ///</summary>
        public bool HasInfiniteMass => InverseMass == 0;

        ///<summary>
        /// 本地坐标逆惯性张量
        ///</summary>
        public Matrix3 InverseInertiaTensor;


        ///<summary>
        /// 线性阻尼系数
        ///</summary>
        public REAL LinearDamping = 0.99;

        ///<summary>
        /// 角速度阻尼系数
        ///</summary>
        public REAL AngularDamping = 0.99;

        ///<summary>
        /// 位置
        ///</summary>
        public Vector3d Position;

        public Vector3d MovePosition;

        ///<summary>
        /// 旋转
        ///</summary>
        public Quaternion Orientation;

        ///<summary>
        /// 线性速度
        ///</summary>
        public Vector3d Velocity;

        /// <summary>
        /// 线性速度限制
        /// </summary>
        public Vector3d VelocityLimit;

        ///<summary>
        /// 角速度
        ///</summary>
        public Vector3d Rotation;

        ///<summary>
        /// 世界坐标逆惯性张量
        ///</summary>
        public Matrix3 InverseInertiaTensorWorld;

        ///<summary>
        /// 运动程度，用于休眠
        ///</summary>
        private REAL m_motion;

        ///<summary>
        /// 是否苏醒状态
        ///</summary>
        private bool m_isAwake;

        ///<summary>
        /// 能否休眠
        ///</summary>
        private bool m_canSleep;

        ///<summary>
        /// 变换矩阵
        ///</summary>
        public Matrix4 Transform;

        ///<summary>
        /// 合作用力
        ///</summary>
        private Vector3d m_forceAccum;

        /// <summary>
        /// 合转矩
        /// </summary>
        private Vector3d m_torqueAccum;

        ///<summary>
        /// 固定加速度
        ///</summary>
        private Vector3d m_acceleration;

        ///<summary>
        /// 上一帧加速度
        ///</summary>
        public Vector3d LastFrameAcceleration;

        /// <summary>
        /// 当前碰撞到的刚体
        /// </summary>
        public List<RigidBody> ContactRigidBodies;

        /// <summary>
        /// 当前碰撞到的刚体次数
        /// </summary>
        public Dictionary<RigidBody, int> ContactRigidBodiesMap;

        /// <summary>
        /// 当前触发中的刚体
        /// </summary>
        public List<RigidBody> TriggerRigidBodies;

        /// <summary>
        /// 当前触发中的刚体触发次数
        /// </summary>
        public Dictionary<RigidBody, int> TriggerRigidBodiesMap;

        public Vector3d ForceAccum { get => m_forceAccum; }

        public Vector3d TorqueAccum { get => m_torqueAccum; }


        public RigidBody()
        {
            Orientation = Quaternion.Identity;
            InverseInertiaTensor = Matrix3.Identity;
            Transform = Matrix4.Identity;
            ContactRigidBodies = new List<RigidBody>();
            ContactRigidBodiesMap = new Dictionary<RigidBody, int>();
            TriggerRigidBodies = new List<RigidBody>();
            TriggerRigidBodiesMap = new Dictionary<RigidBody, int>();
        }

        /// <summary>
        /// 位置冻结
        /// </summary>
        public void ApplyFreezePosConstraints()
        {
            if (FreezePosition != 0)
            {
                if((FreezePosition & 0x01) != 0)
                {
                    Velocity.x = 0;
                }
                if ((FreezePosition & 0x02) != 0)
                {
                    Velocity.y = 0;
                }
                if ((FreezePosition & 0x04) != 0)
                {
                    Velocity.z = 0;
                }
            }
        }

        /// <summary>
        /// 旋转角度冻结
        /// </summary>
        public void ApplyFreezeRotConstraints()
        {
            if (FreezeRotation != 0)
            {
                var Ixx = (FreezeRotation & 0x01) != 0 ? 0 : InverseInertiaTensor.data0;
                var Iyy = (FreezeRotation & 0x02) != 0 ? 0 : InverseInertiaTensor.data4;
                var Izz = (FreezeRotation & 0x04) != 0 ? 0 : InverseInertiaTensor.data8;
                InverseInertiaTensor.data0 = Ixx;
                InverseInertiaTensor.data4 = Iyy;
                InverseInertiaTensor.data8 = Izz;
            }
        }

        /// <summary>
        /// 新增一个碰撞到的刚体
        /// </summary>
        /// <param name="body"></param>
        public void AddContactBody(RigidBody body, Vector3d contactPoint)
        {
            if (!ContactRigidBodies.Contains(body))
            {
                ContactRigidBodies.Add(body);
                ContactRigidBodiesMap.Add(body, 2);
                OnCollisionEnter(body, contactPoint);
            }
            else
            {
                ContactRigidBodiesMap[body] += 1;
                if (ContactRigidBodiesMap[body] > 2)
                    ContactRigidBodiesMap[body] = 2;
            }
        }

        /// <summary>
        /// 移除一个结束碰撞的刚体
        /// </summary>
        /// <param name="body"></param>
        private void RemoveContactBody(RigidBody body)
        {
            if (ContactRigidBodies.Remove(body))
            {
                ContactRigidBodiesMap.Remove(body);
                OnCollisionExit(body);
            }
        }

        /// <summary>
        /// 新增一个触发中的刚体
        /// </summary>
        /// <param name="body"></param>
        public void AddTriggerBody(RigidBody body)
        {
            if (!TriggerRigidBodies.Contains(body))
            {
                TriggerRigidBodies.Add(body);
                TriggerRigidBodiesMap.Add(body, 2);
                OnTriggerEnter(body);
            }
            else
            {
                TriggerRigidBodiesMap[body] += 1;
                if (TriggerRigidBodiesMap[body] > 2)
                    TriggerRigidBodiesMap[body] = 2;
            }
        }

        /// <summary>
        /// 移除一个结束触发的刚体
        /// </summary>
        /// <param name="body"></param>
        private void RemoveTriggerBody(RigidBody body)
        {
            if (TriggerRigidBodies.Remove(body))
            {
                TriggerRigidBodiesMap.Remove(body);
                OnTriggerExit(body);
            }
        }

        public void RemoveContactAndTriggerBodys()
        {
            for (int i = ContactRigidBodies.Count - 1; i >= 0; i--)
            {
                var body = ContactRigidBodies[i];
                ContactRigidBodiesMap[body] -= 1;
                if (ContactRigidBodiesMap[body] <= 0)
                {
                    RemoveContactBody(body);
                }
                else
                {
                    OnCollisionStay(body);
                }
            }
            for (int i = TriggerRigidBodies.Count - 1; i >= 0; i--)
            {
                var body = TriggerRigidBodies[i];
                TriggerRigidBodiesMap[body] -= 1;
                if (TriggerRigidBodiesMap[body] <= 0)
                {
                    RemoveTriggerBody(body);
                }
                else
                {
                    OnTriggerStay(body);
                }
            }
        }

        ///<summary>
        /// 计算内置数据
        ///</summary>
        public void CalculateDerivedData()
        {
            Orientation.Normalise();

            // 计算变换矩阵
            CalculateTransformMatrix(ref Transform, Position, Orientation);

            // 计算世界坐标逆惯性张量
            TransformInertiaTensor(ref InverseInertiaTensorWorld, InverseInertiaTensor, Transform);
        }

        public void Clear()
        {
            Active = false;
            UnityBody = null;
            Position = Vector3d.Zero;
            Orientation = Quaternion.Identity;
            Velocity = Vector3d.Zero;
            Rotation = Vector3d.Zero;
            InverseInertiaTensor = Matrix3.Identity;
            Transform = Matrix4.Identity;
            LastFrameAcceleration = Vector3d.Zero;
            ContactRigidBodies.Clear();
            ContactRigidBodiesMap.Clear();
            TriggerRigidBodies.Clear();
            TriggerRigidBodiesMap.Clear();
        }

        /// <summary>
        /// 运用作用力到速度角速度
        /// </summary>
        /// <param name="dt"></param>
        public void ApplyForceToVelocity(REAL dt)
        {
            if (!m_isAwake || IsStatic) return;

            LastFrameAcceleration = m_acceleration;
            LastFrameAcceleration += m_forceAccum * InverseMass;

            Vector3d angularAcceleration = InverseInertiaTensorWorld * m_torqueAccum;

            Velocity += LastFrameAcceleration * dt;

            Rotation += angularAcceleration * dt;

            Velocity *= REAL.Pow(LinearDamping, dt);
            Rotation *= REAL.Pow(AngularDamping, dt);
        }

        ///<summary>
        /// 迭代位置旋转
        ///</summary>
        public void Integrate(REAL dt)
        {
            RemoveContactAndTriggerBodys();
            ClearAccumulators();

            if (!m_isAwake || IsStatic) return;

            ApplyFreezePosConstraints();

            Position += Velocity * dt;

            //if (Velocity.Magnitude < 1f)
            //{
            //    Position += MovePosition;
            //    MovePosition = Vector3d.Zero;
            //}

            Orientation.AddScaledVector(Rotation, dt);

            if (m_canSleep)
            {
                REAL currentMotion = Vector3d.Dot(Velocity, Velocity) + Vector3d.Dot(Rotation, Rotation);

                REAL bias = 0.92;
                m_motion = bias * m_motion + (1 - bias) * currentMotion;

                if (m_motion < SleepEpsilon)
                    SetAwake(false);
                else if (m_motion > 10 * SleepEpsilon)
                    m_motion = 10 * SleepEpsilon;

            }
        }

        ///<summary>
        /// 赋值质量
        ///</summary>
        public void SetMass(REAL mass)
        {
            if (mass <= 0)
                InverseMass = 0;
            else
                InverseMass = 1.0 / mass;
        }

        ///<summary>
        /// 获取质量
        ///</summary>
        public REAL GetMass()
        {
            if (InverseMass == 0)
                return REAL.MaxValue;
            else
                return 1.0 / InverseMass;
        }

        ///<summary>
        /// 赋值惯性张量
        ///</summary>
        public void SetInertiaTensor(Matrix3 inertiaTensor)
        {
            InverseInertiaTensor = inertiaTensor.Inverse();
        }

        ///<summary>
        /// 获取惯性张量
        ///</summary>
        public Matrix3 GetInertiaTensor()
        {
            return InverseInertiaTensor.Inverse();
        }

        ///<summary>
        /// 获取世界坐标惯性张量
        ///</summary>
        public Matrix3 GetInertiaTensorWorld()
        {
            return InverseInertiaTensorWorld.Inverse();
        }

        ///<summary>
        /// 转换一个点的世界坐标到本地坐标
        ///</summary>
        public Vector3d GetPointInLocalSpace(Vector3d point)
        {
            return Transform.TransformInverse(point);
        }

        ///<summary>
        /// 转换一个点的本地坐标到世界坐标
        ///</summary>
        public Vector3d GetPointInWorldSpace(Vector3d point)
        {
            return Transform.Transform(point);
        }

        ///<summary>
        /// 转换一个向量从世界坐标到本地坐标
        ///</summary>
        public Vector3d GetDirectionInLocalSpace(Vector3d direction)
        {
            return Transform.TransformInverseDirection(direction);
        }

        ///<summary>
        /// 转换一个向量从本地坐标到世界坐标
        ///</summary>
        public Vector3d GetDirectionInWorldSpace(Vector3d direction)
        {
            return Transform.TransformDirection(direction);
        }

        ///<summary>
        /// 获取苏醒状态
        ///</summary>
        public bool GetAwake()
        {
            return m_isAwake;
        }

        ///<summary>
        /// 赋值苏醒状态
        ///</summary>
        public void SetAwake(bool awake = true)
        {
            if (awake)
            {
                if (m_isAwake) return;
                
                m_isAwake = true;
                m_motion = SleepEpsilon * 4; //刚苏醒，需要一个初始移动量，否则可能又立即休眠
            }
            else
            {
                m_isAwake = false;
                Velocity = Vector3d.Zero;
                Rotation = Vector3d.Zero;
                LastFrameAcceleration = Vector3d.Zero;
            }
        }

        public bool GetCanSleep()
        {
            return m_canSleep;
        }

        public void SetCanSleep(bool canSleep = true)
        {
            m_canSleep = canSleep;
            if (!m_canSleep && !m_isAwake) SetAwake();
        }


        ///<summary>
        /// 清除合作用力和合转矩
        ///</summary>
        public void ClearAccumulators()
        {
            m_forceAccum = Vector3d.Zero;
            m_torqueAccum = Vector3d.Zero;
        }

        ///<summary>
        /// 添加力
        ///</summary>
        public void AddForce(Vector3d force, bool awakeBody = false)
        {
            m_forceAccum += force;
            if (awakeBody) m_isAwake = true;
        }

        ///<summary>
        /// 在某个点上添加力
        ///</summary>
        public void AddForceAtPoint(Vector3d force, Vector3d point, bool awakeBody = false)
        {
            Vector3d pt = point - Position;

            m_forceAccum += force;
            m_torqueAccum += Vector3d.Cross(pt, force);
            if (awakeBody) m_isAwake = true;
        }

        ///<summary>
        /// 在某个本地坐标点上添加力
        ///<summary>
        public void AddForceAtBodyPoint(Vector3d force, Vector3d point, bool awakeBody = false)
        {
            Vector3d pt = GetPointInWorldSpace(point);
            AddForceAtPoint(force, pt, awakeBody);
        }

        ///<summary>
        /// 添加转矩
        ///<summary>
        public void AddTorque(Vector3d torque, bool awakeBody = false)
        {
            m_torqueAccum += torque;
            if (awakeBody) m_isAwake = true;
        }

        /// <summary>
        /// 运用线性冲量
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyLinearImpulse(Vector3d impulse, bool awake = false)
        {
            Vector3d linearChange = impulse * InverseMass;
            Velocity += linearChange;
            if (awake) m_isAwake = true;
        }

        /// <summary>
        /// 运用角冲量
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyAngularImpulse(Vector3d impulse, bool awake = false)
        {
            Vector3d angularChange = InverseInertiaTensorWorld * impulse;
            Rotation += angularChange;
            if (awake) m_isAwake = true;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="delta"></param>
        public void Move(Vector3d delta)
        {
            Position += delta;
            CalculateDerivedData();
            SetAwake(true);

            //MovePosition = delta;
            //SetAwake(true);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="delta"></param>
        public void Rotate(Vector3d delta)
        {
            Orientation.AddScaledVector(delta, 1);
            CalculateDerivedData();
            SetAwake(true);
        }

        public void OnCollisionEnter(RigidBody otherBody, Vector3d contactPoint)
        {
            OnCollisionEnterEvent?.Invoke(otherBody.UnityBody, contactPoint);
        }

        public void OnCollisionStay(RigidBody otherBody)
        {
            OnCollisionStayEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnCollisionExit(RigidBody otherBody)
        {
            OnCollisionExitEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnTriggerEnter(RigidBody otherBody)
        {
            OnTriggerEnterEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnTriggerStay(RigidBody otherBody)
        {
            OnTriggerStayEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnTriggerExit(RigidBody otherBody)
        {
            OnTriggerExitEvent?.Invoke(otherBody.UnityBody);
        }

        ///<summary>
        /// 计算世界坐标逆惯性张量
        ///</summary>
        private static void TransformInertiaTensor(ref Matrix3 iitWorld, Matrix3 iitBody, Matrix4 rotmat)
        {
            Matrix3 rotM3 = Matrix3.FromRaw(rotmat.raw0, rotmat.raw1, rotmat.raw2,
                                             rotmat.raw4, rotmat.raw5, rotmat.raw6,
                                             rotmat.raw8, rotmat.raw9, rotmat.raw10);
            iitWorld = rotM3 * iitBody * rotM3.Transpose();
        }

        ///<summary>
        /// 计算变换矩阵
        ///</summary>
        private static void CalculateTransformMatrix(ref Matrix4 transformMatrix, Vector3d position, Quaternion orientation)
        {
            transformMatrix.SetOrientationAndPos(orientation, position);
        }
    }
}