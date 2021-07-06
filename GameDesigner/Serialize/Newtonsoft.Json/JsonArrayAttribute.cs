using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> how to serialize the collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonArrayAttribute : JsonContainerAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether null items are allowed in the collection.
        /// </summary>
        /// <value><c>true</c> if null items are allowed in the collection; otherwise, <c>false</c>.</value>
        public bool AllowNullItems
        {
            get
            {
                return _allowNullItems;
            }
            set
            {
                _allowNullItems = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonArrayAttribute" /> class.
        /// </summary>
        public JsonArrayAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonObjectAttribute" /> class with a flag indicating whether the array can contain null items.
        /// </summary>
        /// <param name="allowNullItems">A flag indicating whether the array can contain null items.</param>
        public JsonArrayAttribute(bool allowNullItems)
        {
            _allowNullItems = allowNullItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonArrayAttribute" /> class with the specified container Id.
        /// </summary>
        /// <param name="id">The container Id.</param>
        public JsonArrayAttribute(string id) : base(id)
        {
        }

        private bool _allowNullItems;
    }
}
