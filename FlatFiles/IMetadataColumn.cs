namespace FlatFiles
{
    /// <summary>
    /// Represents a column whose data is not sourced by the input file.
    /// </summary>
    public interface IMetadataColumn : IColumnDefinition
    {
        /// <summary>
        /// Extracts the relevant metadata.
        /// </summary>
        /// <param name="metadata">The current metadata.</param>
        /// <returns>The relevant metadata.</returns>
        object GetValue(IProcessMetadata metadata);
    }
}
