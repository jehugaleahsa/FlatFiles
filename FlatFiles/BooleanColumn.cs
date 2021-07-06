using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing boolean values.
    /// </summary>
    public sealed class BooleanColumn : ColumnDefinition<bool>
    {
        /// <summary>
        /// Initializes a new instance of a BooleanColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public BooleanColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets or sets the value representing true.
        /// </summary>
        public string TrueString { get; set; } = Boolean.TrueString;

        /// <summary>
        /// Gets or sets the value representing false.
        /// </summary>
        public string FalseString { get; set; } = Boolean.FalseString;

        /// <summary>
        /// Parses the given value into its equivilent boolean value.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>True if the value equals the TrueString; otherwise, false.</returns>
        protected override bool OnParse(IColumnContext context, string value)
        {
            if (String.Equals(value, TrueString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            if (String.Equals(value, FalseString, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext context, bool value)
        {
            return value ? TrueString : FalseString;
        }
    }
}
