using System;
using System.Globalization;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a column containing unsigned 32-bit integers.
    /// </summary>
    public sealed class UInt32Column : ColumnDefinition<uint>
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of an UInt32Column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public UInt32Column(string columnName)
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

        /// <inheritdoc />
        /// <summary>
        /// Parses the given value, returning an UInt32.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed UInt32.</returns>
        protected override uint OnParse(IColumnContext context, string value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            return UInt32.Parse(value, NumberStyles, provider);
        }

        /// <inheritdoc />
        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext context, uint value)
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
