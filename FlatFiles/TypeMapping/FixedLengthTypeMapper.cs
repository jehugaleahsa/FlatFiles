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
    public static class FixedLengthTypeMapper
    {
        /// <summary>
        /// Creates an object that can be used to configure the mapping to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <returns>The configuration object.</returns>
        public static IFixedLengthTypeMapper<TEntity> Define<TEntity>()
            where TEntity : new()
        {
            return new FixedLengthTypeMapper<TEntity>(() => new TEntity());
        }

        /// <summary>
        /// Creates an object that can be used to configure the mapping to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <param name="factory">A method to call when creating a new entity.</param>
        /// <returns>The configuration object.</returns>
        public static IFixedLengthTypeMapper<TEntity> Define<TEntity>(Func<TEntity> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            return new FixedLengthTypeMapper<TEntity>(factory);
        }
    }

    /// <summary>
    /// Supports configuration for mapping between entity properties and flat file columns.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being mapped.</typeparam>
    public interface IFixedLengthTypeConfiguration<TEntity>
    {
        /// <summary>
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        FixedLengthSchema GetSchema();

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> property, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping Property(Expression<Func<TEntity, string>> property, Window window);
        
        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="property">An expression tha returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, ISeparatedValueTypeMapper<TProp> mapper, Window window);
        
        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="property">An expression tha returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, IFixedLengthTypeMapper<TProp> mapper, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> property, Window window) where TEnum : struct;

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> property, Window window) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored(Window window);
    }

    /// <summary>
    /// Supports configuring reading to and writing from flat files for a type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity read and written.</typeparam>
    public interface IFixedLengthTypeMapper<TEntity> : IFixedLengthTypeConfiguration<TEntity>
    {
        /// <summary>
        /// Reads the entities from the given reader.
        /// </summary>
        /// <param name="reader">A reader over the fixed-length document.</param>
        /// <param name="options">The options controlling how the fixed-length document is read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<TEntity> Read(TextReader reader, FixedLengthOptions options = null);

        /// <summary>
        /// Gets a typed reader to read entities from the underlying document.
        /// </summary>
        /// <param name="reader">A reader over the fixed-length document.</param>
        /// <param name="options">The options controlling how the fixed-length document is read.</param>
        /// <returns>A typed reader.</returns>
        ITypedReader<TEntity> GetReader(TextReader reader, FixedLengthOptions options = null);

        /// <summary>
        /// Writes the given entities to the given writer.
        /// </summary>
        /// <param name="writer">A writer over the fixed-length document.</param>
        /// <param name="entities">The entities to write to the document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        void Write(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null);

        /// <summary>
        /// Gets a typed writer to write entities to the underlying document.
        /// </summary>
        /// <param name="writer">The writer over the fixed-length document.</param>
        /// <param name="options">The options controlling how the fixed-length document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<TEntity> GetWriter(TextWriter writer, FixedLengthOptions options = null);
    }

    internal sealed class FixedLengthTypeMapper<TEntity> : IFixedLengthTypeMapper<TEntity>, IRecordMapper<TEntity>
    {
        private readonly Func<TEntity> factory;
        private readonly Dictionary<string, IPropertyMapping> mappingLookup;
        private readonly List<IPropertyMapping> mappings;
        private readonly Dictionary<IPropertyMapping, Window> windowLookup;

        public FixedLengthTypeMapper(Func<TEntity> factory)
        {
            this.factory = factory;
            this.mappingLookup = new Dictionary<string, IPropertyMapping>();
            this.mappings = new List<IPropertyMapping>();
            this.windowLookup = new Dictionary<IPropertyMapping, Window>();
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getBooleanMapping(propertyInfo, window);
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getBooleanMapping(propertyInfo, window);
        }

        private IBooleanPropertyMapping getBooleanMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                BooleanColumn column = new BooleanColumn(propertyInfo.Name);
                mapping = new BooleanPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IBooleanPropertyMapping)mapping;
        }

        public IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getByteArrayMapping(propertyInfo, window);
        }

        private IByteArrayPropertyMapping getByteArrayMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                ByteArrayColumn column = new ByteArrayColumn(propertyInfo.Name);
                mapping = new ByteArrayPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IByteArrayPropertyMapping)mapping;
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getByteMapping(propertyInfo, window);
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getByteMapping(propertyInfo, window);
        }

        private IBytePropertyMapping getByteMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                ByteColumn column = new ByteColumn(propertyInfo.Name);
                mapping = new BytePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IBytePropertyMapping)mapping;
        }

        public ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getCharArrayMapping(propertyInfo, window);
        }

        private ICharArrayPropertyMapping getCharArrayMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                CharArrayColumn column = new CharArrayColumn(propertyInfo.Name);
                mapping = new CharArrayPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (ICharArrayPropertyMapping)mapping;
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getCharMapping(propertyInfo, window);
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getCharMapping(propertyInfo, window);
        }

        private ICharPropertyMapping getCharMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                CharColumn column = new CharColumn(propertyInfo.Name);
                mapping = new CharPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (ICharPropertyMapping)mapping;
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDateTimeMapping(propertyInfo, window);
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDateTimeMapping(propertyInfo, window);
        }

        private IDateTimePropertyMapping getDateTimeMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                DateTimeColumn column = new DateTimeColumn(propertyInfo.Name);
                mapping = new DateTimePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IDateTimePropertyMapping)mapping;
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDecimalMapping(propertyInfo, window);
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDecimalMapping(propertyInfo, window);
        }

        private IDecimalPropertyMapping getDecimalMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                DecimalColumn column = new DecimalColumn(propertyInfo.Name);
                mapping = new DecimalPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IDecimalPropertyMapping)mapping;
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDoubleMapping(propertyInfo, window);
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getDoubleMapping(propertyInfo, window);
        }

        private IDoublePropertyMapping getDoubleMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                DoubleColumn column = new DoubleColumn(propertyInfo.Name);
                mapping = new DoublePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IDoublePropertyMapping)mapping;
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getGuidMapping(propertyInfo, window);
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getGuidMapping(propertyInfo, window);
        }

        private IGuidPropertyMapping getGuidMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                GuidColumn column = new GuidColumn(propertyInfo.Name);
                mapping = new GuidPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IGuidPropertyMapping)mapping;
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt16Mapping(propertyInfo, window);
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt16Mapping(propertyInfo, window);
        }

        private IInt16PropertyMapping getInt16Mapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int16Column column = new Int16Column(propertyInfo.Name);
                mapping = new Int16PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IInt16PropertyMapping)mapping;
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt32Mapping(propertyInfo, window);
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt32Mapping(propertyInfo, window);
        }

        private IInt32PropertyMapping getInt32Mapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int32Column column = new Int32Column(propertyInfo.Name);
                mapping = new Int32PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IInt32PropertyMapping)mapping;
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt64Mapping(propertyInfo, window);
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getInt64Mapping(propertyInfo, window);
        }

        private IInt64PropertyMapping getInt64Mapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int64Column column = new Int64Column(propertyInfo.Name);
                mapping = new Int64PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IInt64PropertyMapping)mapping;
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getSingleMapping(propertyInfo, window);
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getSingleMapping(propertyInfo, window);
        }

        private ISinglePropertyMapping getSingleMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                SingleColumn column = new SingleColumn(propertyInfo.Name);
                mapping = new SinglePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (ISinglePropertyMapping)mapping;
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string>> property, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getStringMapping(propertyInfo, window);
        }

        private IStringPropertyMapping getStringMapping(PropertyInfo propertyInfo, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                StringColumn column = new StringColumn(propertyInfo.Name);
                mapping = new StringPropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IStringPropertyMapping)mapping;
        }

        public ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getComplexMapping(propertyInfo, mapper, window);
        }

        private ISeparatedValueComplexPropertyMapping getComplexMapping<TProp>(PropertyInfo propertyInfo, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                mapping = new SeparatedValueComplexPropertyMapping<TProp>(mapper, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (ISeparatedValueComplexPropertyMapping)mapping;
        }

        public IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> property, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getComplexMapping(propertyInfo, mapper, window);
        }

        private IFixedLengthComplexPropertyMapping getComplexMapping<TProp>(PropertyInfo propertyInfo, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                mapping = new FixedLengthComplexPropertyMapping<TProp>(mapper, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            windowLookup[mapping] = window;
            return (IFixedLengthComplexPropertyMapping)mapping;
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> property, Window window) 
            where TEnum : struct
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getEnumMapping<TEnum>(propertyInfo, window);
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> property, Window window)
            where TEnum : struct
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getEnumMapping<TEnum>(propertyInfo, window);
        }

        private IEnumPropertyMapping<TEnum> getEnumMapping<TEnum>(PropertyInfo propertyInfo, Window window)
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
            windowLookup[mapping] = window;
            return (IEnumPropertyMapping<TEnum>)mapping;
        }

        public IIgnoredMapping Ignored(Window window)
        {
            IgnoredColumn column = new IgnoredColumn();
            IgnoredMapping mapping = new IgnoredMapping(column);
            mappings.Add(mapping);
            windowLookup[mapping] = window;
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

        public IEnumerable<TEntity> Read(TextReader reader, FixedLengthOptions options = null)
        {
            FixedLengthSchema schema = getSchema();
            IReader fixedLengthReader = new FixedLengthReader(reader, schema, options);
            return read(fixedLengthReader);
        }

        private IEnumerable<TEntity> read(IReader reader)
        {
            TypedReader<TEntity> typedReader = getTypedReader(reader);
            while (typedReader.Read())
            {
                yield return typedReader.Current;
            }
        }

        public ITypedReader<TEntity> GetReader(TextReader reader, FixedLengthOptions options = null)
        {
            FixedLengthSchema schema = getSchema();
            IReader fixedLengthReader = new FixedLengthReader(reader, schema, options);
            return getTypedReader(fixedLengthReader);
        }

        private TypedReader<TEntity> getTypedReader(IReader reader)
        {
            var serializer = new TypedRecordReader<TEntity>(this.factory, this.mappings);
            return new TypedReader<TEntity>(reader, serializer);
        }

        public void Write(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            FixedLengthSchema schema = getSchema();
            IWriter fixedLengthWriter = new FixedLengthWriter(writer, schema, options);
            write(fixedLengthWriter, entities);
        }

        private void write(IWriter writer, IEnumerable<TEntity> entities)
        {
            TypedWriter<TEntity> typedWriter = getTypedWriter(writer);
            foreach (TEntity entity in entities)
            {
                typedWriter.Write(entity);
            }
        }

        public ITypedWriter<TEntity> GetWriter(TextWriter writer, FixedLengthOptions options = null)
        {
            FixedLengthSchema schema = getSchema();
            IWriter fixedLengthWriter = new FixedLengthWriter(writer, schema, options);
            return getTypedWriter(fixedLengthWriter);
        }

        private TypedWriter<TEntity> getTypedWriter(IWriter writer)
        {
            var serializer = new TypedRecordWriter<TEntity>(this.mappings);
            return new TypedWriter<TEntity>(writer, serializer);
        }

        public FixedLengthSchema GetSchema()
        {
            return getSchema();
        }

        private FixedLengthSchema getSchema()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            foreach (IPropertyMapping mapping in mappings)
            {
                IColumnDefinition column = mapping.ColumnDefinition;
                Window window = windowLookup[mapping];
                schema.AddColumn(column, window);
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
