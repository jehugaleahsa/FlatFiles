using System;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public interface ISchema
    {
        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        ColumnCollection ColumnDefinitions { get; }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name.</returns>
        /// <exception cref="System.IndexOutOfRangeException">There is not a column with the given name.</exception>
        int GetOrdinal(string name);
    }
}
