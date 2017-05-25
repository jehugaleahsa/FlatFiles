using System;
using FlatFiles.Resources;

namespace FlatFiles
{
    /// <summary>
    /// Represents an error that occurred while parsing a stream.
    /// </summary>
    public sealed class FlatFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a FlatFileException, recording which record caused the error.
        /// </summary>
        /// <param name="message">A message describing the cause of the error.</param>
        /// <param name="recordNumber">The position of the record with the invalid format.</param>
        internal FlatFileException(string message, int recordNumber)
            : base(String.Format(message, recordNumber))
        {
        }

        /// <summary>
        /// Initializes a new instance of a FlatFileException, recording which record caused the error.
        /// </summary>
        /// <param name="message">A message describing the cause of the error.</param>
        /// <param name="recordNumber">The position of the record with the invalid format.</param>
        /// <param name="innerException">An inner exception containing the cause of the underlying error.</param>
        internal FlatFileException(string message, int recordNumber, Exception innerException)
            : base(String.Format(message, recordNumber), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FlatFileException, recording which record caused the error.
        /// </summary>
        /// <param name="recordNumber">The position of the record with the invalid format.</param>
        /// <param name="innerException">An inner exception containing the cause of the underlying error.</param>
        internal FlatFileException(int recordNumber, Exception innerException)
            : base(String.Format(SharedResources.InvalidRecordFormatNumber, recordNumber), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FlatFileException.
        /// </summary>
        /// <param name="message">A message describing the cause of the error.</param>
        /// <param name="innerException">An inner exception containing the cause of the underlying error.</param>
        internal FlatFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
