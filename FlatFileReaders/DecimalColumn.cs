using System;
using System.Globalization;

namespace FlatFileReaders
{
    /// <summary>
    /// Represents a column containing decimals.
    /// </summary>
    public class DecimalColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of an DecimalColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public DecimalColumn(string columnName)
            : base(columnName)
        {
            NumberStyles = NumberStyles.Number;
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(Decimal); }
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
        /// Parses the given value, returning a Decimal.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Decimal.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            return Decimal.Parse(value.Trim(), NumberStyles, provider);
        }
    }
}
