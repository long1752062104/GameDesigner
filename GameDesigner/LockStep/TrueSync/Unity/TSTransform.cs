using GGPhysUnity;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{
    /**
    *  @brief A deterministic version of Unity's Transform component for 3D physics. 
    **/
    [ExecuteInEditMode]
    public class TSTransform : MonoBehaviour
    {
        [SerializeField]
        //[HideInInspector]
        private TSVector3 _localPosition;

        /**
         *  @brief Property access to local position. 
         **/
        public TSVector3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
            }
        }

        [SerializeField]
        //[HideInInspector]

        private TSVector3 _position;
        internal TSVector3 _prevPosition;
        /**
        *  @brief Property access to position. 
        *  
        *  It works as proxy to a Body's position when there is a collider attached.
        **/
        public TSVector3 position
        {
            get
            {
                if (hasBody)
                    _position = rb.Body.Position;
                return _position;
            }
            set
            {
                _prevPosition = _position;
                _position = value;
                if (hasBody)
                    rb.Body.Position = _position;
                UpdateChildPosition();
            }
        }

        [SerializeField]
        //[HideInInspector]
        private TSQuaternion _localRotation;

        /**
         *  @brief Property access to local rotation. 
         **/
        public TSQuaternion localRotation
        {
            get
            {
                return _localRotation;
            }
            set
            {
                _localRotation = value;
                if (hasBody)
                    rb.SetOrientation(_localRotation);
                UpdateChildRotation();
            }
        }

        [SerializeField]
        //[HideInInspector]
        private TSQuaternion _rotation;

        /**
        *  @brief Property access to rotation. 
        *  
        *  It works as proxy to a Body's rotation when there is a collider attached.
        **/
        public TSQuaternion rotation
        {
            get
            {
                if (hasBody)
                    _rotation = rb.Body.Orientation;
                return _rotation;
            }
            set
            {
                _rotation = value;
                if (hasBody)
                    rb.SetOrientation(_rotation);
                UpdateChildRotation();
            }
        }

        [SerializeField]
        //[HideInInspector]

        private TSVector3 _scale;

        /**
        *  @brief Property access to scale. 
        **/
        public TSVector3 scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        [SerializeField]
        //[HideInInspector]

        private TSVector3 _localScale;

        /**
        *  @brief Property access to local scale. 
        **/
        public TSVector3 localScale
        {
            get
            {
                return _localScale;
            }
            set
            {
                _localScale = value;
            }
        }

        //[SerializeField]
        //[HideInInspector]
        private bool _serialized;

        public bool edit;

        /**
        *  @brief Rotates game object to point forward vector to a target position. 
        *  
        *  @param other TSTrasform used to get target position.
        **/
        public void LookAt(TSTransform other)
        {
            LookAt(other.position);
        }

        /**
        *  @brief Rotates game object to point forward vector to a target position. 
        *  
        *  @param target Target position.
        **/
        public void LookAt(TSVector3 target)
        {
            rotation = TSQuaternion.CreateFromMatrix(TSMatrix3.CreateFromLookAt(position, target));
        }

        /**
        *  @brief Moves game object based on provided axis values. 
        **/
        public void Translate(FP x, FP y, FP z)
        {
            Translate(x, y, z, Space.Self);
        }

        /**
        *  @brief Moves game object based on provided axis values and a relative space.
        *  
        *  If relative space is SELF then the game object will move based on its forward vector.
        **/
        public void Translate(FP x, FP y, FP z, Space relativeTo)
        {
            Translate(new TSVector3(x, y, z), relativeTo);
        }

        /**
        *  @brief Moves game object based on provided axis values and a relative {@link TSTransform}.
        *  
        *  The game object will move based on TSTransform's forward vector.
        **/
        public void Translate(FP x, FP y, FP z, TSTransform relativeTo)
        {
            Translate(new TSVector3(x, y, z), relativeTo);
        }

        /**
        *  @brief Moves game object based on provided translation vector.
        **/
        public void Translate(TSVector3 translation)
        {
            Translate(translation, Space.Self);
        }

        /**
        *  @brief Moves game object based on provided translation vector and a relative space.
        *  
        *  If relative space is SELF then the game object will move based on its forward vector.
        **/
        public void Translate(TSVector3 translation, Space relativeTo)
        {
            if (relativeTo == Space.Self)
            {
                Translate(translation, this);
            }
            else
            {
                position += translation;
            }
        }

        /**
        *  @brief Moves game object based on provided translation vector and a relative {@link TSTransform}.
        *  
        *  The game object will move based on TSTransform's forward vector.
        **/
        public void Translate(TSVector3 translation, TSTransform relativeTo)
        {
            position += TSVector3.Transform(translation, TSMatrix3.CreateFromQuaternion(relativeTo.rotation));
        }

        /**
        *  @brief Rotates game object based on provided axis, point and angle of rotation.
        **/
        public void RotateAround(TSVector3 point, TSVector3 axis, FP angle)
        {
            TSVector3 vector = position;
            TSVector3 vector2 = vector - point;
            vector2 = TSVector3.Transform(vector2, TSMatrix3.AngleAxis(angle * FP.Deg2Rad, axis));
            vector = point + vector2;
            position = vector;

            Rotate(axis, angle);
        }

        /**
        *  @brief Rotates game object based on provided axis and angle of rotation.
        **/
        public void RotateAround(TSVector3 axis, FP angle)
        {
            Rotate(axis, angle);
        }

        /**
        *  @brief Rotates game object based on provided axis angles of rotation.
        **/
        public void Rotate(FP xAngle, FP yAngle, FP zAngle)
        {
            Rotate(new TSVector3(xAngle, yAngle, zAngle), Space.Self);
        }

        /**
        *  @brief Rotates game object based on provided axis angles of rotation and a relative space.
        *  
        *  If relative space is SELF then the game object will rotate based on its forward vector.
        **/
        public void Rotate(FP xAngle, FP yAngle, FP zAngle, Space relativeTo)
        {
            Rotate(new TSVector3(xAngle, yAngle, zAngle), relativeTo);
        }

        /**
        *  @brief Rotates game object based on provided axis angles of rotation.
        **/
        public void Rotate(TSVector3 eulerAngles)
        {
            Rotate(eulerAngles, Space.Self);
        }

        /**
        *  @brief Rotates game object based on provided axis and angle of rotation.
        **/
        public void Rotate(TSVector3 axis, FP angle)
        {
            Rotate(axis, angle, Space.Self);
        }

        /**
        *  @brief Rotates game object based on provided axis, angle of rotation and relative space.
        *  
        *  If relative space is SELF then the game object will rotate based on its forward vector.
        **/
        public void Rotate(TSVector3 axis, FP angle, Space relativeTo)
        {
            TSQuaternion result = TSQuaternion.identity;

            if (relativeTo == Space.Self)
            {
                result = rotation * TSQuaternion.AngleAxis(angle, axis);
            }
            else
            {
                result = TSQuaternion.AngleAxis(angle, axis) * rotation;
            }

            result.Normalize();
            rotation = result;
        }

        /**
        *  @brief Rotates game object based on provided axis angles and relative space.
        *  
        *  If relative space is SELF then the game object will rotate based on its forward vector.
        **/
        public void Rotate(TSVector3 eulerAngles, Space relativeTo)
        {
            TSQuaternion rhs = TSQuaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            bool flag = relativeTo == Space.Self;
            if (!flag)
            {
                localRotation *= rhs;
            }
            else
            {
                rotation *= TSQuaternion.Inverse(rotation) * rhs * rotation;
            }
        }

        /**
        *  @brief Current self forward vector.
        **/
        public TSVector3 forward
        {
            get
            {
                return TSVector3.Transform(TSVector3.forward, TSMatrix3.CreateFromQuaternion(rotation));
            }
        }

        /**
        *  @brief Current self right vector.
        **/
        public TSVector3 right
        {
            get
            {
                return TSVector3.Transform(TSVector3.right, TSMatrix3.CreateFromQuaternion(rotation));
            }
        }

        /**
        *  @brief Current self up vector.
        **/
        public TSVector3 up
        {
            get
            {
                return TSVector3.Transform(TSVector3.up, TSMatrix3.CreateFromQuaternion(rotation));
            }
        }

        /**
        *  @brief Returns Euler angles in degrees.
        **/
        public TSVector3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
        }

        public TSMatrix4 localToWorldMatrix
        {
            get
            {
                TSTransform thisTransform = this;
                TSMatrix4 curMatrix = TSMatrix4.TransformToMatrix1(ref thisTransform);
                TSTransform parent = tsParent;
                while (parent != null)
                {
                    curMatrix = TSMatrix4.TransformToMatrix1(ref parent) * curMatrix;
                    parent = parent.tsParent;
                }
                return curMatrix;
            }
        }

        public TSMatrix4 worldToLocalMatrix
        {
            get
            {
                return TSMatrix4.Inverse(localToWorldMatrix);
            }
        }

        /**
         *  @brief Transform a point from local space to world space.
         **/
        public TSVector4 TransformPoint(TSVector4 point)
        {
            Debug.Assert(point.w == FP.One);
            return TSVector4.Transform(point, localToWorldMatrix);
        }

        public TSVector3 TransformPoint(FP x, FP y, FP z)
        {
            return TransformPoint(new TSVector3(x, y, z));
        }

        public TSVector3 TransformPoint(TSVector3 point)
        {
            return TSVector4.Transform(point, localToWorldMatrix).ToTSVector();
        }

        /**
         *  @brief Transform a point from world space to local space.
         **/
        public TSVector4 InverseTransformPoint(TSVector4 point)
        {
            Debug.Assert(point.w == FP.One);
            return TSVector4.Transform(point, worldToLocalMatrix);
        }

        public TSVector3 InverseTransformPoint(TSVector3 point)
        {
            return TSVector4.Transform(point, worldToLocalMatrix).ToTSVector();
        }

        /**
         *  @brief Transform a direction from local space to world space.
         **/
        public TSVector4 TransformDirection(TSVector4 direction)
        {
            Debug.Assert(direction.w == FP.Zero);
            TSMatrix4 matrix = TSMatrix4.Translate(position) * TSMatrix4.Rotate(rotation);
            return TSVector4.Transform(direction, matrix);
        }

        public TSVector3 TransformDirection(TSVector3 direction)
        {
            return TransformDirection(new TSVector4(direction.x, direction.y, direction.z, FP.Zero)).ToTSVector();
        }

        /**
         *  @brief Transform a direction from world space to local space.
         **/
        public TSVector4 InverseTransformDirection(TSVector4 direction)
        {
            Debug.Assert(direction.w == FP.Zero);
            TSMatrix4 matrix = TSMatrix4.Translate(position) * TSMatrix4.Rotate(rotation);
            return TSVector4.Transform(direction, TSMatrix4.Inverse(matrix));
        }

        public TSVector3 InverseTransformDirection(TSVector3 direction)
        {
            return InverseTransformDirection(new TSVector4(direction.x, direction.y, direction.z, FP.Zero)).ToTSVector();
        }

        /**
         *  @brief Transform a vector from local space to world space.
         **/
        public TSVector4 TransformVector(TSVector4 vector)
        {
            Debug.Assert(vector.w == FP.Zero);
            return TSVector4.Transform(vector, localToWorldMatrix);
        }

        public TSVector3 TransformVector(TSVector3 vector)
        {
            return TransformVector(new TSVector4(vector.x, vector.y, vector.z, FP.Zero)).ToTSVector();
        }

        /**
         *  @brief Transform a vector from world space to local space.
         **/
        public TSVector4 InverseTransformVector(TSVector4 vector)
        {
            Debug.Assert(vector.w == FP.Zero);
            return TSVector4.Transform(vector, worldToLocalMatrix);
        }

        public TSVector3 InverseTransformVector(TSVector3 vector)
        {
            return InverseTransformVector(new TSVector4(vector.x, vector.y, vector.z, FP.Zero)).ToTSVector();
        }

        [HideInInspector]
        public BCollider tsCollider;

        [HideInInspector]
        public TSTransform tsParent;

        internal List<TSTransform> tsChildren = new List<TSTransform>();

        [HideInInspector]
        public bool initialized = false;

        [HideInInspector]
        public BRigidBody rb;

        public bool autoSyncTransform = true;
        internal bool hasBody;

        private void Awake()
        {
            if (!Application.isPlaying)
                return;
            Initialize();
        }

        /**
        *  @brief Initializes internal properties based on whether there is a {@link TSCollider} attached.
        **/
        public void Initialize()
        {
            if (initialized)
                return;
            tsCollider = GetComponent<BCollider>();
            rb = GetComponent<BRigidBody>();
            if (transform.parent != null)
                tsParent = transform.parent.GetComponent<TSTransform>();
            _prevPosition = transform.position;
            foreach (Transform child in transform)
            {
                TSTransform tsChild = child.GetComponent<TSTransform>();
                if (tsChild != null)
                {
                    tsChild.Initialize();
                    tsChildren.Add(tsChild);
                }
            }

            if (!_serialized)
                UpdateEditMode();

            if (tsCollider != null & rb != null)
            {
                if (rb.Body != null)
                {
                    rb.Body.Position = _position;
                    rb.Body.Orientation = _rotation;
                    hasBody = true;
                }
            }

            initialized = true;
            enabled = autoSyncTransform;
        }

        public void Update()
        {
            if (Application.isPlaying)
            {
                if (initialized & autoSyncTransform)
                {
                    UpdatePlayMode();
                }
            }
            else
            {
                UpdateEditMode();
            }
        }

        private void UpdateEditMode()
        {
            if (transform.hasChanged)
            {
                _position = transform.position;
                _rotation = transform.rotation;
                _scale = transform.lossyScale;

                _localPosition = transform.localPosition;
                _localRotation = transform.localRotation;
                _localScale = transform.localScale;

                _prevPosition = transform.position;

                _serialized = true;
            }
        }

        private void UpdatePlayMode()
        {
            if (!edit)
            {
                transform.position = position;
                transform.rotation = rotation;
                transform.localScale = localScale;
            }
            else
            {
                position = transform.position;
                rotation = transform.rotation;
                localScale = transform.localScale;
            }
        }

        private void UpdateChildPosition()
        {
            for (int i = 0; i < tsChildren.Count; i++)
            {
                TSTransform child = tsChildren[i];
                //tsChildren[i].Translate(_position - _prevPosition);
                child.position = _position + child.localPosition;//.position + child.localPosition;
            }
        }

        private void UpdateChildRotation()
        {
            for (int i = 0; i < tsChildren.Count; i++)
            {
                TSTransform child = tsChildren[i];
                child.position = TransformPoint(child.localPosition);
                child.rotation = _rotation;
            }
        }

        public override string ToString()
        {
            return $"{name} pos:{position}, rot:{rotation}, size:{localScale}";
        }

        public void Reset()
        {
            position = transform.position;
            rotation = transform.rotation;
            localScale = transform.localScale;
        }
    }
}