using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a Boolean column.
    /// </summary>
    public interface IBooleanPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the value used to represent true.
        /// </summary>
        /// <param name="value">The value used to represent true.</param>
        /// <returns>The property mapping for futher configuration.</returns>
        IBooleanPropertyMapping TrueString(string value);

        /// <summary>
        /// Sets the value used to represent false.
        /// </summary>
        /// <param name="value">The value used to represent false.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping FalseString(string value);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IBooleanPropertyMapping NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class BooleanPropertyMapping : IBooleanPropertyMapping, IMemberMapping
    {
        private readonly BooleanColumn _column;

        public BooleanPropertyMapping(BooleanColumn column, IMemberAccessor member, int fileIndex, int workIndex)
        {
            _column = column;
            Member = member;
            FileIndex = fileIndex;
            WorkIndex = workIndex;
        }

        public IBooleanPropertyMapping ColumnName(string name)
        {
            _column.ColumnName = name;
            return this;
        }

        public IBooleanPropertyMapping TrueString(string value)
        {
            _column.TrueString = value;
            return this;
        }

        public IBooleanPropertyMapping FalseString(string value)
        {
            _column.FalseString = value;
            return this;
        }

        public IBooleanPropertyMapping NullValue(string value)
        {
            _column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IBooleanPropertyMapping NullHandler(INullHandler handler)
        {
            _column.NullHandler = handler;
            return this;
        }

        public IBooleanPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            _column.Preprocessor = preprocessor;
            return this;
        }

        public IMemberAccessor Member { get; }

        public IColumnDefinition ColumnDefinition => _column;

        public int FileIndex { get; }

        public int WorkIndex { get; }
    }
}
