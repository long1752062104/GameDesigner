using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public class JsonPrimitiveContract : JsonContract
    {
        internal PrimitiveTypeCode TypeCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" /> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonPrimitiveContract(Type underlyingType) : base(underlyingType)
        {
            ContractType = JsonContractType.Primitive;
            TypeCode = ConvertUtils.GetTypeCode(underlyingType);
            IsReadOnlyOrFixedSize = true;
            if (JsonPrimitiveContract.ReadTypeMap.TryGetValue(NonNullableUnderlyingType, out ReadType internalReadType))
            {
                InternalReadType = internalReadType;
            }
        }

        // Note: this type is marked as 'beforefieldinit'.
        static JsonPrimitiveContract()
        {
            Dictionary<Type, ReadType> dictionary = new Dictionary<Type, ReadType>();
            Type typeFromHandle = typeof(byte[]);
            dictionary[typeFromHandle] = ReadType.ReadAsBytes;
            Type typeFromHandle2 = typeof(byte);
            dictionary[typeFromHandle2] = ReadType.ReadAsInt32;
            Type typeFromHandle3 = typeof(short);
            dictionary[typeFromHandle3] = ReadType.ReadAsInt32;
            Type typeFromHandle4 = typeof(int);
            dictionary[typeFromHandle4] = ReadType.ReadAsInt32;
            Type typeFromHandle5 = typeof(decimal);
            dictionary[typeFromHandle5] = ReadType.ReadAsDecimal;
            Type typeFromHandle6 = typeof(bool);
            dictionary[typeFromHandle6] = ReadType.ReadAsBoolean;
            Type typeFromHandle7 = typeof(string);
            dictionary[typeFromHandle7] = ReadType.ReadAsString;
            Type typeFromHandle8 = typeof(DateTime);
            dictionary[typeFromHandle8] = ReadType.ReadAsDateTime;
            Type typeFromHandle9 = typeof(DateTimeOffset);
            dictionary[typeFromHandle9] = ReadType.ReadAsDateTimeOffset;
            Type typeFromHandle10 = typeof(float);
            dictionary[typeFromHandle10] = ReadType.ReadAsDouble;
            Type typeFromHandle11 = typeof(double);
            dictionary[typeFromHandle11] = ReadType.ReadAsDouble;
            JsonPrimitiveContract.ReadTypeMap = dictionary;
        }

        private static readonly Dictionary<Type, ReadType> ReadTypeMap;
    }
}
