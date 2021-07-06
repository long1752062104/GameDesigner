using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal delegate TResult MethodCall<T, TResult>(T target, params object[] args);
}
