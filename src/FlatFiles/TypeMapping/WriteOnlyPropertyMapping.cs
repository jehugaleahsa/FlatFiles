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
        private readonly IColumnDefinition column;
        private readonly string name;

        public WriteOnlyPropertyMapping(IColumnDefinition column, string name, int fileIndex, int workIndex)
        {
            this.column = column;
            this.name = name;
            this.FileIndex = fileIndex;
            this.WorkIndex = workIndex;
        }

        public string Name
        {
            get { return name; }
        }

        IMemberAccessor IMemberMapping.Member
        {
            get { return null; }
        }

        public IColumnDefinition ColumnDefinition
        {
            get { return column; }
        }

        public int FileIndex { get; private set; }

        public int WorkIndex { get; private set; }
    }
}
