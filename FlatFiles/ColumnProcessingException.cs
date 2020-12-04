using System;
using FlatFiles.Properties;

namespace FlatFiles
{
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

        internal ColumnProcessingException(IColumnContext context, object value, Exception innerException = null)
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
}
