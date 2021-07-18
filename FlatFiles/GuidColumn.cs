using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of Guid values.
    /// </summary>
    public sealed class GuidColumn : ColumnDefinition<Guid>
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
        /// Gets or sets the format string to use when parsing the Guid.
        /// </summary>
        public string? InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string? OutputFormat { get; set; }

        /// <summary>
        /// Parses the given value and returns a Guid instance.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed Guid.</returns>
        protected override Guid OnParse(IColumnContext? context, string value)
        {
            if (InputFormat == null)
            {
                return Guid.Parse(value);
            }
            return Guid.ParseExact(value, InputFormat);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, Guid value)
        {
            if (OutputFormat == null)
            {
                return value.ToString();
            }
            return value.ToString(OutputFormat);
        }
    }
}
