using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing unsigned 16-bit integers.
    /// </summary>
    public sealed class UInt16Column : ColumnDefinition<ushort>
    {
        /// <summary>
        /// Initializes a new instance of an UInt16Column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public UInt16Column(string columnName)
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
        /// Parses the given value, returning an UInt16.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed UInt16.</returns>
        protected override ushort OnParse(IColumnContext context, string value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            return UInt16.Parse(value, NumberStyles, provider);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext context, ushort value)
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
