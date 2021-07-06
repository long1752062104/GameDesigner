using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// The default serialization binder used when resolving and loading classes from type names.
    /// </summary>
    public class DefaultSerializationBinder : SerializationBinder
    {
        private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
        {
            string assemblyName = typeNameKey.AssemblyName;
            string typeName = typeNameKey.TypeName;
            if (assemblyName == null)
            {
                return Type.GetType(typeName);
            }
            Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
            if (assembly == null)
            {
                foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly2.FullName == assemblyName)
                    {
                        assembly = assembly2;
                        break;
                    }
                }
            }
            if (assembly == null)
            {
                throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, assemblyName));
            }
            Type type = assembly.GetType(typeName);
            if (type == null)
            {
                throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName));
            }
            return type;
        }

        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly" /> name of the serialized object.</param>
        /// <param name="typeName">Specifies the <see cref="T:System.Type" /> name of the serialized object.</param>
        /// <returns>
        /// The type of the object the formatter creates a new instance of.
        /// </returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            return _typeCache.Get(new DefaultSerializationBinder.TypeNameKey(assemblyName, typeName));
        }

        internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

        private readonly ThreadSafeStore<DefaultSerializationBinder.TypeNameKey, Type> _typeCache = new ThreadSafeStore<DefaultSerializationBinder.TypeNameKey, Type>(new Func<DefaultSerializationBinder.TypeNameKey, Type>(DefaultSerializationBinder.GetTypeFromTypeNameKey));

        internal struct TypeNameKey : IEquatable<DefaultSerializationBinder.TypeNameKey>
        {
            public TypeNameKey(string assemblyName, string typeName)
            {
                AssemblyName = assemblyName;
                TypeName = typeName;
            }

            public override int GetHashCode()
            {
                return ((AssemblyName != null) ? AssemblyName.GetHashCode() : 0) ^ ((TypeName != null) ? TypeName.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                return obj is DefaultSerializationBinder.TypeNameKey && Equals((DefaultSerializationBinder.TypeNameKey)obj);
            }

            public bool Equals(DefaultSerializationBinder.TypeNameKey other)
            {
                return AssemblyName == other.AssemblyName && TypeName == other.TypeName;
            }

            internal readonly string AssemblyName;

            internal readonly string TypeName;
        }
    }
}
