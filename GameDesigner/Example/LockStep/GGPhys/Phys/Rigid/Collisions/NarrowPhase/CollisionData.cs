using System;
using System.Collections.Generic;
using GGPhys.Rigid.Constraints;

namespace GGPhys.Rigid.Collisions
{

    ///<summary>
    /// ��ײ������
    ///</summary>
    public class CollisionData
    {

        public ClassObjectPool<RigidContact> ContactsPool;// ��ײ�����
        public ClassObjectPool<RigidContactPotential> PotentialContactsPool; // Ǳ����ײ�����

        public List<RigidContact> Contacts; //ȫ����ײ����
        public List<RigidContactPotential> PotentialContacts; //ȫ��Ǳ����ײ����
        public Dictionary<long, RigidContactPotential> PotentialContactsMap; //Ǳ����ײ��ѯ��

        public CollisionData()
        {
            Contacts = new List<RigidContact>();
            ContactsPool = new ClassObjectPool<RigidContact>(20000);
            PotentialContacts = new List<RigidContactPotential>();
            PotentialContactsPool = new ClassObjectPool<RigidContactPotential>(20000);
            PotentialContactsMap = new Dictionary<long, RigidContactPotential>();
        }

        public void Destroy()
        {
            ContactsPool.Destroy();
            Contacts.Clear();
            PotentialContactsPool.Destroy();
            PotentialContacts.Clear();
            PotentialContactsMap.Clear();
        }

        /// <summary>
        /// ����һ��Ǳ����ײ
        /// </summary>
        /// <param name="primitive1"></param>
        /// <param name="primitive2"></param>
        public void AddPotentialContact(CollisionPrimitive primitive1, CollisionPrimitive primitive2)
        {
            if (!DetectLayer(primitive1, primitive2)) return; 
            var hash = HashToLong(primitive1.HashOrder, primitive2.HashOrder);
            if (!PotentialContactsMap.ContainsKey(hash))
            {
                RigidContactPotential potentialContact = PotentialContactsPool.Spawn();
                potentialContact.Primitive1 = primitive1;
                potentialContact.Primitive2 = primitive2;
                potentialContact.Hash = hash;
                PotentialContactsMap.Add(hash, potentialContact);
                PotentialContacts.Add(potentialContact);
            }
        }

        /// <summary>
        /// ����Ǳ����ײ����
        /// </summary>
        /// <param name="potentialContact"></param>
        public void RecyclePotentialContact(RigidContactPotential potentialContact)
        {
            potentialContact.Clear();
            PotentialContactsPool.Recycle(potentialContact);
        }

        /// <summary>
        /// ����һ���µ���ײ����
        /// </summary>
        /// <returns></returns>
        public RigidContact GetNewContact()
        {
            RigidContact contact = ContactsPool.Spawn();
            Contacts.Add(contact);
            return contact;
        }
        
        /// <summary>
        /// ������ײ����
        /// </summary>
        /// <param name="contact"></param>
        public void RecycleContact(RigidContact contact)
        {
            Contacts.Remove(contact);
            contact.Clear();
            ContactsPool.Recycle(contact);
        }

        public RigidContact GetContact()
        {
            return GetNewContact();
        }

        /// <summary>
        /// �������崥��������
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="intersect"></param>
        public void BodiesTrigger(RigidBody one, RigidBody two)
        {
            one.AddTriggerBody(two);
            two.AddTriggerBody(one);
        }

        /// <summary>
        /// hash�����������壬��ֹ�ظ������ײ
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        long HashToLong(int n1, int n2)
        {
            long ret = Math.Min(n1, n2);
            ret <<= 32;
            ret += Math.Max(n1, n2);
            return ret;
        }

        /// <summary>
        /// ����Ƿ�������ײ����������������򲻽��м��
        /// </summary>
        /// <param name="primitive1"></param>
        /// <param name="primitive2"></param>
        /// <returns></returns>
        private bool DetectLayer(CollisionPrimitive primitive1, CollisionPrimitive primitive2)
        {
            if (primitive1.Body == primitive2.Body) return false;
            if ((primitive1.CollisionLayer & primitive2.CollisionMask) == 0) return false;
            if ((primitive1.CollisionMask & primitive2.CollisionLayer) == 0) return false;
            if (!primitive1.Body.GetAwake() && !primitive2.Body.GetAwake()) return false;
            if (primitive1.Body.IsStatic && primitive2.Body.IsStatic) return false;
            if (!AABBDetect(primitive1, primitive2)) return false;
            return true;
        }
        
        /// <summary>
        /// AABB���
        /// </summary>
        /// <param name="primitive1"></param>
        /// <param name="primitive2"></param>
        /// <returns></returns>
        private bool AABBDetect(CollisionPrimitive primitive1, CollisionPrimitive primitive2)
        {
            if (primitive1.BoundingVolum.maxX < primitive2.BoundingVolum.minX) return false;
            if (primitive1.BoundingVolum.minX > primitive2.BoundingVolum.maxX) return false;
            if (primitive1.BoundingVolum.maxY < primitive2.BoundingVolum.minY) return false;
            if (primitive1.BoundingVolum.minY > primitive2.BoundingVolum.maxY) return false;
            if (primitive1.BoundingVolum.maxZ < primitive2.BoundingVolum.minZ) return false;
            if (primitive1.BoundingVolum.minZ > primitive2.BoundingVolum.maxZ) return false;

            return true;
        }
    }

}