using System;
using System.Globalization;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to an enum column.
    /// </summary>
    public interface IEnumPropertyMapping<TEnum>
        where TEnum : struct
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
    }

    internal sealed class EnumPropertyMapping<TEnum> : IEnumPropertyMapping<TEnum>, IPropertyMapping
        where TEnum : struct
    {
        private readonly EnumColumn<TEnum> column;
        private readonly PropertyInfo property;

        public EnumPropertyMapping(EnumColumn<TEnum> column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public IEnumPropertyMapping<TEnum> ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Parser(Func<string, TEnum> parser)
        {
            this.column.Parser = parser;
            return this;
        }

        public IEnumPropertyMapping<TEnum> Formatter(Func<TEnum, string> formatter)
        {
            this.column.Formatter = formatter;
            return this;
        }

        public IEnumPropertyMapping<TEnum> NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IEnumPropertyMapping<TEnum> NullHandler(INullHandler handler)
        {
            this.column.NullHandler = handler;
            return this;
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public ColumnDefinition ColumnDefinition
        {
            get { return column; }
        }
    }
}
