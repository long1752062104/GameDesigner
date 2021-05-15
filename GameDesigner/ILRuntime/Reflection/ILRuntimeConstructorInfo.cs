using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ILRuntime.Reflection
{
    public class ILRuntimeConstructorInfo : ConstructorInfo
    {
        ILMethod method;
        ILRuntimeParameterInfo[] parameters;
        public ILRuntimeConstructorInfo(ILMethod m)
        {
            method = m;
            parameters = new ILRuntimeParameterInfo[m.ParameterCount];
            for (int i = 0; i < m.ParameterCount; i++)
            {
                Mono.Cecil.ParameterDefinition pd = m.Definition.Parameters[i];
                parameters[i] = new ILRuntimeParameterInfo(pd, m.Parameters[i], this);
            }
        }

        internal ILMethod ILMethod { get { return method; } }
        public override MethodAttributes Attributes
        {
            get
            {
                return MethodAttributes.Public;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                return method.DeclearingType.ReflectionType;
            }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                return method.Name;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                return method.DeclearingType.ReflectionType;
            }
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetParameters()
        {
            return parameters;
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            Runtime.Intepreter.ILTypeInstance res = ((ILType)method.DeclearingType).Instantiate(false);
            method.DeclearingType.AppDomain.Invoke(method, res, parameters);
            return res;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return parameters.Length == 0;
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            Runtime.Intepreter.ILTypeInstance res = ((ILType)method.DeclearingType).Instantiate(false);
            method.DeclearingType.AppDomain.Invoke(method, res, parameters);
            return res;
        }
    }
}
