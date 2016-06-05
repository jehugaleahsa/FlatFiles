using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Represents an error that occurred while parsing a stream.
    /// </summary>
    [Serializable]
    public sealed class FlatFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a ParserException, recording which record caused the error.
        /// </summary>
        /// <param name="message">A message describing the cause of the error.</param>
        /// <param name="recordNumber">The position of the record with the invalid format.</param>
        internal FlatFileException(string message, int recordNumber)
            : base(String.Format(message, recordNumber))
        {
        }

        /// <summary>
        /// Initializes a new instance of a ParserException, recording which record caused the error.
        /// </summary>
        /// <param name="recordNumber">The position of the record with the invalid format.</param>
        /// <param name="innerException">An inner exception containing the cause of the underlying error.</param>
        internal FlatFileException(int recordNumber, Exception innerException)
            : base(String.Format(Resources.InvalidRecordFormatNumber, recordNumber), innerException)
        {
        }
    }
}
