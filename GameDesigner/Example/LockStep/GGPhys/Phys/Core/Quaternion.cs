using System;
using System.Collections.Generic;
using FixPointCS;
using REAL = FixMath.FP;

#if GGPHYS_FIXPOINT32
using RawType = System.Int32;
using FixedType = FixPointCS.Fixed32;
#else
using RawType = System.Int64;
using FixedType = FixPointCS.Fixed64;
#endif

namespace GGPhys.Core
{
    /// <summary>
    /// 四元数
    /// </summary>
    public struct Quaternion
    {
        public static Quaternion Identity { get { return new Quaternion(FixedType.Zero, FixedType.Zero, FixedType.Zero, FixedType.One); } }

        //public RawType RawX;
        //public RawType RawY;
        //public RawType RawZ;
        //public RawType RawW;

        public REAL x;// { get { return REAL.FromRaw(RawX); } set { RawX = value.Raw; } }
        public REAL y;// { get { return REAL.FromRaw(RawY); } set { RawY = value.Raw; } }
        public REAL z;// { get { return REAL.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public REAL w;// { get { return REAL.FromRaw(RawW); } set { RawW = value.Raw; } }

        public Quaternion(REAL x, REAL y, REAL z, REAL w)
        {
            this.x.Raw = x.Raw;
            this.y.Raw = y.Raw;
            this.z.Raw = z.Raw;
            this.w.Raw = w.Raw;
        }

        public Quaternion(Vector3d v, REAL w)
        {
            x.Raw = v.x.Raw;
            y.Raw = v.y.Raw;
            z.Raw = v.z.Raw;
            this.w.Raw = w.Raw;
        }

        private Quaternion(RawType x, RawType y, RawType z, RawType w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Quaternion FromAxisAngle(Vector3d axis, REAL angle)
        {
            REAL half_angle = REAL.Div2(angle);
            return new Quaternion(axis * REAL.SinFastest(half_angle), REAL.CosFastest(half_angle));
        }

        public static Quaternion FromYawPitchRoll(REAL yaw_y, REAL pitch_x, REAL roll_z)
        {
            REAL half_roll = REAL.Div2(roll_z);
            REAL sr = REAL.SinFastest(half_roll);
            REAL cr = REAL.CosFastest(half_roll);

            REAL half_pitch = REAL.Div2(pitch_x);
            REAL sp = REAL.SinFastest(half_pitch);
            REAL cp = REAL.CosFastest(half_pitch);

            REAL half_yaw = REAL.Div2(yaw_y);
            REAL sy = REAL.SinFastest(half_yaw);
            REAL cy = REAL.CosFastest(half_yaw);

            return new Quaternion(
                cy * sp * cr + sy * cp * sr,
                sy * cp * cr - cy * sp * sr,
                cy * cp * sr - sy * sp * cr,
                cy * cp * cr + sy * sp * sr);
        }

        public static Quaternion FromTwoVectors(Vector3d a, Vector3d b)
        {
            REAL epsilon = REAL.Ratio(1, 1000000);

            REAL norm_a_norm_b = REAL.SqrtFastest(a.SqrMagnitude * b.SqrMagnitude);
            REAL real_part = norm_a_norm_b + Vector3d.Dot(a, b);

            Vector3d v;

            if (real_part < (epsilon * norm_a_norm_b))
            {
                real_part = REAL.Zero;
                bool cond = REAL.Abs(a.x) > REAL.Abs(a.z);
                v = cond ? new Vector3d(-a.y, a.x, REAL.Zero)
                         : new Vector3d(REAL.Zero, -a.z, a.y);
            }
            else
            {
                v = Vector3d.Cross(a, b);
            }
            return NormalizeFastest(new Quaternion(v, real_part));
        }

        public static Quaternion LookRotation(Vector3d dir, Vector3d up)
        {
            if (dir == Vector3d.Zero)
                return Identity;

            if (up != dir)
            {
                up.Normalize();
                Vector3d v = dir + up * -Vector3d.Dot(up, dir);
                Quaternion q = FromTwoVectors(Vector3d.UnitZ, v);
                return FromTwoVectors(v, dir) * q;
            }
            else
                return FromTwoVectors(Vector3d.UnitZ, dir);
        }

        public static Quaternion LookAtRotation(Vector3d from, Vector3d to, Vector3d up)
        {
            Vector3d dir = (to - from).Normalized;
            return LookRotation(dir, up);
        }

        public static Quaternion operator *(Quaternion a, Quaternion b) { return Multiply(a, b); }

        public static bool operator ==(Quaternion a, Quaternion b) { return a.x.Raw == b.x.Raw && a.y.Raw == b.y.Raw && a.z.Raw == b.z.Raw && a.w.Raw == b.w.Raw; }
        public static bool operator !=(Quaternion a, Quaternion b) { return a.x.Raw != b.x.Raw || a.y.Raw != b.y.Raw || a.z.Raw != b.z.Raw || a.w.Raw != b.w.Raw; }

        public static Quaternion Negate(Quaternion a) { return new Quaternion(-a.x.Raw, -a.y.Raw, -a.z.Raw, -a.w.Raw); }
        public static Quaternion Conjugate(Quaternion a) { return new Quaternion(-a.x.Raw, -a.y.Raw, -a.z.Raw, a.w.Raw); }
        public static Quaternion Inverse(Quaternion a)
        {
            RawType inv_norm = REAL.Rcp(LengthSqr(a)).Raw;
            return new Quaternion(
                -FixedType.Mul(a.x.Raw, inv_norm),
                -FixedType.Mul(a.y.Raw, inv_norm),
                -FixedType.Mul(a.z.Raw, inv_norm),
                FixedType.Mul(a.w.Raw, inv_norm));
        }

        public static Quaternion InverseUnit(Quaternion a) { return new Quaternion(-a.x.Raw, -a.y.Raw, -a.z.Raw, a.w.Raw); }

        public static Quaternion Multiply(Quaternion value1, Quaternion value2)
        {
            RawType q1x = value1.x.Raw;
            RawType q1y = value1.y.Raw;
            RawType q1z = value1.z.Raw;
            RawType q1w = value1.w.Raw;

            RawType q2x = value2.x.Raw;
            RawType q2y = value2.y.Raw;
            RawType q2z = value2.z.Raw;
            RawType q2w = value2.w.Raw;

            RawType cx = FixedType.Mul(q1y, q2z) - FixedType.Mul(q1z, q2y);
            RawType cy = FixedType.Mul(q1z, q2x) - FixedType.Mul(q1x, q2z);
            RawType cz = FixedType.Mul(q1x, q2y) - FixedType.Mul(q1y, q2x);

            RawType dot = FixedType.Mul(q1x, q2x) + FixedType.Mul(q1y, q2y) + FixedType.Mul(q1z, q2z);

            return new Quaternion(FixedType.Mul(q1x, q2w) + FixedType.Mul(q2x, q1w) + cx,
                                  FixedType.Mul(q1y, q2w) + FixedType.Mul(q2y, q1w) + cy,
                                  FixedType.Mul(q1z, q2w) + FixedType.Mul(q2z, q1w) + cz,
                                  FixedType.Mul(q1w, q2w) - dot);
        }

        public static REAL Length(Quaternion a) { return REAL.Sqrt(LengthSqr(a)); }
        public static REAL LengthFast(Quaternion a) { return REAL.SqrtFast(LengthSqr(a)); }
        public static REAL LengthFastest(Quaternion a) { return REAL.SqrtFastest(LengthSqr(a)); }
        public static REAL LengthSqr(Quaternion a) { return REAL.FromRaw(FixedType.Mul(a.x.Raw, a.x.Raw) + FixedType.Mul(a.y.Raw, a.y.Raw) + FixedType.Mul(a.z.Raw, a.z.Raw) + FixedType.Mul(a.w.Raw, a.w.Raw)); }
        public static Quaternion Normalize(Quaternion a)
        {
            RawType inv_norm = REAL.Rcp(Length(a)).Raw;
            return new Quaternion(
                FixedType.Mul(a.x.Raw, inv_norm),
                FixedType.Mul(a.y.Raw, inv_norm),
                FixedType.Mul(a.z.Raw, inv_norm),
                FixedType.Mul(a.w.Raw, inv_norm));
        }
        public static Quaternion NormalizeFast(Quaternion a)
        {
            RawType inv_norm = REAL.RcpFast(LengthFast(a)).Raw;
            return new Quaternion(
                FixedType.Mul(a.x.Raw, inv_norm),
                FixedType.Mul(a.y.Raw, inv_norm),
                FixedType.Mul(a.z.Raw, inv_norm),
                FixedType.Mul(a.w.Raw, inv_norm));
        }
        public static Quaternion NormalizeFastest(Quaternion a)
        {
            RawType inv_norm = REAL.RcpFastest(LengthFastest(a)).Raw;
            return new Quaternion(
                FixedType.Mul(a.x.Raw, inv_norm),
                FixedType.Mul(a.y.Raw, inv_norm),
                FixedType.Mul(a.z.Raw, inv_norm),
                FixedType.Mul(a.w.Raw, inv_norm));
        }

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, REAL t)
        {
            REAL epsilon = REAL.Ratio(1, 1000000);
            REAL cos_omega = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;

            bool flip = false;

            if (cos_omega < 0)
            {
                flip = true;
                cos_omega = -cos_omega;
            }

            REAL s1, s2;
            if (cos_omega > (REAL.One - epsilon))
            {
                s1 = REAL.One - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                REAL omega = REAL.AcosFastest(cos_omega);
                REAL inv_sin_omega = REAL.RcpFastest(REAL.SinFastest(omega));

                s1 = REAL.SinFastest((REAL.One - t) * omega) * inv_sin_omega;
                s2 = (flip)
                    ? -REAL.SinFastest(t * omega) * inv_sin_omega
                    : REAL.SinFastest(t * omega) * inv_sin_omega;
            }

            return new Quaternion(
                s1 * q1.x + s2 * q2.x,
                s1 * q1.y + s2 * q2.y,
                s1 * q1.z + s2 * q2.z,
                s1 * q1.w + s2 * q2.w);
        }

        public static Quaternion Lerp(Quaternion q1, Quaternion q2, REAL t)
        {
            REAL t1 = REAL.One - t;
            REAL dot = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;

            Quaternion r;
            if (dot >= 0)
                r = new Quaternion(
                    t1 * q1.x + t * q2.x,
                    t1 * q1.y + t * q2.y,
                    t1 * q1.z + t * q2.z,
                    t1 * q1.w + t * q2.w);
            else
                r = new Quaternion(
                    t1 * q1.x - t * q2.x,
                    t1 * q1.y - t * q2.y,
                    t1 * q1.z - t * q2.z,
                    t1 * q1.w - t * q2.w);

            return NormalizeFastest(r);
        }

        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            return Multiply(value2, value1);
        }

        public static Vector3d RotateVector(Quaternion rot, Vector3d v)
        {
            Vector3d u = new Vector3d(rot.x, rot.y, rot.z);
            REAL s = rot.w;

            return
                (REAL.Two * Vector3d.Dot(u, v)) * u +
                (s * s - Vector3d.Dot(u, u)) * v +
                (REAL.Two * s) * Vector3d.Cross(u, v);
        }

        public bool Equals(Quaternion other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion))
                return false;
            return ((Quaternion)obj) == this;
        }

        public override string ToString()
        {
            return "(" + FixedType.ToString(x.Raw) + ", " + FixedType.ToString(y.Raw) + ", " + FixedType.ToString(z.Raw) + ", " + FixedType.ToString(w.Raw) + ")";
        }

        public override int GetHashCode()
        {
            return x.Raw.GetHashCode() ^ (y.Raw.GetHashCode() * 7919) ^ (z.Raw.GetHashCode() * 4513) ^ (w.Raw.GetHashCode() * 8923);
        }

        public void Normalise()
        {
            RawType inv_norm = REAL.RcpFastest(LengthFastest(this)).Raw;
            x.Raw = FixedType.Mul(x.Raw, inv_norm);
            y.Raw = FixedType.Mul(y.Raw, inv_norm);
            z.Raw = FixedType.Mul(z.Raw, inv_norm);
            w.Raw = FixedType.Mul(w.Raw, inv_norm);
        }

        /// <summary>
        /// 用角速度变换旋转
        /// </summary>
        /// <param name="vector">角速度</param>
        /// <param name="scale">缩放</param>
        public void AddScaledVector(Vector3d vector, REAL scale)
        {
            Quaternion q = new Quaternion(FixedType.Mul(vector.x.Raw, scale.Raw),
                                          FixedType.Mul(vector.y.Raw, scale.Raw),
                                          FixedType.Mul(vector.z.Raw, scale.Raw),
                                          0);
            q = q * this;
            w.Raw += FixedType.Mul(q.w.Raw, REAL.Half.Raw);
            x.Raw += FixedType.Mul(q.x.Raw, REAL.Half.Raw);
            y.Raw += FixedType.Mul(q.y.Raw, REAL.Half.Raw);
            z.Raw += FixedType.Mul(q.z.Raw, REAL.Half.Raw);
        }
    }
}