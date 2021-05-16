//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using ILRuntime.Mono.Collections.Generic;
using System;
using System.Text;
using System.Threading;

namespace ILRuntime.Mono.Cecil
{

    public class MethodReference : MemberReference, IMethodSignature, IGenericParameterProvider, IGenericContext
    {
        int hashCode = -1;
        static int instance_id;
        internal ParameterDefinitionCollection parameters;
        MethodReturnType return_type;

        bool has_this;
        bool explicit_this;
        MethodCallingConvention calling_convention;
        internal Collection<GenericParameter> generic_parameters;

        public virtual bool HasThis
        {
            get { return has_this; }
            set { has_this = value; }
        }

        public virtual bool ExplicitThis
        {
            get { return explicit_this; }
            set { explicit_this = value; }
        }

        public virtual MethodCallingConvention CallingConvention
        {
            get { return calling_convention; }
            set { calling_convention = value; }
        }

        public virtual bool HasParameters
        {
            get { return !parameters.IsNullOrEmpty(); }
        }

        public virtual Collection<ParameterDefinition> Parameters
        {
            get
            {
                if (parameters == null)
                    Interlocked.CompareExchange(ref parameters, new ParameterDefinitionCollection(this), null);

                return parameters;
            }
        }

        IGenericParameterProvider IGenericContext.Type
        {
            get
            {
                TypeReference declaring_type = DeclaringType;
                GenericInstanceType instance = declaring_type as GenericInstanceType;
                if (instance != null)
                    return instance.ElementType;

                return declaring_type;
            }
        }

        IGenericParameterProvider IGenericContext.Method
        {
            get { return this; }
        }

        GenericParameterType IGenericParameterProvider.GenericParameterType
        {
            get { return GenericParameterType.Method; }
        }

        public virtual bool HasGenericParameters
        {
            get { return !generic_parameters.IsNullOrEmpty(); }
        }

        public virtual Collection<GenericParameter> GenericParameters
        {
            get
            {
                if (generic_parameters == null)
                    Interlocked.CompareExchange(ref generic_parameters, new GenericParameterCollection(this), null);

                return generic_parameters;
            }
        }

        public TypeReference ReturnType
        {
            get
            {
                MethodReturnType return_type = MethodReturnType;
                return return_type != null ? return_type.ReturnType : null;
            }
            set
            {
                MethodReturnType return_type = MethodReturnType;
                if (return_type != null)
                    return_type.ReturnType = value;
            }
        }

        public virtual MethodReturnType MethodReturnType
        {
            get { return return_type; }
            set { return_type = value; }
        }

        public override string FullName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(ReturnType.FullName)
                    .Append(" ")
                    .Append(MemberFullName());
                this.MethodSignatureFullName(builder);
                return builder.ToString();
            }
        }

        public virtual bool IsGenericInstance
        {
            get { return false; }
        }

        public override bool ContainsGenericParameter
        {
            get
            {
                if (ReturnType.ContainsGenericParameter || base.ContainsGenericParameter)
                    return true;

                if (!HasParameters)
                    return false;

                Collection<ParameterDefinition> parameters = Parameters;

                for (int i = 0; i < parameters.Count; i++)
                    if (parameters[i].ParameterType.ContainsGenericParameter)
                        return true;

                return false;
            }
        }

        public override int GetHashCode()
        {
            if (hashCode == -1)
                hashCode = System.Threading.Interlocked.Add(ref instance_id, 1);
            return hashCode;
        }

        internal MethodReference()
        {
            return_type = new MethodReturnType(this);
            token = new MetadataToken(TokenType.MemberRef);
        }

        public MethodReference(string name, TypeReference returnType)
            : base(name)
        {
            Mixin.CheckType(returnType, Mixin.Argument.returnType);

            return_type = new MethodReturnType(this);
            return_type.ReturnType = returnType;
            token = new MetadataToken(TokenType.MemberRef);
        }

        public MethodReference(string name, TypeReference returnType, TypeReference declaringType)
            : this(name, returnType)
        {
            Mixin.CheckType(declaringType, Mixin.Argument.declaringType);

            DeclaringType = declaringType;
        }

        public virtual MethodReference GetElementMethod()
        {
            return this;
        }

        protected override IMemberDefinition ResolveDefinition()
        {
            return Resolve();
        }

        public new virtual MethodDefinition Resolve()
        {
            ModuleDefinition module = Module;
            if (module == null)
                throw new NotSupportedException();

            return module.Resolve(this);
        }
    }

    static partial class Mixin
    {

        public static bool IsVarArg(this IMethodSignature self)
        {
            return self.CallingConvention == MethodCallingConvention.VarArg;
        }

        public static int GetSentinelPosition(this IMethodSignature self)
        {
            if (!self.HasParameters)
                return -1;

            Collection<ParameterDefinition> parameters = self.Parameters;
            for (int i = 0; i < parameters.Count; i++)
                if (parameters[i].ParameterType.IsSentinel)
                    return i;

            return -1;
        }
    }
}
