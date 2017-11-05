using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a string column.
    /// </summary>
    public interface IStringPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IStringPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets whether the value should be trimmed.
        /// </summary>
        /// <param name="trim">True if the parsed value should be trimmed -or- false, if the value should be returned without modification.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IStringPropertyMapping Trim(bool trim);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IStringPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IStringPropertyMapping NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IStringPropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class StringPropertyMapping : IStringPropertyMapping, IMemberMapping
    {
        private readonly StringColumn column;
        private readonly IMemberAccessor member;

        public StringPropertyMapping(StringColumn column, IMemberAccessor member, int fileIndex, int workFile)
        {
            this.column = column;
            this.member = member;
            this.FileIndex = fileIndex;
            this.WorkIndex = workFile;
        }

        public IStringPropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public IStringPropertyMapping Trim(bool trim)
        {
            this.column.Trim = trim;
            return this;
        }

        public IStringPropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IStringPropertyMapping NullHandler(INullHandler handler)
        {
            this.column.NullHandler = handler;
            return this;
        }

        public IStringPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            this.column.Preprocessor = preprocessor;
            return this;
        }

        public IMemberAccessor Member
        {
            get { return member; }
        }

        public IColumnDefinition ColumnDefinition
        {
            get { return column; }
        }

        public int FileIndex { get; private set; }

        public int WorkIndex { get; private set; }
    }
}
