using System;

namespace FlatFiles
{
    /// <summary>
    /// Raised when a error occurs while parsing a record.
    /// </summary>
    public sealed class ProcessingErrorEventArgs : EventArgs
    {
        internal ProcessingErrorEventArgs(RecordProcessingException exception)
        {
            this.Exception = exception;
            this.RecordNumber = exception.RecordNumber;
            if (exception.InnerException != null && exception.InnerException is ColumnProcessingException columnException)
            {
                this.Schema = columnException.Schema;
                this.ColumnDefinition = columnException.ColumnDefinition;
                this.ColumnValue = columnException.ColumnValue;
            }
        }

        /// <summary>
        /// Gets the index of the record being processed when the error occurred.
        /// </summary>
        public int RecordNumber { get; private set; }

        /// <summary>
        /// Gets the schema being used when the error occurred.
        /// </summary>
        public ISchema Schema { get; private set; }

        /// <summary>
        /// Gets the column definition being processed when the error occurred.
        /// </summary>
        public IColumnDefinition ColumnDefinition { get; private set; }

        /// <summary>
        /// Gets the value that was being parsed when the error occurred.
        /// </summary>
        public string ColumnValue { get; private set; }

        /// <summary>
        /// Gets the exception that was raised.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets whether the parser should attempt to continue parsing.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}
