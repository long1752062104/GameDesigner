using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Serialization
{
    internal class DefaultContractResolverState
    {
        public Dictionary<ResolverContractKey, JsonContract> ContractCache;

        public PropertyNameTable NameTable = new PropertyNameTable();
    }
}
