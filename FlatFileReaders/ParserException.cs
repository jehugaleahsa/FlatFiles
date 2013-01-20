using System;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents an error that occurred while parsing a stream.
    /// </summary>
    [Serializable]
    public sealed class ParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a ParserException, recording which record caused the error.
        /// </summary>
        /// <param name="recordNumber">The position of the record with the invalid format.</param>
        internal ParserException(int recordNumber)
            : base(String.Format(Resources.InvalidRecordFormatNumber, recordNumber))
        {
        }
    }
}
