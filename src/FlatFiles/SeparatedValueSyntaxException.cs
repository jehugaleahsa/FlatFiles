using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents an error that occurred while parsing a separated value stream.
    /// </summary>
    public sealed class SeparatedValueSyntaxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a SeparatedValueSyntaxException.
        /// </summary>
        /// <param name="message">The details of the syntax error.</param>
        internal SeparatedValueSyntaxException(string message)
            : base(message)
        {
        }
    }
}
