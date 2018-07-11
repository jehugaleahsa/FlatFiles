using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing 16-bit integers.
    /// </summary>
    public sealed class Int16Column : ColumnDefinition<short>
    {
        /// <summary>
        /// Initializes a new instance of an Int16Column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public Int16Column(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets or sets the format provider to use when parsing.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Integer;

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Parses the given value, returning an Int16.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Int16.</returns>
        protected override short OnParse(IColumnContext context, string value)
        {
            var provider = FormatProvider ?? CultureInfo.CurrentCulture;
            return Int16.Parse(value, NumberStyles, provider);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext context, short value)
        {
            if (OutputFormat == null)
            {
                return value.ToString(FormatProvider ?? CultureInfo.CurrentCulture);
            }
            return value.ToString(OutputFormat, FormatProvider ?? CultureInfo.CurrentCulture);
        }
    }
}
