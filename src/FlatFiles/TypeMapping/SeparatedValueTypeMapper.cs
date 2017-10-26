using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using FlatFiles.Resources;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides methods for creating type mappers.
    /// </summary>
    public static class SeparatedValueTypeMapper
    {
        /// <summary>
        /// Creates a configuration object that can be used to map to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <returns>The configuration object.</returns>
        public static ISeparatedValueTypeMapper<TEntity> Define<TEntity>()
            where TEntity : new()
        {
            return new SeparatedValueTypeMapper<TEntity>(() => new TEntity());
        }

        /// <summary>
        /// Creates a configuration object that can be used to map to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <param name="factory">A method that generates an instance of the entity.</param>
        /// <returns>The configuration object.</returns>
        public static ISeparatedValueTypeMapper<TEntity> Define<TEntity>(Func<TEntity> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            return new SeparatedValueTypeMapper<TEntity>(factory);
        }

        /// <summary>
        /// Creates a configuration object that can be used to map to and from a runtime entity and a flat file record.
        /// </summary>
        /// <param name="entityType">The type of the entity whose properties will be mapped.</param>
        /// <returns>The configuration object.</returns>
        /// <remarks>The entity type must have a default constructor.</remarks>
        public static IDynamicSeparatedValueTypeMapper DefineDynamic(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            var mapperType = typeof(SeparatedValueTypeMapper<>).MakeGenericType(entityType);
            var mapper = Activator.CreateInstance(mapperType);
            return (IDynamicSeparatedValueTypeMapper)mapper;
        }

        /// <summary>
        /// Creates a configuration object that can be used to map to and from a runtime entity and a flat file record.
        /// </summary>
        /// <param name="entityType">The type of the entity whose properties will be mapped.</param>
        /// <param name="factory">A method that generates an instance of the entity.</param>
        /// <returns>The configuration object.</returns>
        /// <remarks>The entity type must have a default constructor.</remarks>
        public static IDynamicSeparatedValueTypeMapper DefineDynamic(Type entityType, Func<object> factory)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            var mapperType = typeof(SeparatedValueTypeMapper<>).MakeGenericType(entityType);
            var mapper = Activator.CreateInstance(mapperType, factory);
            return (IDynamicSeparatedValueTypeMapper)mapper;
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
        ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> property);

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
        IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> property);

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
        IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> property);

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
        IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> property);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="property">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> property);

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
    /// Supports reading to and writing from flat files for a type.
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
        /// <param name="reader">A reader over the separated value document.</param>
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
        /// <param name="writer">The writer over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<TEntity> GetWriter(TextWriter writer, SeparatedValueOptions options = null);

        /// <summary>
        /// When optimized (the default), mappers will use System.Reflection.Emit to generate 
        /// code to get and set entity properties, resulting in significant performance improvements. 
        /// However, some environments do not support runtime JIT, so disabling optimization will allow
        /// FlatFiles to work.
        /// </summary>
        /// <param name="isOptimized">Specifies whether the mapping process should be optimized.</param>
        void OptimizeMapping(bool isOptimized = true);
    }

    /// <summary>
    /// Supports runtime configuration for mapping between runtime type entity properties and flat file columns.
    /// </summary>
    public interface IDynamicSeparatedValueTypeConfiguration
    {
        /// <summary>
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        SeparatedValueSchema GetSchema();

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping BooleanProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping ByteArrayProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping ByteProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping SByteProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping CharArrayProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping CharProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping DateTimeProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping DecimalProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping DoubleProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping GuidProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Int16Property(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping UInt16Property(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Int32Property(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping UInt32Property(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Int64Property(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping UInt64Property(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping SingleProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping StringProperty(string propertyName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(string propertyName, ISeparatedValueTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(string propertyName, IFixedLengthTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(string propertyName) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored();
    }

    /// <summary>
    /// Supports reading to and writing from flat files for a runtime type. 
    /// </summary>
    public interface IDynamicSeparatedValueTypeMapper : IDynamicSeparatedValueTypeConfiguration
    {
        /// <summary>
        /// Reads the entities from the given reader.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<object> Read(TextReader reader, SeparatedValueOptions options = null);

        /// <summary>
        /// Gets a typed reader to read entities from the underlying document.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <returns>A typed reader.</returns>
        ITypedReader<object> GetReader(TextReader reader, SeparatedValueOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        void Write(TextWriter writer, IEnumerable<object> entities, SeparatedValueOptions options = null);

        /// <summary>
        /// Gets a typed writer to write entities to the underlying document.
        /// </summary>
        /// <param name="writer">The writer over the fixed-length document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<object> GetWriter(TextWriter writer, SeparatedValueOptions options = null);

        /// <summary>
        /// When optimized (the default), mappers will use System.Reflection.Emit to generate 
        /// code to get and set entity properties, resulting in significant performance improvements. 
        /// However, some environments do not support runtime JIT, so disabling optimization will allow
        /// FlatFiles to work.
        /// </summary>
        /// <param name="isOptimized">Specifies whether the mapping process should be optimized.</param>
        void OptimizeMapping(bool isOptimized = true);
    }

    internal sealed class SeparatedValueTypeMapper<TEntity>
        : ISeparatedValueTypeMapper<TEntity>,
        IDynamicSeparatedValueTypeMapper,
        IRecordMapper<TEntity>
    {
        private readonly Func<TEntity> factory;
        private readonly Dictionary<string, IPropertyMapping> mappingLookup;
        private readonly List<IPropertyMapping> mappings;
        private bool isOptimized;

        public SeparatedValueTypeMapper()
            : this(null)
        {
        }

        public SeparatedValueTypeMapper(Func<object> factory)
            : this(() => (TEntity)factory())
        {
        }

        public SeparatedValueTypeMapper(Func<TEntity> factory)
        {
            this.factory = factory;
            this.mappingLookup = new Dictionary<string, IPropertyMapping>();
            this.mappings = new List<IPropertyMapping>();
            this.isOptimized = true;
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

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getSByteMapping(propertyInfo);
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getSByteMapping(propertyInfo);
        }

        private ISBytePropertyMapping getSByteMapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                SByteColumn column = new SByteColumn(propertyInfo.Name);
                mapping = new SBytePropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (ISBytePropertyMapping)mapping;
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

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getUInt16Mapping(propertyInfo);
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getUInt16Mapping(propertyInfo);
        }

        private IUInt16PropertyMapping getUInt16Mapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                UInt16Column column = new UInt16Column(propertyInfo.Name);
                mapping = new UInt16PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IUInt16PropertyMapping)mapping;
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

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getUInt32Mapping(propertyInfo);
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getUInt32Mapping(propertyInfo);
        }

        private IUInt32PropertyMapping getUInt32Mapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                UInt32Column column = new UInt32Column(propertyInfo.Name);
                mapping = new UInt32PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IUInt32PropertyMapping)mapping;
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

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getUInt64Mapping(propertyInfo);
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> property)
        {
            PropertyInfo propertyInfo = getProperty(property);
            return getUInt64Mapping(propertyInfo);
        }

        private IUInt64PropertyMapping getUInt64Mapping(PropertyInfo propertyInfo)
        {
            IPropertyMapping mapping;
            if (!mappingLookup.TryGetValue(propertyInfo.Name, out mapping))
            {
                UInt64Column column = new UInt64Column(propertyInfo.Name);
                mapping = new UInt64PropertyMapping(column, propertyInfo);
                mappings.Add(mapping);
                mappingLookup.Add(propertyInfo.Name, mapping);
            }
            return (IUInt64PropertyMapping)mapping;
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
                throw new ArgumentException(SharedResources.BadPropertySelector, "property");
            }
            PropertyInfo propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(SharedResources.BadPropertySelector, "property");
            }
            if (!propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
            {
                throw new ArgumentException(SharedResources.BadPropertySelector, "property");
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
            var factory = getLateBoundFactory();
            var codeGenerator = getCodeGenerator();
            var deserializer = new TypedRecordReader<TEntity>(factory, codeGenerator, this.mappings);
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
            var codeGenerator = getCodeGenerator();
            var serializer = new TypedRecordWriter<TEntity>(codeGenerator, this.mappings);
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
            var factory = getLateBoundFactory();
            var codeGenerator = getCodeGenerator();
            return new TypedRecordReader<TEntity>(factory, codeGenerator, this.mappings);
        }

        public TypedRecordWriter<TEntity> GetWriter()
        {
            var codeGenerator = getCodeGenerator();
            return new TypedRecordWriter<TEntity>(codeGenerator, this.mappings);
        }

        SeparatedValueSchema IDynamicSeparatedValueTypeConfiguration.GetSchema()
        {
            return GetSchema();
        }

        IBooleanPropertyMapping IDynamicSeparatedValueTypeConfiguration.BooleanProperty(string propertyName)
        {
            var property = getProperty<bool?>(propertyName);
            return getBooleanMapping(property);
        }

        IByteArrayPropertyMapping IDynamicSeparatedValueTypeConfiguration.ByteArrayProperty(string propertyName)
        {
            var property = getProperty<byte[]>(propertyName);
            return getByteArrayMapping(property);
        }

        IBytePropertyMapping IDynamicSeparatedValueTypeConfiguration.ByteProperty(string propertyName)
        {
            var property = getProperty<byte?>(propertyName);
            return getByteMapping(property);
        }

        ISBytePropertyMapping IDynamicSeparatedValueTypeConfiguration.SByteProperty(string propertyName)
        {
            var property = getProperty<sbyte?>(propertyName);
            return getSByteMapping(property);
        }

        ICharArrayPropertyMapping IDynamicSeparatedValueTypeConfiguration.CharArrayProperty(string propertyName)
        {
            var property = getProperty<char[]>(propertyName);
            return getCharArrayMapping(property);
        }

        ICharPropertyMapping IDynamicSeparatedValueTypeConfiguration.CharProperty(string propertyName)
        {
            var property = getProperty<char?>(propertyName);
            return getCharMapping(property);
        }

        IDateTimePropertyMapping IDynamicSeparatedValueTypeConfiguration.DateTimeProperty(string propertyName)
        {
            var property = getProperty<DateTime?>(propertyName);
            return getDateTimeMapping(property);
        }

        IDecimalPropertyMapping IDynamicSeparatedValueTypeConfiguration.DecimalProperty(string propertyName)
        {
            var property = getProperty<decimal?>(propertyName);
            return getDecimalMapping(property);
        }

        IDoublePropertyMapping IDynamicSeparatedValueTypeConfiguration.DoubleProperty(string propertyName)
        {
            var property = getProperty<double?>(propertyName);
            return getDoubleMapping(property);
        }

        IGuidPropertyMapping IDynamicSeparatedValueTypeConfiguration.GuidProperty(string propertyName)
        {
            var property = getProperty<Guid?>(propertyName);
            return getGuidMapping(property);
        }

        IInt16PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int16Property(string propertyName)
        {
            var property = getProperty<short?>(propertyName);
            return getInt16Mapping(property);
        }

        IUInt16PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt16Property(string propertyName)
        {
            var property = getProperty<ushort?>(propertyName);
            return getUInt16Mapping(property);
        }

        IInt32PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int32Property(string propertyName)
        {
            var property = getProperty<int?>(propertyName);
            return getInt32Mapping(property);
        }

        IUInt32PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt32Property(string propertyName)
        {
            var property = getProperty<uint?>(propertyName);
            return getUInt32Mapping(property);
        }

        IInt64PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int64Property(string propertyName)
        {
            var property = getProperty<long?>(propertyName);
            return getInt64Mapping(property);
        }

        IUInt64PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt64Property(string propertyName)
        {
            var property = getProperty<ulong?>(propertyName);
            return getUInt64Mapping(property);
        }

        ISinglePropertyMapping IDynamicSeparatedValueTypeConfiguration.SingleProperty(string propertyName)
        {
            var property = getProperty<float?>(propertyName);
            return getSingleMapping(property);
        }

        IStringPropertyMapping IDynamicSeparatedValueTypeConfiguration.StringProperty(string propertyName)
        {
            var property = getProperty<string>(propertyName);
            return getStringMapping(property);
        }

        ISeparatedValueComplexPropertyMapping IDynamicSeparatedValueTypeConfiguration.ComplexProperty<TProp>(string propertyName, ISeparatedValueTypeMapper<TProp> mapper)
        {
            var property = getProperty<string>(propertyName);
            return getComplexMapping(property, mapper);
        }

        IFixedLengthComplexPropertyMapping IDynamicSeparatedValueTypeConfiguration.ComplexProperty<TProp>(string propertyName, IFixedLengthTypeMapper<TProp> mapper)
        {
            var property = getProperty<string>(propertyName);
            return getComplexMapping(property, mapper);
        }

        IEnumPropertyMapping<TEnum> IDynamicSeparatedValueTypeConfiguration.EnumProperty<TEnum>(string propertyName)
        {
            var property = getProperty<TEnum?>(propertyName);
            return getEnumMapping<TEnum>(property);
        }

        IIgnoredMapping IDynamicSeparatedValueTypeConfiguration.Ignored()
        {
            return Ignored();
        }

        private static PropertyInfo getProperty<TProp>(string propertyName)
        {
            var propertyInfo = typeof(TEntity).GetTypeInfo().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException(SharedResources.BadPropertySelector, "property");
            }
            if (!propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
            {
                throw new ArgumentException(SharedResources.BadPropertySelector, "property");
            }
            if (propertyInfo.PropertyType != typeof(TProp) && propertyInfo.PropertyType != Nullable.GetUnderlyingType(typeof(TProp)))
            {
                throw new ArgumentException(SharedResources.WrongPropertyType);
            }
            return propertyInfo;
        }

        IEnumerable<object> IDynamicSeparatedValueTypeMapper.Read(TextReader reader, SeparatedValueOptions options)
        {
            return (IEnumerable<object>)Read(reader, options);
        }

        ITypedReader<object> IDynamicSeparatedValueTypeMapper.GetReader(TextReader reader, SeparatedValueOptions options)
        {
            return (ITypedReader<object>)GetReader(reader, options);
        }

        void IDynamicSeparatedValueTypeMapper.Write(TextWriter writer, IEnumerable<object> entities, SeparatedValueOptions options)
        {
            var converted = entities.Cast<TEntity>();
            Write(writer, converted, options);
        }

        ITypedWriter<object> IDynamicSeparatedValueTypeMapper.GetWriter(TextWriter writer, SeparatedValueOptions options)
        {
            return new UntypedWriter<TEntity>(GetWriter(writer, options));
        }

        public void OptimizeMapping(bool isOptimized = true)
        {
            this.isOptimized = isOptimized;
        }

        void IDynamicSeparatedValueTypeMapper.OptimizeMapping(bool isOptimized)
        {
            OptimizeMapping(isOptimized);
        }

        private Func<TEntity> getLateBoundFactory()
        {
            if (factory == null)
            {
                var codeGenerator = getCodeGenerator();
                var factory = codeGenerator.GetFactory(typeof(TEntity));
                return () => (TEntity)factory();
            }
            else
            {
                return factory;
            }
        }

        private ICodeGenerator getCodeGenerator()
        {
            return isOptimized 
                ? (ICodeGenerator)new EmitCodeGenerator() 
                : new ReflectionCodeGenerator();
        }
    }
}
