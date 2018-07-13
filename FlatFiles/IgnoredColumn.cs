using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column that should be ignored when reading a document and used as a placeholder
    /// when writing a document.
    /// </summary>
    public class IgnoredColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new IgnoredColumn.
        /// </summary>
        public IgnoredColumn() 
            : base(String.Empty, true)
        {
        }

        /// <summary>
        /// Initializes a new IgnoredColumn with a header name.
        /// </summary>
        /// <param name="columnName"></param>
        public IgnoredColumn(string columnName)
            : base(columnName, true)
        {
        }

        /// <summary>
        /// Gets the type of data in the column.
        /// </summary>
        public override Type ColumnType => typeof(string);

        /// <summary>
        /// Ignores the values that was parsed from the document.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value that was parsed from the document.</param>
        /// <returns>A null.</returns>
        public override object Parse(IColumnContext context, string value)
        {
            return null;
        }

        /// <summary>
        /// Returns null so nothing is written to the document.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value that needs written to the document.</param>
        /// <returns>A null.</returns>
        public override string Format(IColumnContext context, object value)
        {
            return NullHandler.GetNullValue(context);
        }
    }
}
