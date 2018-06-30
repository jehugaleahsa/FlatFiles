using System;
using System.Text;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of a byte[] values.
    /// </summary>
    public class ByteArrayColumn : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance instance of a ByteArrayColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public ByteArrayColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(byte[]);

        /// <summary>
        /// Gets or sets the encoding to use when parsing the value.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Parses the given value as a byte array.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed byte array.</returns>
        public override object Parse(IColumnContext context, string value)
        {
            if (Preprocessor != null)
            {
                value = Preprocessor(value);
            }
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            value = TrimValue(value);
            Encoding actualEncoding = Encoding ?? new UTF8Encoding(false);
            return actualEncoding.GetBytes(value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(IColumnContext context, object value)
        {
            if (value == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            byte[] actual = (byte[])value;
            Encoding actualEncoding = Encoding ?? new UTF8Encoding(false);
            return actualEncoding.GetString(actual);
        }
    }
}
