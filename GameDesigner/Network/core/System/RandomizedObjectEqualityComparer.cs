using System.Collections;
using System.Security;

namespace Net.System
{
    // Token: 0x0200049D RID: 1181
    internal sealed class RandomizedObjectEqualityComparer : IEqualityComparer, IWellKnownStringEqualityComparer
    {
        // Token: 0x0600398D RID: 14733 RVA: 0x000DADAA File Offset: 0x000D8FAA
        public RandomizedObjectEqualityComparer()
        {
            _entropy = HashHelpers.GetEntropy();
        }

        // Token: 0x0600398E RID: 14734 RVA: 0x000DADBD File Offset: 0x000D8FBD
        public new bool Equals(object x, object y)
        {
            if (x != null)
            {
                return y != null && x.Equals(y);
            }
            return y == null;
        }

        // Token: 0x0600398F RID: 14735 RVA: 0x000DADD8 File Offset: 0x000D8FD8
        [SecuritySafeCritical]
        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            string text = obj as string;
            if (text != null)
            {
                //return string.InternalMarvin32HashString(text, text.Length, this._entropy);
            }
            return obj.GetHashCode();
        }

        // Token: 0x06003990 RID: 14736 RVA: 0x000DAE10 File Offset: 0x000D9010
        public override bool Equals(object obj)
        {
            RandomizedObjectEqualityComparer randomizedObjectEqualityComparer = obj as RandomizedObjectEqualityComparer;
            return randomizedObjectEqualityComparer != null && _entropy == randomizedObjectEqualityComparer._entropy;
        }

        // Token: 0x06003991 RID: 14737 RVA: 0x000DAE37 File Offset: 0x000D9037
        public override int GetHashCode()
        {
            return GetType().Name.GetHashCode() ^ (int)(_entropy & 2147483647L);
        }

        // Token: 0x06003992 RID: 14738 RVA: 0x000DAE58 File Offset: 0x000D9058
        IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer()
        {
            return new RandomizedObjectEqualityComparer();
        }

        // Token: 0x06003993 RID: 14739 RVA: 0x000DAE5F File Offset: 0x000D905F
        IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization()
        {
            return null;
        }

        // Token: 0x04001895 RID: 6293
        private long _entropy;
    }
}