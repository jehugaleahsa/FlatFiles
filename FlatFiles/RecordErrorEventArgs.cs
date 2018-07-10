using System;

namespace FlatFiles
{
    /// <summary>
    /// Raised when a error occurs while reading or writing a record.
    /// </summary>
    public sealed class RecordErrorEventArgs : EventArgs
    {
        private readonly RecordProcessingException exception;

        internal RecordErrorEventArgs(RecordProcessingException exception)
        {
            this.exception = exception;
        }

        /// <summary>
        /// Gets the metadata for the record being processed when the error occurred.
        /// </summary>
        public IRecordContext RecordContext => exception.RecordContext;

        /// <summary>
        /// Gets the exception that was thrown.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets or sets whether the parser should attempt to continue reading/writing.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}
