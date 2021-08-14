using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents an error that was thrown while parsing a record.
    /// </summary>
    public sealed class RecordProcessingException : FlatFileException
    {
        internal RecordProcessingException(IRecordContext context, string message)
            : base(string.Format(null, message, context.PhysicalRecordNumber))
        {
            RecordContext = context;
        }

        internal RecordProcessingException(IRecordContext context, string message, Exception innerException)
            : base(string.Format(null, message, context.PhysicalRecordNumber), innerException)
        {
            RecordContext = context;
        }

        /// <summary>
        /// Gets the metadata for the record being processed when the error occurred.
        /// </summary>
        public IRecordContext RecordContext { get; }
    }
}
