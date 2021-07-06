using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> how to serialize the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonObjectAttribute : JsonContainerAttribute
    {
        /// <summary>
        /// Gets or sets the member serialization.
        /// </summary>
        /// <value>The member serialization.</value>
        public MemberSerialization MemberSerialization
        {
            get
            {
                return _memberSerialization;
            }
            set
            {
                _memberSerialization = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the object's properties are required.
        /// </summary>
        /// <value>
        /// 	A value indicating whether the object's properties are required.
        /// </value>
        public Required ItemRequired
        {
            get
            {
                Required? itemRequired = _itemRequired;
                if (itemRequired == null)
                {
                    return Required.Default;
                }
                return itemRequired.GetValueOrDefault();
            }
            set
            {
                _itemRequired = new Required?(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonObjectAttribute" /> class.
        /// </summary>
        public JsonObjectAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonObjectAttribute" /> class with the specified member serialization.
        /// </summary>
        /// <param name="memberSerialization">The member serialization.</param>
        public JsonObjectAttribute(MemberSerialization memberSerialization)
        {
            MemberSerialization = memberSerialization;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonObjectAttribute" /> class with the specified container Id.
        /// </summary>
        /// <param name="id">The container Id.</param>
        public JsonObjectAttribute(string id) : base(id)
        {
        }

        private MemberSerialization _memberSerialization;

        internal Required? _itemRequired;
    }
}
