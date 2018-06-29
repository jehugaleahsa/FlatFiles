using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a WriteOnly column.
    /// </summary>
    public interface IWriteOnlyPropertyMapping
    {
        /// <summary>
        /// Gets the name given to the read-only mapping.
        /// </summary>
        string Name { get; }
    }

    internal sealed class WriteOnlyPropertyMapping : IWriteOnlyPropertyMapping, IMemberMapping
    {
        public WriteOnlyPropertyMapping(IColumnDefinition column, string name, int physicalIndex, int logicalIndex)
        {
            ColumnDefinition = column;
            Name = name;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public string Name { get; }

        IMemberAccessor IMemberMapping.Member => null;

        public Action<IColumnContext, object, object> Reader => null;

        public Action<IColumnContext, object, object[]> Writer => null;

        public IColumnDefinition ColumnDefinition { get; }

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }
    }
}
