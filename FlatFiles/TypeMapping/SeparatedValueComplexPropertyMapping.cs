using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to an object.
    /// </summary>
    public interface ISeparatedValueComplexPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the options to use when reading/writing the complex type.
        /// </summary>
        /// <param name="options">The options to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping WithOptions(SeparatedValueOptions options);

        /// <summary>
        /// Sets the value to treat as null.
        /// </summary>
        /// <param name="value">The value to treat as null.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping NullValue(string value);

        /// <summary>
        /// Sets a custom handler for nulls.
        /// </summary>
        /// <param name="handler">The handler to use to recognize nulls.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Setting the handler to null with use the default handler.</remarks>
        ISeparatedValueComplexPropertyMapping NullHandler(INullHandler handler);
    }

    internal sealed class SeparatedValueComplexPropertyMapping<TEntity> : ISeparatedValueComplexPropertyMapping, IComplexPropertyMapping
    {
        private readonly ISeparatedValueTypeMapper<TEntity> mapper;
        private readonly PropertyInfo property;
        private string columnName;
        private SeparatedValueOptions options;
        private INullHandler nullHandler;

        public SeparatedValueComplexPropertyMapping(ISeparatedValueTypeMapper<TEntity> mapper, PropertyInfo property)
        {
            this.mapper = mapper;
            this.property = property;
            this.columnName = property.Name;
        }

        public ColumnDefinition ColumnDefinition
        {
            get
            {
                SeparatedValueSchema schema = mapper.GetSchema();
                SeparatedValueComplexColumn column = new SeparatedValueComplexColumn(columnName, schema);
                column.Options = options;
                column.NullHandler = nullHandler;
                return column;
            }
        }

        public IRecordMapper RecordMapper
        {
            get { return (IRecordMapper)mapper; }
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public ISeparatedValueComplexPropertyMapping ColumnName(string name)
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

        public ISeparatedValueComplexPropertyMapping WithOptions(SeparatedValueOptions options)
        {
            this.options = options;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping NullHandler(INullHandler handler)
        {
            this.nullHandler = handler;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping NullValue(string value)
        {
            this.nullHandler = new ConstantNullHandler(value);
            return this;
        }
    }
}
