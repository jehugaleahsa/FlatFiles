using System;
using System.Text;

namespace FlatFiles
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
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            value = TrimValue(value);
            return value.ToCharArray();
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(object value)
        {
            if (value == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            char[] actual = (char[])value;
            return new String(actual);
        }
    }
}
