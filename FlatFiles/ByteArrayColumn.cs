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
        /// <param name="encoding">The encoding of the outer document.</param>
        /// <returns>The parsed byte array.</returns>
        public override object Parse(string value, Encoding encoding)
        {
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            value = TrimValue(value);
            Encoding actualEncoding = Encoding ?? encoding;
            return actualEncoding.GetBytes(value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <param name="encoding">The encoding of the outer document.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(object value, Encoding encoding)
        {
            if (value == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            byte[] actual = (byte[])value;
            Encoding actualEncoding = Encoding ?? encoding;
            return actualEncoding.GetString(actual);
        }
    }
}
