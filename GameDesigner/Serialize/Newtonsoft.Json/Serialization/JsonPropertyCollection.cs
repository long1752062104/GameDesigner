using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// A collection of <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> objects.
    /// </summary>
    public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonPropertyCollection" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public JsonPropertyCollection(Type type) : base(StringComparer.Ordinal)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            _type = type;
            _list = (List<JsonProperty>)base.Items;
        }

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override string GetKeyForItem(JsonProperty item)
        {
            return item.PropertyName;
        }

        /// <summary>
        /// Adds a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> object.
        /// </summary>
        /// <param name="property">The property to add to the collection.</param>
        public void AddProperty(JsonProperty property)
        {
            if (base.Contains(property.PropertyName))
            {
                if (property.Ignored)
                {
                    return;
                }
                JsonProperty jsonProperty = base[property.PropertyName];
                bool flag = true;
                if (jsonProperty.Ignored)
                {
                    base.Remove(jsonProperty);
                    flag = false;
                }
                else if (property.DeclaringType != null && jsonProperty.DeclaringType != null)
                {
                    if (property.DeclaringType.IsSubclassOf(jsonProperty.DeclaringType) || (jsonProperty.DeclaringType.IsInterface() && property.DeclaringType.ImplementInterface(jsonProperty.DeclaringType)))
                    {
                        base.Remove(jsonProperty);
                        flag = false;
                    }
                    if (jsonProperty.DeclaringType.IsSubclassOf(property.DeclaringType) || (property.DeclaringType.IsInterface() && jsonProperty.DeclaringType.ImplementInterface(property.DeclaringType)))
                    {
                        return;
                    }
                }
                if (flag)
                {
                    throw new JsonSerializationException("A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, _type));
                }
            }
            base.Add(property);
        }

        /// <summary>
        /// Gets the closest matching <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> object.
        /// First attempts to get an exact case match of <paramref name="propertyName" /> and then
        /// a case insensitive match.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A matching property if found.</returns>
        public JsonProperty GetClosestMatchProperty(string propertyName)
        {
            JsonProperty property = GetProperty(propertyName, StringComparison.Ordinal);
            if (property == null)
            {
                property = GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);
            }
            return property;
        }

        private bool TryGetValue(string key, out JsonProperty item)
        {
            if (base.Dictionary == null)
            {
                item = null;
                return false;
            }
            return base.Dictionary.TryGetValue(key, out item);
        }

        /// <summary>
        /// Gets a property by property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="comparisonType">Type property name string comparison.</param>
        /// <returns>A matching property if found.</returns>
        public JsonProperty GetProperty(string propertyName, StringComparison comparisonType)
        {
            if (comparisonType != StringComparison.Ordinal)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    JsonProperty jsonProperty = _list[i];
                    if (string.Equals(propertyName, jsonProperty.PropertyName, comparisonType))
                    {
                        return jsonProperty;
                    }
                }
                return null;
            }
            if (TryGetValue(propertyName, out JsonProperty result))
            {
                return result;
            }
            return null;
        }

        private readonly Type _type;

        private readonly List<JsonProperty> _list;
    }
}
