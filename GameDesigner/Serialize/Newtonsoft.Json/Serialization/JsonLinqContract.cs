using System;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Contract details for a <see cref="T:System.Type" /> used by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public class JsonLinqContract : JsonContract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.JsonLinqContract" /> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonLinqContract(Type underlyingType) : base(underlyingType)
        {
            ContractType = JsonContractType.Linq;
        }
    }
}
