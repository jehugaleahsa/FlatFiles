using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing strings.
    /// </summary>
    public sealed class StringColumn : ColumnDefinition<string>
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
        public override Type ColumnType => typeof(string);

        /// <summary>
        /// Gets or sets whether the value should be trimmed.
        /// </summary>
        public bool Trim { get; set; } = true;

        /// <summary>
        /// Returns the given value trimmed.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to trim.</param>
        /// <returns>The value trimmed.</returns>
        protected override string OnParse(IColumnContext? context, string value)
        {
            return value;
        }

        /// <summary>
        /// Gets whether the value should be trimmed prior to parsing.
        /// </summary>
        protected override bool IsTrimmed => Trim;

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, string value)
        {
            return value;
        }
    }
}
