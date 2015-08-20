using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a char column.
    /// </summary>
    public interface ICharPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICharPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets whether the parser should ignore extra characters.
        /// </summary>
        /// <param name="allow">True if the parser should ignore extra characters -or- false, if an error should occur.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICharPropertyMapping AllowTrailing(bool allow);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ICharPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        ICharPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class CharPropertyMapping : ICharPropertyMapping, IPropertyMapping
    {
        private readonly CharColumn column;
        private readonly PropertyInfo property;

        public CharPropertyMapping(CharColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public ICharPropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public ICharPropertyMapping AllowTrailing(bool allow)
        {
            this.column.AllowTrailing = allow;
            return this;
        }

        public ICharPropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public ICharPropertyMapping NullHandler(INullHandler handler)
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
