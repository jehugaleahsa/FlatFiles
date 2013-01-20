using System;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents a column containing strings.
    /// </summary>
    public class StringColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of a StringColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public StringColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(String); }
        }

        /// <summary>
        /// Returns the given value trimmed.
        /// </summary>
        /// <param name="value">The value to trim.</param>
        /// <returns>The value trimmed.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return value.Trim();
        }
    }
}
