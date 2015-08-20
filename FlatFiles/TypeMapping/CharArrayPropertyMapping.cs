using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a char array column.
    /// </summary>
    public interface ICharArrayPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICharArrayPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICharArrayPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        ICharArrayPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class CharArrayPropertyMapping : ICharArrayPropertyMapping, IPropertyMapping
    {
        private readonly CharArrayColumn column;
        private readonly PropertyInfo property;

        public CharArrayPropertyMapping(CharArrayColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public ICharArrayPropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public ICharArrayPropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public ICharArrayPropertyMapping NullHandler(INullHandler handler)
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
