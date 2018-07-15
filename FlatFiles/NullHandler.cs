using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Specifies which value represents nulls within a file.
    /// </summary>
    public interface INullHandler
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

        /// <summary>
        /// Gets the value to use as the substitute when null is encountered on an non-nullable column.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>The substitute value.</returns>
        /// <remarks>If there is no valid substitute, this method should throw an exception.</remarks>
        object GetDefaultValue(IColumnContext context);
    }

    /// <summary>
    /// Assists in building an <see cref="INullHandler"/> to describe how unexpected nulls are handled.
    /// </summary>
    public interface IUnexpectedNullHandler
    {
        /// <summary>
        /// Configures the <see cref="INullHandler"/> to throw an exception when nulls are encountered on non-nullable columns.
        /// This is the default action.
        /// </summary>
        /// <returns>The generated null handler.</returns>
        INullHandler Throw();

        /// <summary>
        /// Configures the <see cref="INullHandler"/> to return a substitute value when nulls are encountered on non-nullable columns.
        /// </summary>
        /// <returns>The generated null handler.</returns>
        INullHandler UseDefault(object value);

        /// <summary>
        /// Configures the <see cref="INullHandler"/> to return a substitute value when nulls are encountered on non-nullable columns.
        /// </summary>
        /// <returns>The generated null handler.</returns>
        INullHandler UseDefault(Func<IColumnContext, object> factory);
    }

    /// <summary>
    /// Provides factory methods for generating instances of <see cref="INullHandler"/>.
    /// </summary>
    public sealed class NullHandler : INullHandler, IUnexpectedNullHandler
    {
        private readonly Func<IColumnContext, string, bool> isNullValue;
        private readonly Func<IColumnContext, string> formatNull;
        private Func<IColumnContext, object> getDefaultValue;

        private NullHandler(Func<IColumnContext, string, bool> isNullValue, Func<IColumnContext, string> formatNull)
        {
            this.isNullValue = isNullValue;
            this.formatNull = formatNull;
            ForUnexpectedNulls.Throw();
        }

        /// <summary>
        /// Creates a new <see cref="INullHandler"/> that uses the given value to represent null.
        /// </summary>
        /// <param name="value">The constant used to represent null in the flat file.</param>
        /// <returns>An object for configuring how nulls are handled.</returns>
        public static NullHandler ForValue(string value) => new NullHandler((ctx, v) => v == null || v == value, ctx => value);

        /// <summary>
        /// Creates a new <see cref="INullHandler"/> that treats solid whitespace as null.
        /// </summary>
        public static NullHandler Default => new NullHandler((ctx, v) => String.IsNullOrWhiteSpace(v), ctx => null);

        /// <summary>
        /// Allows configuring how nulls should be handled when encountered on a non-nullable column.
        /// </summary>
        public IUnexpectedNullHandler ForUnexpectedNulls => this;

        bool INullHandler.IsNullValue(IColumnContext context, string value) => isNullValue(context, value);

        string INullHandler.FormatNull(IColumnContext context) => formatNull(context);

        object INullHandler.GetDefaultValue(IColumnContext context) => getDefaultValue(context);

        INullHandler IUnexpectedNullHandler.Throw()
        {
            getDefaultValue = ctx => throw new InvalidCastException(Resources.AssignNullToNonNullable);
            return this;
        }

        INullHandler IUnexpectedNullHandler.UseDefault(object value)
        {
            getDefaultValue = ctx => value;
            return this;
        }

        INullHandler IUnexpectedNullHandler.UseDefault(Func<IColumnContext, object> factory)
        {
            getDefaultValue = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }
    }
}
