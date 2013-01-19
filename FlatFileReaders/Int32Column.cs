using System;

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
        }

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
            return Int32.Parse(value.Trim());
        }
    }
}
