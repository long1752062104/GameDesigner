using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ILRuntime.Reflection
{
    public class ILRuntimeParameterInfo : ParameterInfo
    {
        IType type;
        MethodBase method;
        Mono.Cecil.ParameterDefinition definition;

        public ILRuntimeParameterInfo(Mono.Cecil.ParameterDefinition definition, IType type, MethodBase method)
        {
            this.type = type;
            this.method = method;
            MemberImpl = method;
            this.definition = definition;
            NameImpl = definition.Name;
        }
        public override Type ParameterType
        {
            get
            {
                return type.ReflectionType;
            }
        }
    }
}
