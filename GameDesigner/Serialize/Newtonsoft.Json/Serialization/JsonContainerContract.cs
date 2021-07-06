using Newtonsoft_X.Json.Utilities;
using System;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public class JsonContainerContract : JsonContract
    {
        internal JsonContract ItemContract
        {
            get
            {
                return _itemContract;
            }
            set
            {
                _itemContract = value;
                if (_itemContract != null)
                {
                    _finalItemContract = (_itemContract.UnderlyingType.IsSealed() ? _itemContract : null);
                    return;
                }
                _finalItemContract = null;
            }
        }

        internal JsonContract FinalItemContract
        {
            get
            {
                return _finalItemContract;
            }
        }

        /// <summary>
        /// Gets or sets the default collection items <see cref="T:Newtonsoft.Json.JsonConverter" />.
        /// </summary>
        /// <value>The converter.</value>
        public JsonConverter ItemConverter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the collection items preserve object references.
        /// </summary>
        /// <value><c>true</c> if collection items preserve object references; otherwise, <c>false</c>.</value>
        public bool? ItemIsReference { get; set; }

        /// <summary>
        /// Gets or sets the collection item reference loop handling.
        /// </summary>
        /// <value>The reference loop handling.</value>
        public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

        /// <summary>
        /// Gets or sets the collection item type name handling.
        /// </summary>
        /// <value>The type name handling.</value>
        public TypeNameHandling? ItemTypeNameHandling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonContainerContract" /> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        internal JsonContainerContract(Type underlyingType) : base(underlyingType)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(underlyingType);
            if (cachedAttribute != null)
            {
                if (cachedAttribute.ItemConverterType != null)
                {
                    ItemConverter = JsonTypeReflector.CreateJsonConverterInstance(cachedAttribute.ItemConverterType, cachedAttribute.ItemConverterParameters);
                }
                ItemIsReference = cachedAttribute._itemIsReference;
                ItemReferenceLoopHandling = cachedAttribute._itemReferenceLoopHandling;
                ItemTypeNameHandling = cachedAttribute._itemTypeNameHandling;
            }
        }

        private JsonContract _itemContract;

        private JsonContract _finalItemContract;
    }
}
