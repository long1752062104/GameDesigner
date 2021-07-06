using System;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Provides methods to get attributes.
    /// </summary>
    public interface IAttributeProvider
    {
        /// <summary>
        /// Returns a collection of all of the attributes, or an empty collection if there are no attributes.
        /// </summary>
        /// <param name="inherit">When <c>true</c>, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>A collection of <see cref="T:System.Attribute" />s, or an empty collection.</returns>
        IList<Attribute> GetAttributes(bool inherit);

        /// <summary>
        /// Returns a collection of attributes, identified by type, or an empty collection if there are no attributes.
        /// </summary>
        /// <param name="attributeType">The type of the attributes.</param>
        /// <param name="inherit">When <c>true</c>, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>A collection of <see cref="T:System.Attribute" />s, or an empty collection.</returns>
        IList<Attribute> GetAttributes(Type attributeType, bool inherit);
    }
}
