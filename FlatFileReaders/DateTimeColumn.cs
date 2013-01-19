using System;
using System.Globalization;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents a column of DateTime values.
    /// </summary>
    public class DateTimeColumn : ColumnDefinition
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
        public string DateTimeFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format provider to use when parsing the date and time.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Parses the given value and returns a DateTime instance.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed DateTime instance.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            if (DateTimeFormat == null)
            {
                return DateTime.Parse(value, provider);
            }
            else
            {
                return DateTime.ParseExact(value, DateTimeFormat, provider);
            }
        }
    }
}
