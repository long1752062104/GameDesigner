using System;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Provides methods to get and set values.
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="target">The target to set the value on.</param>
        /// <param name="value">The value to set on the target.</param>
        void SetValue(object target, object value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="target">The target to get the value from.</param>
        /// <returns>The value.</returns>
        object GetValue(object target);
    }
}
