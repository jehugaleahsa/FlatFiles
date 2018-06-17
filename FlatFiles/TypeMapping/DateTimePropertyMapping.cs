using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a DateTime column.
    /// </summary>
    public interface IDateTimePropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the date/time format the input is expected to be in.
        /// </summary>
        /// <param name="format">The format to expect.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping InputFormat(string format);

        /// <summary>
        /// Sets the date/time format to use for output.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping OutputFormat(string format);

        /// <summary>
        /// Sets the format provider to use when reading and writing date/times.
        /// </summary>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping FormatProvider(IFormatProvider provider);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IDateTimePropertyMapping NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDateTimePropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class DateTimePropertyMapping : IDateTimePropertyMapping, IMemberMapping
    {
        private readonly DateTimeColumn _column;

        public DateTimePropertyMapping(DateTimeColumn column, IMemberAccessor member, int fileIndex, int workIndex)
        {
            _column = column;
            Member = member;
            FileIndex = fileIndex;
            WorkIndex = workIndex;
        }

        public IDateTimePropertyMapping ColumnName(string name)
        {
            _column.ColumnName = name;
            return this;
        }

        public IDateTimePropertyMapping InputFormat(string format)
        {
            _column.InputFormat = format;
            return this;
        }

        public IDateTimePropertyMapping OutputFormat(string format)
        {
            _column.OutputFormat = format;
            return this;
        }

        public IDateTimePropertyMapping FormatProvider(IFormatProvider provider)
        {
            _column.FormatProvider = provider;
            return this;
        }

        public IDateTimePropertyMapping NullValue(string value)
        {
            _column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IDateTimePropertyMapping NullHandler(INullHandler handler)
        {
            _column.NullHandler = handler;
            return this;
        }

        public IDateTimePropertyMapping Preprocessor(Func<string, string> preprocessor)
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
