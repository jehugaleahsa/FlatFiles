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
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping Property(Expression<Func<TEntity, string>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="accessor">An expression tha returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, ISeparatedValueTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IFixedLengthTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor) where TEnum : struct;

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored();

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
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping BooleanProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping ByteArrayProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping ByteProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping SByteProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping CharArrayProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping CharProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping DateTimeProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping DecimalProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping DoubleProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping GuidProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Int16Property(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping UInt16Property(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Int32Property(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping UInt32Property(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Int64Property(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping UInt64Property(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping SingleProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping StringProperty(string memberName);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(string memberName, ISeparatedValueTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(string memberName, IFixedLengthTypeMapper<TProp> mapper);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(string memberName) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored();

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
    }

    internal sealed class SeparatedValueTypeMapper<TEntity>
        : ISeparatedValueTypeMapper<TEntity>,
        IDynamicSeparatedValueTypeMapper,
        IRecordMapper<TEntity>
    {
        private readonly Func<TEntity> factory;
        private readonly Dictionary<string, IMemberMapping> mappingLookup;
        private readonly List<IMemberMapping> mappings;
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
            this.mappingLookup = new Dictionary<string, IMemberMapping>();
            this.mappings = new List<IMemberMapping>();
            this.isOptimized = true;
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> accessor)
        {
            var member = getMember(accessor);
            return getBooleanMapping(member);
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> accessor)
        {
            var member = getMember(accessor);
            return getBooleanMapping(member);
        }

        private IBooleanPropertyMapping getBooleanMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                BooleanColumn column = new BooleanColumn(member.Name);
                mapping = new BooleanPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IBooleanPropertyMapping)mapping;
        }

        public IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> accessor)
        {
            var member = getMember(accessor);
            return getByteArrayMapping(member);
        }

        private IByteArrayPropertyMapping getByteArrayMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                ByteArrayColumn column = new ByteArrayColumn(member.Name);
                mapping = new ByteArrayPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);

            }
            return (IByteArrayPropertyMapping)mapping;
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte>> accessor)
        {
            var member = getMember(accessor);
            return getByteMapping(member);
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> accessor)
        {
            var member = getMember(accessor);
            return getByteMapping(member);
        }

        private IBytePropertyMapping getByteMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                ByteColumn column = new ByteColumn(member.Name);
                mapping = new BytePropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IBytePropertyMapping)mapping;
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> accessor)
        {
            var member = getMember(accessor);
            return getSByteMapping(member);
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> accessor)
        {
            var member = getMember(accessor);
            return getSByteMapping(member);
        }

        private ISBytePropertyMapping getSByteMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                SByteColumn column = new SByteColumn(member.Name);
                mapping = new SBytePropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (ISBytePropertyMapping)mapping;
        }

        public ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> accessor)
        {
            var member = getMember(accessor);
            return getCharArrayMapping(member);
        }

        private ICharArrayPropertyMapping getCharArrayMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                CharArrayColumn column = new CharArrayColumn(member.Name);
                mapping = new CharArrayPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (ICharArrayPropertyMapping)mapping;
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char>> accessor)
        {
            var member = getMember(accessor);
            return getCharMapping(member);
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char?>> accessor)
        {
            var member = getMember(accessor);
            return getCharMapping(member);
        }

        private ICharPropertyMapping getCharMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                CharColumn column = new CharColumn(member.Name);
                mapping = new CharPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (ICharPropertyMapping)mapping;
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> accessor)
        {
            var member = getMember(accessor);
            return getDateTimeMapping(member);
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> accessor)
        {
            var member = getMember(accessor);
            return getDateTimeMapping(member);
        }

        private IDateTimePropertyMapping getDateTimeMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                DateTimeColumn column = new DateTimeColumn(member.Name);
                mapping = new DateTimePropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IDateTimePropertyMapping)mapping;
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> accessor)
        {
            var member = getMember(accessor);
            return getDecimalMapping(member);
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> accessor)
        {
            var member = getMember(accessor);
            return getDecimalMapping(member);
        }

        private IDecimalPropertyMapping getDecimalMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                DecimalColumn column = new DecimalColumn(member.Name);
                mapping = new DecimalPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IDecimalPropertyMapping)mapping;
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double>> accessor)
        {
            var member = getMember(accessor);
            return getDoubleMapping(member);
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> accessor)
        {
            var member = getMember(accessor);
            return getDoubleMapping(member);
        }

        private IDoublePropertyMapping getDoubleMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                DoubleColumn column = new DoubleColumn(member.Name);
                mapping = new DoublePropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IDoublePropertyMapping)mapping;
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> accessor)
        {
            var member = getMember(accessor);
            return getGuidMapping(member);
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> accessor)
        {
            var member = getMember(accessor);
            return getGuidMapping(member);
        }

        private IGuidPropertyMapping getGuidMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                GuidColumn column = new GuidColumn(member.Name);
                mapping = new GuidPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IGuidPropertyMapping)mapping;
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short>> accessor)
        {
            var member = getMember(accessor);
            return getInt16Mapping(member);
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> accessor)
        {
            var member = getMember(accessor);
            return getInt16Mapping(member);
        }

        private IInt16PropertyMapping getInt16Mapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                Int16Column column = new Int16Column(member.Name);
                mapping = new Int16PropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IInt16PropertyMapping)mapping;
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> accessor)
        {
            var member = getMember(accessor);
            return getUInt16Mapping(member);
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> accessor)
        {
            var member = getMember(accessor);
            return getUInt16Mapping(member);
        }

        private IUInt16PropertyMapping getUInt16Mapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                UInt16Column column = new UInt16Column(member.Name);
                mapping = new UInt16PropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IUInt16PropertyMapping)mapping;
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int>> accessor)
        {
            var member = getMember(accessor);
            return getInt32Mapping(member);
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> accessor)
        {
            var member = getMember(accessor);
            return getInt32Mapping(member);
        }

        private IInt32PropertyMapping getInt32Mapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                Int32Column column = new Int32Column(member.Name);
                mapping = new Int32PropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IInt32PropertyMapping)mapping;
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> accessor)
        {
            var member = getMember(accessor);
            return getUInt32Mapping(member);
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> accessor)
        {
            var member = getMember(accessor);
            return getUInt32Mapping(member);
        }

        private IUInt32PropertyMapping getUInt32Mapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                UInt32Column column = new UInt32Column(member.Name);
                mapping = new UInt32PropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IUInt32PropertyMapping)mapping;
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long>> accessor)
        {
            var member = getMember(accessor);
            return getInt64Mapping(member);
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> accessor)
        {
            var member = getMember(accessor);
            return getInt64Mapping(member);
        }

        private IInt64PropertyMapping getInt64Mapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                Int64Column column = new Int64Column(member.Name);
                mapping = new Int64PropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IInt64PropertyMapping)mapping;
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> accessor)
        {
            var member = getMember(accessor);
            return getUInt64Mapping(member);
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> accessor)
        {
            var member = getMember(accessor);
            return getUInt64Mapping(member);
        }

        private IUInt64PropertyMapping getUInt64Mapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                UInt64Column column = new UInt64Column(member.Name);
                mapping = new UInt64PropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IUInt64PropertyMapping)mapping;
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float>> accessor)
        {
            var member = getMember(accessor);
            return getSingleMapping(member);
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> accessor)
        {
            var member = getMember(accessor);
            return getSingleMapping(member);
        }

        private ISinglePropertyMapping getSingleMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                SingleColumn column = new SingleColumn(member.Name);
                mapping = new SinglePropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (ISinglePropertyMapping)mapping;
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string>> accessor)
        {
            var member = getMember(accessor);
            return getStringMapping(member);
        }

        private IStringPropertyMapping getStringMapping(IMemberAccessor member)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                StringColumn column = new StringColumn(member.Name);
                mapping = new StringPropertyMapping(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IStringPropertyMapping)mapping;
        }

        public ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, ISeparatedValueTypeMapper<TProp> mapper)
        {
            var member = getMember(accessor);
            return getComplexMapping(member, mapper);
        }

        private ISeparatedValueComplexPropertyMapping getComplexMapping<TProp>(IMemberAccessor member, ISeparatedValueTypeMapper<TProp> mapper)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                mapping = new SeparatedValueComplexPropertyMapping<TProp>(mapper, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (ISeparatedValueComplexPropertyMapping)mapping;
        }

        public IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IFixedLengthTypeMapper<TProp> mapper)
        {
            var member = getMember(accessor);
            return getComplexMapping(member, mapper);
        }

        private IFixedLengthComplexPropertyMapping getComplexMapping<TProp>(IMemberAccessor member, IFixedLengthTypeMapper<TProp> mapper)
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                mapping = new FixedLengthComplexPropertyMapping<TProp>(mapper, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
            }
            return (IFixedLengthComplexPropertyMapping)mapping;
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor)
            where TEnum : struct
        {
            var member = getMember(accessor);
            return getEnumMapping<TEnum>(member);
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor)
            where TEnum : struct
        {
            var member = getMember(accessor);
            return getEnumMapping<TEnum>(member);
        }

        private IEnumPropertyMapping<TEnum> getEnumMapping<TEnum>(IMemberAccessor member)
            where TEnum : struct
        {
            IMemberMapping mapping;
            if (!mappingLookup.TryGetValue(member.Name, out mapping))
            {
                var column = new EnumColumn<TEnum>(member.Name);
                mapping = new EnumPropertyMapping<TEnum>(column, member);
                mappings.Add(mapping);
                mappingLookup.Add(member.Name, mapping);
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

        private static IMemberAccessor getMember<TProp>(Expression<Func<TEntity, TProp>> accessor)
        {
            if (accessor == null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }
            MemberExpression member = accessor.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(SharedResources.BadPropertySelector, nameof(accessor));
            }
            if (member.Member is PropertyInfo propertyInfo)
            {
                if (!propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    throw new ArgumentException(SharedResources.BadPropertySelector, nameof(accessor));
                }
                return new PropertyAccessor(propertyInfo);
            }
            else if (member.Member is FieldInfo fieldInfo)
            {
                if (!fieldInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    throw new ArgumentException(SharedResources.BadPropertySelector, nameof(accessor));
                }
                return new FieldAccessor(fieldInfo);
            }
            else
            {
                throw new ArgumentException(SharedResources.BadPropertySelector, nameof(accessor));
            }
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
            foreach (IMemberMapping mapping in mappings)
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

        IBooleanPropertyMapping IDynamicSeparatedValueTypeConfiguration.BooleanProperty(string memberName)
        {
            var member = getMember<bool?>(memberName);
            return getBooleanMapping(member);
        }

        IByteArrayPropertyMapping IDynamicSeparatedValueTypeConfiguration.ByteArrayProperty(string memberName)
        {
            var member = getMember<byte[]>(memberName);
            return getByteArrayMapping(member);
        }

        IBytePropertyMapping IDynamicSeparatedValueTypeConfiguration.ByteProperty(string memberName)
        {
            var member = getMember<byte?>(memberName);
            return getByteMapping(member);
        }

        ISBytePropertyMapping IDynamicSeparatedValueTypeConfiguration.SByteProperty(string memberName)
        {
            var member = getMember<sbyte?>(memberName);
            return getSByteMapping(member);
        }

        ICharArrayPropertyMapping IDynamicSeparatedValueTypeConfiguration.CharArrayProperty(string memberName)
        {
            var member = getMember<char[]>(memberName);
            return getCharArrayMapping(member);
        }

        ICharPropertyMapping IDynamicSeparatedValueTypeConfiguration.CharProperty(string memberName)
        {
            var member = getMember<char?>(memberName);
            return getCharMapping(member);
        }

        IDateTimePropertyMapping IDynamicSeparatedValueTypeConfiguration.DateTimeProperty(string memberName)
        {
            var member = getMember<DateTime?>(memberName);
            return getDateTimeMapping(member);
        }

        IDecimalPropertyMapping IDynamicSeparatedValueTypeConfiguration.DecimalProperty(string memberName)
        {
            var member = getMember<decimal?>(memberName);
            return getDecimalMapping(member);
        }

        IDoublePropertyMapping IDynamicSeparatedValueTypeConfiguration.DoubleProperty(string memberName)
        {
            var member = getMember<double?>(memberName);
            return getDoubleMapping(member);
        }

        IGuidPropertyMapping IDynamicSeparatedValueTypeConfiguration.GuidProperty(string memberName)
        {
            var member = getMember<Guid?>(memberName);
            return getGuidMapping(member);
        }

        IInt16PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int16Property(string memberName)
        {
            var member = getMember<short?>(memberName);
            return getInt16Mapping(member);
        }

        IUInt16PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt16Property(string memberName)
        {
            var member = getMember<ushort?>(memberName);
            return getUInt16Mapping(member);
        }

        IInt32PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int32Property(string memberName)
        {
            var member = getMember<int?>(memberName);
            return getInt32Mapping(member);
        }

        IUInt32PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt32Property(string memberName)
        {
            var member = getMember<uint?>(memberName);
            return getUInt32Mapping(member);
        }

        IInt64PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int64Property(string memberName)
        {
            var member = getMember<long?>(memberName);
            return getInt64Mapping(member);
        }

        IUInt64PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt64Property(string memberName)
        {
            var member = getMember<ulong?>(memberName);
            return getUInt64Mapping(member);
        }

        ISinglePropertyMapping IDynamicSeparatedValueTypeConfiguration.SingleProperty(string memberName)
        {
            var member = getMember<float?>(memberName);
            return getSingleMapping(member);
        }

        IStringPropertyMapping IDynamicSeparatedValueTypeConfiguration.StringProperty(string memberName)
        {
            var member = getMember<string>(memberName);
            return getStringMapping(member);
        }

        ISeparatedValueComplexPropertyMapping IDynamicSeparatedValueTypeConfiguration.ComplexProperty<TProp>(string memberName, ISeparatedValueTypeMapper<TProp> mapper)
        {
            var member = getMember<string>(memberName);
            return getComplexMapping(member, mapper);
        }

        IFixedLengthComplexPropertyMapping IDynamicSeparatedValueTypeConfiguration.ComplexProperty<TProp>(string memberName, IFixedLengthTypeMapper<TProp> mapper)
        {
            var member = getMember<string>(memberName);
            return getComplexMapping(member, mapper);
        }

        IEnumPropertyMapping<TEnum> IDynamicSeparatedValueTypeConfiguration.EnumProperty<TEnum>(string memberName)
        {
            var member = getMember<TEnum?>(memberName);
            return getEnumMapping<TEnum>(member);
        }

        IIgnoredMapping IDynamicSeparatedValueTypeConfiguration.Ignored()
        {
            return Ignored();
        }

        private static IMemberAccessor getMember<TProp>(string memberName)
        {
            var propertyInfo = typeof(TEntity).GetTypeInfo().GetProperty(memberName);
            if (propertyInfo != null)
            {
                if (!propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    throw new ArgumentException(SharedResources.BadPropertySelector, nameof(memberName));
                }
                if (propertyInfo.PropertyType != typeof(TProp) && propertyInfo.PropertyType != Nullable.GetUnderlyingType(typeof(TProp)))
                {
                    throw new ArgumentException(SharedResources.WrongPropertyType);
                }
                return new PropertyAccessor(propertyInfo);
            }
            var fieldInfo = typeof(TEntity).GetTypeInfo().GetField(memberName);
            if (fieldInfo != null)
            {
                if (!fieldInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    throw new ArgumentException(SharedResources.BadPropertySelector, nameof(memberName));
                }
                if (fieldInfo.FieldType != typeof(TProp) && fieldInfo.FieldType != Nullable.GetUnderlyingType(typeof(TProp)))
                {
                    throw new ArgumentException(SharedResources.WrongPropertyType);
                }
                return new FieldAccessor(fieldInfo);
            }
            throw new ArgumentException(SharedResources.BadPropertySelector, nameof(memberName));
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

        void IDynamicSeparatedValueTypeConfiguration.OptimizeMapping(bool isOptimized)
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
