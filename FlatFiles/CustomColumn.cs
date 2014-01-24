using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Allows a custom converter to be used when parsing a column.
    /// </summary>
    public sealed class CustomColumn : ColumnDefinition
    {
        private readonly Type columnType;
        private readonly Func<string, object> parser;
        private readonly Func<object, string> formatter;

        /// <summary>
        /// Initializes a new instance of a CustomConverter.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="columnType">The type of the column.</param>
        /// <param name="parser">The converter to use to parse the string representation of the column value.</param>
        /// <param name="formatter">The converter to use to format the column value.</param>
        public CustomColumn(string columnName, 
            Type columnType, 
            Func<string, object> parser,
            Func<object, string> formatter)
            : base(columnName)
        {
            if (columnType == null)
            {
                throw new ArgumentNullException("columnType");
            }
            this.columnType = columnType;
            this.parser = parser ?? (s => Convert(columnType, String.IsNullOrWhiteSpace(s) ? null : s));
            this.formatter = formatter ?? (t => (string)Convert(typeof(String), t));
        }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return columnType; }
        }

        /// <summary>
        /// Converts the given string into the desired type.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public override object Parse(string value)
        {
            return parser(value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(object value)
        {
            return formatter(value);
        }

        /// <summary>
        /// Converts the given value to the request type handling nullables.
        /// </summary>
        /// <param name="desiredType">The desired type to convert to.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        internal static object Convert(Type desiredType, object value)
        {
            Type underlyingType = Nullable.GetUnderlyingType(desiredType);
            if (desiredType.IsValueType && underlyingType == null && value == null)
            {
                throw new InvalidCastException(Resources.RequiredColumnNull);
            }
            return System.Convert.ChangeType(value, underlyingType ?? desiredType);
        }
    }
}
