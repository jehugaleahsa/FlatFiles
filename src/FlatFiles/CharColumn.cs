using System;
using System.Globalization;
using System.Text;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of character values.
    /// </summary>
    public class CharColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of a CharColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public CharColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(Char); }
        }

        /// <summary>
        /// Gets or sets whether the parser should allow for trailing characters.
        /// </summary>
        public bool AllowTrailing { get; set; }

        /// <summary>
        /// Parses the given value as a char.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed char.</returns>
        public override object Parse(string value)
        {
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            value = TrimValue(value);
            if (AllowTrailing || value.Length == 1)
            {
                return value[0];
            }
            else
            {
                throw new InvalidCastException();
            }
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
            char actual = (char)value;
            return actual.ToString();
        }
    }
}
