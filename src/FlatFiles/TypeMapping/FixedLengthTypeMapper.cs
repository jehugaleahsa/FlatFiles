using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FlatFiles.Resources;

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
                throw new ArgumentNullException("factory");
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
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor, Window window) where TEnum : struct;

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor, Window window) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored(Window window);

        /// <summary>
        /// Specifies that the next column is a custom definition and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TProp">The type of the property that the custom column definition parses and formats.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <param name="column">The custom column definition for parsing and formatting the column.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICustomPropertyMapping CustomProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IColumnDefinition column, Window window);

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
        ITypedReader<TEntity> GetReader(TextReader reader, FixedLengthOptions options = null);

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
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(string memberName, Window window) where TEnum : struct;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored(Window window);

        /// <summary>
        /// Specifies that the next column is a custom definition and returns an object for configuration.
        /// </summary>
        /// <param name="memberName">The name of the property to map.</param>
        /// <param name="column">The custom column definition for parsing and formatting the column.</param>
        /// <param name="window">Specifies how the fixed-width column appears in a flat file.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ICustomPropertyMapping CustomProperty(string memberName, IColumnDefinition column, Window window);

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
        ITypedReader<object> GetReader(TextReader reader, FixedLengthOptions options = null);

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
        private readonly MemberLookup lookup;
        private readonly Dictionary<IMemberMapping, Window> windowLookup;
        private bool isOptimized;

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
            this.lookup = new MemberLookup();
            if (factory != null)
            {
                lookup.SetFactory<TEntity>(factory);
            }
            this.windowLookup = new Dictionary<IMemberMapping, Window>();
            this.isOptimized = true;
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getBooleanMapping(member, window);
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getBooleanMapping(member, window);
        }

        private IBooleanPropertyMapping getBooleanMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                BooleanColumn column = new BooleanColumn(member.Name);
                return new BooleanPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getByteArrayMapping(member, window);
        }

        private IByteArrayPropertyMapping getByteArrayMapping(IMemberAccessor member, Window window)
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
            var member = getMember(accessor);
            return getByteMapping(member, window);
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getByteMapping(member, window);
        }

        private IBytePropertyMapping getByteMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                ByteColumn column = new ByteColumn(member.Name);
                return new BytePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getSByteMapping(member, window);
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getSByteMapping(member, window);
        }

        private ISBytePropertyMapping getSByteMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                SByteColumn column = new SByteColumn(member.Name);
                return new SBytePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getCharArrayMapping(member, window);
        }

        private ICharArrayPropertyMapping getCharArrayMapping(IMemberAccessor member, Window window)
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
            var member = getMember(accessor);
            return getCharMapping(member, window);
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getCharMapping(member, window);
        }

        private ICharPropertyMapping getCharMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                CharColumn column = new CharColumn(member.Name);
                return new CharPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getDateTimeMapping(member, window);
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getDateTimeMapping(member, window);
        }

        private IDateTimePropertyMapping getDateTimeMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DateTimeColumn column = new DateTimeColumn(member.Name);
                return new DateTimePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getDecimalMapping(member, window);
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getDecimalMapping(member, window);
        }

        private IDecimalPropertyMapping getDecimalMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DecimalColumn column = new DecimalColumn(member.Name);
                return new DecimalPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getDoubleMapping(member, window);
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getDoubleMapping(member, window);
        }

        private IDoublePropertyMapping getDoubleMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DoubleColumn column = new DoubleColumn(member.Name);
                return new DoublePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getGuidMapping(member, window);
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getGuidMapping(member, window);
        }

        private IGuidPropertyMapping getGuidMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                GuidColumn column = new GuidColumn(member.Name);
                return new GuidPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getInt16Mapping(member, window);
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getInt16Mapping(member, window);
        }

        private IInt16PropertyMapping getInt16Mapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int16Column column = new Int16Column(member.Name);
                return new Int16PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getUInt16Mapping(member, window);
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getUInt16Mapping(member, window);
        }

        private IUInt16PropertyMapping getUInt16Mapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt16Column column = new UInt16Column(member.Name);
                return new UInt16PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getInt32Mapping(member, window);
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getInt32Mapping(member, window);
        }

        private IInt32PropertyMapping getInt32Mapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int32Column column = new Int32Column(member.Name);
                return new Int32PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getUInt32Mapping(member, window);
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getUInt32Mapping(member, window);
        }

        private IUInt32PropertyMapping getUInt32Mapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt32Column column = new UInt32Column(member.Name);
                return new UInt32PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getInt64Mapping(member, window);
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getInt64Mapping(member, window);
        }

        private IInt64PropertyMapping getInt64Mapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int64Column column = new Int64Column(member.Name);
                return new Int64PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getUInt64Mapping(member, window);
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getUInt64Mapping(member, window);
        }

        private IUInt64PropertyMapping getUInt64Mapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt64Column column = new UInt64Column(member.Name);
                return new UInt64PropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getSingleMapping(member, window);
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> accessor, Window window)
        {
            var member = getMember(accessor);
            return getSingleMapping(member, window);
        }

        private ISinglePropertyMapping getSingleMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                SingleColumn column = new SingleColumn(member.Name);
                return new SinglePropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string>> accesor, Window window)
        {
            var member = getMember(accesor);
            return getStringMapping(member, window);
        }

        private IStringPropertyMapping getStringMapping(IMemberAccessor member, Window window)
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
            var member = getMember(accessor);
            return getComplexMapping(member, mapper, window);
        }

        private ISeparatedValueComplexPropertyMapping getComplexMapping<TProp>(IMemberAccessor member, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                return new SeparatedValueComplexPropertyMapping<TProp>(mapper, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            var member = getMember(accessor);
            return getComplexMapping(member, mapper, window);
        }

        private IFixedLengthComplexPropertyMapping getComplexMapping<TProp>(IMemberAccessor member, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                return new FixedLengthComplexPropertyMapping<TProp>(mapper, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor, Window window) 
            where TEnum : struct
        {
            var member = getMember(accessor);
            return getEnumMapping<TEnum>(member, window);
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor, Window window)
            where TEnum : struct
        {
            var member = getMember(accessor);
            return getEnumMapping<TEnum>(member, window);
        }

        private IEnumPropertyMapping<TEnum> getEnumMapping<TEnum>(IMemberAccessor member, Window window)
            where TEnum : struct
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new EnumColumn<TEnum>(member.Name);
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

        public ICustomPropertyMapping CustomProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IColumnDefinition column, Window window)
        {
            var member = getMember(accessor);
            return getCustomMapping(member, column, window);
        }

        private ICustomPropertyMapping getCustomMapping(IMemberAccessor member, IColumnDefinition column, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                return new CustomPropertyMapping(column, member, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IWriteOnlyPropertyMapping WriteOnlyProperty(string name, IColumnDefinition column, Window window)
        {
            return getWriteOnlyMapping(name, column, window);
        }

        private IWriteOnlyPropertyMapping getWriteOnlyMapping(string name, IColumnDefinition column, Window window)
        {
            var mapping = lookup.GetOrAddWriteOnlyMember(name, (fileIndex, workIndex) =>
            {
                return new WriteOnlyPropertyMapping(column, name, fileIndex, workIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        private static IMemberAccessor getMember<TProp>(Expression<Func<TEntity, TProp>> accessor)
        {
            return MemberAccessorBuilder.GetMember(accessor);
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
            var mapper = new Mapper<TEntity>(lookup, getCodeGenerator());
            return new TypedReader<TEntity>(reader, mapper);
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

        public async Task WriteAsync(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            FixedLengthSchema schema = getSchema();
            IWriter fixedLengthWriter = new FixedLengthWriter(writer, schema, options);
            await writeAsync(fixedLengthWriter, entities);
        }

        private async Task writeAsync(IWriter writer, IEnumerable<TEntity> entities)
        {
            TypedWriter<TEntity> typedWriter = getTypedWriter(writer);
            foreach (TEntity entity in entities)
            {
                await typedWriter.WriteAsync(entity);
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
            var mapper = new Mapper<TEntity>(lookup, getCodeGenerator());
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
            var member = getMember<bool?>(memberName);
            return getBooleanMapping(member, window);
        }

        IByteArrayPropertyMapping IDynamicFixedLengthTypeConfiguration.ByteArrayProperty(string memberName, Window window)
        {
            var member = getMember<byte[]>(memberName);
            return getByteArrayMapping(member, window);
        }

        IBytePropertyMapping IDynamicFixedLengthTypeConfiguration.ByteProperty(string memberName, Window window)
        {
            var member = getMember<byte?>(memberName);
            return getByteMapping(member, window);
        }

        ISBytePropertyMapping IDynamicFixedLengthTypeConfiguration.SByteProperty(string memberName, Window window)
        {
            var member = getMember<sbyte?>(memberName);
            return getSByteMapping(member, window);
        }

        ICharArrayPropertyMapping IDynamicFixedLengthTypeConfiguration.CharArrayProperty(string memberName, Window window)
        {
            var member = getMember<char[]>(memberName);
            return getCharArrayMapping(member, window);
        }

        ICharPropertyMapping IDynamicFixedLengthTypeConfiguration.CharProperty(string memberName, Window window)
        {
            var member = getMember<char?>(memberName);
            return getCharMapping(member, window);
        }

        IDateTimePropertyMapping IDynamicFixedLengthTypeConfiguration.DateTimeProperty(string memberName, Window window)
        {
            var member = getMember<DateTime?>(memberName);
            return getDateTimeMapping(member, window);
        }

        IDecimalPropertyMapping IDynamicFixedLengthTypeConfiguration.DecimalProperty(string memberName, Window window)
        {
            var member = getMember<decimal?>(memberName);
            return getDecimalMapping(member, window);
        }

        IDoublePropertyMapping IDynamicFixedLengthTypeConfiguration.DoubleProperty(string memberName, Window window)
        {
            var member = getMember<double?>(memberName);
            return getDoubleMapping(member, window);
        }

        IGuidPropertyMapping IDynamicFixedLengthTypeConfiguration.GuidProperty(string memberName, Window window)
        {
            var member = getMember<Guid?>(memberName);
            return getGuidMapping(member, window);
        }

        IInt16PropertyMapping IDynamicFixedLengthTypeConfiguration.Int16Property(string memberName, Window window)
        {
            var member = getMember<short?>(memberName);
            return getInt16Mapping(member, window);
        }

        IUInt16PropertyMapping IDynamicFixedLengthTypeConfiguration.UInt16Property(string memberName, Window window)
        {
            var member = getMember<ushort?>(memberName);
            return getUInt16Mapping(member, window);
        }

        IInt32PropertyMapping IDynamicFixedLengthTypeConfiguration.Int32Property(string memberName, Window window)
        {
            var member = getMember<int?>(memberName);
            return getInt32Mapping(member, window);
        }

        IUInt32PropertyMapping IDynamicFixedLengthTypeConfiguration.UInt32Property(string memberName, Window window)
        {
            var member = getMember<uint?>(memberName);
            return getUInt32Mapping(member, window);
        }

        IInt64PropertyMapping IDynamicFixedLengthTypeConfiguration.Int64Property(string memberName, Window window)
        {
            var member = getMember<long?>(memberName);
            return getInt64Mapping(member, window);
        }

        IUInt64PropertyMapping IDynamicFixedLengthTypeConfiguration.UInt64Property(string memberName, Window window)
        {
            var member = getMember<ulong?>(memberName);
            return getUInt64Mapping(member, window);
        }

        ISinglePropertyMapping IDynamicFixedLengthTypeConfiguration.SingleProperty(string memberName, Window window)
        {
            var member = getMember<float?>(memberName);
            return getSingleMapping(member, window);
        }

        IStringPropertyMapping IDynamicFixedLengthTypeConfiguration.StringProperty(string memberName, Window window)
        {
            var member = getMember<string>(memberName);
            return getStringMapping(member, window);
        }

        ISeparatedValueComplexPropertyMapping IDynamicFixedLengthTypeConfiguration.ComplexProperty<TProp>(string memberName, ISeparatedValueTypeMapper<TProp> mapper, Window window)
        {
            var member = getMember<string>(memberName);
            return getComplexMapping(member, mapper, window);
        }

        IFixedLengthComplexPropertyMapping IDynamicFixedLengthTypeConfiguration.ComplexProperty<TProp>(string memberName, IFixedLengthTypeMapper<TProp> mapper, Window window)
        {
            var member = getMember<string>(memberName);
            return getComplexMapping(member, mapper, window);
        }

        IEnumPropertyMapping<TEnum> IDynamicFixedLengthTypeConfiguration.EnumProperty<TEnum>(string memberName, Window window)
        {
            var member = getMember<TEnum?>(memberName);
            return getEnumMapping<TEnum>(member, window);
        }

        IIgnoredMapping IDynamicFixedLengthTypeConfiguration.Ignored(Window window)
        {
            return Ignored(window);
        }

        ICustomPropertyMapping IDynamicFixedLengthTypeConfiguration.CustomProperty(string memberName, IColumnDefinition column, Window window)
        {
            var member = MemberAccessorBuilder.GetMember<TEntity>(null, memberName);
            return getCustomMapping(member, column, window);
        }

        private static IMemberAccessor getMember<TProp>(string memberName)
        {
            return MemberAccessorBuilder.GetMember<TEntity, TProp>(memberName);
        }

        IEnumerable<object> IDynamicFixedLengthTypeMapper.Read(TextReader reader, FixedLengthOptions options)
        {
            return (IEnumerable<object>)Read(reader, options);
        }

        ITypedReader<object> IDynamicFixedLengthTypeMapper.GetReader(TextReader reader, FixedLengthOptions options)
        {
            return (ITypedReader<object>)GetReader(reader, options);
        }

        void IDynamicFixedLengthTypeMapper.Write(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions options)
        {
            var converted = entities.Cast<TEntity>();
            Write(writer, converted, options);
        }

        async Task IDynamicFixedLengthTypeMapper.WriteAsync(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions options)
        {
            var converted = entities.Cast<TEntity>();
            await WriteAsync(writer, converted, options);
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
            return new Mapper<TEntity>(lookup, getCodeGenerator());
        }

        private ICodeGenerator getCodeGenerator()
        {
            return isOptimized 
                ? (ICodeGenerator)new EmitCodeGenerator() 
                : new ReflectionCodeGenerator();
        }
    }
}
