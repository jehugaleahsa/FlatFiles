using System;
using System.Globalization;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a column containing unsigned 32-bit integers.
    /// </summary>
    public class UInt32Column : ColumnDefinition
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

        /// <inheritdoc />
        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(uint);

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
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed UInt32.</returns>
        public override object Parse(string value)
        {
            if (Preprocessor != null)
            {
                value = Preprocessor(value);
            }
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            value = TrimValue(value);
            return uint.Parse(value, NumberStyles, provider);
        }

        /// <inheritdoc />
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

            uint actual = (uint)value;
            if (OutputFormat == null)
            {
                return actual.ToString(FormatProvider ?? CultureInfo.CurrentCulture);
            }

            return actual.ToString(OutputFormat, FormatProvider ?? CultureInfo.CurrentCulture);
        }
    }
}
