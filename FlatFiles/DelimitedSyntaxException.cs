using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents an error that occurred while parsing a delimited stream.
    /// </summary>
    public sealed class DelimitedSyntaxException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of a DelimitedSyntaxException.
        /// </summary>
        /// <param name="message">The details of the syntax error.</param>
        internal DelimitedSyntaxException(string message)
            : base(message)
        {
        }
    }
}
