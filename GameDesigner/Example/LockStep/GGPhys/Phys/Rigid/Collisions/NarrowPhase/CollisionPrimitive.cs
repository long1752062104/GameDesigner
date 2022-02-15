using System;
using System.Collections.Generic;
using GGPhys.Core;

namespace GGPhys.Rigid.Collisions
{
    ///<summary>
    /// ��ײ��״������
    ///</summary>
    public abstract class CollisionPrimitive
    {
        /// <summary>
        /// ������ײ�㼶
        /// </summary>
        public uint CollisionLayer = 0x00000001;

        /// <summary>
        /// ����Щ�㼶������ײ��λ����
        /// </summary>
        public uint CollisionMask = 0xffffffff;

        ///<summary>
        /// ��������
        ///</summary>
        public RigidBody Body;

        /// <summary>
        /// �Ƿ�Ϊ������
        /// </summary>
        public bool IsTrigger;

        ///<summary>
        /// ����ڸ����ƫ�ƾ���
        ///</summary>
        public Matrix4 Offset;

        ///<summary>
        /// ������任����
        ///</summary>
        public Matrix4 Transform;

        /// <summary>
        /// �������Χ�У����ڴ�����ײ���
        /// </summary>
        public BoundingVolum BoundingVolum;

        /// <summary>
        /// hash����˳���
        /// </summary>
        public int HashOrder;


        public CollisionPrimitive()
        {
            Transform = Matrix4.Identity;
        }

        ///<summary>
        /// ���¼����״̬����
        ///</summary>
        public virtual void CalculateInternals()
        {
            Transform = Body.Transform * Offset;
        }

        ///<summary>
        /// ��ȡĳ���������������귽��
        ///</summary>
        public Vector3d GetAxis(int index)
        {
            return Transform.GetAxisVector(index);
        }

    }
}
