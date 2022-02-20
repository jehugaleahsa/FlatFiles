namespace FlatFiles
{
    /// <summary>
    ///  Specifies how FlatFiles should quote delimited file fields.
    /// </summary>
    public enum QuoteBehavior
    {
        /// <summary>
        /// FlatFiles will only put quotes around values that need to be quoted.
        /// </summary>
        Default = 0,
        /// <summary>
        /// FlatFiles will put quotes around all values.
        /// </summary>
        AlwaysQuote = 1,
        /// <summary>
        /// FlatFiles will never put quotes around values.
        /// </summary>
        /// <remarks>This can result in the generation of invalid files.</remarks>
        Never = 2
    }
}
