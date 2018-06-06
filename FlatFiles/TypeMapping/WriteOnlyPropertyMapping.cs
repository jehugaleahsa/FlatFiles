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
        public WriteOnlyPropertyMapping(IColumnDefinition column, string name, int fileIndex, int workIndex)
        {
            this.ColumnDefinition = column;
            this.Name = name;
            this.FileIndex = fileIndex;
            this.WorkIndex = workIndex;
        }

        public string Name { get; private set; }

        IMemberAccessor IMemberMapping.Member
        {
            get { return null; }
        }

        public IColumnDefinition ColumnDefinition { get; private set; }

        public int FileIndex { get; private set; }

        public int WorkIndex { get; private set; }
    }
}
