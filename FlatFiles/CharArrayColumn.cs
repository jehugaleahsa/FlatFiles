using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of a char[] values.
    /// </summary>
    public sealed class CharArrayColumn : ColumnDefinition<char[]>
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
        /// Gets or sets whether the value should be trimmed.
        /// </summary>
        public bool Trim { get; set; } = true;

        /// <summary>
        /// Parses the given value as a char array.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed char array.</returns>
        protected override char[] OnParse(IColumnContext? context, string value)
        {
            return value.ToCharArray();
        }

        /// <summary>
        /// Gets whether the value should be trimmed prior to parsing.
        /// </summary>
        protected override bool IsTrimmed { get => Trim; }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, char[] value)
        {
            return new String(value);
        }
    }
}
