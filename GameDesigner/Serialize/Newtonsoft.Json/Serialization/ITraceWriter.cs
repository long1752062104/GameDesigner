using System;
using System.Diagnostics;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Represents a trace writer.
    /// </summary>
    public interface ITraceWriter
    {
        /// <summary>
        /// Gets the <see cref="T:System.Diagnostics.TraceLevel" /> that will be used to filter the trace messages passed to the writer.
        /// For example a filter level of <see cref="F:System.Diagnostics.TraceLevel.Info" /> will exclude <see cref="F:System.Diagnostics.TraceLevel.Verbose" /> messages and include <see cref="F:System.Diagnostics.TraceLevel.Info" />,
        /// <see cref="F:System.Diagnostics.TraceLevel.Warning" /> and <see cref="F:System.Diagnostics.TraceLevel.Error" /> messages.
        /// </summary>
        /// <value>The <see cref="T:System.Diagnostics.TraceLevel" /> that will be used to filter the trace messages passed to the writer.</value>
        TraceLevel LevelFilter { get; }

        /// <summary>
        /// Writes the specified trace level, message and optional exception.
        /// </summary>
        /// <param name="level">The <see cref="T:System.Diagnostics.TraceLevel" /> at which to write this trace.</param>
        /// <param name="message">The trace message.</param>
        /// <param name="ex">The trace exception. This parameter is optional.</param>
        void Trace(TraceLevel level, string message, Exception ex);
    }
}
