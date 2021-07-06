using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public abstract class JsonContract
    {
        /// <summary>
        /// Gets the underlying type for the contract.
        /// </summary>
        /// <value>The underlying type for the contract.</value>
        public Type UnderlyingType { get; private set; }

        /// <summary>
        /// Gets or sets the type created during deserialization.
        /// </summary>
        /// <value>The type created during deserialization.</value>
        public Type CreatedType
        {
            get
            {
                return _createdType;
            }
            set
            {
                _createdType = value;
                IsSealed = _createdType.IsSealed();
                IsInstantiable = (!_createdType.IsInterface() && !_createdType.IsAbstract());
            }
        }

        /// <summary>
        /// Gets or sets whether this type contract is serialized as a reference.
        /// </summary>
        /// <value>Whether this type contract is serialized as a reference.</value>
        public bool? IsReference { get; set; }

        /// <summary>
        /// Gets or sets the default <see cref="T:Newtonsoft.Json.JsonConverter" /> for this contract.
        /// </summary>
        /// <value>The converter.</value>
        public JsonConverter Converter { get; set; }

        /// <summary>
        /// Gets the internally resolved <see cref="T:Newtonsoft.Json.JsonConverter" /> for the contract's type.
        /// This converter is used as a fallback converter when no other converter is resolved.
        /// Setting <see cref="P:Newtonsoft.Json.Serialization.JsonContract.Converter" /> will always override this converter.
        /// </summary>
        internal JsonConverter InternalConverter { get; set; }

        /// <summary>
        /// Gets or sets all methods called immediately after deserialization of the object.
        /// </summary>
        /// <value>The methods called immediately after deserialization of the object.</value>
        public IList<SerializationCallback> OnDeserializedCallbacks
        {
            get
            {
                if (_onDeserializedCallbacks == null)
                {
                    _onDeserializedCallbacks = new List<SerializationCallback>();
                }
                return _onDeserializedCallbacks;
            }
        }

        /// <summary>
        /// Gets or sets all methods called during deserialization of the object.
        /// </summary>
        /// <value>The methods called during deserialization of the object.</value>
        public IList<SerializationCallback> OnDeserializingCallbacks
        {
            get
            {
                if (_onDeserializingCallbacks == null)
                {
                    _onDeserializingCallbacks = new List<SerializationCallback>();
                }
                return _onDeserializingCallbacks;
            }
        }

        /// <summary>
        /// Gets or sets all methods called after serialization of the object graph.
        /// </summary>
        /// <value>The methods called after serialization of the object graph.</value>
        public IList<SerializationCallback> OnSerializedCallbacks
        {
            get
            {
                if (_onSerializedCallbacks == null)
                {
                    _onSerializedCallbacks = new List<SerializationCallback>();
                }
                return _onSerializedCallbacks;
            }
        }

        /// <summary>
        /// Gets or sets all methods called before serialization of the object.
        /// </summary>
        /// <value>The methods called before serialization of the object.</value>
        public IList<SerializationCallback> OnSerializingCallbacks
        {
            get
            {
                if (_onSerializingCallbacks == null)
                {
                    _onSerializingCallbacks = new List<SerializationCallback>();
                }
                return _onSerializingCallbacks;
            }
        }

        /// <summary>
        /// Gets or sets all method called when an error is thrown during the serialization of the object.
        /// </summary>
        /// <value>The methods called when an error is thrown during the serialization of the object.</value>
        public IList<SerializationErrorCallback> OnErrorCallbacks
        {
            get
            {
                if (_onErrorCallbacks == null)
                {
                    _onErrorCallbacks = new List<SerializationErrorCallback>();
                }
                return _onErrorCallbacks;
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnDeserializedCallbacks collection.")]
        public MethodInfo OnDeserialized
        {
            get
            {
                if (OnDeserializedCallbacks.Count <= 0)
                {
                    return null;
                }
                return OnDeserializedCallbacks[0].Method();
            }
            set
            {
                OnDeserializedCallbacks.Clear();
                OnDeserializedCallbacks.Add(JsonContract.CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnDeserializingCallbacks collection.")]
        public MethodInfo OnDeserializing
        {
            get
            {
                if (OnDeserializingCallbacks.Count <= 0)
                {
                    return null;
                }
                return OnDeserializingCallbacks[0].Method();
            }
            set
            {
                OnDeserializingCallbacks.Clear();
                OnDeserializingCallbacks.Add(JsonContract.CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnSerializedCallbacks collection.")]
        public MethodInfo OnSerialized
        {
            get
            {
                if (OnSerializedCallbacks.Count <= 0)
                {
                    return null;
                }
                return OnSerializedCallbacks[0].Method();
            }
            set
            {
                OnSerializedCallbacks.Clear();
                OnSerializedCallbacks.Add(JsonContract.CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnSerializingCallbacks collection.")]
        public MethodInfo OnSerializing
        {
            get
            {
                if (OnSerializingCallbacks.Count <= 0)
                {
                    return null;
                }
                return OnSerializingCallbacks[0].Method();
            }
            set
            {
                OnSerializingCallbacks.Clear();
                OnSerializingCallbacks.Add(JsonContract.CreateSerializationCallback(value));
            }
        }

        [Obsolete("This property is obsolete and has been replaced by the OnErrorCallbacks collection.")]
        public MethodInfo OnError
        {
            get
            {
                if (OnErrorCallbacks.Count <= 0)
                {
                    return null;
                }
                return OnErrorCallbacks[0].Method();
            }
            set
            {
                OnErrorCallbacks.Clear();
                OnErrorCallbacks.Add(JsonContract.CreateSerializationErrorCallback(value));
            }
        }

        /// <summary>
        /// Gets or sets the default creator method used to create the object.
        /// </summary>
        /// <value>The default creator method used to create the object.</value>
        public Func<object> DefaultCreator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the default creator is non-public.
        /// </summary>
        /// <value><c>true</c> if the default object creator is non-public; otherwise, <c>false</c>.</value>
        public bool DefaultCreatorNonPublic { get; set; }

        internal JsonContract(Type underlyingType)
        {
            ValidationUtils.ArgumentNotNull(underlyingType, "underlyingType");
            UnderlyingType = underlyingType;
            IsNullable = ReflectionUtils.IsNullable(underlyingType);
            NonNullableUnderlyingType = ((IsNullable && ReflectionUtils.IsNullableType(underlyingType)) ? Nullable.GetUnderlyingType(underlyingType) : underlyingType);
            CreatedType = NonNullableUnderlyingType;
            IsConvertable = ConvertUtils.IsConvertible(NonNullableUnderlyingType);
            IsEnum = NonNullableUnderlyingType.IsEnum();
            InternalReadType = ReadType.Read;
        }

        internal void InvokeOnSerializing(object o, StreamingContext context)
        {
            if (_onSerializingCallbacks != null)
            {
                foreach (SerializationCallback serializationCallback in _onSerializingCallbacks)
                {
                    serializationCallback(o, context);
                }
            }
        }

        internal void InvokeOnSerialized(object o, StreamingContext context)
        {
            if (_onSerializedCallbacks != null)
            {
                foreach (SerializationCallback serializationCallback in _onSerializedCallbacks)
                {
                    serializationCallback(o, context);
                }
            }
        }

        internal void InvokeOnDeserializing(object o, StreamingContext context)
        {
            if (_onDeserializingCallbacks != null)
            {
                foreach (SerializationCallback serializationCallback in _onDeserializingCallbacks)
                {
                    serializationCallback(o, context);
                }
            }
        }

        internal void InvokeOnDeserialized(object o, StreamingContext context)
        {
            if (_onDeserializedCallbacks != null)
            {
                foreach (SerializationCallback serializationCallback in _onDeserializedCallbacks)
                {
                    serializationCallback(o, context);
                }
            }
        }

        internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
        {
            if (_onErrorCallbacks != null)
            {
                foreach (SerializationErrorCallback serializationErrorCallback in _onErrorCallbacks)
                {
                    serializationErrorCallback(o, context, errorContext);
                }
            }
        }

        internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
        {
            return delegate (object o, StreamingContext context)
            {
                callbackMethodInfo.Invoke(o, new object[]
                {
                    context
                });
            };
        }

        internal static SerializationErrorCallback CreateSerializationErrorCallback(MethodInfo callbackMethodInfo)
        {
            return delegate (object o, StreamingContext context, ErrorContext econtext)
            {
                callbackMethodInfo.Invoke(o, new object[]
                {
                    context,
                    econtext
                });
            };
        }

        internal bool IsNullable;

        internal bool IsConvertable;

        internal bool IsEnum;

        internal Type NonNullableUnderlyingType;

        internal ReadType InternalReadType;

        internal JsonContractType ContractType;

        internal bool IsReadOnlyOrFixedSize;

        internal bool IsSealed;

        internal bool IsInstantiable;

        private List<SerializationCallback> _onDeserializedCallbacks;

        private IList<SerializationCallback> _onDeserializingCallbacks;

        private IList<SerializationCallback> _onSerializedCallbacks;

        private IList<SerializationCallback> _onSerializingCallbacks;

        private IList<SerializationErrorCallback> _onErrorCallbacks;

        private Type _createdType;
    }
}
