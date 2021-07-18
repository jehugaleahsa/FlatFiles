using System.Text;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of a byte[] values.
    /// </summary>
    public sealed class ByteArrayColumn : ColumnDefinition<byte[]>
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
        /// Gets or sets the encoding to use when parsing the value.
        /// </summary>
        public Encoding? Encoding { get; set; }

        /// <summary>
        /// Parses the given value as a byte array.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed byte array.</returns>
        protected override byte[] OnParse(IColumnContext? context, string value)
        {
            var actualEncoding = Encoding ?? Encoding.UTF8;
            return actualEncoding.GetBytes(value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, byte[] value)
        {
            var actualEncoding = Encoding ?? Encoding.UTF8;
            return actualEncoding.GetString(value);
        }
    }
}
