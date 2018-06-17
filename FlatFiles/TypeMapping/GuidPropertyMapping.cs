namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a Guid column.
    /// </summary>
    public interface IGuidPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IGuidPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the format to use when reading input.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IGuidPropertyMapping InputFormat(string format);

        /// <summary>
        /// Sets the format to use when writing output.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IGuidPropertyMapping OutputFormat(string format);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IGuidPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IGuidPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class GuidPropertyMapping : IGuidPropertyMapping, IMemberMapping
    {
        private readonly GuidColumn column;

        public GuidPropertyMapping(GuidColumn column, IMemberAccessor member, int fileIndex, int workIndex)
        {
            this.column = column;
            Member = member;
            FileIndex = fileIndex;
            WorkIndex = workIndex;
        }

        public IGuidPropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IGuidPropertyMapping InputFormat(string format)
        {
            column.InputFormat = format;
            return this;
        }

        public IGuidPropertyMapping OutputFormat(string format)
        {
            column.OutputFormat = format;
            return this;
        }

        public IGuidPropertyMapping NullValue(string value)
        {
            column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IGuidPropertyMapping NullHandler(INullHandler handler)
        {
            column.NullHandler = handler;
            return this;
        }

        public IMemberAccessor Member { get; private set; }

        public IColumnDefinition ColumnDefinition => column;

        public int FileIndex { get; private set; }

        public int WorkIndex { get; private set; }
    }
}
