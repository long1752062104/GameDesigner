using System;

namespace Newtonsoft_X.Json.Serialization
{
    internal struct ResolverContractKey : IEquatable<ResolverContractKey>
    {
        public ResolverContractKey(Type resolverType, Type contractType)
        {
            _resolverType = resolverType;
            _contractType = contractType;
        }

        public override int GetHashCode()
        {
            return _resolverType.GetHashCode() ^ _contractType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ResolverContractKey && Equals((ResolverContractKey)obj);
        }

        public bool Equals(ResolverContractKey other)
        {
            return _resolverType == other._resolverType && _contractType == other._contractType;
        }

        private readonly Type _resolverType;

        private readonly Type _contractType;
    }
}
