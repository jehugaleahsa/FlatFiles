namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a Boolean column.
    /// </summary>
    public interface IIgnoredMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IIgnoredMapping ColumnName(string name);
    }

    internal sealed class IgnoredMapping : IIgnoredMapping, IMemberMapping
    {
        private readonly IgnoredColumn _column;

        public IgnoredMapping(IgnoredColumn column, int fileIndex)
        {
            _column = column;
            FileIndex = fileIndex;
        }

        public IIgnoredMapping ColumnName(string name)
        {
            _column.ColumnName = name;
            return this;
        }

        public IMemberAccessor Member => null;

        public IColumnDefinition ColumnDefinition => _column;

        public int FileIndex { get; }

        public int WorkIndex => -1;
    }
}
