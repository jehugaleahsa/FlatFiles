﻿using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column of signed byte values.
    /// </summary>
    public sealed class SByteColumn : ColumnDefinition<sbyte>
    {
        /// <summary>
        /// Initializes a new instance of a SByteColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public SByteColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets or sets the format provider to use to parse the value.
        /// </summary>
        public IFormatProvider? FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the number styles to use when parsing the value.
        /// </summary>
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Integer;

        /// <summary>
        /// Gets or sets the formatting to use when converting the value to a string.
        /// </summary>
        public string? OutputFormat { get; set; }

        /// <summary>
        /// Parses the given value into a signed byte.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed signed byte value.</returns>
        protected override sbyte OnParse(IColumnContext? context, string value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            return sbyte.Parse(value, NumberStyles, provider);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, sbyte value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            if (OutputFormat == null)
            {
                return value.ToString(provider);
            }
            return value.ToString(OutputFormat, provider);
        }
    }
}
