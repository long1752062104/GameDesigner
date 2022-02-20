using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Core;
using QUATERNION = GGPhys.Core.Quaternion;
using REAL = FixMath.FP;

namespace UnityEngine
{

    public static class MathExtension
    {

        public static QUATERNION ToQuaternion(this Quaternion q)
        {
            return new QUATERNION(q.x, q.y, q.z, q.w);
        }

        public static Quaternion ToQuaternion(this QUATERNION q)
        {
            return new Quaternion((float)q.x, (float)q.y, (float)q.z, (float)q.w);
        }

        public static Vector3 ToVector3(this Vector3d v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }

        public static Vector4 ToVector4(this Vector3d v)
        {
            return new Vector4((float)v.x, (float)v.y, (float)v.z, 1);
        }

        public static Vector3d ToVector3d(this Vector3 v)
        {
            return new Vector3d(v.x, v.y, v.z);
        }
    }

}
