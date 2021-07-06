using System;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Specifies how JSON comments are handled when loading JSON.
    /// </summary>
    public enum CommentHandling
    {
        /// <summary>
        /// Ignore comments.
        /// </summary>
        Ignore,
        /// <summary>
        /// Load comments as a <see cref="T:Newtonsoft.Json.Linq.JValue" /> with type <see cref="F:Newtonsoft.Json.Linq.JTokenType.Comment" />.
        /// </summary>
        Load
    }
}
