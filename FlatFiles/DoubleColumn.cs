using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing doubles.
    /// </summary>
    public sealed class DoubleColumn : ColumnDefinition<double>
    {
        /// <summary>
        /// Initializes a new instance of an DoubleColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public DoubleColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets or sets the format provider to use when parsing.
        /// </summary>
        public IFormatProvider? FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Float;

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string? OutputFormat { get; set; }

        /// <summary>
        /// Parses the given value, returning a Double.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Double.</returns>
        protected override double OnParse(IColumnContext? context, string value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            return Double.Parse(value, NumberStyles, provider);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, double value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            if (OutputFormat == null)
            {
                return value.ToString(provider);
            }
            return value.ToString(OutputFormat, provider);
        }
    }
}
