using GGPhys.Rigid.Constraints;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GGPhys.Rigid.Collisions
{

    ///<summary>
    /// 碰撞数据类
    ///</summary>
    public class CollisionData
    {

        public ClassObjectPool<RigidContact> ContactsPool;// 碰撞对象池
        public ClassObjectPool<RigidContactPotential> PotentialContactsPool; // 潜在碰撞对象池

        public List<RigidContact> Contacts; //全部碰撞对象
        public List<RigidContactPotential> PotentialContacts; //全部潜在碰撞对象
        public Dictionary<long, RigidContactPotential> PotentialContactsMap; //潜在碰撞查询表

        public CollisionData()
        {
            Contacts = new List<RigidContact>();
            ContactsPool = new ClassObjectPool<RigidContact>(2000);
            PotentialContacts = new List<RigidContactPotential>();
            PotentialContactsPool = new ClassObjectPool<RigidContactPotential>(2000);
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
        /// 生成一个潜在碰撞
        /// </summary>
        /// <param name="primitive1"></param>
        /// <param name="primitive2"></param>
        public void AddPotentialContact(CollisionPrimitive primitive1, CollisionPrimitive primitive2)
        {
            if (!DetectLayer(primitive1, primitive2)) return; //检查是否发生碰撞
            long hash = HashToLong(primitive1.HashOrder, primitive2.HashOrder);
            if (!PotentialContactsMap.ContainsKey(hash))
            {
                RigidContactPotential potentialContact = PotentialContactsPool.Spawn();//从栈弹出一个对象
                potentialContact.Primitive1 = primitive1;
                potentialContact.Primitive2 = primitive2;
                potentialContact.Hash = hash;
                PotentialContactsMap.Add(hash, potentialContact);//将两个即将发生碰撞的对象放入字典
                PotentialContacts.Add(potentialContact);
            }
        }

        /// <summary>
        /// 回收潜在碰撞对象
        /// </summary>
        /// <param name="potentialContact"></param>
        public void RecyclePotentialContact(RigidContactPotential potentialContact)
        {
            potentialContact.Clear();
            PotentialContactsPool.Recycle(potentialContact);
        }

        /// <summary>
        /// 产生一个新的碰撞对象
        /// </summary>
        /// <returns></returns>
        public RigidContact GetNewContact()
        {
            RigidContact contact = ContactsPool.Spawn();
            Contacts.Add(contact);
            return contact;
        }

        /// <summary>
        /// 回收碰撞对象
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
        /// 两个刚体触发器触发
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
        /// hash编码两个刚体，防止重复添加碰撞
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
        /// 检测是否满足碰撞检测条件，不满足则不进行检测
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
        /// AABB检测
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