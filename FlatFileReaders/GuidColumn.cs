using System;
using System.Globalization;

namespace FlatFileReaders
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
        public string GuidFormat { get; set; }

        /// <summary>
        /// Parses the given value and returns a Guid instance.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Guid.</returns>
        public override object Parse(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            if (GuidFormat == null)
            {
                return Guid.Parse(value);
            }
            else
            {
                return Guid.ParseExact(value, GuidFormat);
            }
        }
    }
}
