using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of DateTime values.
    /// </summary>
    public sealed class DateTimeOffsetColumn : ColumnDefinition<DateTimeOffset>
    {
        /// <summary>
        /// Initializes a new instance of a DateTimeOffsetColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public DateTimeOffsetColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets or sets the format string to use when parsing the date and time.
        /// </summary>
        public string InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the format provider to use when parsing the date and time.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <inheritdoc />
        protected override DateTimeOffset OnParse(IColumnContext context, string value)
        {
            var provider = FormatProvider ?? CultureInfo.CurrentCulture;
            if (InputFormat == null)
            {
                return DateTimeOffset.Parse(value, provider);
            }
            return DateTimeOffset.ParseExact(value, InputFormat, provider);
        }

        /// <inheritdoc />
        protected override string OnFormat(IColumnContext context, DateTimeOffset value)
        {
            var provider = FormatProvider ?? CultureInfo.CurrentCulture;
            if (OutputFormat == null)
            {
                return value.ToString(provider);
            }
            return value.ToString(OutputFormat, provider);
        }
    }
}
