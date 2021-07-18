using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of character values.
    /// </summary>
    public class CharColumn : ColumnDefinition<char>
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
        /// Gets or sets whether the parser should allow for trailing characters.
        /// </summary>
        public bool AllowTrailing { get; set; }

        /// <summary>
        /// Parses the given value as a char.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed char.</returns>
        protected override char OnParse(IColumnContext? context, string value)
        {
            if (AllowTrailing || value.Length == 1)
            {
                return value[0];
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, char value)
        {
            return value.ToString();
        }
    }
}
