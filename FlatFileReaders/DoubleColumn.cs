using System;
using System.Globalization;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents a column containing doubles.
    /// </summary>
    public class DoubleColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of an DoubleColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public DoubleColumn(string columnName)
            : base(columnName)
        {
            NumberStyles = NumberStyles.Float;
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(Double); }
        }

        /// <summary>
        /// Gets or sets the format provider to use when parsing.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles
        {
            get;
            set;
        }

        /// <summary>
        /// Parses the given value, returning a Double.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Double.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            return Double.Parse(value.Trim(), NumberStyles, provider);
        }
    }
}
