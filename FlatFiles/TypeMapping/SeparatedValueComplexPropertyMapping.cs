using System;
using FlatFiles.Properties;

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
        /// Sets what value(s) are treated as null.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause the default formatter to be used.</remarks>
        ISeparatedValueComplexPropertyMapping NullFormatter(INullFormatter formatter);

        /// <summary>
        /// Sets the default value to use when a null is encountered on a non-null property.
        /// </summary>
        /// <param name="defaultValue">The default value to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause an exception to be thrown for unexpected nulls.</remarks>
        ISeparatedValueComplexPropertyMapping DefaultValue(IDefaultValue defaultValue);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ISeparatedValueComplexPropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class SeparatedValueComplexPropertyMapping<TEntity> : ISeparatedValueComplexPropertyMapping, IMemberMapping
    {
        private readonly ISeparatedValueTypeMapper<TEntity> mapper;
        private string columnName;
        private SeparatedValueOptions options;
        private INullFormatter nullFormatter;
        private IDefaultValue defaultValue;
        private Func<string, string> preprocessor;

        public SeparatedValueComplexPropertyMapping(
            ISeparatedValueTypeMapper<TEntity> mapper, 
            IMemberAccessor member, 
            int physicalIndex, 
            int logicalIndex)
        {
            this.mapper = mapper;
            Member = member;
            columnName = member.Name;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IColumnDefinition ColumnDefinition
        {
            get
            {
                SeparatedValueSchema schema = mapper.GetSchema();
                SeparatedValueComplexColumn column = new SeparatedValueComplexColumn(columnName, schema)
                {
                    Options = options,
                    NullFormatter = nullFormatter,
                    DefaultValue = defaultValue,
                    Preprocessor = preprocessor
                };

                var mapperSource = (IMapperSource<TEntity>)mapper;
                var recordMapper = mapperSource.GetMapper();
                return new ComplexMapperColumn<TEntity>(column, recordMapper);
            }
        }

        public IMemberAccessor Member { get; }

        public Action<IColumnContext, object, object> Reader => null;

        public Action<IColumnContext, object, object[]> Writer => null;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }

        public ISeparatedValueComplexPropertyMapping ColumnName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(Resources.BlankColumnName);
            }
            columnName = name;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping WithOptions(SeparatedValueOptions options)
        {
            this.options = options;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping NullFormatter(INullFormatter formatter)
        {
            nullFormatter = formatter;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            this.defaultValue = defaultValue;
            return this;
        }

        public ISeparatedValueComplexPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            this.preprocessor = preprocessor;
            return this;
        }
    }
}
