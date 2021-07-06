using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft_X.Json.Serialization
{
    internal class DefaultReferenceResolver : IReferenceResolver
    {
        private BidirectionalDictionary<string, object> GetMappings(object context)
        {
            JsonSerializerInternalBase jsonSerializerInternalBase;
            if (context is JsonSerializerInternalBase)
            {
                jsonSerializerInternalBase = (JsonSerializerInternalBase)context;
            }
            else
            {
                if (!(context is JsonSerializerProxy))
                {
                    throw new JsonException("The DefaultReferenceResolver can only be used internally.");
                }
                jsonSerializerInternalBase = ((JsonSerializerProxy)context).GetInternalSerializer();
            }
            return jsonSerializerInternalBase.DefaultReferenceMappings;
        }

        public object ResolveReference(object context, string reference)
        {
            GetMappings(context).TryGetByFirst(reference, out object result);
            return result;
        }

        public string GetReference(object context, object value)
        {
            BidirectionalDictionary<string, object> mappings = GetMappings(context);
            if (!mappings.TryGetBySecond(value, out string text))
            {
                _referenceCount++;
                text = _referenceCount.ToString(CultureInfo.InvariantCulture);
                mappings.Set(text, value);
            }
            return text;
        }

        public void AddReference(object context, string reference, object value)
        {
            GetMappings(context).Set(reference, value);
        }

        public bool IsReferenced(object context, object value)
        {
            return GetMappings(context).TryGetBySecond(value, out string text);
        }

        private int _referenceCount;
    }
}
