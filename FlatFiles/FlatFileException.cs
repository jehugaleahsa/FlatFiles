using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Represents an error that occurred while parsing a stream.
    /// </summary>
    public class FlatFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a FlatFileException, recording which record caused the error.
        /// </summary>
        /// <param name="message">A message describing the cause of the error.</param>
        internal FlatFileException(string message)
            : base(message)
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

    /// <summary>
    /// Represents an error that was thrown while parsing a column value.
    /// </summary>
    public sealed class ColumnProcessingException : FlatFileException
    {
        internal ColumnProcessingException(IColumnDefinition definition, int position, object value, Exception innerException)
            : base(GetErrorMessage(definition, position, value), innerException)
        {
            ColumnContext = null;
            ColumnValue = value;
        }

        internal ColumnProcessingException(IColumnContext context, object value, Exception innerException)
            : base(GetErrorMessage(context.ColumnDefinition, context.PhysicalIndex, value), innerException)
        {
            ColumnContext = context;
            ColumnValue = value;
        }

        private static string GetErrorMessage(IColumnDefinition definition, int position, object value)
        {
            var message = String.Format(null, Resources.InvalidColumnConversion, value, definition.ColumnType.FullName, definition.ColumnName, position);
            return message;
        }

        /// <summary>
        /// Gets the schema that was being used to parse when the error occurred.
        /// </summary>
        public IColumnContext ColumnContext { get; internal set; }

        /// <summary>
        /// Gets the value that was being parsed when the error occurred.
        /// </summary>
        public object ColumnValue { get; }
    }

    /// <summary>
    /// Represents an error that was thrown while parsing a record.
    /// </summary>
    public sealed class RecordProcessingException : FlatFileException
    {
        internal RecordProcessingException(IRecordContext context, string message)
            : base(String.Format(null, message, context.PhysicalRecordNumber))
        {
            RecordContext = context;
        }

        internal RecordProcessingException(IRecordContext context, string message, Exception innerException)
            : base(String.Format(null, message, context.PhysicalRecordNumber), innerException)
        {
            RecordContext = context;
        }

        /// <summary>
        /// Gets the metadata for the record being processed when the error occurred.
        /// </summary>
        public IRecordContext RecordContext { get; }
    }
}
