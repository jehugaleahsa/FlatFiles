using System;
using System.Globalization;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents a column containing 32-bit integers.
    /// </summary>
    public class Int32Column : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of an Int32Column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public Int32Column(string columnName)
            : base(columnName)
        {
            NumberStyles = NumberStyles.Integer;
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(Int32); }
        }

        /// <summary>
        /// Gets or sets the format provider to use when parsing.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles { get; set; }

        /// <summary>
        /// Parses the given value, returning an Int32.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Int32.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            return Int32.Parse(value.Trim(), NumberStyles, provider);
        }
    }
}
