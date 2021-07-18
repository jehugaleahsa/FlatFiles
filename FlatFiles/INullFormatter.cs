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
        bool IsNullValue(IColumnContext? context, string? value);

        /// <summary>
        /// Gets the value used to represent null when writing to a flat file.
        /// </summary>
        /// <param name="context">The column context.</param>
        /// <returns>The string used to represent null in the flat file.</returns>
        string? FormatNull(IColumnContext? context);
    }
}
