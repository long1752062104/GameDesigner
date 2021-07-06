using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Get and set values for a <see cref="T:System.Reflection.MemberInfo" /> using reflection.
    /// </summary>
    public class ReflectionValueProvider : IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.ReflectionValueProvider" /> class.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        public ReflectionValueProvider(MemberInfo memberInfo)
        {
            ValidationUtils.ArgumentNotNull(memberInfo, "memberInfo");
            _memberInfo = memberInfo;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="target">The target to set the value on.</param>
        /// <param name="value">The value to set on the target.</param>
        public void SetValue(object target, object value)
        {
            try
            {
                ReflectionUtils.SetMemberValue(_memberInfo, target, value);
            }
            catch (Exception innerException)
            {
                throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, _memberInfo.Name, target.GetType()), innerException);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="target">The target to get the value from.</param>
        /// <returns>The value.</returns>
        public object GetValue(object target)
        {
            object memberValue;
            try
            {
                memberValue = ReflectionUtils.GetMemberValue(_memberInfo, target);
            }
            catch (Exception innerException)
            {
                throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, _memberInfo.Name, target.GetType()), innerException);
            }
            return memberValue;
        }

        private readonly MemberInfo _memberInfo;
    }
}
