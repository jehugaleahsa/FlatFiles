using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides methods for creating type mappers.
    /// </summary>
    public static class SeparatedValueTypeMapper
    {
        /// <summary>
        /// Creates an object that can be used to configure the mapping to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <returns>The configuration object.</returns>
        public static ISeparatedValueTypeMapper<TEntity> Define<TEntity>()
            where TEntity : new()
        {
            return new SeparatedValueTypeMapper<TEntity>(() => new TEntity());
        }

        /// <summary>
        /// Creates an object that can be used to configure the mapping to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <param name="factory">A method to call when creating a new entity.</param>
        /// <returns>The configuration object.</returns>
        public static ISeparatedValueTypeMapper<TEntity> Define<TEntity>(Func<TEntity> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            return new SeparatedValueTypeMapper<TEntity>(factory);
        }
    }

    /// <summary>
    /// Supports configuration for mapping between entity properties and flat file columns.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being mapped.</typeparam>
    public interface ISeparatedValueTypeConfiguration<TEntity>
    {
        /// <summary>
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        SeparatedValueSchema GetSchema();

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping Property(Expression<Func<TEntity, string>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="property">An expression tha returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, ISeparatedValueTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, IFixedLengthTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> property) where TEnum : struct;

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> property) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored();
    }

    /// <summary>
    /// Supports configuring reading to and writing from flat files for a type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity read and written.</typeparam>
    public interface ISeparatedValueTypeMapper<TEntity> : ISeparatedValueTypeConfiguration<TEntity>
    {
        /// <summary>
        /// Reads the entities from the given reader.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<TEntity> Read(TextReader reader, SeparatedValueOptions options = null);

        /// <summary>
        /// Gets a typed reader to read entities from the underlying document.
        /// </summary>
        /// <param name="reader">A reader over the fixed-length document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <returns>A typed reader.</returns>
        ITypedReader<TEntity> GetReader(TextReader reader, SeparatedValueOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        void Write(TextWriter writer, IEnumerable<TEntity> entities, SeparatedValueOptions options = null);

        /// <summary>
        /// Gets a typed writer to write entities to the underlying document.
        /// </summary>
        /// <param name="writer">The writer over the fixed-length document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<TEntity> GetWriter(TextWriter writer, SeparatedValueOptions options = null);
    }

    internal sealed class SeparatedValueTypeMapper<TEntity> : ISeparatedValueTypeMapper<TEntity>, IRecordMapper<TEntity>
    {
        private readonly Func<TEntity> factory;
        private readonly Dictionary<string, IPropertyMapping> mappingLookup;
        private readonly List<IPropertyMapping> mappings;

        public SeparatedValueTypeMapper(Func<TEntity> factory)
        {
            this.factory = factory;
            this.mappingLookup = new Dictionary<string, IPropertyMapping>();
            this.mappings = new List<IPropertyMapping>();
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getBooleanMapping(propertyInfo);
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getBooleanMapping(propertyInfo);
        }

        private IBooleanPropertyMapping getBooleanMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                BooleanColumn column = new BooleanColumn(propertyInfo.Name);
                mapping = new BooleanPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IBooleanPropertyMapping)mapping;
        }

        public IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getByteArrayMapping(propertyInfo);
        }

        private IByteArrayPropertyMapping getByteArrayMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                ByteArrayColumn column = new ByteArrayColumn(propertyInfo.Name);
                mapping = new ByteArrayPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
                
            }
            return (IByteArrayPropertyMapping)mapping;
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getByteMapping(propertyInfo);
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getByteMapping(propertyInfo);
        }

        private IBytePropertyMapping getByteMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                ByteColumn column = new ByteColumn(propertyInfo.Name);
                mapping = new BytePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IBytePropertyMapping)mapping;
        }

        public ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getCharArrayMapping(propertyInfo);
        }

        private ICharArrayPropertyMapping getCharArrayMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                CharArrayColumn column = new CharArrayColumn(propertyInfo.Name);
                mapping = new CharArrayPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (ICharArrayPropertyMapping)mapping;
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getCharMapping(propertyInfo);
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getCharMapping(propertyInfo);
        }

        private ICharPropertyMapping getCharMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                CharColumn column = new CharColumn(propertyInfo.Name);
                mapping = new CharPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (ICharPropertyMapping)mapping;
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDateTimeMapping(propertyInfo);
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDateTimeMapping(propertyInfo);
        }

        private IDateTimePropertyMapping getDateTimeMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                DateTimeColumn column = new DateTimeColumn(propertyInfo.Name);
                mapping = new DateTimePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IDateTimePropertyMapping)mapping;
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDecimalMapping(propertyInfo);
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDecimalMapping(propertyInfo);
        }

        private IDecimalPropertyMapping getDecimalMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                DecimalColumn column = new DecimalColumn(propertyInfo.Name);
                mapping = new DecimalPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IDecimalPropertyMapping)mapping;
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDoubleMapping(propertyInfo);
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDoubleMapping(propertyInfo);
        }

        private IDoublePropertyMapping getDoubleMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                DoubleColumn column = new DoubleColumn(propertyInfo.Name);
                mapping = new DoublePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IDoublePropertyMapping)mapping;
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getGuidMapping(propertyInfo);
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getGuidMapping(propertyInfo);
        }

        private IGuidPropertyMapping getGuidMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                GuidColumn column = new GuidColumn(propertyInfo.Name);
                mapping = new GuidPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IGuidPropertyMapping)mapping;
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt16Mapping(propertyInfo);
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt16Mapping(propertyInfo);
        }

        private IInt16PropertyMapping getInt16Mapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int16Column column = new Int16Column(propertyInfo.Name);
                mapping = new Int16PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IInt16PropertyMapping)mapping;
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt32Mapping(propertyInfo);
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt32Mapping(propertyInfo);
        }

        private IInt32PropertyMapping getInt32Mapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int32Column column = new Int32Column(propertyInfo.Name);
                mapping = new Int32PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IInt32PropertyMapping)mapping;
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt64Mapping(propertyInfo);
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt64Mapping(propertyInfo);
        }

        private IInt64PropertyMapping getInt64Mapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int64Column column = new Int64Column(propertyInfo.Name);
                mapping = new Int64PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IInt64PropertyMapping)mapping;
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getSingleMapping(propertyInfo);
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getSingleMapping(propertyInfo);
        }

        private ISinglePropertyMapping getSingleMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                SingleColumn column = new SingleColumn(propertyInfo.Name);
                mapping = new SinglePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (ISinglePropertyMapping)mapping;
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getStringMapping(propertyInfo);
        }

        private IStringPropertyMapping getStringMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                StringColumn column = new StringColumn(propertyInfo.Name);
                mapping = new StringPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IStringPropertyMapping)mapping;
        }

        public ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, ISeparatedValueTypeMapper<TProp> mapper)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getComplexMapping(propertyInfo, mapper);
        }

        private ISeparatedValueComplexPropertyMapping getComplexMapping<TProp>(PropertyInfo propertyInfo, ISeparatedValueTypeMapper<TProp> mapper)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                mapping = new SeparatedValueComplexPropertyMapping<TProp>(mapper, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (ISeparatedValueComplexPropertyMapping)mapping;
        }

        public IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, IFixedLengthTypeMapper<TProp> mapper)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getComplexMapping(propertyInfo, mapper);
        }

        private IFixedLengthComplexPropertyMapping getComplexMapping<TProp>(PropertyInfo propertyInfo, IFixedLengthTypeMapper<TProp> mapper)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                mapping = new FixedLengthComplexPropertyMapping<TProp>(mapper, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IFixedLengthComplexPropertyMapping)mapping;
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> property)
            where TEnum : struct
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getEnumMapping<TEnum>(propertyInfo);
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> property)
            where TEnum : struct
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getEnumMapping<TEnum>(propertyInfo);
        }

        private IEnumPropertyMapping<TEnum> getEnumMapping<TEnum>(PropertyInfo propertyInfo)
            where TEnum : struct
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                var column = new EnumColumn<TEnum>(propertyInfo.Name);
                mapping = new EnumPropertyMapping<TEnum>(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IEnumPropertyMapping<TEnum>)mapping;
        }

        public IIgnoredMapping Ignored()
        {
            var column = new IgnoredColumn();
            var mapping = new IgnoredMapping(column);
            mappings.Add(mapping);
            return mapping;
        }

        private static PropertyInfo getProperty<TProp>(Expression<Func<TEntity, TProp>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            MemberExpression member = property.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(Resources.BadPropertySelector, "property");
            }
            PropertyInfo propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(Resources.BadPropertySelector, "property");
            }
            if (!propertyInfo.DeclaringType.IsAssignableFrom(typeof(TEntity)))
            {
                throw new ArgumentException(Resources.BadPropertySelector, "property");
            }
            return propertyInfo;
        }

        public IEnumerable<TEntity> Read(TextReader reader, SeparatedValueOptions options = null)
        {
            SeparatedValueSchema schema = getSchema();
            IReader separatedValueReader = new SeparatedValueReader(reader, schema, options);
            return read(separatedValueReader);
        }

        private IEnumerable<TEntity> read(IReader reader)
        {
            TypedReader<TEntity> typedReader = getTypedReader(reader);
            while (typedReader.Read())
            {
                yield return typedReader.Current;
            }
        }

        public ITypedReader<TEntity> GetReader(TextReader reader, SeparatedValueOptions options = null)
        {
            SeparatedValueSchema schema = getSchema();
            IReader separatedValueReader = new SeparatedValueReader(reader, schema, options);
            return getTypedReader(separatedValueReader);
        }

        private TypedReader<TEntity> getTypedReader(IReader reader)
        {
            var deserializer = new TypedRecordReader<TEntity>(this.factory, this.mappings);
            TypedReader<TEntity> typedReader = new TypedReader<TEntity>(reader, deserializer);
            return typedReader;
        }

        public void Write(TextWriter writer, IEnumerable<TEntity> entities, SeparatedValueOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            SeparatedValueSchema schema = getSchema();
            IWriter separatedValueWriter = new SeparatedValueWriter(writer, schema, options);
            write(separatedValueWriter, entities);
        }

        private void write(IWriter writer, IEnumerable<TEntity> entities)
        {
            TypedWriter<TEntity> typedWriter = getTypedWriter(writer);
            foreach (TEntity entity in entities)
            {
                typedWriter.Write(entity);
            }
        }

        public ITypedWriter<TEntity> GetWriter(TextWriter writer, SeparatedValueOptions options = null)
        {
            SeparatedValueSchema schema = getSchema();
            IWriter separatedValueWriter = new SeparatedValueWriter(writer, schema, options);
            return getTypedWriter(separatedValueWriter);
        }

        private TypedWriter<TEntity> getTypedWriter(IWriter writer)
        {
            var serializer = new TypedRecordWriter<TEntity>(this.mappings);
            return new TypedWriter<TEntity>(writer, serializer);
        }

        public SeparatedValueSchema GetSchema()
        {
            return getSchema();
        }

        private SeparatedValueSchema getSchema()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            foreach (IPropertyMapping mapping in mappings)
            {
                IColumnDefinition column = mapping.ColumnDefinition;
                schema.AddColumn(column);
            }
            return schema;
        }

        public TypedRecordReader<TEntity> GetReader()
        {
            return new TypedRecordReader<TEntity>(this.factory, this.mappings);
        }

        public TypedRecordWriter<TEntity> GetWriter()
        {
            return new TypedRecordWriter<TEntity>(this.mappings);
        }
    }
}
