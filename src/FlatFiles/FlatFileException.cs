using System;
using FlatFiles.Resources;

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
        protected FlatFileException(string message)
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
        internal ColumnProcessingException(ISchema schema, IColumnDefinition definition, string value, Exception innerException)
            : base(getErrorMessage(schema, definition, value), innerException)
        {
            this.Schema = schema;
            this.ColumnDefinition = definition;
            this.ColumnValue = value;
        }

        private static string getErrorMessage(ISchema schema, IColumnDefinition definition, string value)
        {
            int position = schema.GetOrdinal(definition.ColumnName);
            string message = String.Format(null, SharedResources.InvalidColumnConversion, value, definition.ColumnType.FullName, definition.ColumnName, position);
            return message;
        }

        /// <summary>
        /// Gets the schema that was being used to parse when the error occurred.
        /// </summary>
        public ISchema Schema { get; private set; }

        /// <summary>
        /// Gets the column that was being used to parse when the error occurred.
        /// </summary>
        public IColumnDefinition ColumnDefinition { get; private set; }

        /// <summary>
        /// Gets the value that was being parsed when the error occurred.
        /// </summary>
        public string ColumnValue { get; private set; }
    }

    /// <summary>
    /// Represents an error that was thrown while parsing a record.
    /// </summary>
    public sealed class RecordProcessingException : FlatFileException
    {
        internal RecordProcessingException(int recordNumber, string message)
            : base(String.Format(null, message, recordNumber))
        {
            this.RecordNumber = recordNumber;
        }

        internal RecordProcessingException(int recordNumber, string message, Exception innerException)
            : base(String.Format(null, message, recordNumber), innerException)
        {
            this.RecordNumber = recordNumber;
        }

        /// <summary>
        /// Gets the zero-based index of the record that was being parsed when the error occurred.
        /// </summary>
        public int RecordNumber { get; private set; }
    }
}
