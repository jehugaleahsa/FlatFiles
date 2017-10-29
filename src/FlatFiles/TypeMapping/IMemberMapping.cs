using System;
using System.Reflection;

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
        /// Gets the column that is mapped to. 
        /// </summary>
        IColumnDefinition ColumnDefinition { get; }
    }
}
