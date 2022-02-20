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
    /// ������
    ///</summary>
    public class RigidBody
    {

        public event OnCollisionEnterCallBack OnCollisionEnterEvent; // ��ײ�����ص�
        public event OnCollisionStayCallBack OnCollisionStayEvent; // ��ײͣ���ص�
        public event OnCollisionExitCallBack OnCollisionExitEvent; // ��ײ�����ص�

        public event OnTriggerEnterCallBack OnTriggerEnterEvent; // ���������ص�
        public event OnTriggerStayCallBack OnTriggerStayEvent; // ����ͣ���ص�
        public event OnTriggerExitCallBack OnTriggerExitEvent; // ���������ص�

        /// <summary>
        /// Unity�е�RigidBody
        /// </summary>
        public BRigidBody UnityBody;

        /// <summary>
        /// �Ƿ��ڼ���״̬
        /// </summary>
        public bool Active;

        /// <summary>
        /// �����ʶ
        /// </summary>
        public int ID;

        /// <summary>
        /// �Ƿ��Ǿ�̬����
        /// </summary>
        public bool IsStatic = false;

        /// <summary>
        /// �Ƿ��ܳ���(������)Ӱ��
        /// </summary>
        public bool UseAreaForce = true;

        /// <summary>
        /// ����λ�ã�����X��Y��Z
        /// </summary>
        public byte FreezePosition = 0x00;
        /// <summary>
        /// ������ת��������X��Y��Z
        /// </summary>
        public byte FreezeRotation = 0x00;

        /// <summary>
        /// Ħ����
        /// </summary>
        public REAL Friction = 0.1;

        /// <summary>
        /// �ص�ϵ��
        /// </summary>
        public REAL Restitution = 0;

        /// <summary>
        /// ����ϵ��
        /// </summary>
        public REAL SleepEpsilon = 0.12;

        /// <summary>
        /// �����ٶ�����
        /// </summary>
        public REAL AwakeVelocityLimit = 0.1;

        ///<summary>
        /// �����ĵ���
        ///</summary>
        public REAL InverseMass;

        ///<summary>
        /// �Ƿ�Ϊ��������
        ///</summary>
        public bool HasFiniteMass => InverseMass != 0;

        ///<summary>
        /// �Ƿ�Ϊ��������
        ///</summary>
        public bool HasInfiniteMass => InverseMass == 0;

        ///<summary>
        /// �����������������
        ///</summary>
        public Matrix3 InverseInertiaTensor;


        ///<summary>
        /// ��������ϵ��
        ///</summary>
        public REAL LinearDamping = 0.99;

        ///<summary>
        /// ���ٶ�����ϵ��
        ///</summary>
        public REAL AngularDamping = 0.99;

        ///<summary>
        /// λ��
        ///</summary>
        public Vector3d Position;

        public Vector3d MovePosition;

        ///<summary>
        /// ��ת
        ///</summary>
        public Quaternion Orientation;

        ///<summary>
        /// �����ٶ�
        ///</summary>
        public Vector3d Velocity;

        /// <summary>
        /// �����ٶ�����
        /// </summary>
        public Vector3d VelocityLimit;

        ///<summary>
        /// ���ٶ�
        ///</summary>
        public Vector3d Rotation;

        ///<summary>
        /// �����������������
        ///</summary>
        public Matrix3 InverseInertiaTensorWorld;

        ///<summary>
        /// �˶��̶ȣ���������
        ///</summary>
        private REAL m_motion;

        ///<summary>
        /// �Ƿ�����״̬
        ///</summary>
        private bool m_isAwake;

        ///<summary>
        /// �ܷ�����
        ///</summary>
        private bool m_canSleep;

        ///<summary>
        /// �任����
        ///</summary>
        public Matrix4 Transform;

        ///<summary>
        /// ��������
        ///</summary>
        private Vector3d m_forceAccum;

        /// <summary>
        /// ��ת��
        /// </summary>
        private Vector3d m_torqueAccum;

        ///<summary>
        /// �̶����ٶ�
        ///</summary>
        private Vector3d m_acceleration;

        ///<summary>
        /// ��һ֡���ٶ�
        ///</summary>
        public Vector3d LastFrameAcceleration;

        /// <summary>
        /// ��ǰ��ײ���ĸ���
        /// </summary>
        public List<RigidBody> ContactRigidBodies;

        /// <summary>
        /// ��ǰ��ײ���ĸ������
        /// </summary>
        public Dictionary<RigidBody, int> ContactRigidBodiesMap;

        /// <summary>
        /// ��ǰ�����еĸ���
        /// </summary>
        public List<RigidBody> TriggerRigidBodies;

        /// <summary>
        /// ��ǰ�����еĸ��崥������
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
        /// λ�ö���
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
        /// ��ת�Ƕȶ���
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
        /// ����һ����ײ���ĸ���
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
        /// �Ƴ�һ��������ײ�ĸ���
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
        /// ����һ�������еĸ���
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
        /// �Ƴ�һ�����������ĸ���
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
        /// ������������
        ///</summary>
        public void CalculateDerivedData()
        {
            Orientation.Normalise();

            // ����任����
            CalculateTransformMatrix(ref Transform, Position, Orientation);

            // ���������������������
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
        /// �������������ٶȽ��ٶ�
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
        /// ����λ����ת
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
        /// ��ֵ����
        ///</summary>
        public void SetMass(REAL mass)
        {
            if (mass <= 0)
                InverseMass = 0;
            else
                InverseMass = 1.0 / mass;
        }

        ///<summary>
        /// ��ȡ����
        ///</summary>
        public REAL GetMass()
        {
            if (InverseMass == 0)
                return REAL.MaxValue;
            else
                return 1.0 / InverseMass;
        }

        ///<summary>
        /// ��ֵ��������
        ///</summary>
        public void SetInertiaTensor(Matrix3 inertiaTensor)
        {
            InverseInertiaTensor = inertiaTensor.Inverse();
        }

        ///<summary>
        /// ��ȡ��������
        ///</summary>
        public Matrix3 GetInertiaTensor()
        {
            return InverseInertiaTensor.Inverse();
        }

        ///<summary>
        /// ��ȡ���������������
        ///</summary>
        public Matrix3 GetInertiaTensorWorld()
        {
            return InverseInertiaTensorWorld.Inverse();
        }

        ///<summary>
        /// ת��һ������������굽��������
        ///</summary>
        public Vector3d GetPointInLocalSpace(Vector3d point)
        {
            return Transform.TransformInverse(point);
        }

        ///<summary>
        /// ת��һ����ı������굽��������
        ///</summary>
        public Vector3d GetPointInWorldSpace(Vector3d point)
        {
            return Transform.Transform(point);
        }

        ///<summary>
        /// ת��һ���������������굽��������
        ///</summary>
        public Vector3d GetDirectionInLocalSpace(Vector3d direction)
        {
            return Transform.TransformInverseDirection(direction);
        }

        ///<summary>
        /// ת��һ�������ӱ������굽��������
        ///</summary>
        public Vector3d GetDirectionInWorldSpace(Vector3d direction)
        {
            return Transform.TransformDirection(direction);
        }

        ///<summary>
        /// ��ȡ����״̬
        ///</summary>
        public bool GetAwake()
        {
            return m_isAwake;
        }

        ///<summary>
        /// ��ֵ����״̬
        ///</summary>
        public void SetAwake(bool awake = true)
        {
            if (awake)
            {
                if (m_isAwake) return;
                
                m_isAwake = true;
                m_motion = SleepEpsilon * 4; //�����ѣ���Ҫһ����ʼ�ƶ����������������������
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
        /// ������������ͺ�ת��
        ///</summary>
        public void ClearAccumulators()
        {
            m_forceAccum = Vector3d.Zero;
            m_torqueAccum = Vector3d.Zero;
        }

        ///<summary>
        /// �����
        ///</summary>
        public void AddForce(Vector3d force, bool awakeBody = false)
        {
            m_forceAccum += force;
            if (awakeBody) m_isAwake = true;
        }

        ///<summary>
        /// ��ĳ�����������
        ///</summary>
        public void AddForceAtPoint(Vector3d force, Vector3d point, bool awakeBody = false)
        {
            Vector3d pt = point - Position;

            m_forceAccum += force;
            m_torqueAccum += Vector3d.Cross(pt, force);
            if (awakeBody) m_isAwake = true;
        }

        ///<summary>
        /// ��ĳ������������������
        ///<summary>
        public void AddForceAtBodyPoint(Vector3d force, Vector3d point, bool awakeBody = false)
        {
            Vector3d pt = GetPointInWorldSpace(point);
            AddForceAtPoint(force, pt, awakeBody);
        }

        ///<summary>
        /// ���ת��
        ///<summary>
        public void AddTorque(Vector3d torque, bool awakeBody = false)
        {
            m_torqueAccum += torque;
            if (awakeBody) m_isAwake = true;
        }

        /// <summary>
        /// �������Գ���
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyLinearImpulse(Vector3d impulse, bool awake = false)
        {
            Vector3d linearChange = impulse * InverseMass;
            Velocity += linearChange;
            if (awake) m_isAwake = true;
        }

        /// <summary>
        /// ���ýǳ���
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyAngularImpulse(Vector3d impulse, bool awake = false)
        {
            Vector3d angularChange = InverseInertiaTensorWorld * impulse;
            Rotation += angularChange;
            if (awake) m_isAwake = true;
        }

        /// <summary>
        /// �ƶ�
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
        /// ��ת
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
        /// ���������������������
        ///</summary>
        private static void TransformInertiaTensor(ref Matrix3 iitWorld, Matrix3 iitBody, Matrix4 rotmat)
        {
            Matrix3 rotM3 = Matrix3.FromRaw(rotmat.raw0, rotmat.raw1, rotmat.raw2,
                                             rotmat.raw4, rotmat.raw5, rotmat.raw6,
                                             rotmat.raw8, rotmat.raw9, rotmat.raw10);
            iitWorld = rotM3 * iitBody * rotM3.Transpose();
        }

        ///<summary>
        /// ����任����
        ///</summary>
        private static void CalculateTransformMatrix(ref Matrix4 transformMatrix, Vector3d position, Quaternion orientation)
        {
            transformMatrix.SetOrientationAndPos(orientation, position);
        }
    }
}