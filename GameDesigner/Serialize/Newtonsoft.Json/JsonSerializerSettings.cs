using Newtonsoft_X.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies the settings on a <see cref="T:Newtonsoft.Json.JsonSerializer" /> object.
    /// </summary>
    public class JsonSerializerSettings
    {
        /// <summary>
        /// Gets or sets how reference loops (e.g. a class referencing itself) are handled.
        /// The default value is <see cref="F:Newtonsoft.Json.ReferenceLoopHandling.Error" />.
        /// </summary>
        /// <value>Reference loop handling.</value>
        public ReferenceLoopHandling ReferenceLoopHandling
        {
            get
            {
                ReferenceLoopHandling? referenceLoopHandling = _referenceLoopHandling;
                if (referenceLoopHandling == null)
                {
                    return ReferenceLoopHandling.Error;
                }
                return referenceLoopHandling.GetValueOrDefault();
            }
            set
            {
                _referenceLoopHandling = new ReferenceLoopHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.MissingMemberHandling.Ignore" />.
        /// </summary>
        /// <value>Missing member handling.</value>
        public MissingMemberHandling MissingMemberHandling
        {
            get
            {
                MissingMemberHandling? missingMemberHandling = _missingMemberHandling;
                if (missingMemberHandling == null)
                {
                    return MissingMemberHandling.Ignore;
                }
                return missingMemberHandling.GetValueOrDefault();
            }
            set
            {
                _missingMemberHandling = new MissingMemberHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how objects are created during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.ObjectCreationHandling.Auto" />.
        /// </summary>
        /// <value>The object creation handling.</value>
        public ObjectCreationHandling ObjectCreationHandling
        {
            get
            {
                ObjectCreationHandling? objectCreationHandling = _objectCreationHandling;
                if (objectCreationHandling == null)
                {
                    return ObjectCreationHandling.Auto;
                }
                return objectCreationHandling.GetValueOrDefault();
            }
            set
            {
                _objectCreationHandling = new ObjectCreationHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how null values are handled during serialization and deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.NullValueHandling.Include" />.
        /// </summary>
        /// <value>Null value handling.</value>
        public NullValueHandling NullValueHandling
        {
            get
            {
                NullValueHandling? nullValueHandling = _nullValueHandling;
                if (nullValueHandling == null)
                {
                    return NullValueHandling.Include;
                }
                return nullValueHandling.GetValueOrDefault();
            }
            set
            {
                _nullValueHandling = new NullValueHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how default values are handled during serialization and deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.DefaultValueHandling.Include" />.
        /// </summary>
        /// <value>The default value handling.</value>
        public DefaultValueHandling DefaultValueHandling
        {
            get
            {
                DefaultValueHandling? defaultValueHandling = _defaultValueHandling;
                if (defaultValueHandling == null)
                {
                    return DefaultValueHandling.Include;
                }
                return defaultValueHandling.GetValueOrDefault();
            }
            set
            {
                _defaultValueHandling = new DefaultValueHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Newtonsoft.Json.JsonConverter" /> collection that will be used during serialization.
        /// </summary>
        /// <value>The converters.</value>
        public IList<JsonConverter> Converters { get; set; }

        /// <summary>
        /// Gets or sets how object references are preserved by the serializer.
        /// The default value is <see cref="F:Newtonsoft.Json.PreserveReferencesHandling.None" />.
        /// </summary>
        /// <value>The preserve references handling.</value>
        public PreserveReferencesHandling PreserveReferencesHandling
        {
            get
            {
                PreserveReferencesHandling? preserveReferencesHandling = _preserveReferencesHandling;
                if (preserveReferencesHandling == null)
                {
                    return PreserveReferencesHandling.None;
                }
                return preserveReferencesHandling.GetValueOrDefault();
            }
            set
            {
                _preserveReferencesHandling = new PreserveReferencesHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how type name writing and reading is handled by the serializer.
        /// The default value is <see cref="F:Newtonsoft.Json.TypeNameHandling.None" />.
        /// </summary>
        /// <remarks>
        /// <see cref="P:Newtonsoft.Json.JsonSerializerSettings.TypeNameHandling" /> should be used with caution when your application deserializes JSON from an external source.
        /// Incoming types should be validated with a custom <see cref="P:Newtonsoft.Json.JsonSerializerSettings.SerializationBinder" />
        /// when deserializing with a value other than <see cref="F:Newtonsoft.Json.TypeNameHandling.None" />.
        /// </remarks>
        /// <value>The type name handling.</value>
        public TypeNameHandling TypeNameHandling
        {
            get
            {
                TypeNameHandling? typeNameHandling = _typeNameHandling;
                if (typeNameHandling == null)
                {
                    return TypeNameHandling.None;
                }
                return typeNameHandling.GetValueOrDefault();
            }
            set
            {
                _typeNameHandling = new TypeNameHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how metadata properties are used during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.MetadataPropertyHandling.Default" />.
        /// </summary>
        /// <value>The metadata properties handling.</value>
        public MetadataPropertyHandling MetadataPropertyHandling
        {
            get
            {
                MetadataPropertyHandling? metadataPropertyHandling = _metadataPropertyHandling;
                if (metadataPropertyHandling == null)
                {
                    return MetadataPropertyHandling.Default;
                }
                return metadataPropertyHandling.GetValueOrDefault();
            }
            set
            {
                _metadataPropertyHandling = new MetadataPropertyHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how a type name assembly is written and resolved by the serializer.
        /// The default value is <see cref="F:System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple" />.
        /// </summary>
        /// <value>The type name assembly format.</value>
        public FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get
            {
                FormatterAssemblyStyle? typeNameAssemblyFormat = _typeNameAssemblyFormat;
                if (typeNameAssemblyFormat == null)
                {
                    return FormatterAssemblyStyle.Simple;
                }
                return typeNameAssemblyFormat.GetValueOrDefault();
            }
            set
            {
                _typeNameAssemblyFormat = new FormatterAssemblyStyle?(value);
            }
        }

        /// <summary>
        /// Gets or sets how constructors are used during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.ConstructorHandling.Default" />.
        /// </summary>
        /// <value>The constructor handling.</value>
        public ConstructorHandling ConstructorHandling
        {
            get
            {
                ConstructorHandling? constructorHandling = _constructorHandling;
                if (constructorHandling == null)
                {
                    return ConstructorHandling.Default;
                }
                return constructorHandling.GetValueOrDefault();
            }
            set
            {
                _constructorHandling = new ConstructorHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets the contract resolver used by the serializer when
        /// serializing .NET objects to JSON and vice versa.
        /// </summary>
        /// <value>The contract resolver.</value>
        public IContractResolver ContractResolver { get; set; }

        /// <summary>
        /// Gets or sets the equality comparer used by the serializer when comparing references.
        /// </summary>
        /// <value>The equality comparer.</value>
        public IEqualityComparer EqualityComparer { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.
        /// </summary>
        /// <value>The reference resolver.</value>
        [Obsolete("ReferenceResolver property is obsolete. Use the ReferenceResolverProvider property to set the IReferenceResolver: settings.ReferenceResolverProvider = () => resolver")]
        public IReferenceResolver ReferenceResolver
        {
            get
            {
                if (ReferenceResolverProvider == null)
                {
                    return null;
                }
                return ReferenceResolverProvider();
            }
            set
            {
                if (value != null)
                    ReferenceResolverProvider = () => value;
            }
        }

        /// <summary>
        /// Gets or sets a function that creates the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.
        /// </summary>
        /// <value>A function that creates the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.</value>
        public Func<IReferenceResolver> ReferenceResolverProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.ITraceWriter" /> used by the serializer when writing trace messages.
        /// </summary>
        /// <value>The trace writer.</value>
        public ITraceWriter TraceWriter { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="P:Newtonsoft.Json.JsonSerializerSettings.SerializationBinder" /> used by the serializer when resolving type names.
        /// </summary>
        /// <value>The binder.</value>
        public SerializationBinder Binder { get; set; }

        /// <summary>
        /// Gets or sets the error handler called during serialization and deserialization.
        /// </summary>
        /// <value>The error handler called during serialization and deserialization.</value>
        public EventHandler<ErrorEventArgs> Error { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext" /> used by the serializer when invoking serialization callback methods.
        /// </summary>
        /// <value>The context.</value>
        public StreamingContext Context
        {
            get
            {
                StreamingContext? context = _context;
                if (context == null)
                {
                    return JsonSerializerSettings.DefaultContext;
                }
                return context.GetValueOrDefault();
            }
            set
            {
                _context = new StreamingContext?(value);
            }
        }

        /// <summary>
        /// Gets or sets how <see cref="T:System.DateTime" /> and <see cref="T:System.DateTimeOffset" /> values are formatted when writing JSON text,
        /// and the expected date format when reading JSON text.
        /// The default value is <c>"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"</c>.
        /// </summary>
        public string DateFormatString
        {
            get
            {
                return _dateFormatString ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
            }
            set
            {
                _dateFormatString = value;
                _dateFormatStringSet = true;
            }
        }

        /// <summary>
        /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
        /// A null value means there is no maximum.
        /// The default value is <c>null</c>.
        /// </summary>
        public int? MaxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be positive.", "value");
                }
                _maxDepth = value;
                _maxDepthSet = true;
            }
        }

        /// <summary>
        /// Indicates how JSON text output is formatted.
        /// The default value is <see cref="F:Newtonsoft.Json.Formatting.None" />.
        /// </summary>
        public Formatting Formatting
        {
            get
            {
                Formatting? formatting = _formatting;
                if (formatting == null)
                {
                    return Formatting.None;
                }
                return formatting.GetValueOrDefault();
            }
            set
            {
                _formatting = new Formatting?(value);
            }
        }

        /// <summary>
        /// Gets or sets how dates are written to JSON text.
        /// The default value is <see cref="F:Newtonsoft.Json.DateFormatHandling.IsoDateFormat" />.
        /// </summary>
        public DateFormatHandling DateFormatHandling
        {
            get
            {
                DateFormatHandling? dateFormatHandling = _dateFormatHandling;
                if (dateFormatHandling == null)
                {
                    return DateFormatHandling.IsoDateFormat;
                }
                return dateFormatHandling.GetValueOrDefault();
            }
            set
            {
                _dateFormatHandling = new DateFormatHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how <see cref="T:System.DateTime" /> time zones are handled during serialization and deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind" />.
        /// </summary>
        public DateTimeZoneHandling DateTimeZoneHandling
        {
            get
            {
                DateTimeZoneHandling? dateTimeZoneHandling = _dateTimeZoneHandling;
                if (dateTimeZoneHandling == null)
                {
                    return DateTimeZoneHandling.RoundtripKind;
                }
                return dateTimeZoneHandling.GetValueOrDefault();
            }
            set
            {
                _dateTimeZoneHandling = new DateTimeZoneHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how date formatted strings, e.g. <c>"\/Date(1198908717056)\/"</c> and <c>"2012-03-21T05:40Z"</c>, are parsed when reading JSON.
        /// The default value is <see cref="F:Newtonsoft.Json.DateParseHandling.DateTime" />.
        /// </summary>
        public DateParseHandling DateParseHandling
        {
            get
            {
                DateParseHandling? dateParseHandling = _dateParseHandling;
                if (dateParseHandling == null)
                {
                    return DateParseHandling.DateTime;
                }
                return dateParseHandling.GetValueOrDefault();
            }
            set
            {
                _dateParseHandling = new DateParseHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
        /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" />,
        /// are written as JSON.
        /// The default value is <see cref="F:Newtonsoft.Json.FloatFormatHandling.String" />.
        /// </summary>
        public FloatFormatHandling FloatFormatHandling
        {
            get
            {
                FloatFormatHandling? floatFormatHandling = _floatFormatHandling;
                if (floatFormatHandling == null)
                {
                    return FloatFormatHandling.String;
                }
                return floatFormatHandling.GetValueOrDefault();
            }
            set
            {
                _floatFormatHandling = new FloatFormatHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
        /// The default value is <see cref="F:Newtonsoft.Json.FloatParseHandling.Double" />.
        /// </summary>
        public FloatParseHandling FloatParseHandling
        {
            get
            {
                FloatParseHandling? floatParseHandling = _floatParseHandling;
                if (floatParseHandling == null)
                {
                    return FloatParseHandling.Double;
                }
                return floatParseHandling.GetValueOrDefault();
            }
            set
            {
                _floatParseHandling = new FloatParseHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets how strings are escaped when writing JSON text.
        /// The default value is <see cref="F:Newtonsoft.Json.StringEscapeHandling.Default" />.
        /// </summary>
        public StringEscapeHandling StringEscapeHandling
        {
            get
            {
                StringEscapeHandling? stringEscapeHandling = _stringEscapeHandling;
                if (stringEscapeHandling == null)
                {
                    return StringEscapeHandling.Default;
                }
                return stringEscapeHandling.GetValueOrDefault();
            }
            set
            {
                _stringEscapeHandling = new StringEscapeHandling?(value);
            }
        }

        /// <summary>
        /// Gets or sets the culture used when reading JSON.
        /// The default value is <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return _culture ?? JsonSerializerSettings.DefaultCulture;
            }
            set
            {
                _culture = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there will be a check for additional content after deserializing an object.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if there will be a check for additional content after deserializing an object; otherwise, <c>false</c>.
        /// </value>
        public bool CheckAdditionalContent
        {
            get
            {
                return _checkAdditionalContent ?? false;
            }
            set
            {
                _checkAdditionalContent = new bool?(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializerSettings" /> class.
        /// </summary>
        public JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>();
        }

        internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;

        internal const MissingMemberHandling DefaultMissingMemberHandling = MissingMemberHandling.Ignore;

        internal const NullValueHandling DefaultNullValueHandling = NullValueHandling.Include;

        internal const DefaultValueHandling DefaultDefaultValueHandling = DefaultValueHandling.Include;

        internal const ObjectCreationHandling DefaultObjectCreationHandling = ObjectCreationHandling.Auto;

        internal const PreserveReferencesHandling DefaultPreserveReferencesHandling = PreserveReferencesHandling.None;

        internal const ConstructorHandling DefaultConstructorHandling = ConstructorHandling.Default;

        internal const TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;

        internal const MetadataPropertyHandling DefaultMetadataPropertyHandling = MetadataPropertyHandling.Default;

        internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;

        internal static readonly StreamingContext DefaultContext = default(StreamingContext);

        internal const Formatting DefaultFormatting = Formatting.None;

        internal const DateFormatHandling DefaultDateFormatHandling = DateFormatHandling.IsoDateFormat;

        internal const DateTimeZoneHandling DefaultDateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

        internal const DateParseHandling DefaultDateParseHandling = DateParseHandling.DateTime;

        internal const FloatParseHandling DefaultFloatParseHandling = FloatParseHandling.Double;

        internal const FloatFormatHandling DefaultFloatFormatHandling = FloatFormatHandling.String;

        internal const StringEscapeHandling DefaultStringEscapeHandling = StringEscapeHandling.Default;

        internal const FormatterAssemblyStyle DefaultFormatterAssemblyStyle = FormatterAssemblyStyle.Simple;

        internal static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;

        internal const bool DefaultCheckAdditionalContent = false;

        internal const string DefaultDateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        internal Formatting? _formatting;

        internal DateFormatHandling? _dateFormatHandling;

        internal DateTimeZoneHandling? _dateTimeZoneHandling;

        internal DateParseHandling? _dateParseHandling;

        internal FloatFormatHandling? _floatFormatHandling;

        internal FloatParseHandling? _floatParseHandling;

        internal StringEscapeHandling? _stringEscapeHandling;

        internal CultureInfo _culture;

        internal bool? _checkAdditionalContent;

        internal int? _maxDepth;

        internal bool _maxDepthSet;

        internal string _dateFormatString;

        internal bool _dateFormatStringSet;

        internal FormatterAssemblyStyle? _typeNameAssemblyFormat;

        internal DefaultValueHandling? _defaultValueHandling;

        internal PreserveReferencesHandling? _preserveReferencesHandling;

        internal NullValueHandling? _nullValueHandling;

        internal ObjectCreationHandling? _objectCreationHandling;

        internal MissingMemberHandling? _missingMemberHandling;

        internal ReferenceLoopHandling? _referenceLoopHandling;

        internal StreamingContext? _context;

        internal ConstructorHandling? _constructorHandling;

        internal TypeNameHandling? _typeNameHandling;

        internal MetadataPropertyHandling? _metadataPropertyHandling;
    }
}
