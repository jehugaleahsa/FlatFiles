using System;

namespace FlatFiles
{
    /// <summary>
    /// Raised when a error occurs while parsing a record.
    /// </summary>
    public sealed class ProcessingErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the zero-based index of the record being processed when the error occurred.
        /// </summary>
        public int RecordNumber { get; internal set; }

        /// <summary>
        /// Gets or sets the schema being used when the error occurred.
        /// </summary>
        public ISchema Schema { get; internal set; }

        /// <summary>
        /// Gets or sets the column definition being processed when the error occurred.
        /// </summary>
        public IColumnDefinition ColumnDefinition { get; internal set; }

        /// <summary>
        /// Gets or sets the value that was being parsed when the error occurred.
        /// </summary>
        public string ColumnValue { get; internal set; }

        /// <summary>
        /// Gets or sets whether the parser should attempt to continue parsing.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}
