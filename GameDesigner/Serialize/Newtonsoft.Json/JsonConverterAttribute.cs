using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> to use the specified <see cref="T:Newtonsoft.Json.JsonConverter" /> when serializing the member or class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class JsonConverterAttribute : Attribute
    {
        /// <summary>
        /// Gets the <see cref="T:System.Type" /> of the <see cref="T:Newtonsoft.Json.JsonConverter" />.
        /// </summary>
        /// <value>The <see cref="T:System.Type" /> of the <see cref="T:Newtonsoft.Json.JsonConverter" />.</value>
        public Type ConverterType
        {
            get
            {
                return _converterType;
            }
        }

        /// <summary>
        /// The parameter list to use when constructing the <see cref="T:Newtonsoft.Json.JsonConverter" /> described by <see cref="P:Newtonsoft.Json.JsonConverterAttribute.ConverterType" />.
        /// If <c>null</c>, the default constructor is used.
        /// </summary>
        public object[] ConverterParameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonConverterAttribute" /> class.
        /// </summary>
        /// <param name="converterType">Type of the <see cref="T:Newtonsoft.Json.JsonConverter" />.</param>
        public JsonConverterAttribute(Type converterType)
        {
            if (converterType == null)
            {
                throw new ArgumentNullException("converterType");
            }
            _converterType = converterType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonConverterAttribute" /> class.
        /// </summary>
        /// <param name="converterType">Type of the <see cref="T:Newtonsoft.Json.JsonConverter" />.</param>
        /// <param name="converterParameters">Parameter list to use when constructing the <see cref="T:Newtonsoft.Json.JsonConverter" />. Can be <c>null</c>.</param>
        public JsonConverterAttribute(Type converterType, params object[] converterParameters) : this(converterType)
        {
            ConverterParameters = converterParameters;
        }

        private readonly Type _converterType;
    }
}
