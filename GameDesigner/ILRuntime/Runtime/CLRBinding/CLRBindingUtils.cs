using System;

namespace ILRuntime.Runtime.CLRBinding
{
    public class CLRBindingUtils
    {
        static private Action<Enviorment.AppDomain> initializeAction;
        static public void RegisterBindingAction(Action<Enviorment.AppDomain> action)
        {
            initializeAction = action;
        }

        /// <summary>
        /// This method can instead of CLRBindings.Initialize for avoid compile error when hasn't generator bindingCode.
        /// </summary>
        /// <param name="appDomain"></param>
        static public void Initialize(Enviorment.AppDomain appDomain)
        {
            initializeAction?.Invoke(appDomain);
        }
    }
}
