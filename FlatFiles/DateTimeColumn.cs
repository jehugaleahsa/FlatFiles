using System;
using System.Globalization;
using System.Text;

namespace FlatFiles
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
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(DateTime); }
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
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed DateTime instance.</returns>
        public override object Parse(string value)
        {
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            if (InputFormat == null)
            {
                return DateTime.Parse(value, provider);
            }
            else
            {
                return DateTime.ParseExact(value, InputFormat, provider);
            }
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(object value)
        {
            if (value == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            DateTime actual = (DateTime)value;
            if (OutputFormat == null)
            {
                return actual.ToString(FormatProvider ?? CultureInfo.CurrentCulture);
            }
            else
            {
                return actual.ToString(OutputFormat, FormatProvider ?? CultureInfo.CurrentCulture);
            }
        }
    }
}
