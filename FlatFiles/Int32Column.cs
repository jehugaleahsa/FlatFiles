using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing 32-bit integers.
    /// </summary>
    public sealed class Int32Column : ColumnDefinition<int>
    {
        /// <summary>
        /// Initializes a new instance of an Int32Column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public Int32Column(string columnName)
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
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Integer;

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string? OutputFormat { get; set; }

        /// <summary>
        /// Parses the given value, returning an Int32.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Int32.</returns>
        protected override int OnParse(IColumnContext? context, string value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            return Int32.Parse(value, NumberStyles, provider);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, int value)
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
