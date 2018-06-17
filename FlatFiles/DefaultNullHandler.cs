namespace FlatFiles
{
    /// <summary>
    /// Interprets solid whitespace as representing null.
    /// </summary>
    public class DefaultNullHandler : INullHandler
    {
        internal static readonly INullHandler Instance = new DefaultNullHandler();

        /// <summary>
        /// Gets whether the given string should be interpreted as null.
        /// </summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns>True if the value represents null; otherwise, false.</returns>
        public bool IsNullRepresentation(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Gets the value used to represent null when writing to a flat file.
        /// </summary>
        /// <returns>The string used to represent null in the flat file.</returns>
        public string GetNullRepresentation()
        {
            return string.Empty;
        }
    }
}
