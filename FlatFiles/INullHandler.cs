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
        bool IsNullRepresentation(IColumnContext context, string value);

        /// <summary>
        /// Gets the value used to represent null when writing to a flat file.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>The string used to represent null in the flat file.</returns>
        string GetNullRepresentation(IColumnContext context);

        /// <summary>
        /// Gets the value to use as the substitute when null is encountered on an non-nullable type.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>The substitute value.</returns>
        object GetNullSubstitute(IColumnContext context);
    }

    /// <summary>
    /// Provides factory methods for generating instances of <see cref="INullHandler"/>.
    /// </summary>
    public static class NullHandler
    {
        /// <summary>
        /// Creates a new <see cref="INullHandler"/> that uses the given value to represent null.
        /// </summary>
        /// <param name="value">The constant used to represent null in the flat file.</param>
        /// <returns>An object for configuring how nulls are handled.</returns>
        public static NullHandlerBuilder ForValue(string value)
        {
            return new NullHandlerBuilder((ctx, v) => v == null || v == value, ctx => value);
        }

        /// <summary>
        /// Creates a new <see cref="INullHandler"/> that treats solid whitespace as null.
        /// </summary>
        public static NullHandlerBuilder Default => new NullHandlerBuilder(IsNullRepresentation, GetNullRepresentation);

        /// <summary>
        /// Gets whether the given string should be interpreted as null.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <param name="value">The value to inspect.</param>
        /// <returns>True if the value represents null; otherwise, false.</returns>
        private static bool IsNullRepresentation(IColumnContext context, string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Gets the value used to represent null when writing to a flat file.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>The string used to represent null in the flat file.</returns>
        private static string GetNullRepresentation(IColumnContext context)
        {
            return null;
        }
    }

    /// <summary>
    /// Assists in building an <see cref="INullHandler"/>.
    /// </summary>
    public sealed class NullHandlerBuilder : INullHandler
    {
        private readonly Func<IColumnContext, string, bool> isNullRepresentation;
        private readonly Func<IColumnContext, string> getNullRepresentation;
        private Func<IColumnContext, object> getNullSubstitute;

        internal NullHandlerBuilder(
            Func<IColumnContext, string, bool> isNullRepresentation, 
            Func<IColumnContext, string> getNullRepresentation)
        {
            this.isNullRepresentation = isNullRepresentation;
            this.getNullRepresentation = getNullRepresentation;
            getNullSubstitute = GetNullSubstitute;
        }

        /// <summary>
        /// Configures the <see cref="INullHandler"/> to throw an exception when nulls are encountered on non-nullable columns.
        /// </summary>
        /// <returns>The generated null handler.</returns>
        public INullHandler ThrowWhenNull()
        {
            getNullSubstitute = GetNullSubstitute;
            return this;
        }

        /// <summary>
        /// Configures the <see cref="INullHandler"/> to return a substitute value when nulls are encountered on non-nullable columns.
        /// </summary>
        /// <returns>The generated null handler.</returns>
        public INullHandler SubstituteForNull(object value)
        {
            getNullSubstitute = ctx => value;
            return this;
        }

        /// <summary>
        /// Configures the <see cref="INullHandler"/> to return a substitute value when nulls are encountered on non-nullable columns.
        /// </summary>
        /// <returns>The generated null handler.</returns>
        public INullHandler SubstituteForNull(Func<IColumnContext, object> factory)
        {
            getNullSubstitute = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }

        string INullHandler.GetNullRepresentation(IColumnContext context) => getNullRepresentation(context);

        object INullHandler.GetNullSubstitute(IColumnContext context) => getNullSubstitute(context);

        bool INullHandler.IsNullRepresentation(IColumnContext context, string value) => isNullRepresentation(context, value);

        /// <summary>
        /// By default, an exception is thrown when a null is encountered for a non-nullable column.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>Nothing.</returns>
        private static object GetNullSubstitute(IColumnContext context)
        {
            throw new InvalidCastException(Resources.AssignNullToNonNullable);
        }
    }
}
