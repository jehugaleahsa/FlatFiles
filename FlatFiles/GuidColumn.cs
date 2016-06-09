using System;
using System.Globalization;
using System.Text;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of Guid values.
    /// </summary>
    public class GuidColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of a GuidColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public GuidColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(Guid); }
        }

        /// <summary>
        /// Gets or sets the format string to use when parsing the Guid.
        /// </summary>
        public string InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Parses the given value and returns a Guid instance.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Guid.</returns>
        public override object Parse(string value)
        {
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            if (InputFormat == null)
            {
                return Guid.Parse(value);
            }
            else
            {
                return Guid.ParseExact(value, InputFormat);
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

            Guid actual = (Guid)value;
            if (OutputFormat == null)
            {
                return actual.ToString();
            }
            else
            {
                return actual.ToString(OutputFormat);
            }
        }
    }
}
