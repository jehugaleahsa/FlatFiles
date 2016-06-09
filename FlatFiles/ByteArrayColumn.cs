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
        public override Type ColumnType
        {
            get { return typeof(byte[]); }
        }

        /// <summary>
        /// Gets or sets the encoding to use when parsing the value.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Parses the given value as a byte array.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed byte array.</returns>
        public override object Parse(string value)
        {
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
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(object value)
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
