using System;
using System.Text;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents a column of a char[] values.
    /// </summary>
    public class CharArrayColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance instance of a CharArrayColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public CharArrayColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(char[]); }
        }

        /// <summary>
        /// Parses the given value as a char array.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed char array.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            value = value.Trim();
            return value.ToCharArray();
        }
    }
}
