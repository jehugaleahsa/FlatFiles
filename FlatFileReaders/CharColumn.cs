using System;

namespace FlatFileReaders
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
        public bool AllowTrailing
        {
            get;
            set;
        }

        /// <summary>
        /// Parses the given value as a char.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed char.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            value = value.Trim();
            if (AllowTrailing || value.Length == 1)
            {
                return value[0];
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
