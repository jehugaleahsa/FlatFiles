using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property/field to a column.
    /// </summary>
    internal interface IMemberMapping
    {
        /// <summary>
        /// Gets the property/field that is mapped to.
        /// </summary>
        IMemberAccessor Member { get; }

        /// <summary>
        /// Gets a custom reader to use when populating entities.
        /// </summary>
        Action<IColumnContext, object, object> Reader { get; }

        /// <summary>
        /// Gets a custom writer to use when writing entities.
        /// </summary>
        Action<IColumnContext, object, object[]> Writer { get; }

        /// <summary>
        /// Gets the column that is mapped to. 
        /// </summary>
        IColumnDefinition ColumnDefinition { get; }

        /// <summary>
        /// Gets the index of the column as it appears in the file.
        /// </summary>
        int PhysicalIndex { get; }

        /// <summary>
        /// Gets the index of the column, excluding ignored columns.
        /// </summary>
        int LogicalIndex { get; }
    }
}
