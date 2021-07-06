using Newtonsoft_X.Json.Linq;
using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public class JsonObjectContract : JsonContainerContract
    {
        /// <summary>
        /// Gets or sets the object member serialization.
        /// </summary>
        /// <value>The member object serialization.</value>
        public MemberSerialization MemberSerialization { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the object's properties are required.
        /// </summary>
        /// <value>
        /// 	A value indicating whether the object's properties are required.
        /// </value>
        public Required? ItemRequired { get; set; }

        /// <summary>
        /// Gets the object's properties.
        /// </summary>
        /// <value>The object's properties.</value>
        public JsonPropertyCollection Properties { get; private set; }

        [Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
        public JsonPropertyCollection ConstructorParameters
        {
            get
            {
                return CreatorParameters;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> instances that define the parameters used with <see cref="P:Newtonsoft.Json.Serialization.JsonObjectContract.OverrideCreator" />.
        /// </summary>
        public JsonPropertyCollection CreatorParameters
        {
            get
            {
                if (_creatorParameters == null)
                {
                    _creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
                }
                return _creatorParameters;
            }
        }

        [Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
        public ConstructorInfo OverrideConstructor
        {
            get
            {
                return _overrideConstructor;
            }
            set
            {
                _overrideConstructor = value;
                _overrideCreator = ((value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null);
            }
        }

        [Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
        public ConstructorInfo ParametrizedConstructor
        {
            get
            {
                return _parametrizedConstructor;
            }
            set
            {
                _parametrizedConstructor = value;
                _parameterizedCreator = ((value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null);
            }
        }

        /// <summary>
        /// Gets or sets the function used to create the object. When set this function will override <see cref="P:Newtonsoft.Json.Serialization.JsonContract.DefaultCreator" />.
        /// This function is called with a collection of arguments which are defined by the <see cref="P:Newtonsoft.Json.Serialization.JsonObjectContract.CreatorParameters" /> collection.
        /// </summary>
        /// <value>The function used to create the object.</value>
        public ObjectConstructor<object> OverrideCreator
        {
            get
            {
                return _overrideCreator;
            }
            set
            {
                _overrideCreator = value;
                _overrideConstructor = null;
            }
        }

        internal ObjectConstructor<object> ParameterizedCreator
        {
            get
            {
                return _parameterizedCreator;
            }
        }

        /// <summary>
        /// Gets or sets the extension data setter.
        /// </summary>
        public ExtensionDataSetter ExtensionDataSetter { get; set; }

        /// <summary>
        /// Gets or sets the extension data getter.
        /// </summary>
        public ExtensionDataGetter ExtensionDataGetter { get; set; }

        /// <summary>
        /// Gets or sets the extension data value type.
        /// </summary>
        public Type ExtensionDataValueType
        {
            get
            {
                return _extensionDataValueType;
            }
            set
            {
                _extensionDataValueType = value;
                ExtensionDataIsJToken = (value != null && typeof(JToken).IsAssignableFrom(value));
            }
        }

        internal bool HasRequiredOrDefaultValueProperties
        {
            get
            {
                if (_hasRequiredOrDefaultValueProperties == null)
                {
                    _hasRequiredOrDefaultValueProperties = new bool?(false);
                    if (ItemRequired.GetValueOrDefault(Required.Default) != Required.Default)
                    {
                        _hasRequiredOrDefaultValueProperties = new bool?(true);
                    }
                    else
                    {
                        foreach (JsonProperty jsonProperty in Properties)
                        {
                            if (jsonProperty.Required != Required.Default || (jsonProperty.DefaultValueHandling & DefaultValueHandling.Populate) == DefaultValueHandling.Populate)
                            {
                                _hasRequiredOrDefaultValueProperties = new bool?(true);
                                break;
                            }
                        }
                    }
                }
                return _hasRequiredOrDefaultValueProperties.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract" /> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonObjectContract(Type underlyingType) : base(underlyingType)
        {
            ContractType = JsonContractType.Object;
            Properties = new JsonPropertyCollection(base.UnderlyingType);
        }

        internal object GetUninitializedObject()
        {
            if (!JsonTypeReflector.FullyTrusted)
            {
                throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, NonNullableUnderlyingType));
            }
            return FormatterServices.GetUninitializedObject(NonNullableUnderlyingType);
        }

        internal bool ExtensionDataIsJToken;

        private bool? _hasRequiredOrDefaultValueProperties;

        private ConstructorInfo _parametrizedConstructor;

        private ConstructorInfo _overrideConstructor;

        private ObjectConstructor<object> _overrideCreator;

        private ObjectConstructor<object> _parameterizedCreator;

        private JsonPropertyCollection _creatorParameters;

        private Type _extensionDataValueType;
    }
}
