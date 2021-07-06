using Newtonsoft_X.Json.Serialization;
using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Serializes and deserializes objects into and from the JSON format.
    /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> enables you to control how objects are encoded into JSON.
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Occurs when the <see cref="T:Newtonsoft.Json.JsonSerializer" /> errors during serialization and deserialization.
        /// </summary>
        public virtual event EventHandler<Newtonsoft_X.Json.Serialization.ErrorEventArgs> Error;

        /// <summary>
        /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.IReferenceResolver" /> used by the serializer when resolving references.
        /// </summary>
        public virtual IReferenceResolver ReferenceResolver
        {
            get
            {
                return GetReferenceResolver();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Reference resolver cannot be null.");
                }
                _referenceResolver = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="P:Newtonsoft.Json.JsonSerializer.SerializationBinder" /> used by the serializer when resolving type names.
        /// </summary>
        public virtual SerializationBinder Binder
        {
            get
            {
                return _binder;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Serialization binder cannot be null.");
                }
                _binder = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Newtonsoft.Json.Serialization.ITraceWriter" /> used by the serializer when writing trace messages.
        /// </summary>
        /// <value>The trace writer.</value>
        public virtual ITraceWriter TraceWriter
        {
            get
            {
                return _traceWriter;
            }
            set
            {
                _traceWriter = value;
            }
        }

        /// <summary>
        /// Gets or sets the equality comparer used by the serializer when comparing references.
        /// </summary>
        /// <value>The equality comparer.</value>
        public virtual IEqualityComparer EqualityComparer
        {
            get
            {
                return _equalityComparer;
            }
            set
            {
                _equalityComparer = value;
            }
        }

        /// <summary>
        /// Gets or sets how type name writing and reading is handled by the serializer.
        /// The default value is <see cref="F:Newtonsoft.Json.TypeNameHandling.None" />.
        /// </summary>
        /// <remarks>
        /// <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> should be used with caution when your application deserializes JSON from an external source.
        /// Incoming types should be validated with a custom <see cref="P:Newtonsoft.Json.JsonSerializer.SerializationBinder" />
        /// when deserializing with a value other than <see cref="F:Newtonsoft.Json.TypeNameHandling.None" />.
        /// </remarks>
        public virtual TypeNameHandling TypeNameHandling
        {
            get
            {
                return _typeNameHandling;
            }
            set
            {
                if (value < TypeNameHandling.None || value > TypeNameHandling.Auto)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _typeNameHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how a type name assembly is written and resolved by the serializer.
        /// The default value is <see cref="F:System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple" />.
        /// </summary>
        /// <value>The type name assembly format.</value>
        public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get
            {
                return _typeNameAssemblyFormat;
            }
            set
            {
                if (value < FormatterAssemblyStyle.Simple || value > FormatterAssemblyStyle.Full)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _typeNameAssemblyFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets how object references are preserved by the serializer.
        /// The default value is <see cref="F:Newtonsoft.Json.PreserveReferencesHandling.None" />.
        /// </summary>
        public virtual PreserveReferencesHandling PreserveReferencesHandling
        {
            get
            {
                return _preserveReferencesHandling;
            }
            set
            {
                if (value < PreserveReferencesHandling.None || value > PreserveReferencesHandling.All)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _preserveReferencesHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how reference loops (e.g. a class referencing itself) is handled.
        /// The default value is <see cref="F:Newtonsoft.Json.ReferenceLoopHandling.Error" />.
        /// </summary>
        public virtual ReferenceLoopHandling ReferenceLoopHandling
        {
            get
            {
                return _referenceLoopHandling;
            }
            set
            {
                if (value < ReferenceLoopHandling.Error || value > ReferenceLoopHandling.Serialize)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _referenceLoopHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.MissingMemberHandling.Ignore" />.
        /// </summary>
        public virtual MissingMemberHandling MissingMemberHandling
        {
            get
            {
                return _missingMemberHandling;
            }
            set
            {
                if (value < MissingMemberHandling.Ignore || value > MissingMemberHandling.Error)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _missingMemberHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how null values are handled during serialization and deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.NullValueHandling.Include" />.
        /// </summary>
        public virtual NullValueHandling NullValueHandling
        {
            get
            {
                return _nullValueHandling;
            }
            set
            {
                if (value < NullValueHandling.Include || value > NullValueHandling.Ignore)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _nullValueHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how default values are handled during serialization and deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.DefaultValueHandling.Include" />.
        /// </summary>
        public virtual DefaultValueHandling DefaultValueHandling
        {
            get
            {
                return _defaultValueHandling;
            }
            set
            {
                if (value < DefaultValueHandling.Include || value > DefaultValueHandling.IgnoreAndPopulate)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _defaultValueHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how objects are created during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.ObjectCreationHandling.Auto" />.
        /// </summary>
        /// <value>The object creation handling.</value>
        public virtual ObjectCreationHandling ObjectCreationHandling
        {
            get
            {
                return _objectCreationHandling;
            }
            set
            {
                if (value < ObjectCreationHandling.Auto || value > ObjectCreationHandling.Replace)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _objectCreationHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how constructors are used during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.ConstructorHandling.Default" />.
        /// </summary>
        /// <value>The constructor handling.</value>
        public virtual ConstructorHandling ConstructorHandling
        {
            get
            {
                return _constructorHandling;
            }
            set
            {
                if (value < ConstructorHandling.Default || value > ConstructorHandling.AllowNonPublicDefaultConstructor)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _constructorHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how metadata properties are used during deserialization.
        /// The default value is <see cref="F:Newtonsoft.Json.MetadataPropertyHandling.Default" />.
        /// </summary>
        /// <value>The metadata properties handling.</value>
        public virtual MetadataPropertyHandling MetadataPropertyHandling
        {
            get
            {
                return _metadataPropertyHandling;
            }
            set
            {
                if (value < MetadataPropertyHandling.Default || value > MetadataPropertyHandling.Ignore)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _metadataPropertyHandling = value;
            }
        }

        /// <summary>
        /// Gets a collection <see cref="T:Newtonsoft.Json.JsonConverter" /> that will be used during serialization.
        /// </summary>
        /// <value>Collection <see cref="T:Newtonsoft.Json.JsonConverter" /> that will be used during serialization.</value>
        public virtual JsonConverterCollection Converters
        {
            get
            {
                if (_converters == null)
                {
                    _converters = new JsonConverterCollection();
                }
                return _converters;
            }
        }

        /// <summary>
        /// Gets or sets the contract resolver used by the serializer when
        /// serializing .NET objects to JSON and vice versa.
        /// </summary>
        public virtual IContractResolver ContractResolver
        {
            get
            {
                return _contractResolver;
            }
            set
            {
                _contractResolver = (value ?? DefaultContractResolver.Instance);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext" /> used by the serializer when invoking serialization callback methods.
        /// </summary>
        /// <value>The context.</value>
        public virtual StreamingContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        /// <summary>
        /// Indicates how JSON text output is formatted.
        /// The default value is <see cref="F:Newtonsoft.Json.Formatting.None" />.
        /// </summary>
        public virtual Formatting Formatting
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
        public virtual DateFormatHandling DateFormatHandling
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
        public virtual DateTimeZoneHandling DateTimeZoneHandling
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
        public virtual DateParseHandling DateParseHandling
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
        /// Gets or sets how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
        /// The default value is <see cref="F:Newtonsoft.Json.FloatParseHandling.Double" />.
        /// </summary>
        public virtual FloatParseHandling FloatParseHandling
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
        /// Gets or sets how special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
        /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" />,
        /// are written as JSON text.
        /// The default value is <see cref="F:Newtonsoft.Json.FloatFormatHandling.String" />.
        /// </summary>
        public virtual FloatFormatHandling FloatFormatHandling
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
        /// Gets or sets how strings are escaped when writing JSON text.
        /// The default value is <see cref="F:Newtonsoft.Json.StringEscapeHandling.Default" />.
        /// </summary>
        public virtual StringEscapeHandling StringEscapeHandling
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
        /// Gets or sets how <see cref="T:System.DateTime" /> and <see cref="T:System.DateTimeOffset" /> values are formatted when writing JSON text,
        /// and the expected date format when reading JSON text.
        /// The default value is <c>"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"</c>.
        /// </summary>
        public virtual string DateFormatString
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
        /// Gets or sets the culture used when reading JSON.
        /// The default value is <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
        /// </summary>
        public virtual CultureInfo Culture
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
        /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
        /// A null value means there is no maximum.
        /// The default value is <c>null</c>.
        /// </summary>
        public virtual int? MaxDepth
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
        /// Gets a value indicating whether there will be a check for additional JSON content after deserializing an object.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if there will be a check for additional JSON content after deserializing an object; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CheckAdditionalContent
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

        internal bool IsCheckAdditionalContentSet()
        {
            return _checkAdditionalContent != null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializer" /> class.
        /// </summary>
        public JsonSerializer()
        {
            _referenceLoopHandling = ReferenceLoopHandling.Error;
            _missingMemberHandling = MissingMemberHandling.Ignore;
            _nullValueHandling = NullValueHandling.Include;
            _defaultValueHandling = DefaultValueHandling.Include;
            _objectCreationHandling = ObjectCreationHandling.Auto;
            _preserveReferencesHandling = PreserveReferencesHandling.None;
            _constructorHandling = ConstructorHandling.Default;
            _typeNameHandling = TypeNameHandling.None;
            _metadataPropertyHandling = MetadataPropertyHandling.Default;
            _context = JsonSerializerSettings.DefaultContext;
            _binder = DefaultSerializationBinder.Instance;
            _culture = JsonSerializerSettings.DefaultCulture;
            _contractResolver = DefaultContractResolver.Instance;
        }

        /// <summary>
        /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" />.
        /// </returns>
        public static JsonSerializer Create()
        {
            return new JsonSerializer();
        }

        /// <summary>
        /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" />.
        /// </summary>
        /// <param name="settings">The settings to be applied to the <see cref="T:Newtonsoft.Json.JsonSerializer" />.</param>
        /// <returns>
        /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will not use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" />.
        /// </returns>
        public static JsonSerializer Create(JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create();
            if (settings != null)
            {
                JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);
            }
            return jsonSerializer;
        }

        /// <summary>
        /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" />.
        /// </returns>
        public static JsonSerializer CreateDefault()
        {
            Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
            return JsonSerializer.Create((defaultSettings != null) ? defaultSettings() : null);
        }

        /// <summary>
        /// Creates a new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" /> as well as the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </summary>
        /// <param name="settings">The settings to be applied to the <see cref="T:Newtonsoft.Json.JsonSerializer" />.</param>
        /// <returns>
        /// A new <see cref="T:Newtonsoft.Json.JsonSerializer" /> instance using the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// The <see cref="T:Newtonsoft.Json.JsonSerializer" /> will use default settings 
        /// from <see cref="P:Newtonsoft.Json.JsonConvert.DefaultSettings" /> as well as the specified <see cref="T:Newtonsoft.Json.JsonSerializerSettings" />.
        /// </returns>
        public static JsonSerializer CreateDefault(JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
            if (settings != null)
            {
                JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);
            }
            return jsonSerializer;
        }

        private static void ApplySerializerSettings(JsonSerializer serializer, JsonSerializerSettings settings)
        {
            if (!CollectionUtils.IsNullOrEmpty<JsonConverter>(settings.Converters))
            {
                for (int i = 0; i < settings.Converters.Count; i++)
                {
                    serializer.Converters.Insert(i, settings.Converters[i]);
                }
            }
            if (settings._typeNameHandling != null)
            {
                serializer.TypeNameHandling = settings.TypeNameHandling;
            }
            if (settings._metadataPropertyHandling != null)
            {
                serializer.MetadataPropertyHandling = settings.MetadataPropertyHandling;
            }
            if (settings._typeNameAssemblyFormat != null)
            {
                serializer.TypeNameAssemblyFormat = settings.TypeNameAssemblyFormat;
            }
            if (settings._preserveReferencesHandling != null)
            {
                serializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
            }
            if (settings._referenceLoopHandling != null)
            {
                serializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
            }
            if (settings._missingMemberHandling != null)
            {
                serializer.MissingMemberHandling = settings.MissingMemberHandling;
            }
            if (settings._objectCreationHandling != null)
            {
                serializer.ObjectCreationHandling = settings.ObjectCreationHandling;
            }
            if (settings._nullValueHandling != null)
            {
                serializer.NullValueHandling = settings.NullValueHandling;
            }
            if (settings._defaultValueHandling != null)
            {
                serializer.DefaultValueHandling = settings.DefaultValueHandling;
            }
            if (settings._constructorHandling != null)
            {
                serializer.ConstructorHandling = settings.ConstructorHandling;
            }
            if (settings._context != null)
            {
                serializer.Context = settings.Context;
            }
            if (settings._checkAdditionalContent != null)
            {
                serializer._checkAdditionalContent = settings._checkAdditionalContent;
            }
            if (settings.Error != null)
            {
                serializer.Error += settings.Error;
            }
            if (settings.ContractResolver != null)
            {
                serializer.ContractResolver = settings.ContractResolver;
            }
            if (settings.ReferenceResolverProvider != null)
            {
                serializer.ReferenceResolver = settings.ReferenceResolverProvider();
            }
            if (settings.TraceWriter != null)
            {
                serializer.TraceWriter = settings.TraceWriter;
            }
            if (settings.EqualityComparer != null)
            {
                serializer.EqualityComparer = settings.EqualityComparer;
            }
            if (settings.Binder != null)
            {
                serializer.Binder = settings.Binder;
            }
            if (settings._formatting != null)
            {
                serializer._formatting = settings._formatting;
            }
            if (settings._dateFormatHandling != null)
            {
                serializer._dateFormatHandling = settings._dateFormatHandling;
            }
            if (settings._dateTimeZoneHandling != null)
            {
                serializer._dateTimeZoneHandling = settings._dateTimeZoneHandling;
            }
            if (settings._dateParseHandling != null)
            {
                serializer._dateParseHandling = settings._dateParseHandling;
            }
            if (settings._dateFormatStringSet)
            {
                serializer._dateFormatString = settings._dateFormatString;
                serializer._dateFormatStringSet = settings._dateFormatStringSet;
            }
            if (settings._floatFormatHandling != null)
            {
                serializer._floatFormatHandling = settings._floatFormatHandling;
            }
            if (settings._floatParseHandling != null)
            {
                serializer._floatParseHandling = settings._floatParseHandling;
            }
            if (settings._stringEscapeHandling != null)
            {
                serializer._stringEscapeHandling = settings._stringEscapeHandling;
            }
            if (settings._culture != null)
            {
                serializer._culture = settings._culture;
            }
            if (settings._maxDepthSet)
            {
                serializer._maxDepth = settings._maxDepth;
                serializer._maxDepthSet = settings._maxDepthSet;
            }
        }

        /// <summary>
        /// Populates the JSON values onto the target object.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.IO.TextReader" /> that contains the JSON structure to read values from.</param>
        /// <param name="target">The target object to populate values onto.</param>
        public void Populate(TextReader reader, object target)
        {
            Populate(new JsonTextReader(reader), target);
        }

        /// <summary>
        /// Populates the JSON values onto the target object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> that contains the JSON structure to read values from.</param>
        /// <param name="target">The target object to populate values onto.</param>
        public void Populate(JsonReader reader, object target)
        {
            PopulateInternal(reader, target);
        }

        internal virtual void PopulateInternal(JsonReader reader, object target)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            ValidationUtils.ArgumentNotNull(target, "target");
            SetupReader(reader, out CultureInfo previousCulture, out DateTimeZoneHandling? previousDateTimeZoneHandling, out DateParseHandling? previousDateParseHandling, out FloatParseHandling? previousFloatParseHandling, out int? previousMaxDepth, out string previousDateFormatString);
            TraceJsonReader traceJsonReader = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose) ? new TraceJsonReader(reader) : null;
            new JsonSerializerInternalReader(this).Populate(traceJsonReader ?? reader, target);
            if (traceJsonReader != null)
            {
                TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), null);
            }
            ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);
        }

        /// <summary>
        /// Deserializes the JSON structure contained by the specified <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> that contains the JSON structure to deserialize.</param>
        /// <returns>The <see cref="T:System.Object" /> being deserialized.</returns>
        public object Deserialize(JsonReader reader)
        {
            return Deserialize(reader, null);
        }

        /// <summary>
        /// Deserializes the JSON structure contained by the specified <see cref="T:System.IO.TextReader" />
        /// into an instance of the specified type.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.IO.TextReader" /> containing the object.</param>
        /// <param name="objectType">The <see cref="T:System.Type" /> of object being deserialized.</param>
        /// <returns>The instance of <paramref name="objectType" /> being deserialized.</returns>
        public object Deserialize(TextReader reader, Type objectType)
        {
            return Deserialize(new JsonTextReader(reader), objectType);
        }

        /// <summary>
        /// Deserializes the JSON structure contained by the specified <see cref="T:Newtonsoft.Json.JsonReader" />
        /// into an instance of the specified type.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> containing the object.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The instance of <typeparamref name="T" /> being deserialized.</returns>
        public T Deserialize<T>(JsonReader reader)
        {
            return (T)Deserialize(reader, typeof(T));
        }

        /// <summary>
        /// Deserializes the JSON structure contained by the specified <see cref="T:Newtonsoft.Json.JsonReader" />
        /// into an instance of the specified type.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> containing the object.</param>
        /// <param name="objectType">The <see cref="T:System.Type" /> of object being deserialized.</param>
        /// <returns>The instance of <paramref name="objectType" /> being deserialized.</returns>
        public object Deserialize(JsonReader reader, Type objectType)
        {
            return DeserializeInternal(reader, objectType);
        }

        internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            SetupReader(reader, out CultureInfo previousCulture, out DateTimeZoneHandling? previousDateTimeZoneHandling, out DateParseHandling? previousDateParseHandling, out FloatParseHandling? previousFloatParseHandling, out int? previousMaxDepth, out string previousDateFormatString);
            TraceJsonReader traceJsonReader = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose) ? new TraceJsonReader(reader) : null;
            object result = new JsonSerializerInternalReader(this).Deserialize(traceJsonReader ?? reader, objectType, CheckAdditionalContent);
            if (traceJsonReader != null)
            {
                TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), null);
            }
            ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);
            return result;
        }

        private void SetupReader(JsonReader reader, out CultureInfo previousCulture, out DateTimeZoneHandling? previousDateTimeZoneHandling, out DateParseHandling? previousDateParseHandling, out FloatParseHandling? previousFloatParseHandling, out int? previousMaxDepth, out string previousDateFormatString)
        {
            if (_culture != null && !_culture.Equals(reader.Culture))
            {
                previousCulture = reader.Culture;
                reader.Culture = _culture;
            }
            else
            {
                previousCulture = null;
            }
            if (_dateTimeZoneHandling != null && reader.DateTimeZoneHandling != _dateTimeZoneHandling)
            {
                previousDateTimeZoneHandling = new DateTimeZoneHandling?(reader.DateTimeZoneHandling);
                reader.DateTimeZoneHandling = _dateTimeZoneHandling.GetValueOrDefault();
            }
            else
            {
                previousDateTimeZoneHandling = null;
            }
            if (_dateParseHandling != null && reader.DateParseHandling != _dateParseHandling)
            {
                previousDateParseHandling = new DateParseHandling?(reader.DateParseHandling);
                reader.DateParseHandling = _dateParseHandling.GetValueOrDefault();
            }
            else
            {
                previousDateParseHandling = null;
            }
            if (_floatParseHandling != null && reader.FloatParseHandling != _floatParseHandling)
            {
                previousFloatParseHandling = new FloatParseHandling?(reader.FloatParseHandling);
                reader.FloatParseHandling = _floatParseHandling.GetValueOrDefault();
            }
            else
            {
                previousFloatParseHandling = null;
            }
            if (_maxDepthSet && reader.MaxDepth != _maxDepth)
            {
                previousMaxDepth = reader.MaxDepth;
                reader.MaxDepth = _maxDepth;
            }
            else
            {
                previousMaxDepth = null;
            }
            if (_dateFormatStringSet && reader.DateFormatString != _dateFormatString)
            {
                previousDateFormatString = reader.DateFormatString;
                reader.DateFormatString = _dateFormatString;
            }
            else
            {
                previousDateFormatString = null;
            }
            JsonTextReader jsonTextReader = reader as JsonTextReader;
            if (jsonTextReader != null)
            {
                DefaultContractResolver defaultContractResolver = _contractResolver as DefaultContractResolver;
                if (defaultContractResolver != null)
                {
                    jsonTextReader.NameTable = defaultContractResolver.GetState().NameTable;
                }
            }
        }

        private void ResetReader(JsonReader reader, CultureInfo previousCulture, DateTimeZoneHandling? previousDateTimeZoneHandling, DateParseHandling? previousDateParseHandling, FloatParseHandling? previousFloatParseHandling, int? previousMaxDepth, string previousDateFormatString)
        {
            if (previousCulture != null)
            {
                reader.Culture = previousCulture;
            }
            if (previousDateTimeZoneHandling != null)
            {
                reader.DateTimeZoneHandling = previousDateTimeZoneHandling.GetValueOrDefault();
            }
            if (previousDateParseHandling != null)
            {
                reader.DateParseHandling = previousDateParseHandling.GetValueOrDefault();
            }
            if (previousFloatParseHandling != null)
            {
                reader.FloatParseHandling = previousFloatParseHandling.GetValueOrDefault();
            }
            if (_maxDepthSet)
            {
                reader.MaxDepth = previousMaxDepth;
            }
            if (_dateFormatStringSet)
            {
                reader.DateFormatString = previousDateFormatString;
            }
            JsonTextReader jsonTextReader = reader as JsonTextReader;
            if (jsonTextReader != null)
            {
                jsonTextReader.NameTable = null;
            }
        }

        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
        /// using the specified <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        /// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> used to write the JSON structure.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
        public void Serialize(TextWriter textWriter, object value)
        {
            Serialize(new JsonTextWriter(textWriter), value);
        }

        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
        /// using the specified <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="T:Newtonsoft.Json.JsonWriter" /> used to write the JSON structure.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
        /// <param name="objectType">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> is <see cref="F:Newtonsoft.Json.TypeNameHandling.Auto" /> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
        {
            SerializeInternal(jsonWriter, value, objectType);
        }

        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
        /// using the specified <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        /// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> used to write the JSON structure.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
        /// <param name="objectType">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> is Auto to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        public void Serialize(TextWriter textWriter, object value, Type objectType)
        {
            Serialize(new JsonTextWriter(textWriter), value, objectType);
        }

        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and writes the JSON structure
        /// using the specified <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="T:Newtonsoft.Json.JsonWriter" /> used to write the JSON structure.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to serialize.</param>
        public void Serialize(JsonWriter jsonWriter, object value)
        {
            SerializeInternal(jsonWriter, value, null);
        }

        internal virtual void SerializeInternal(JsonWriter jsonWriter, object value, Type objectType)
        {
            ValidationUtils.ArgumentNotNull(jsonWriter, "jsonWriter");
            Formatting? formatting = null;
            if (_formatting != null && jsonWriter.Formatting != _formatting)
            {
                formatting = new Formatting?(jsonWriter.Formatting);
                jsonWriter.Formatting = _formatting.GetValueOrDefault();
            }
            DateFormatHandling? dateFormatHandling = null;
            if (_dateFormatHandling != null && jsonWriter.DateFormatHandling != _dateFormatHandling)
            {
                dateFormatHandling = new DateFormatHandling?(jsonWriter.DateFormatHandling);
                jsonWriter.DateFormatHandling = _dateFormatHandling.GetValueOrDefault();
            }
            DateTimeZoneHandling? dateTimeZoneHandling = null;
            if (_dateTimeZoneHandling != null && jsonWriter.DateTimeZoneHandling != _dateTimeZoneHandling)
            {
                dateTimeZoneHandling = new DateTimeZoneHandling?(jsonWriter.DateTimeZoneHandling);
                jsonWriter.DateTimeZoneHandling = _dateTimeZoneHandling.GetValueOrDefault();
            }
            FloatFormatHandling? floatFormatHandling = null;
            if (_floatFormatHandling != null && jsonWriter.FloatFormatHandling != _floatFormatHandling)
            {
                floatFormatHandling = new FloatFormatHandling?(jsonWriter.FloatFormatHandling);
                jsonWriter.FloatFormatHandling = _floatFormatHandling.GetValueOrDefault();
            }
            StringEscapeHandling? stringEscapeHandling = null;
            if (_stringEscapeHandling != null && jsonWriter.StringEscapeHandling != _stringEscapeHandling)
            {
                stringEscapeHandling = new StringEscapeHandling?(jsonWriter.StringEscapeHandling);
                jsonWriter.StringEscapeHandling = _stringEscapeHandling.GetValueOrDefault();
            }
            CultureInfo cultureInfo = null;
            if (_culture != null && !_culture.Equals(jsonWriter.Culture))
            {
                cultureInfo = jsonWriter.Culture;
                jsonWriter.Culture = _culture;
            }
            string dateFormatString = null;
            if (_dateFormatStringSet && jsonWriter.DateFormatString != _dateFormatString)
            {
                dateFormatString = jsonWriter.DateFormatString;
                jsonWriter.DateFormatString = _dateFormatString;
            }
            TraceJsonWriter traceJsonWriter = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose) ? new TraceJsonWriter(jsonWriter) : null;
            new JsonSerializerInternalWriter(this).Serialize(traceJsonWriter ?? jsonWriter, value, objectType);
            if (traceJsonWriter != null)
            {
                TraceWriter.Trace(TraceLevel.Verbose, traceJsonWriter.GetSerializedJsonMessage(), null);
            }
            if (formatting != null)
            {
                jsonWriter.Formatting = formatting.GetValueOrDefault();
            }
            if (dateFormatHandling != null)
            {
                jsonWriter.DateFormatHandling = dateFormatHandling.GetValueOrDefault();
            }
            if (dateTimeZoneHandling != null)
            {
                jsonWriter.DateTimeZoneHandling = dateTimeZoneHandling.GetValueOrDefault();
            }
            if (floatFormatHandling != null)
            {
                jsonWriter.FloatFormatHandling = floatFormatHandling.GetValueOrDefault();
            }
            if (stringEscapeHandling != null)
            {
                jsonWriter.StringEscapeHandling = stringEscapeHandling.GetValueOrDefault();
            }
            if (_dateFormatStringSet)
            {
                jsonWriter.DateFormatString = dateFormatString;
            }
            if (cultureInfo != null)
            {
                jsonWriter.Culture = cultureInfo;
            }
        }

        internal IReferenceResolver GetReferenceResolver()
        {
            if (_referenceResolver == null)
            {
                _referenceResolver = new DefaultReferenceResolver();
            }
            return _referenceResolver;
        }

        internal JsonConverter GetMatchingConverter(Type type)
        {
            return JsonSerializer.GetMatchingConverter(_converters, type);
        }

        internal static JsonConverter GetMatchingConverter(IList<JsonConverter> converters, Type objectType)
        {
            if (converters != null)
            {
                for (int i = 0; i < converters.Count; i++)
                {
                    JsonConverter jsonConverter = converters[i];
                    if (jsonConverter.CanConvert(objectType))
                    {
                        return jsonConverter;
                    }
                }
            }
            return null;
        }

        internal void OnError(Newtonsoft_X.Json.Serialization.ErrorEventArgs e)
        {
            EventHandler<Newtonsoft_X.Json.Serialization.ErrorEventArgs> error = Error;
            if (error != null)
            {
                error(this, e);
            }
        }

        internal TypeNameHandling _typeNameHandling;

        internal FormatterAssemblyStyle _typeNameAssemblyFormat;

        internal PreserveReferencesHandling _preserveReferencesHandling;

        internal ReferenceLoopHandling _referenceLoopHandling;

        internal MissingMemberHandling _missingMemberHandling;

        internal ObjectCreationHandling _objectCreationHandling;

        internal NullValueHandling _nullValueHandling;

        internal DefaultValueHandling _defaultValueHandling;

        internal ConstructorHandling _constructorHandling;

        internal MetadataPropertyHandling _metadataPropertyHandling;

        internal JsonConverterCollection _converters;

        internal IContractResolver _contractResolver;

        internal ITraceWriter _traceWriter;

        internal IEqualityComparer _equalityComparer;

        internal SerializationBinder _binder;

        internal StreamingContext _context;

        private IReferenceResolver _referenceResolver;

        private Formatting? _formatting;

        private DateFormatHandling? _dateFormatHandling;

        private DateTimeZoneHandling? _dateTimeZoneHandling;

        private DateParseHandling? _dateParseHandling;

        private FloatFormatHandling? _floatFormatHandling;

        private FloatParseHandling? _floatParseHandling;

        private StringEscapeHandling? _stringEscapeHandling;

        private CultureInfo _culture;

        private int? _maxDepth;

        private bool _maxDepthSet;

        private bool? _checkAdditionalContent;

        private string _dateFormatString;

        private bool _dateFormatStringSet;
    }
}
