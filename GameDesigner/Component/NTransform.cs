namespace Net.Component
{
    using global::System;
    using global::System.ComponentModel;
    using UnityEngine;
    using Matrix4x4 = Matrix4x4;
    using Quaternion = Quaternion;
    using Vector3 = Vector3;

    /// <summary>
    /// 游戏物体转换实体组建
    /// 作者:彼岸流年  QQ:317392507
    /// 后期修改:龙兄 QQ:1752062104
    /// </summary>
    [Serializable]
    public class NTransform
    {
        public Matrix4x4 matrix;
        public Vector3 position
        {
            get { return matrix.GetPosition(); }
            set
            {
                matrix = Matrix4Utils.GetPosition(value);
                matrix *= Matrix4x4.Rotate(rotation);
            }
        }
        public Quaternion rotation
        {
            get { return matrix.GetRotation(); }
            set
            {
                matrix = Matrix4Utils.GetPosition(position);
                matrix *= Matrix4x4.Rotate(value);
            }
        }

        public UnityEngine.Vector3 localScale
        {
            get { return matrix.GetScale(); }
            set { }
        }

        public UnityEngine.Quaternion localRotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public NTransform()
        {
            matrix = Matrix4Utils.GetPosition(Vector3.zero);
        }

        public NTransform(Vector3 position, Quaternion rotation)
        {
            matrix = Matrix4Utils.GetPosition(position);
            matrix *= Matrix4x4.Rotate(rotation);
        }

        public void Translate(float x, float y, float z)
        {
            Translate(new Vector3(x, y, z));
        }

        public void Translate(Vector3 direction)
        {
            matrix *= Matrix4x4.Translate(direction);
        }

        public Vector3 right
        {
            get
            {
                return rotation * Vector3.right;
            }
            set
            {
                rotation = Quaternion.FromToRotation(Vector3.right, value);
            }
        }

        public Vector3 up
        {
            get
            {
                return rotation * Vector3.up;
            }
            set
            {
                rotation = Quaternion.FromToRotation(Vector3.up, value);
            }
        }

        public Vector3 forward
        {
            get
            {
                return rotation * Vector3.forward;
            }
            set
            {
                rotation = Quaternion.LookRotation(value, Vector3.up);
            }
        }

        public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
        {
            if (relativeTo == Space.World)
            {
                position += translation;
            }
            else
            {
                matrix *= Matrix4x4.Translate(translation);
            }
        }

        public void Rotate(Vector3 eulers, [DefaultValue("Space.Self")] Space relativeTo)
        {
            Quaternion rhs = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
            if (relativeTo == Space.Self)
            {
                matrix *= Matrix4x4.Rotate(rhs);
            }
            else
            {
                rotation *= Quaternion.Inverse(rotation) * rhs * rotation;
            }
        }

        public void Rotate(Vector3 eulers)
        {
            Rotate(eulers, Space.Self);
        }

        public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
        }

        public void Rotate(float xAngle, float yAngle, float zAngle)
        {
            Rotate(new Vector3(xAngle, yAngle, zAngle), Space.Self);
        }

        public void LookAt(Vector3 position)
        {
            Vector3 v = (position - this.position).normalized;
            forward = v;
        }
    }
}