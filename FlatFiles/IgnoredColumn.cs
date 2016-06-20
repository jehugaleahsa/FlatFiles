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
        public override Type ColumnType
        {
            get { return typeof(String); }
        }

        /// <summary>
        /// Ignores the values that was parsed from the document.
        /// </summary>
        /// <param name="value">The value that was parsed from the document.</param>
        /// <returns>A null.</returns>
        public override object Parse(string value)
        {
            return null;
        }

        /// <summary>
        /// Returns null so nothing is written to the document.
        /// </summary>
        /// <param name="value">The value that needs written to the document.</param>
        /// <returns>A null.</returns>
        public override string Format(object value)
        {
            return NullHandler.GetNullRepresentation();
        }
    }
}
