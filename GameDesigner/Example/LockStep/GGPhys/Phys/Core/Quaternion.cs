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

        public RawType RawX;
        public RawType RawY;
        public RawType RawZ;
        public RawType RawW;

        public REAL i { get { return REAL.FromRaw(RawX); } set { RawX = value.Raw; } }
        public REAL j { get { return REAL.FromRaw(RawY); } set { RawY = value.Raw; } }
        public REAL k { get { return REAL.FromRaw(RawZ); } set { RawZ = value.Raw; } }
        public REAL r { get { return REAL.FromRaw(RawW); } set { RawW = value.Raw; } }

        public Quaternion(REAL x, REAL y, REAL z, REAL w)
        {
            RawX = x.Raw;
            RawY = y.Raw;
            RawZ = z.Raw;
            RawW = w.Raw;
        }

        public Quaternion(Vector3d v, REAL w)
        {
            RawX = v.RawX;
            RawY = v.RawY;
            RawZ = v.RawZ;
            RawW = w.Raw;
        }

        private Quaternion(RawType x, RawType y, RawType z, RawType w)
        {
            RawX = x;
            RawY = y;
            RawZ = z;
            RawW = w;
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

        public static bool operator ==(Quaternion a, Quaternion b) { return a.RawX == b.RawX && a.RawY == b.RawY && a.RawZ == b.RawZ && a.RawW == b.RawW; }
        public static bool operator !=(Quaternion a, Quaternion b) { return a.RawX != b.RawX || a.RawY != b.RawY || a.RawZ != b.RawZ || a.RawW != b.RawW; }

        public static Quaternion Negate(Quaternion a) { return new Quaternion(-a.RawX, -a.RawY, -a.RawZ, -a.RawW); }
        public static Quaternion Conjugate(Quaternion a) { return new Quaternion(-a.RawX, -a.RawY, -a.RawZ, a.RawW); }
        public static Quaternion Inverse(Quaternion a)
        {
            RawType inv_norm = REAL.Rcp(LengthSqr(a)).Raw;
            return new Quaternion(
                -FixedType.Mul(a.RawX, inv_norm),
                -FixedType.Mul(a.RawY, inv_norm),
                -FixedType.Mul(a.RawZ, inv_norm),
                FixedType.Mul(a.RawW, inv_norm));
        }

        public static Quaternion InverseUnit(Quaternion a) { return new Quaternion(-a.RawX, -a.RawY, -a.RawZ, a.RawW); }

        public static Quaternion Multiply(Quaternion value1, Quaternion value2)
        {
            RawType q1x = value1.RawX;
            RawType q1y = value1.RawY;
            RawType q1z = value1.RawZ;
            RawType q1w = value1.RawW;

            RawType q2x = value2.RawX;
            RawType q2y = value2.RawY;
            RawType q2z = value2.RawZ;
            RawType q2w = value2.RawW;

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
        public static REAL LengthSqr(Quaternion a) { return REAL.FromRaw(FixedType.Mul(a.RawX, a.RawX) + FixedType.Mul(a.RawY, a.RawY) + FixedType.Mul(a.RawZ, a.RawZ) + FixedType.Mul(a.RawW, a.RawW)); }
        public static Quaternion Normalize(Quaternion a)
        {
            RawType inv_norm = REAL.Rcp(Length(a)).Raw;
            return new Quaternion(
                FixedType.Mul(a.RawX, inv_norm),
                FixedType.Mul(a.RawY, inv_norm),
                FixedType.Mul(a.RawZ, inv_norm),
                FixedType.Mul(a.RawW, inv_norm));
        }
        public static Quaternion NormalizeFast(Quaternion a)
        {
            RawType inv_norm = REAL.RcpFast(LengthFast(a)).Raw;
            return new Quaternion(
                FixedType.Mul(a.RawX, inv_norm),
                FixedType.Mul(a.RawY, inv_norm),
                FixedType.Mul(a.RawZ, inv_norm),
                FixedType.Mul(a.RawW, inv_norm));
        }
        public static Quaternion NormalizeFastest(Quaternion a)
        {
            RawType inv_norm = REAL.RcpFastest(LengthFastest(a)).Raw;
            return new Quaternion(
                FixedType.Mul(a.RawX, inv_norm),
                FixedType.Mul(a.RawY, inv_norm),
                FixedType.Mul(a.RawZ, inv_norm),
                FixedType.Mul(a.RawW, inv_norm));
        }

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, REAL t)
        {
            REAL epsilon = REAL.Ratio(1, 1000000);
            REAL cos_omega = q1.i * q2.i + q1.j * q2.j + q1.k * q2.k + q1.r * q2.r;

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
                s1 * q1.i + s2 * q2.i,
                s1 * q1.j + s2 * q2.j,
                s1 * q1.k + s2 * q2.k,
                s1 * q1.r + s2 * q2.r);
        }

        public static Quaternion Lerp(Quaternion q1, Quaternion q2, REAL t)
        {
            REAL t1 = REAL.One - t;
            REAL dot = q1.i * q2.i + q1.j * q2.j + q1.k * q2.k + q1.r * q2.r;

            Quaternion r;
            if (dot >= 0)
                r = new Quaternion(
                    t1 * q1.i + t * q2.i,
                    t1 * q1.j + t * q2.j,
                    t1 * q1.k + t * q2.k,
                    t1 * q1.r + t * q2.r);
            else
                r = new Quaternion(
                    t1 * q1.i - t * q2.i,
                    t1 * q1.j - t * q2.j,
                    t1 * q1.k - t * q2.k,
                    t1 * q1.r - t * q2.r);

            return NormalizeFastest(r);
        }

        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            return Multiply(value2, value1);
        }

        public static Vector3d RotateVector(Quaternion rot, Vector3d v)
        {
            Vector3d u = new Vector3d(rot.i, rot.j, rot.k);
            REAL s = rot.r;

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
            return "(" + FixedType.ToString(RawX) + ", " + FixedType.ToString(RawY) + ", " + FixedType.ToString(RawZ) + ", " + FixedType.ToString(RawW) + ")";
        }

        public override int GetHashCode()
        {
            return RawX.GetHashCode() ^ (RawY.GetHashCode() * 7919) ^ (RawZ.GetHashCode() * 4513) ^ (RawW.GetHashCode() * 8923);
        }

        public void Normalise()
        {
            RawType inv_norm = REAL.RcpFastest(LengthFastest(this)).Raw;
            RawX = FixedType.Mul(RawX, inv_norm);
            RawY = FixedType.Mul(RawY, inv_norm);
            RawZ = FixedType.Mul(RawZ, inv_norm);
            RawW = FixedType.Mul(RawW, inv_norm);
        }

        /// <summary>
        /// 用角速度变换旋转
        /// </summary>
        /// <param name="vector">角速度</param>
        /// <param name="scale">缩放</param>
        public void AddScaledVector(Vector3d vector, REAL scale)
        {
            Quaternion q = new Quaternion(FixedType.Mul(vector.RawX, scale.Raw),
                                          FixedType.Mul(vector.RawY, scale.Raw),
                                          FixedType.Mul(vector.RawZ, scale.Raw),
                                          0);
            q = q * this;
            RawW += FixedType.Mul(q.RawW, REAL.Half.Raw);
            RawX += FixedType.Mul(q.RawX, REAL.Half.Raw);
            RawY += FixedType.Mul(q.RawY, REAL.Half.Raw);
            RawZ += FixedType.Mul(q.RawZ, REAL.Half.Raw);
        }
    }
}