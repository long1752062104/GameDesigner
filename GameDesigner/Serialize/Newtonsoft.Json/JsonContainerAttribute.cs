using Newtonsoft_X.Json.Serialization;
using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> how to serialize the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public abstract class JsonContainerAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the collection's items converter.
        /// </summary>
        /// <value>The collection's items converter.</value>
        public Type ItemConverterType { get; set; }

        /// <summary>
        /// The parameter list to use when constructing the <see cref="T:Newtonsoft.Json.JsonConverter" /> described by <see cref="P:Newtonsoft.Json.JsonContainerAttribute.ItemConverterType" />.
        /// If <c>null</c>, the default constructor is used.
        /// When non-<c>null</c>, there must be a constructor defined in the <see cref="T:Newtonsoft.Json.JsonConverter" /> that exactly matches the number,
        /// order, and type of these parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// [JsonContainer(ItemConverterType = typeof(MyContainerConverter), ItemConverterParameters = new object[] { 123, "Four" })]
        /// </code>
        /// </example>
        public object[] ItemConverterParameters { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Type" /> of the <see cref="T:Newtonsoft.Json.Serialization.NamingStrategy" />.
        /// </summary>
        /// <value>The <see cref="T:System.Type" /> of the <see cref="T:Newtonsoft.Json.Serialization.NamingStrategy" />.</value>
        public Type NamingStrategyType
        {
            get
            {
                return _namingStrategyType;
            }
            set
            {
                _namingStrategyType = value;
                NamingStrategyInstance = null;
            }
        }

        /// <summary>
        /// The parameter list to use when constructing the <see cref="T:Newtonsoft.Json.Serialization.NamingStrategy" /> described by <see cref="P:Newtonsoft.Json.JsonContainerAttribute.NamingStrategyType" />.
        /// If <c>null</c>, the default constructor is used.
        /// When non-<c>null</c>, there must be a constructor defined in the <see cref="T:Newtonsoft.Json.Serialization.NamingStrategy" /> that exactly matches the number,
        /// order, and type of these parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// [JsonContainer(NamingStrategyType = typeof(MyNamingStrategy), NamingStrategyParameters = new object[] { 123, "Four" })]
        /// </code>
        /// </example>
        public object[] NamingStrategyParameters
        {
            get
            {
                return _namingStrategyParameters;
            }
            set
            {
                _namingStrategyParameters = value;
                NamingStrategyInstance = null;
            }
        }

        internal NamingStrategy NamingStrategyInstance { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to preserve object references.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to keep object reference; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool IsReference
        {
            get
            {
                return _isReference ?? false;
            }
            set
            {
                _isReference = new bool?(value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to preserve collection's items references.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to keep collection's items object references; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool ItemIsReference
        {
            get
            {
                return _itemIsReference ?? false;
            }
            set
            {
                _itemIsReference = new bool?(value);
            }
        }

        /// <summary>
        /// Gets or sets the reference loop handling used when serializing the collection's items.
        /// </summary>
        /// <value>The reference loop handling.</value>
        public ReferenceLoopHandling ItemReferenceLoopHandling
        {
            get
            {
                ReferenceLoopHandling? itemReferenceLoopHandling = _itemReferenceLoopHandling;
                if (itemReferenceLoopHandling == null)
                {
                    return ReferenceLoopHandling.Error;
                }
                return itemReferenceLoopHandling.GetValueOrDefault();
            }
            set
            {
                _itemReferenceLoopHandling = new ReferenceLoopHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets the type name handling used when serializing the collection's items.
        /// </summary>
        /// <value>The type name handling.</value>
        public TypeNameHandling ItemTypeNameHandling
        {
            get
            {
                TypeNameHandling? itemTypeNameHandling = _itemTypeNameHandling;
                if (itemTypeNameHandling == null)
                {
                    return TypeNameHandling.None;
                }
                return itemTypeNameHandling.GetValueOrDefault();
            }
            set
            {
                _itemTypeNameHandling = new TypeNameHandling?(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonContainerAttribute" /> class.
        /// </summary>
        protected JsonContainerAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonContainerAttribute" /> class with the specified container Id.
        /// </summary>
        /// <param name="id">The container Id.</param>
        protected JsonContainerAttribute(string id)
        {
            Id = id;
        }

        internal bool? _isReference;

        internal bool? _itemIsReference;

        internal ReferenceLoopHandling? _itemReferenceLoopHandling;

        internal TypeNameHandling? _itemTypeNameHandling;

        private Type _namingStrategyType;

        private object[] _namingStrategyParameters;
    }
}
