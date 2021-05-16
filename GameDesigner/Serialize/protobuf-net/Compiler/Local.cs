//#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
using System;
#if FEAT_IKVM
using IKVM.Reflection.Emit;
using Type  = IKVM.Reflection.Type;
#else
using System.Reflection.Emit;
#endif

namespace ProtoBuf.Compiler
{
    internal sealed class Local : IDisposable
    {
        // public static readonly Local InputValue = new Local(null, null);
        LocalBuilder value;
        public Type Type { get { return type; } }
        public Local AsCopy()
        {
            if (ctx == null) return this; // can re-use if context-free
            return new Local(value, type);
        }
        internal LocalBuilder Value
        {
            get
            {
                if (value == null)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                return value;
            }
        }
        CompilerContext ctx;
        public void Dispose()
        {
            if (ctx != null)
            {
                // only *actually* dispose if this is context-bound; note that non-bound
                // objects are cheekily re-used, and *must* be left intact agter a "using" etc
                ctx.ReleaseToPool(value);
                value = null;
                ctx = null;
            }

        }
        private Local(LocalBuilder value, Type type)
        {
            this.value = value;
            this.type = type;
        }
        private readonly Type type;
        internal Local(CompilerContext ctx, Type type)
        {
            this.ctx = ctx;
            if (ctx != null) { value = ctx.GetFromPool(type); }
            this.type = type;
        }

        internal bool IsSame(Local other)
        {
            if (this == other) return true;

            object ourVal = value; // use prop to ensure obj-disposed etc
            return other != null && ourVal == other.value;
        }
    }


}
//#endif