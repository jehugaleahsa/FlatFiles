using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Specifies which value represents nulls within a file.
    /// </summary>
    public interface INullFormatter
    {
        /// <summary>
        /// Gets whether the given string should be interpreted as null.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <param name="value">The value to inspect.</param>
        /// <returns>True if the value represents null; otherwise, false.</returns>
        bool IsNullValue(IColumnContext context, string value);

        /// <summary>
        /// Gets the value used to represent null when writing to a flat file.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>The string used to represent null in the flat file.</returns>
        string FormatNull(IColumnContext context);
    }

    /// <summary>
    /// Provides factory methods for generating instances of <see cref="INullFormatter"/>.
    /// </summary>
    public sealed class NullFormatter : INullFormatter
    {
        private readonly Func<IColumnContext, string, bool> isNullValue;
        private readonly Func<IColumnContext, string> formatNull;

        private NullFormatter(Func<IColumnContext, string, bool> isNullValue, Func<IColumnContext, string> formatNull)
        {
            this.isNullValue = isNullValue;
            this.formatNull = formatNull;
        }

        /// <summary>
        /// Creates a new <see cref="INullFormatter"/> that uses the given value to represent null.
        /// </summary>
        /// <param name="value">The constant used to represent null in the flat file.</param>
        /// <returns>An object for configuring how nulls are handled.</returns>
        public static NullFormatter ForValue(string value) => new NullFormatter((ctx, v) => v == null || v == value, ctx => value);

        /// <summary>
        /// Creates a new <see cref="INullFormatter"/> that treats solid whitespace as null.
        /// </summary>
        public static NullFormatter Default => new NullFormatter((ctx, v) => String.IsNullOrWhiteSpace(v), ctx => String.Empty);

        bool INullFormatter.IsNullValue(IColumnContext context, string value) => isNullValue(context, value);

        string INullFormatter.FormatNull(IColumnContext context) => formatNull(context);
    }
}
