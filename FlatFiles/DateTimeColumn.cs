using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of DateTime values.
    /// </summary>
    public sealed class DateTimeColumn : ColumnDefinition<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of a DateTimeColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public DateTimeColumn(string columnName)
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

        /// <summary>
        /// Parses the given value and returns a DateTime instance.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed DateTime instance.</returns>
        protected override DateTime OnParse(IColumnContext context, string value)
        {
            var provider = FormatProvider ?? CultureInfo.CurrentCulture;
            if (InputFormat == null)
            {
                return DateTime.Parse(value, provider);
            }
            return DateTime.ParseExact(value, InputFormat, provider);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext context, DateTime value)
        {
            if (OutputFormat == null)
            {
                return value.ToString(FormatProvider ?? CultureInfo.CurrentCulture);
            }
            return value.ToString(OutputFormat, FormatProvider ?? CultureInfo.CurrentCulture);
        }
    }
}
