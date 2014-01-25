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
    }

    internal sealed class StringPropertyMapping : IStringPropertyMapping, IPropertyMapping
    {
        private readonly StringColumn column;
        private readonly PropertyInfo property;

        public StringPropertyMapping(StringColumn column, PropertyInfo property)
        {
            this.column = column;
            this.property = property;
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
