using Newtonsoft_X.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Newtonsoft_X.Json.Utilities
{
    internal class ReflectionObject
    {
        public ObjectConstructor<object> Creator { get; private set; }

        public IDictionary<string, ReflectionMember> Members { get; private set; }

        public ReflectionObject()
        {
            Members = new Dictionary<string, ReflectionMember>();
        }

        public object GetValue(object target, string member)
        {
            return Members[member].Getter(target);
        }

        public void SetValue(object target, string member, object value)
        {
            Members[member].Setter(target, value);
        }

        public Type GetType(string member)
        {
            return Members[member].MemberType;
        }

        public static ReflectionObject Create(Type t, params string[] memberNames)
        {
            return ReflectionObject.Create(t, null, memberNames);
        }

        public static ReflectionObject Create(Type t, MethodBase creator, params string[] memberNames)
        {
            ReflectionObject reflectionObject = new ReflectionObject();
            ReflectionDelegateFactory reflectionDelegateFactory = JsonTypeReflector.ReflectionDelegateFactory;
            if (creator != null)
            {
                reflectionObject.Creator = reflectionDelegateFactory.CreateParameterizedConstructor(creator);
            }
            else if (ReflectionUtils.HasDefaultConstructor(t, false))
            {
                Func<object> ctor = reflectionDelegateFactory.CreateDefaultConstructor<object>(t);
                reflectionObject.Creator = ((object[] args) => ctor());
            }
            int i = 0;
            while (i < memberNames.Length)
            {
                string text = memberNames[i];
                MemberInfo[] member = t.GetMember(text, BindingFlags.Instance | BindingFlags.Public);
                if (member.Length != 1)
                {
                    throw new ArgumentException("Expected a single member with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, text));
                }
                MemberInfo memberInfo = member.Single<MemberInfo>();
                ReflectionMember reflectionMember = new ReflectionMember();
                MemberTypes memberTypes = memberInfo.MemberType();
                if (memberTypes == MemberTypes.Field)
                {
                    goto IL_AD;
                }
                if (memberTypes != MemberTypes.Method)
                {
                    if (memberTypes == MemberTypes.Property)
                    {
                        goto IL_AD;
                    }
                    throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith(CultureInfo.InvariantCulture, memberInfo.MemberType(), memberInfo.Name));
                }
                else
                {
                    MethodInfo methodInfo = (MethodInfo)memberInfo;
                    if (methodInfo.IsPublic)
                    {
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (parameters.Length == 0 && methodInfo.ReturnType != typeof(void))
                        {
                            MethodCall<object, object> call = reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
                            reflectionMember.Getter = ((object target) => call(target, new object[0]));
                        }
                        else if (parameters.Length == 1 && methodInfo.ReturnType == typeof(void))
                        {
                            MethodCall<object, object> call = reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
                            reflectionMember.Setter = delegate (object target, object arg)
                            {
                                call(target, new object[]
                                {
                                    arg
                                });
                            };
                        }
                    }
                }
            IL_1B8:
                if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
                {
                    reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
                }
                if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
                {
                    reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
                }
                reflectionMember.MemberType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
                reflectionObject.Members[text] = reflectionMember;
                i++;
                continue;
            IL_AD:
                if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
                {
                    reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
                }
                if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
                {
                    reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
                    goto IL_1B8;
                }
                goto IL_1B8;
            }
            return reflectionObject;
        }
    }
}
