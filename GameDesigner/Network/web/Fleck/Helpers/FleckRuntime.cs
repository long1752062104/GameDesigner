using System;

namespace Fleck.Helpers
{
    internal static class FleckRuntime
    {
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static bool IsRunningOnWindows() =>
#if NET45 || NET40
            true;
#else
            //System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            true;
#endif
    }
}