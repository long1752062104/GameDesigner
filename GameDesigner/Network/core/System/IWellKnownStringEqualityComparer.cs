using System.Collections;

namespace Net.System
{
    // Token: 0x0200007A RID: 122
    internal interface IWellKnownStringEqualityComparer
    {
        // Token: 0x060005A8 RID: 1448
        IEqualityComparer GetRandomizedEqualityComparer();

        // Token: 0x060005A9 RID: 1449
        IEqualityComparer GetEqualityComparerForSerialization();
    }
}