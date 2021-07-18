using System;

namespace FlatFiles
{
    /// <summary>
    /// Raised when a error occurs while reading or writing a column.
    /// </summary>
    public sealed class ColumnErrorEventArgs : EventArgs
    {
        private readonly ColumnProcessingException exception;

        internal ColumnErrorEventArgs(ColumnProcessingException exception)
        {
            this.exception = exception;
        }

        /// <summary>
        /// Gets the schema that was being used to parse when the error occurred.
        /// </summary>
        public IColumnContext? ColumnContext => exception.ColumnContext;

        /// <summary>
        /// Gets the value that was being parsed when the error occurred.
        /// </summary>
        public object? ColumnValue => exception.ColumnValue;

        /// <summary>
        /// Gets the exception that was thrown.
        /// </summary>
        public Exception Exception => exception;

        /// <summary>
        /// Gets or sets whether the parser should attempt to continue parsing.
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// Gets or sets the replacement value to use when an error occurs.
        /// </summary>
        /// <remarks>
        /// When reading, the type of the substitution must match the type of the column.
        /// When writing, the type of the substitution must be a string.
        /// </remarks>
        public object? Substitution { get; set; }
    }
}
