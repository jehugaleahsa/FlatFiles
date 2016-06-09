using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to an object.
    /// </summary>
    public interface IFixedLengthComplexPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the options to use when reading/writing the complex type.
        /// </summary>
        /// <param name="options">The options to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping WithOptions(FixedLengthOptions options);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IFixedLengthComplexPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        IFixedLengthComplexPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class FixedLengthComplexPropertyMapping<TEntity> : IFixedLengthComplexPropertyMapping, IComplexPropertyMapping
    {
        private readonly IFixedLengthTypeMapper<TEntity> mapper;
        private readonly PropertyInfo property;
        private string columnName;
        private FixedLengthOptions options;
        private INullHandler nullHandler;

        public FixedLengthComplexPropertyMapping(IFixedLengthTypeMapper<TEntity> mapper, PropertyInfo property)
        {
            this.mapper = mapper;
            this.property = property;
            this.columnName = property.Name;
        }

        public IRecordMapper RecordMapper
        {
            get { return (IRecordMapper)mapper; }
        }

        public ColumnDefinition ColumnDefinition
        {
            get
            {
                FixedLengthSchema schema = mapper.GetSchema();
                FixedLengthComplexColumn column = new FixedLengthComplexColumn(columnName, schema);
                column.Options = options;
                column.NullHandler = nullHandler;
                return column;
            }
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public IFixedLengthComplexPropertyMapping ColumnName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                this.columnName = property.Name;
            }
            else
            {
                this.columnName = name;
            }
            return this;
        }

        public IFixedLengthComplexPropertyMapping WithOptions(FixedLengthOptions options)
        {
            this.options = options;
            return this;
        }

        public IFixedLengthComplexPropertyMapping NullHandler(INullHandler handler)
        {
            this.nullHandler = handler;
            return this;
        }

        public IFixedLengthComplexPropertyMapping NullValue(string value)
        {
            this.nullHandler = new ConstantNullHandler(value);
            return this;
        }
    }
}
