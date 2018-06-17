using System;
using System.Globalization;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a double column.
    /// </summary>
    public interface IDoublePropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDoublePropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the format provider to use.
        /// </summary>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDoublePropertyMapping FormatProvider(IFormatProvider provider);

        /// <summary>
        /// Sets the number styles to expect when parsing the input.
        /// </summary>
        /// <param name="styles">The number styles used in the input.</param>
        /// <returns>The property mappig for further configuration.</returns>
        IDoublePropertyMapping NumberStyles(NumberStyles styles);

        /// <summary>
        /// Sets the output format to use.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDoublePropertyMapping OutputFormat(string format);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDoublePropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IDoublePropertyMapping NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IDoublePropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class DoublePropertyMapping : IDoublePropertyMapping, IMemberMapping
    {
        private readonly DoubleColumn _column;

        public DoublePropertyMapping(DoubleColumn column, IMemberAccessor member, int fileIndex, int workIndex)
        {
            _column = column;
            Member = member;
            FileIndex = fileIndex;
            WorkIndex = workIndex;
        }

        public IDoublePropertyMapping ColumnName(string name)
        {
            _column.ColumnName = name;
            return this;
        }

        public IDoublePropertyMapping FormatProvider(IFormatProvider provider)
        {
            _column.FormatProvider = provider;
            return this;
        }

        public IDoublePropertyMapping NumberStyles(NumberStyles styles)
        {
            _column.NumberStyles = styles;
            return this;
        }

        public IDoublePropertyMapping OutputFormat(string format)
        {
            _column.OutputFormat = format;
            return this;
        }

        public IDoublePropertyMapping NullValue(string value)
        {
            _column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IDoublePropertyMapping NullHandler(INullHandler handler)
        {
            _column.NullHandler = handler;
            return this;
        }

        public IDoublePropertyMapping Preprocessor(Func<string, string> preprocessor)
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
