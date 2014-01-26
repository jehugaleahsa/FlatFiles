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
        {
            return new FixedLengthTypeMapper<TEntity>(() => Activator.CreateInstance<TEntity>());
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
    public interface IFixedLengthTypeMapper<TEntity>
    {
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
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        FixedLengthSchema GetSchema();

        /// <summary>
        /// Reads the entities from the file at the given path.
        /// </summary>
        /// <param name="fileName">The path of the file to read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<TEntity> Read(string fileName);

        /// <summary>
        /// Reads the entities from the file at the given path.
        /// </summary>
        /// <param name="fileName">The path of the file to read.</param>
        /// <param name="options">The options to use.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<TEntity> Read(string fileName, FixedLengthOptions options);

        /// <summary>
        /// Reads the entities from the given stream.
        /// </summary>
        /// <param name="stream">The input stream to read.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<TEntity> Read(Stream stream);

        /// <summary>
        /// Reads the entities from the given stream.
        /// </summary>
        /// <param name="stream">The input stream to read.</param>
        /// <param name="options">The options to use.</param>
        /// <returns>The entities that are extracted from the file.</returns>
        IEnumerable<TEntity> Read(Stream stream, FixedLengthOptions options);

        /// <summary>
        /// Writes the given entities to the file at the given path.
        /// </summary>
        /// <param name="fileName">The path of the file to write to.</param>
        /// <param name="entities">The entities to write to the file.</param>
        void Write(string fileName, IEnumerable<TEntity> entities);

        /// <summary>
        /// Writes the given entities to the file at the given path.
        /// </summary>
        /// <param name="fileName">The path of the file to write to.</param>
        /// <param name="options">The options to use.</param>
        /// <param name="entities">The entities to write to the file.</param>
        void Write(string fileName, FixedLengthOptions options, IEnumerable<TEntity> entities);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        void Write(Stream stream, IEnumerable<TEntity> entities);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="options">The options to use.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        void Write(Stream stream, FixedLengthOptions options, IEnumerable<TEntity> entities);
    }

    internal sealed class FixedLengthTypeMapper<TEntity> : IFixedLengthTypeMapper<TEntity>
    {
        private readonly Func<TEntity> factory;
        private readonly Dictionary<string, IPropertyMapping> mappings;
        private readonly Dictionary<string, int> indexes;
        private readonly Dictionary<string, Window> windows;
        private int columnCount;

        internal FixedLengthTypeMapper(Func<TEntity> factory)
        {
            this.factory = factory;
            this.mappings = new Dictionary<string, IPropertyMapping>();
            this.indexes = new Dictionary<string, int>();
            this.windows = new Dictionary<string, Window>();
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                BooleanColumn column = new BooleanColumn(propertyInfo.Name);
                mapping = new BooleanPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                ByteArrayColumn column = new ByteArrayColumn(propertyInfo.Name);
                mapping = new ByteArrayPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                ByteColumn column = new ByteColumn(propertyInfo.Name);
                mapping = new BytePropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                CharArrayColumn column = new CharArrayColumn(propertyInfo.Name);
                mapping = new CharArrayPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                CharColumn column = new CharColumn(propertyInfo.Name);
                mapping = new CharPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                DateTimeColumn column = new DateTimeColumn(propertyInfo.Name);
                mapping = new DateTimePropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                DecimalColumn column = new DecimalColumn(propertyInfo.Name);
                mapping = new DecimalPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                DoubleColumn column = new DoubleColumn(propertyInfo.Name);
                mapping = new DoublePropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                GuidColumn column = new GuidColumn(propertyInfo.Name);
                mapping = new GuidPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int16Column column = new Int16Column(propertyInfo.Name);
                mapping = new Int16PropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int32Column column = new Int32Column(propertyInfo.Name);
                mapping = new Int32PropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                Int64Column column = new Int64Column(propertyInfo.Name);
                mapping = new Int64PropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                SingleColumn column = new SingleColumn(propertyInfo.Name);
                mapping = new SinglePropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
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
            if (!mappings.TryGetValue(propertyInfo.Name, out mapping))
            {
                StringColumn column = new StringColumn(propertyInfo.Name);
                mapping = new StringPropertyMapping(column, propertyInfo);
                mappings.Add(propertyInfo.Name, mapping);
                indexes.Add(propertyInfo.Name, columnCount);
                ++columnCount;
            }
            windows[propertyInfo.Name] = window;
            return (IStringPropertyMapping)mapping;
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

        public IEnumerable<TEntity> Read(string fileName)
        {
            FixedLengthSchema schema = getSchema();
            using (IReader reader = new FixedLengthReader(fileName, schema))
            {
                return read(reader);
            }
        }

        public IEnumerable<TEntity> Read(string fileName, FixedLengthOptions options)
        {
            FixedLengthSchema schema = getSchema();
            using (IReader reader = new FixedLengthReader(fileName, schema, options))
            {
                return read(reader);
            }
        }

        public IEnumerable<TEntity> Read(Stream stream)
        {
            FixedLengthSchema schema = getSchema();
            using (IReader reader = new FixedLengthReader(stream, schema))
            {
                return read(reader);
            }
        }

        public IEnumerable<TEntity> Read(Stream stream, FixedLengthOptions options)
        {
            FixedLengthSchema schema = getSchema();
            using (IReader reader = new FixedLengthReader(stream, schema, options))
            {
                return read(reader);
            }
        }

        private IEnumerable<TEntity> read(IReader reader)
        {
            List<TEntity> entities = new List<TEntity>();
            while (reader.Read())
            {
                object[] values = reader.GetValues();
                TEntity entity = factory();
                mapProperties(values, entity);
                entities.Add(entity);
            }
            return entities;
        }

        private void mapProperties(object[] values, TEntity entity)
        {
            foreach (string propertyName in mappings.Keys)
            {
                IPropertyMapping mapping = mappings[propertyName];
                int index = indexes[propertyName];
                mapping.Property.SetValue(entity, values[index], null);
            }
        }

        public void Write(string fileName, IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            FixedLengthSchema schema = getSchema();
            using (IWriter writer = new FixedLengthWriter(fileName, schema))
            {
                write(writer, entities);
            }
        }

        public void Write(string fileName, FixedLengthOptions options, IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            FixedLengthSchema schema = getSchema();
            using (IWriter writer = new FixedLengthWriter(fileName, schema, options))
            {
                write(writer, entities);
            }
        }

        public void Write(Stream stream, IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            FixedLengthSchema schema = getSchema();
            using (IWriter writer = new FixedLengthWriter(stream, schema))
            {
                write(writer, entities);
            }
        }

        public void Write(Stream stream, FixedLengthOptions options, IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            FixedLengthSchema schema = getSchema();
            using (IWriter writer = new FixedLengthWriter(stream, schema, options))
            {
                write(writer, entities);
            }
        }

        private void write(IWriter writer, IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                object[] values = new object[columnCount];
                foreach (string propertyName in mappings.Keys)
                {
                    IPropertyMapping mapping = mappings[propertyName];
                    int index = indexes[propertyName];
                    values[index] = mapping.Property.GetValue(entity, null);
                }
                writer.Write(values);
            }
        }

        public FixedLengthSchema GetSchema()
        {
            return getSchema();
        }

        private FixedLengthSchema getSchema()
        {
            var items = getColumnDefinitions();
            FixedLengthSchema schema = new FixedLengthSchema();
            foreach (var item in items)
            {
                schema.AddColumn(item.Item1, item.Item2);
            }
            return schema;
        }

        private Tuple<ColumnDefinition, Window>[] getColumnDefinitions()
        {
            var definitions = new Tuple<ColumnDefinition, Window>[columnCount];
            foreach (string propertyName in mappings.Keys)
            {
                IPropertyMapping mapping = mappings[propertyName];
                int index = indexes[propertyName];
                Window window = windows[propertyName];
                definitions[index] = Tuple.Create(mapping.ColumnDefinition, window);
            }
            return definitions;
        }
    }
}
