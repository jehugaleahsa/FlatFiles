namespace FlatFiles
{
    /// <summary>
    /// Generates a default value whenever a null is encountered on a non-nullable column.
    /// </summary>
    public interface IDefaultValue
    {
        /// <summary>
        /// Gets the default value to use when a null is encountered on a non-nullable column.
        /// </summary>
        /// <param name="context">The current column context.</param>
        /// <returns>The default value.</returns>
        object? GetDefaultValue(IColumnContext? context);
    }
}
