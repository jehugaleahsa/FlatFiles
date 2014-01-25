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
