using System;

namespace FlatFileReaders
{
    /// <summary>
    /// Allows a custom converter to be used when parsing a column.
    /// </summary>
    public sealed class CustomColumn : ColumnDefinition
    {
        private readonly Type columnType;
        private readonly Func<string, object> converter;

        /// <summary>
        /// Initializes a new instance of a CustomConverter.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="columnType">The type of the column.</param>
        /// <param name="converter">The converter to use to parse the string representation of the column value.</param>
        public CustomColumn(string columnName, Type columnType, Func<string, object> converter)
            : base(columnName)
        {
            if (columnType == null)
            {
                throw new ArgumentNullException("columnType");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            this.columnType = columnType;
            this.converter = converter;
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
            return converter(value);
        }
    }
}
