using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a column.
    /// </summary>
    internal interface IPropertyMapping
    {
        /// <summary>
        /// Gets the property that is mapped to.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Gets the column that is mapped to. 
        /// </summary>
        ColumnDefinition ColumnDefinition { get; }
    }

    internal interface IComplexPropertyMapping : IPropertyMapping
    {
        IRecordMapper RecordMapper { get; }
    }
}
