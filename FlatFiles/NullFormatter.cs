using System;

namespace FlatFiles
{
    /// <summary>
    /// Provides factory methods for generating instances of <see cref="INullFormatter"/>.
    /// </summary>
    public sealed class NullFormatter : INullFormatter
    {
        /// <summary>
        /// Creates a new <see cref="INullFormatter"/> that treats solid whitespace as null.
        /// </summary>
        public static readonly INullFormatter Default = new NullFormatter(
            (ctx, v) => string.IsNullOrWhiteSpace(v), 
            ctx => string.Empty
        );

        private readonly Func<IColumnContext?, string?, bool> isNullValue;
        private readonly Func<IColumnContext?, string?> formatNull;

        private NullFormatter(Func<IColumnContext?, string?, bool> isNullValue, Func<IColumnContext?, string?> formatNull)
        {
            this.isNullValue = isNullValue;
            this.formatNull = formatNull;
        }

        /// <summary>
        /// Creates a new <see cref="INullFormatter"/> that uses the given value to represent null.
        /// </summary>
        /// <param name="value">The constant used to represent null in the flat file.</param>
        /// <returns>An object for configuring how nulls are handled.</returns>
        public static NullFormatter ForValue(string? value)
        {
            return new NullFormatter((ctx, v) => v == null || v == value, ctx => value);
        }

        /// <inheritdoc/>
        public bool IsNullValue(IColumnContext? context, string? value) => isNullValue(context, value);

        /// <inheritdoc/>
        public string? FormatNull(IColumnContext? context) => formatNull(context);
    }
}
