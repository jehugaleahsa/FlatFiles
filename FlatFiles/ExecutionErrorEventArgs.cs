using System;

namespace FlatFiles
{
    /// <summary>
    /// Raised when a error occurs while parsing a record.
    /// </summary>
    public sealed class ExecutionErrorEventArgs : EventArgs
    {
        internal ExecutionErrorEventArgs(RecordProcessingException exception)
        {
            Exception = exception;
            RecordContext = exception.Context;
            if (exception.InnerException != null && exception.InnerException is ColumnProcessingException columnException)
            {
                ColumnContext = columnException.ColumnContext;
                ColumnValue = columnException.ColumnValue;
            }
        }

        /// <summary>
        /// Gets the metata for the column that was being processed when the error occurred.
        /// </summary>
        public IColumnContext ColumnContext { get; }

        /// <summary>
        /// Gets the metata for the record that was being processed when the error occurred.
        /// </summary>
        public IRecordContext RecordContext { get; }

        /// <summary>
        /// Gets the value that was being parsed when the error occurred.
        /// </summary>
        public string ColumnValue { get; }

        /// <summary>
        /// Gets the exception that was raised.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets or sets whether the parser should attempt to continue parsing.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}
