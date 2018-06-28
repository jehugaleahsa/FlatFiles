using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to an enum column.
    /// </summary>
    public interface IEnumPropertyMapping<TEnum>
        where TEnum : Enum
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IEnumPropertyMapping<TEnum> ColumnName(string name);

        /// <summary>
        /// Sets the parser to use to convert from a string to an enum.
        /// </summary>
        /// <param name="parser">The parsing function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IEnumPropertyMapping<TEnum> Parser(Func<string, TEnum> parser);

        /// <summary>
        /// Sets the formatter to use to convert from an enum to a string.
        /// </summary>
        /// <param name="formatter">The formatting function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IEnumPropertyMapping<TEnum> Formatter(Func<TEnum, string> formatter);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IEnumPropertyMapping<TEnum> NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IEnumPropertyMapping<TEnum> NullHandler(INullHandler handler);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IEnumPropertyMapping<TEnum> Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class EnumPropertyMapping<TEnum> : IEnumPropertyMapping<TEnum>, IMemberMapping
        where TEnum : Enum
    {
        private readonly EnumColumn<TEnum> column;

        public EnumPropertyMapping(EnumColumn<TEnum> column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IEnumPropertyMapping<TEnum> ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Parser(Func<string, TEnum> parser)
        {
            column.Parser = parser;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Formatter(Func<TEnum, string> formatter)
        {
            column.Formatter = formatter;
            return this;
        }

        public IEnumPropertyMapping<TEnum> NullValue(string value)
        {
            column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IEnumPropertyMapping<TEnum> NullHandler(INullHandler handler)
        {
            column.NullHandler = handler;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Preprocessor(Func<string, string> preprocessor)
        {
            column.Preprocessor = preprocessor;
            return this;
        }

        public IMemberAccessor Member { get; }

        public IColumnDefinition ColumnDefinition => column;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }
    }
}
