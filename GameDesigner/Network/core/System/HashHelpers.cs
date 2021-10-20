using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;

namespace Net.System
{
    // Token: 0x0200046B RID: 1131
    //[FriendAccessAllowed]
    internal static class HashHelpers
    {
        // Token: 0x17000851 RID: 2129
        // (get) Token: 0x06003773 RID: 14195 RVA: 0x000D4F8C File Offset: 0x000D318C
        internal static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
        {
            get
            {
                if (s_SerializationInfoTable == null)
                {
                    ConditionalWeakTable<object, SerializationInfo> value = new ConditionalWeakTable<object, SerializationInfo>();
                    Interlocked.CompareExchange(ref s_SerializationInfoTable, value, null);
                }
                return s_SerializationInfoTable;
            }
        }

        // Token: 0x06003774 RID: 14196 RVA: 0x000D4FB8 File Offset: 0x000D31B8
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int num = (int)Math.Sqrt(candidate);
                for (int i = 3; i <= num; i += 2)
                {
                    if (candidate % i == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return candidate == 2;
        }

        // Token: 0x06003775 RID: 14197 RVA: 0x000D4FEC File Offset: 0x000D31EC
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException("Arg_HTCapacityOverflow");
            }
            for (int i = 0; i < primes.Length; i++)
            {
                int num = primes[i];
                if (num >= min)
                {
                    return num;
                }
            }
            for (int j = min | 1; j < 2147483647; j += 2)
            {
                if (IsPrime(j) && (j - 1) % 101 != 0)
                {
                    return j;
                }
            }
            return min;
        }

        // Token: 0x06003776 RID: 14198 RVA: 0x000D5052 File Offset: 0x000D3252
        public static int GetMinPrime()
        {
            return primes[0];
        }

        // Token: 0x06003777 RID: 14199 RVA: 0x000D505C File Offset: 0x000D325C
        public static int ExpandPrime(int oldSize)
        {
            int num = 2 * oldSize;
            if (num > 2146435069 && 2146435069 > oldSize)
            {
                return 2146435069;
            }
            return GetPrime(num);
        }

        // Token: 0x06003778 RID: 14200 RVA: 0x000D5089 File Offset: 0x000D3289
        public static bool IsWellKnownEqualityComparer(object comparer)
        {
            return comparer == null || comparer == EqualityComparer<string>.Default || comparer is IWellKnownStringEqualityComparer;
        }

        // Token: 0x06003779 RID: 14201 RVA: 0x000D50A4 File Offset: 0x000D32A4
        public static global::System.Collections.IEqualityComparer GetRandomizedEqualityComparer(object comparer)
        {
            if (comparer == null)
            {
                return null;
            }
            if (comparer == EqualityComparer<string>.Default)
            {
                return null;
            }
            IWellKnownStringEqualityComparer wellKnownStringEqualityComparer = comparer as IWellKnownStringEqualityComparer;
            if (wellKnownStringEqualityComparer != null)
            {
                return wellKnownStringEqualityComparer.GetRandomizedEqualityComparer();
            }
            return null;
        }

        // Token: 0x0600377A RID: 14202 RVA: 0x000D50DC File Offset: 0x000D32DC
        public static object GetEqualityComparerForSerialization(object comparer)
        {
            if (comparer == null)
            {
                return null;
            }
            IWellKnownStringEqualityComparer wellKnownStringEqualityComparer = comparer as IWellKnownStringEqualityComparer;
            if (wellKnownStringEqualityComparer != null)
            {
                return wellKnownStringEqualityComparer.GetEqualityComparerForSerialization();
            }
            return comparer;
        }

        // Token: 0x0600377B RID: 14203 RVA: 0x000D5100 File Offset: 0x000D3300
        internal static long GetEntropy()
        {
            object obj = lockObj;
            long result;
            lock (obj)
            {
                if (currentIndex == 1024)
                {
                    if (rng == null)
                    {
                        rng = RandomNumberGenerator.Create();
                        data = new byte[1024];
                    }
                    rng.GetBytes(data);
                    currentIndex = 0;
                }
                long num = BitConverter.ToInt64(data, currentIndex);
                currentIndex += 8;
                result = num;
            }
            return result;
        }

        // Token: 0x04001839 RID: 6201
        public const int HashCollisionThreshold = 100;

        // Token: 0x0400183A RID: 6202
        //public static bool s_UseRandomizedStringHashing = string.UseRandomizedHashing();

        // Token: 0x0400183B RID: 6203
        public static readonly int[] primes = new int[]
        {
            3,
            7,
            11,
            17,
            23,
            29,
            37,
            47,
            59,
            71,
            89,
            107,
            131,
            163,
            197,
            239,
            293,
            353,
            431,
            521,
            631,
            761,
            919,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369
        };

        // Token: 0x0400183C RID: 6204
        private static ConditionalWeakTable<object, SerializationInfo> s_SerializationInfoTable;

        // Token: 0x0400183D RID: 6205
        public const int MaxPrimeArrayLength = 2146435069;

        // Token: 0x0400183E RID: 6206
        private const int bufferSize = 1024;

        // Token: 0x0400183F RID: 6207
        private static RandomNumberGenerator rng;

        // Token: 0x04001840 RID: 6208
        private static byte[] data;

        // Token: 0x04001841 RID: 6209
        private static int currentIndex = 1024;

        // Token: 0x04001842 RID: 6210
        private static readonly object lockObj = new object();
    }
}
