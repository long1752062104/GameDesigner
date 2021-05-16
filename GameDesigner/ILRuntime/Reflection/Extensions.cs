using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILRuntime.Reflection
{
    static class Extensions
    {
        public static object CreateInstance(this CustomAttribute attribute, IType at, Runtime.Enviorment.AppDomain appdomain)
        {
            object ins;
            List<IType> param = null;
            if (at is ILType)
            {
                ILType it = (ILType)at;
                if (!attribute.HasConstructorArguments)
                    ins = it.Instantiate(true);
                else
                {
                    ins = it.Instantiate(false);
                    if (param == null)
                        param = new List<IType>();
                    param.Clear();
                    object[] p = new object[attribute.ConstructorArguments.Count];
                    for (int j = 0; j < attribute.ConstructorArguments.Count; j++)
                    {
                        CustomAttributeArgument ca = attribute.ConstructorArguments[j];
                        param.Add(appdomain.GetType(ca.Type, null, null));
                        p[j] = ca.Value;
                    }
                    IMethod ctor = it.GetConstructor(param);
                    appdomain.Invoke(ctor, ins, p);
                }

                if (attribute.HasProperties)
                {
                    object[] p = new object[1];
                    foreach (CustomAttributeNamedArgument j in attribute.Properties)
                    {
                        p[0] = j.Argument.Value;
                        IMethod setter = it.GetMethod("set_" + j.Name, 1);
                        appdomain.Invoke(setter, ins, p);
                    }
                }
                if (attribute.HasFields)
                {
                    foreach (CustomAttributeNamedArgument j in attribute.Fields)
                    {
                        IType field = it.GetField(j.Name, out int index);
                        if (field != null)
                            ((ILRuntime.Runtime.Intepreter.ILTypeInstance)ins)[index] = j.Argument.Value;
                    }
                }
                ins = ((ILRuntime.Runtime.Intepreter.ILTypeInstance)ins).CLRInstance;
            }
            else
            {
                param = new List<IType>();
                object[] p = null;
                attribute.Resolve();
                if (attribute.arguments != null)
                {
                    p = new object[attribute.ConstructorArguments.Count];
                    for (int j = 0; j < attribute.ConstructorArguments.Count; j++)
                    {
                        CustomAttributeArgument ca = attribute.ConstructorArguments[j];
                        param.Add(appdomain.GetType(ca.Type, null, null));
                        p[j] = ca.Value;
                    }
                }
                ins = ((CLRMethod)at.GetConstructor(param)).ConstructorInfo.Invoke(p);
                if (attribute.properties != null)
                {
                    foreach (CustomAttributeNamedArgument j in attribute.Properties)
                    {
                        System.Reflection.PropertyInfo prop = at.TypeForCLR.GetProperty(j.Name);
                        prop.SetValue(ins, j.Argument.Value, null);
                    }
                }
                if (attribute.fields != null)
                {
                    foreach (CustomAttributeNamedArgument j in attribute.Fields)
                    {
                        System.Reflection.FieldInfo field = at.TypeForCLR.GetField(j.Name);
                        field.SetValue(ins, j.Argument.Value);
                    }
                }
            }

            return ins;
        }
    }
}
