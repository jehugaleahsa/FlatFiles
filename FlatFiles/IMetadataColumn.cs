using System;

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
        /// <param name="context">Metadata about the record currently being processed.</param>
        /// <returns>The relevant metadata.</returns>
        object GetValue(IRecordContext context);
    }
}
