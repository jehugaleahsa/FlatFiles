using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides methods for creating type mappers.
    /// </summary>
    public static class FixedLengthTypeMapper
    {
        /// <summary>
        /// Creates a configuration object that can be used to map to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <returns>The configuration object.</returns>
        public static IFixedLengthTypeMapper<TEntity> Define<TEntity>()
            where TEntity : new()
        {
            return new FixedLengthTypeMapper<TEntity>(() => new TEntity());
        }

        /// <summary>
        /// Creates a configuration object that can be used to map to and from an entity and a flat file record.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity whose properties will be mapped.</typeparam>
        /// <param name="factory">A method that generates an instance of the entity.</param>
        /// <returns>The configuration object.</returns>
        public static IFixedLengthTypeMapper<TEntity> Define<TEntity>(Func<TEntity> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            return new FixedLengthTypeMapper<TEntity>(factory);
        }

        /// <summary>
        /// Creates a configuration object that can be used to map to and from a runtime entity and a flat file record.
        /// </summary>
        /// <param name="entityType">The type of the entity whose properties will be mapped.</param>
        /// <returns>The configuration object.</returns>
        /// <remarks>The entity type must have a default constructor.</remarks>
        public static IDynamicFixedLengthTypeMapper DefineDynamic(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            var mapperType = typeof(FixedLengthTypeMapper<>).MakeGenericType(entityType);
            var mapper = Activator.CreateInstance(mapperType);
            return (IDynamicFixedLengthTypeMapper)mapper;
        }

        /// <summary>
        /// Creates a configuration object that can be used to map to and from a runtime entity and a flat file record.
        /// </summary>
        /// <param name="entityType">The type of the entity whose properties will be mapped.</param>
        /// <param name="factory">A method that generates an instance of the entity.</param>
        /// <returns>The configuration object.</returns>
        /// <remarks>The entity type must have a default constructor.</remarks>
        public static IDynamicFixedLengthTypeMapper DefineDynamic(Type entityType, Func<object> factory)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            var mapperType = typeof(FixedLengthTypeMapper<>).MakeGenericType(entityType);
            var mapper = Activator.CreateInstance(mapperType, factory);
            return (IDynamicFixedLengthTypeMapper)mapper;
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
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping Property(Expression<Func<TEntity, char?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> accessor, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping Property(Expression<Func<TEntity, string>> accessor, Window window);
        
        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="accessor">An expression tha returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, ISeparatedValueTypeMapper<TProp> mapper, Window window);
        
        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="accessor">An expression tha returns the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IFixedLengthTypeMapper<TProp> mapper, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor, Window window) where TEnum : Enum;

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor, Window window) where TEnum : struct, Enum;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored(Window window);
        /// <summary>
        /// Specifies the next column will be mapped using custom functions.
        /// </summary>
        /// <param name="column">The custom column definition for parsing and formatting the column.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the custom mapping.</returns>
        ICustomMapping<TEntity> CustomMapping(IColumnDefinition column, Window window);

        /// <summary>
        /// When optimized (the default), mappers will use System.Reflection.Emit to generate 
        /// code to get and set entity properties, resulting in significant performance improvements. 
        /// However, some environments do not support runtime JIT, so disabling optimization will allow
        /// FlatFiles to work.
        /// </summary>
        /// <param name="isOptimized">Specifies whether the mapping process should be optimized.</param>
        void OptimizeMapping(bool isOptimized = true);

        /// <summary>
        /// Specifies a different factory method to use when initializing nested members.
        /// </summary>
        /// <typeparam name="TOther">The type of the entity created by the factory.</typeparam>
        /// <param name="factory">A method that generates an instance of the entity.</param>
        void UseFactory<TOther>(Func<TOther> factory);
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
        IFixedLengthTypedReader<TEntity> GetReader(TextReader reader, FixedLengthOptions options = null);

        /// <summary>
        /// Writes the given entities to the given writer.
        /// </summary>
        /// <param name="writer">A writer over the fixed-length document.</param>
        /// <param name="entities">The entities to write to the document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        void Write(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null);

        /// <summary>
        /// Writes the given entities to the given writer.
        /// </summary>
        /// <param name="writer">A writer over the fixed-length document.</param>
        /// <param name="entities">The entities to write to the document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        Task WriteAsync(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null);

        /// <summary>
        /// Gets a typed writer to write entities to the underlying document.
        /// </summary>
        /// <param name="writer">The writer over the fixed-length document.</param>
        /// <param name="options">The options controlling how the fixed-length document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<TEntity> GetWriter(TextWriter writer, FixedLengthOptions options = null);
    }

    /// <summary>
    /// Supports runtime configuration for mapping between runtime type entity properties and flat file columns.
    /// </summary>
    public interface IDynamicFixedLengthTypeConfiguration
    {
        /// <summary>
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        FixedLengthSchema GetSchema();

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBooleanPropertyMapping BooleanProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IByteArrayPropertyMapping ByteArrayProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IBytePropertyMapping ByteProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISBytePropertyMapping SByteProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharArrayPropertyMapping CharArrayProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICharPropertyMapping CharProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimePropertyMapping DateTimeProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimeOffsetPropertyMapping DateTimeOffsetProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDecimalPropertyMapping DecimalProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDoublePropertyMapping DoubleProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IGuidPropertyMapping GuidProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt16PropertyMapping Int16Property(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt16PropertyMapping UInt16Property(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt32PropertyMapping Int32Property(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt32PropertyMapping UInt32Property(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IInt64PropertyMapping Int64Property(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IUInt64PropertyMapping UInt64Property(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISinglePropertyMapping SingleProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IStringPropertyMapping StringProperty(string memberName, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(string memberName, ISeparatedValueTypeMapper<TProp> mapper, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="mapper">A type mapper describing the schema of the complex type.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(string memberName, IFixedLengthTypeMapper<TProp> mapper, Window window);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(string memberName, Window window) where TEnum : struct, Enum;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored(Window window);

        /// <summary>
        /// Specifies the next column will be mapped using custom functions.
        /// </summary>
        /// <param name="column">The custom column definition for parsing and formatting the column.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the custom mapping.</returns>
        ICustomMapping CustomMapping(IColumnDefinition column, Window window);

        /// <summary>
        /// When optimized (the default), mappers will use System.Reflection.Emit to generate 
        /// code to get and set entity properties, resulting in significant performance improvements. 
        /// However, some environments do not support runtime JIT, so disabling optimization will allow
        /// FlatFiles to work.
        /// </summary>
        /// <param name="isOptimized">Specifies whether the mapping process should be optimized.</param>
        void OptimizeMapping(bool isOptimized = true);

        /// <summary>
        /// Specifies a different factory method to use when initializing nested members.
        /// </summary>
        /// <param name="entityType">
        /// The type of the entity to associate the factory with. The factory must return instances of that type.
        /// </param>
        /// <param name="factory">A method that generates an instance of the entity.</param>
        void UseFactory(Type entityType, Func<object> factory);
    }

    /// <summary>
    /// Supports reading to and writing from flat files for a runtime type. 
    /// </summary>
    public interface IDynamicFixedLengthTypeMapper : IDynamicFixedLengthTypeConfiguration
    {
        /// <summary>
        /// Reads the entities from the given reader.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<object> Read(TextReader reader, FixedLengthOptions options = null);

        /// <summary>
        /// Gets a typed reader to read entities from the underlying document.
        /// </summary>
        /// <param name="reader">A reader over the separated value document.</param>
        /// <param name="options">The options controlling how the separated value document is read.</param>
        /// <returns>A typed reader.</returns>
        IFixedLengthTypedReader<object> GetReader(TextReader reader, FixedLengthOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        void Write(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        Task WriteAsync(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions options = null);

        /// <summary>
        /// Gets a typed writer to write entities to the underlying document.
        /// </summary>
        /// <param name="writer">The writer over the fixed-length document.</param>
        /// <param name="options">The options controlling how the separated value document is written.</param>
        /// <returns>A typed writer.</returns>
        ITypedWriter<object> GetWriter(TextWriter writer, FixedLengthOptions options = null);
    }

    internal sealed class FixedLengthTypeMapper<TEntity> 
        : IFixedLengthTypeMapper<TEntity>, 
        IDynamicFixedLengthTypeMapper,
        IMapperSource<TEntity>
    {
        private readonly MemberLookup lookup = new MemberLookup();
        private readonly Dictionary<IMemberMapping, Window> windowLookup = new Dictionary<IMemberMapping, Window>();
        private bool isOptimized = true;

        public FixedLengthTypeMapper()
            : this(null)
        {
        }

        public FixedLengthTypeMapper(Func<object> factory)
            : this(() => (TEntity)factory())
        {
        }

        public FixedLengthTypeMapper(Func<TEntity> factory)
        {
            if (factory != null)
            {
                lookup.SetFactory(factory);
            }
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetBooleanMapping(member, window, false);
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetBooleanMapping(member, window, true);
        }

        private IBooleanPropertyMapping GetBooleanMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                BooleanColumn column = new BooleanColumn(member.Name) { IsNullable = isNullable };
                return new BooleanPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetByteArrayMapping(member, window);
        }

        private IByteArrayPropertyMapping GetByteArrayMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                ByteArrayColumn column = new ByteArrayColumn(member.Name);
                return new ByteArrayPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetByteMapping(member, window, false);
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetByteMapping(member, window, true);
        }

        private IBytePropertyMapping GetByteMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                ByteColumn column = new ByteColumn(member.Name) { IsNullable = isNullable };
                return new BytePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetSByteMapping(member, window, false);
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetSByteMapping(member, window, true);
        }

        private ISBytePropertyMapping GetSByteMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                SByteColumn column = new SByteColumn(member.Name) { IsNullable = isNullable };
                return new SBytePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetCharArrayMapping(member, window);
        }

        private ICharArrayPropertyMapping GetCharArrayMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                CharArrayColumn column = new CharArrayColumn(member.Name);
                return new CharArrayPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetCharMapping(member, window, false);
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetCharMapping(member, window, true);
        }

        private ICharPropertyMapping GetCharMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                CharColumn column = new CharColumn(member.Name) { IsNullable = isNullable };
                return new CharPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDateTimeMapping(member, window, false);
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDateTimeMapping(member, window, true);
        }

        private IDateTimePropertyMapping GetDateTimeMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DateTimeColumn column = new DateTimeColumn(member.Name) { IsNullable = isNullable };
                return new DateTimePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDateTimeOffsetMapping(member, window, false);
        }

        public IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDateTimeOffsetMapping(member, window, true);
        }

        private IDateTimeOffsetPropertyMapping GetDateTimeOffsetMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new DateTimeOffsetColumn(member.Name) { IsNullable = isNullable };
                return new DateTimeOffsetPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDecimalMapping(member, window, false);
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDecimalMapping(member, window, true);
        }

        private IDecimalPropertyMapping GetDecimalMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DecimalColumn column = new DecimalColumn(member.Name) { IsNullable = isNullable };
                return new DecimalPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDoubleMapping(member, window, false);
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetDoubleMapping(member, window, true);
        }

        private IDoublePropertyMapping GetDoubleMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DoubleColumn column = new DoubleColumn(member.Name) { IsNullable = isNullable };
                return new DoublePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetGuidMapping(member, window, false);
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetGuidMapping(member, window, true);
        }

        private IGuidPropertyMapping GetGuidMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                GuidColumn column = new GuidColumn(member.Name) { IsNullable = isNullable };
                return new GuidPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetInt16Mapping(member, window, false);
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetInt16Mapping(member, window, true);
        }

        private IInt16PropertyMapping GetInt16Mapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int16Column column = new Int16Column(member.Name) { IsNullable = isNullable };
                return new Int16PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetUInt16Mapping(member, window, false);
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetUInt16Mapping(member, window, true);
        }

        private IUInt16PropertyMapping GetUInt16Mapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt16Column column = new UInt16Column(member.Name) { IsNullable = isNullable };
                return new UInt16PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetInt32Mapping(member, window, false);
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetInt32Mapping(member, window, true);
        }

        private IInt32PropertyMapping GetInt32Mapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int32Column column = new Int32Column(member.Name) { IsNullable = isNullable };
                return new Int32PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetUInt32Mapping(member, window, false);
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetUInt32Mapping(member, window, true);
        }

        private IUInt32PropertyMapping GetUInt32Mapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt32Column column = new UInt32Column(member.Name) { IsNullable = isNullable };
                return new UInt32PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetInt64Mapping(member, window, false);
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetInt64Mapping(member, window, true);
        }

        private IInt64PropertyMapping GetInt64Mapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int64Column column = new Int64Column(member.Name) { IsNullable = isNullable };
                return new Int64PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetUInt64Mapping(member, window, false);
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetUInt64Mapping(member, window, true);
        }

        private IUInt64PropertyMapping GetUInt64Mapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt64Column column = new UInt64Column(member.Name) { IsNullable = isNullable };
                return new UInt64PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetSingleMapping(member, window, false);
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetSingleMapping(member, window, true);
        }

        private ISinglePropertyMapping GetSingleMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                SingleColumn column = new SingleColumn(member.Name) { IsNullable = isNullable };
                return new SinglePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string>> accesor, Window window)
        {
            var member = GetMember(accesor);
            return GetStringMapping(member, window);
        }

        private IStringPropertyMapping GetStringMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                StringColumn column = new StringColumn(member.Name);
                return new StringPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            var member = GetMember(accessor);
            return GetComplexMapping(member, mapper, window);
        }

        private ISeparatedValueComplexPropertyMapping GetComplexMapping<TProp>(IMemberAccessor member, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) => new SeparatedValueComplexPropertyMapping<TProp>(mapper, member, fileIndex, workIndex));
            windowLookup[mapping] = window;
            return mapping;
        }

        public IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            var member = GetMember(accessor);
            return GetComplexMapping(member, mapper, window);
        }

        private IFixedLengthComplexPropertyMapping GetComplexMapping<TProp>(IMemberAccessor member, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) => new FixedLengthComplexPropertyMapping<TProp>(mapper, member, fileIndex, workIndex));
            windowLookup[mapping] = window;
            return mapping;
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor, Window window) 
            where TEnum : Enum
        {
            var member = GetMember(accessor);
            return GetEnumMapping<TEnum>(member, window, false);
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor, Window window)
            where TEnum : struct, Enum
        {
            var member = GetMember(accessor);
            return GetEnumMapping<TEnum>(member, window, true);
        }

        private IEnumPropertyMapping<TEnum> GetEnumMapping<TEnum>(IMemberAccessor member, Window window, bool isNullable)
            where TEnum : Enum
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new EnumColumn<TEnum>(member.Name) { IsNullable = isNullable };
                return new EnumPropertyMapping<TEnum>(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IIgnoredMapping Ignored(Window window)
        {
            var mapping = lookup.AddIgnored();
            windowLookup[mapping] = window;
            return mapping;
        }

        public ICustomMapping<TEntity> CustomMapping(IColumnDefinition column, Window window)
        {
            var mapping = lookup.GetOrAddCustomMapping(column.ColumnName, (fileIndex, workIndex) => new CustomMapping<TEntity>(column, fileIndex, workIndex));
            windowLookup[mapping] = window;
            return mapping;
        }

        private static IMemberAccessor GetMember<TProp>(Expression<Func<TEntity, TProp>> accessor)
        {
            return MemberAccessorBuilder.GetMember(accessor);
        }

        public IEnumerable<TEntity> Read(TextReader reader, FixedLengthOptions options = null)
        {
            var schema = getSchema();
            var fixedLengthReader = new FixedLengthReader(reader, schema, options);
            return Read(fixedLengthReader);
        }

        private IEnumerable<TEntity> Read(FixedLengthReader reader)
        {
            var typedReader = GetTypedReader(reader);
            while (typedReader.Read())
            {
                yield return typedReader.Current;
            }
        }

        public IFixedLengthTypedReader<TEntity> GetReader(TextReader reader, FixedLengthOptions options = null)
        {
            var schema = getSchema();
            var fixedLengthReader = new FixedLengthReader(reader, schema, options);
            return GetTypedReader(fixedLengthReader);
        }

        private FixedLengthTypedReader<TEntity> GetTypedReader(FixedLengthReader reader)
        {
            var mapper = new Mapper<TEntity>(lookup, GetCodeGenerator());
            return new FixedLengthTypedReader<TEntity>(reader, mapper);
        }

        public void Write(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var schema = getSchema();
            var fixedLengthWriter = new FixedLengthWriter(writer, schema, options);
            Write(fixedLengthWriter, entities);
        }

        private void Write(IWriterWithMetadata writer, IEnumerable<TEntity> entities)
        {
            var typedWriter = GetTypedWriter(writer);
            foreach (TEntity entity in entities)
            {
                typedWriter.Write(entity);
            }
        }

        public async Task WriteAsync(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var schema = getSchema();
            var fixedLengthWriter = new FixedLengthWriter(writer, schema, options);
            await WriteAsync(fixedLengthWriter, entities).ConfigureAwait(false);
        }

        private async Task WriteAsync(IWriterWithMetadata writer, IEnumerable<TEntity> entities)
        {
            var typedWriter = GetTypedWriter(writer);
            foreach (TEntity entity in entities)
            {
                await typedWriter.WriteAsync(entity).ConfigureAwait(false);
            }
        }

        public ITypedWriter<TEntity> GetWriter(TextWriter writer, FixedLengthOptions options = null)
        {
            var schema = getSchema();
            var fixedLengthWriter = new FixedLengthWriter(writer, schema, options);
            return GetTypedWriter(fixedLengthWriter);
        }

        private TypedWriter<TEntity> GetTypedWriter(IWriterWithMetadata writer)
        {
            var mapper = new Mapper<TEntity>(lookup, GetCodeGenerator());
            return new TypedWriter<TEntity>(writer, mapper);
        }

        public FixedLengthSchema GetSchema()
        {
            return getSchema();
        }

        private FixedLengthSchema getSchema()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            var mappings = lookup.GetMappings();
            foreach (IMemberMapping mapping in mappings)
            {
                IColumnDefinition column = mapping.ColumnDefinition;
                Window window = windowLookup[mapping];
                schema.AddColumn(column, window);
            }
            return schema;
        }

        FixedLengthSchema IDynamicFixedLengthTypeConfiguration.GetSchema()
        {
            return GetSchema();
        }

        IBooleanPropertyMapping IDynamicFixedLengthTypeConfiguration.BooleanProperty(string memberName, Window window)
        {
            var member = GetMember<bool?>(memberName);
            return GetBooleanMapping(member, window, IsNullable(member));
        }

        IByteArrayPropertyMapping IDynamicFixedLengthTypeConfiguration.ByteArrayProperty(string memberName, Window window)
        {
            var member = GetMember<byte[]>(memberName);
            return GetByteArrayMapping(member, window);
        }

        IBytePropertyMapping IDynamicFixedLengthTypeConfiguration.ByteProperty(string memberName, Window window)
        {
            var member = GetMember<byte?>(memberName);
            return GetByteMapping(member, window, IsNullable(member));
        }

        ISBytePropertyMapping IDynamicFixedLengthTypeConfiguration.SByteProperty(string memberName, Window window)
        {
            var member = GetMember<sbyte?>(memberName);
            return GetSByteMapping(member, window, IsNullable(member));
        }

        ICharArrayPropertyMapping IDynamicFixedLengthTypeConfiguration.CharArrayProperty(string memberName, Window window)
        {
            var member = GetMember<char[]>(memberName);
            return GetCharArrayMapping(member, window);
        }

        ICharPropertyMapping IDynamicFixedLengthTypeConfiguration.CharProperty(string memberName, Window window)
        {
            var member = GetMember<char?>(memberName);
            return GetCharMapping(member, window, IsNullable(member));
        }

        IDateTimePropertyMapping IDynamicFixedLengthTypeConfiguration.DateTimeProperty(string memberName, Window window)
        {
            var member = GetMember<DateTime?>(memberName);
            return GetDateTimeMapping(member, window, IsNullable(member));
        }

        IDateTimeOffsetPropertyMapping IDynamicFixedLengthTypeConfiguration.DateTimeOffsetProperty(string memberName, Window window)
        {
            var member = GetMember<DateTimeOffset?>(memberName);
            return GetDateTimeOffsetMapping(member, window, IsNullable(member));
        }

        IDecimalPropertyMapping IDynamicFixedLengthTypeConfiguration.DecimalProperty(string memberName, Window window)
        {
            var member = GetMember<decimal?>(memberName);
            return GetDecimalMapping(member, window, IsNullable(member));
        }

        IDoublePropertyMapping IDynamicFixedLengthTypeConfiguration.DoubleProperty(string memberName, Window window)
        {
            var member = GetMember<double?>(memberName);
            return GetDoubleMapping(member, window, IsNullable(member));
        }

        IGuidPropertyMapping IDynamicFixedLengthTypeConfiguration.GuidProperty(string memberName, Window window)
        {
            var member = GetMember<Guid?>(memberName);
            return GetGuidMapping(member, window, IsNullable(member));
        }

        IInt16PropertyMapping IDynamicFixedLengthTypeConfiguration.Int16Property(string memberName, Window window)
        {
            var member = GetMember<short?>(memberName);
            return GetInt16Mapping(member, window, IsNullable(member));
        }

        IUInt16PropertyMapping IDynamicFixedLengthTypeConfiguration.UInt16Property(string memberName, Window window)
        {
            var member = GetMember<ushort?>(memberName);
            return GetUInt16Mapping(member, window, IsNullable(member));
        }

        IInt32PropertyMapping IDynamicFixedLengthTypeConfiguration.Int32Property(string memberName, Window window)
        {
            var member = GetMember<int?>(memberName);
            return GetInt32Mapping(member, window, IsNullable(member));
        }

        IUInt32PropertyMapping IDynamicFixedLengthTypeConfiguration.UInt32Property(string memberName, Window window)
        {
            var member = GetMember<uint?>(memberName);
            return GetUInt32Mapping(member, window, IsNullable(member));
        }

        IInt64PropertyMapping IDynamicFixedLengthTypeConfiguration.Int64Property(string memberName, Window window)
        {
            var member = GetMember<long?>(memberName);
            return GetInt64Mapping(member, window, IsNullable(member));
        }

        IUInt64PropertyMapping IDynamicFixedLengthTypeConfiguration.UInt64Property(string memberName, Window window)
        {
            var member = GetMember<ulong?>(memberName);
            return GetUInt64Mapping(member, window, IsNullable(member));
        }

        ISinglePropertyMapping IDynamicFixedLengthTypeConfiguration.SingleProperty(string memberName, Window window)
        {
            var member = GetMember<float?>(memberName);
            return GetSingleMapping(member, window, IsNullable(member));
        }

        IStringPropertyMapping IDynamicFixedLengthTypeConfiguration.StringProperty(string memberName, Window window)
        {
            var member = GetMember<string>(memberName);
            return GetStringMapping(member, window);
        }

        ISeparatedValueComplexPropertyMapping IDynamicFixedLengthTypeConfiguration.ComplexProperty<TProp>(string memberName, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            var member = GetMember<string>(memberName);
            return GetComplexMapping(member, mapper, window);
        }

        IFixedLengthComplexPropertyMapping IDynamicFixedLengthTypeConfiguration.ComplexProperty<TProp>(string memberName, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            var member = GetMember<string>(memberName);
            return GetComplexMapping(member, mapper, window);
        }

        IEnumPropertyMapping<TEnum> IDynamicFixedLengthTypeConfiguration.EnumProperty<TEnum>(string memberName, Window window)
        {
            var member = GetMember<TEnum?>(memberName);
            return GetEnumMapping<TEnum>(member, window, IsNullable(member));
        }

        IIgnoredMapping IDynamicFixedLengthTypeConfiguration.Ignored(Window window)
        {
            return Ignored(window);
        }

        ICustomMapping IDynamicFixedLengthTypeConfiguration.CustomMapping(IColumnDefinition column, Window window)
        {
            var mapping = lookup.GetOrAddCustomMapping(column.ColumnName, (fileIndex, workIndex) => new CustomMapping<TEntity>(column, fileIndex, workIndex));
            windowLookup[mapping] = window;
            return mapping;
        }

        private static IMemberAccessor GetMember<TProp>(string memberName)
        {
            return MemberAccessorBuilder.GetMember<TEntity, TProp>(memberName);
        }

        private static bool IsNullable(IMemberAccessor accessor)
        {
            if (!accessor.Type.GetTypeInfo().IsValueType)
            {
                return true;
            }
            return Nullable.GetUnderlyingType(accessor.Type) != null;
        }

        IEnumerable<object> IDynamicFixedLengthTypeMapper.Read(TextReader reader, FixedLengthOptions options)
        {
            return (IEnumerable<object>)Read(reader, options);
        }

        IFixedLengthTypedReader<object> IDynamicFixedLengthTypeMapper.GetReader(TextReader reader, FixedLengthOptions options)
        {
            return (IFixedLengthTypedReader<object>)GetReader(reader, options);
        }

        void IDynamicFixedLengthTypeMapper.Write(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions options)
        {
            var converted = entities.Cast<TEntity>();
            Write(writer, converted, options);
        }

        async Task IDynamicFixedLengthTypeMapper.WriteAsync(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions options)
        {
            var converted = entities.Cast<TEntity>();
            await WriteAsync(writer, converted, options).ConfigureAwait(false);
        }

        ITypedWriter<object> IDynamicFixedLengthTypeMapper.GetWriter(TextWriter writer, FixedLengthOptions options)
        {
            return new UntypedWriter<TEntity>(GetWriter(writer, options));
        }

        public void OptimizeMapping(bool isOptimized = true)
        {
            this.isOptimized = isOptimized;
        }

        void IDynamicFixedLengthTypeConfiguration.OptimizeMapping(bool isOptimized)
        {
            OptimizeMapping(isOptimized);
        }

        public void UseFactory<TOther>(Func<TOther> factory)
        {
            lookup.SetFactory(factory);
        }

        void IDynamicFixedLengthTypeConfiguration.UseFactory(Type entityType, Func<object> factory)
        {
            lookup.SetFactory(entityType, factory);
        }

        public IMapper<TEntity> GetMapper()
        {
            return new Mapper<TEntity>(lookup, GetCodeGenerator());
        }

        IMapper IMapperSource.GetMapper()
        {
            return GetMapper();
        }

        private ICodeGenerator GetCodeGenerator()
        {
            return isOptimized 
                ? (ICodeGenerator)new EmitCodeGenerator() 
                : new ReflectionCodeGenerator();
        }
    }
}
