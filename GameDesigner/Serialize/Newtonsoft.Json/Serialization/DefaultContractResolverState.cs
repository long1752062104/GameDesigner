using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Serialization
{
    internal class DefaultContractResolverState
    {
        public Dictionary<ResolverContractKey, JsonContract> ContractCache;

        public PropertyNameTable NameTable = new PropertyNameTable();
    }
}
