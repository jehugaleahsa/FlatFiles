using System;
using System.Reflection;

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
    }

    internal sealed class BooleanPropertyMapping : IBooleanPropertyMapping, IPropertyMapping
    {
        private readonly BooleanColumn column;
        private readonly PropertyInfo property;

        public BooleanPropertyMapping(BooleanColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
        }

        public IBooleanPropertyMapping ColumnName(string name)
        {
            this.column.ColumnName = name;
            return this;
        }

        public IBooleanPropertyMapping TrueString(string value)
        {
            this.column.TrueString = value;
            return this;
        }

        public IBooleanPropertyMapping FalseString(string value)
        {
            this.column.FalseString = value;
            return this;
        }

        public IBooleanPropertyMapping NullValue(string value)
        {
            this.column.NullHandler = new ConstantNullHandler(value);
            return this;
        }

        public IBooleanPropertyMapping NullHandler(INullHandler handler)
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
