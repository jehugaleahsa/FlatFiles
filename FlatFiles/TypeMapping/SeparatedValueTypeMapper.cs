using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides methods for creating type mappers.
    /// </summary>
    public static class SeparatedValueTypeMapper
    {
        private static readonly Dictionary<Type, Func<string, IColumnDefinition>> columnLookup = new Dictionary<Type, Func<string, IColumnDefinition>>()
        {
            { typeof(bool), n => new BooleanColumn(n) },
            { typeof(bool?), n => new BooleanColumn(n) },
            { typeof(byte[]), n => new ByteArrayColumn(n) },
            { typeof(byte), n => new ByteColumn(n) },
            { typeof(byte?), n => new ByteColumn(n) },
            { typeof(char[]), n => new CharArrayColumn(n) },
            { typeof(char), n => new CharColumn(n) },
            { typeof(char?), n => new CharColumn(n) },
            { typeof(DateTime), n => new DateTimeColumn(n) },
            { typeof(DateTime?), n => new DateTimeColumn(n) },
            { typeof(DateTimeOffset), n => new DateTimeOffsetColumn(n) },
            { typeof(DateTimeOffset?), n => new DateTimeOffsetColumn(n) },
            { typeof(decimal), n => new DecimalColumn(n) },
            { typeof(decimal?), n => new DecimalColumn(n) },
            { typeof(double), n => new DoubleColumn(n) },
            { typeof(double?), n => new DoubleColumn(n) },
            { typeof(Guid), n => new GuidColumn(n) },
            { typeof(Guid?), n => new GuidColumn(n) },
            { typeof(short), n => new Int16Column(n) },
            { typeof(short?), n => new Int16Column(n) },
            { typeof(int), n => new Int32Column(n) },
            { typeof(int?), n => new Int32Column(n) },
            { typeof(long), n => new Int64Column(n) },
            { typeof(long?), n => new Int64Column(n) },
            { typeof(sbyte), n => new SByteColumn(n) },
            { typeof(sbyte?), n => new SByteColumn(n) },
            { typeof(float), n => new SingleColumn(n) },
            { typeof(float?), n => new SingleColumn(n) },
            { typeof(string), n => new StringColumn(n) },
            { typeof(TimeSpan), n => new TimeSpanColumn(n) },
            { typeof(TimeSpan?), n => new TimeSpanColumn(n) },
            { typeof(ushort), n => new UInt16Column(n) },
            { typeof(ushort?), n => new UInt16Column(n) },
            { typeof(uint), n => new UInt32Column(n) },
            { typeof(uint?), n => new UInt32Column(n) },
            { typeof(ulong), n => new UInt64Column(n) },
            { typeof(ulong?), n => new UInt64Column(n) }
        };

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

        /// <summary>
        /// Gets a reader whose column types are deduced by matching the entity property names to the column names.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to deduce the column types for.</typeparam>
        /// <param name="reader">The text reader containing the schema and records.</param>
        /// <param name="options">Options used to read the data.</param>
        /// <param name="matcher">An object that can determine if a column should be mapped to a property.</param>
        /// <returns>A reader object for iterating the parsed records.</returns>
        /// <remarks>
        /// The first line of the input file must contain the schema, whose column names must match the property
        /// names in the provided entity type. The data for each column must be in a format that .NET can
        /// parse without customization.
        /// </remarks>
        public static ITypedReader<TEntity> GetAutoMappedReader<TEntity>(TextReader reader, SeparatedValueOptions options = null, IAutoMapMatcher matcher = null)
            where TEntity : new()
        {
            var optionsCopy = options == null ? new SeparatedValueOptions() : options.Clone();
            optionsCopy.IsFirstRecordSchema = true;
            var recordReader = new SeparatedValueReader(reader, optionsCopy);
            var schema = recordReader.GetSchema();
            return GetAutoMapReader<TEntity>(schema, recordReader, matcher);
        }

        /// <summary>
        /// Gets a reader whose column types are deduced by matching the entity property names to the column names.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to deduce the column types for.</typeparam>
        /// <param name="reader">The text reader containing the schema and records.</param>
        /// <param name="options">Options used to read the data.</param>
        /// <param name="matcher">An object that can determine if a column should be mapped to a property.</param>
        /// <returns>A reader object for iterating the parsed records.</returns>
        /// <remarks>
        /// The first line of the input file must contain the schema, whose column names must match the property
        /// names in the provided entity type. The data for each column must be in a format that .NET can
        /// parse without customization.
        /// </remarks>
        public static async Task<ITypedReader<TEntity>> GetAutoMappedReaderAsync<TEntity>(TextReader reader, SeparatedValueOptions options = null, IAutoMapMatcher matcher = null)
            where TEntity : new()
        {
            var optionsCopy = options == null ? new SeparatedValueOptions() : options.Clone();
            optionsCopy.IsFirstRecordSchema = true;
            var recordReader = new SeparatedValueReader(reader, optionsCopy);
            var schema = await recordReader.GetSchemaAsync();
            return GetAutoMapReader<TEntity>(schema, recordReader, matcher);
        }

        private static ITypedReader<TEntity> GetAutoMapReader<TEntity>(SeparatedValueSchema schema, SeparatedValueReader reader, IAutoMapMatcher matcher)
            where TEntity : new()
        {
            var typedMapper = new SeparatedValueTypeMapper<TEntity>();
            IDynamicSeparatedValueTypeMapper dynamicmapper = typedMapper;
            foreach (var column in schema.ColumnDefinitions)
            {
                var contextParameter = Expression.Parameter(typeof(IColumnContext), "context");
                var entityParameter = Expression.Parameter(typeof(object), "entity");
                var valueParameter = Expression.Parameter(typeof(object), "value");
                var memberExpression = GetMemberExpression(entityParameter, typeof(TEntity), column, matcher ?? AutoMapMatcher.Default);
                var body = Expression.Assign(memberExpression, Expression.Convert(valueParameter, memberExpression.Type));
                var columnDefinition = GetColumnDefinition(memberExpression.Type, column.ColumnName);
                if (columnDefinition == null)
                {
                    throw new FlatFileException(String.Format(null, Resources.NoAutoMapPropertyType, column.ColumnName));
                }
                var lambdaExpression = Expression.Lambda<Action<IColumnContext, object, object>>(body, contextParameter, entityParameter, valueParameter);
                var compiledSetter = lambdaExpression.Compile();
                dynamicmapper.CustomMapping(columnDefinition).WithReader(compiledSetter);
            }
            reader.SetSchema(typedMapper.GetSchema());
            return typedMapper.GetReader(reader);
        }

        private static MemberExpression GetMemberExpression(ParameterExpression entityParameter, Type entityType, IColumnDefinition column, IAutoMapMatcher matcher)
        {
            var propertyInfo = GetProperty(entityType, column, matcher);
            if (propertyInfo != null)
            {
                if (!propertyInfo.CanWrite)
                {
                    throw new FlatFileException(String.Format(null, Resources.ReadOnlyProperty, column.ColumnName));
                }
                return Expression.Property(Expression.Convert(entityParameter, entityType), propertyInfo);
            }
            var fieldInfo = GetField(entityType, column, matcher);
            if (fieldInfo != null)
            {
                return Expression.Field(Expression.Convert(entityParameter, entityType), fieldInfo);
            }
            throw new FlatFileException(Resources.BadPropertySelector);
        }

        private static PropertyInfo GetProperty(Type entityType, IColumnDefinition column, IAutoMapMatcher matcher)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var propertyInfos = entityType.GetTypeInfo().GetProperties(bindingFlags)
                .Where(p => matcher.IsMatch(column, p))
                .ToArray();
            if (propertyInfos.Length > 1)
            {
                if (!matcher.UseFallback)
                {
                    return null;
                }
                // If there is more than one match, do an exact match
                return propertyInfos.SingleOrDefault(i => i.Name == column.ColumnName);
            }
            return propertyInfos.SingleOrDefault();
        }

        private static FieldInfo GetField(Type entityType, IColumnDefinition column, IAutoMapMatcher matcher)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fieldInfos = entityType.GetTypeInfo().GetFields(bindingFlags)
                .Where(p => matcher.IsMatch(column, p))
                .ToArray();
            if (fieldInfos.Length > 1)
            {
                if (!matcher.UseFallback)
                {
                    return null;
                }
                // If there is more than one match, do an exact match
                return fieldInfos.SingleOrDefault(i => i.Name == column.ColumnName);
            }
            return fieldInfos.SingleOrDefault();
        }

        /// <summary>
        /// Gets a writer whose column types are deduced from the entity properties.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to deduce the column types for.</typeparam>
        /// <param name="writer">The text writer to write the schema and records to.</param>
        /// <param name="options">Options used to read the data.</param>
        /// <param name="resolver">An object that will determine the name of the generated columns.</param>
        /// <returns>A writer object for serializing entities.</returns>
        /// <remarks>Unless options are provided, by default this method will write the schema before the first record.</remarks>
        public static ITypedWriter<TEntity> GetAutoMappedWriter<TEntity>(TextWriter writer, SeparatedValueOptions options = null, IAutoMapResolver resolver = null)
        {
            var optionsCopy = options == null ? new SeparatedValueOptions() { IsFirstRecordSchema = true } : options.Clone();
            var entityType = typeof(TEntity);
            var typedMapper = Define(() => default(TEntity));
            var dynamicMapper = (IDynamicSeparatedValueTypeMapper)typedMapper;
            var nameResolver = resolver ?? AutoMapResolver.Default;

            var context = Expression.Parameter(typeof(IColumnContext), "ctx");
            var entity = Expression.Parameter(typeof(object), "e");

            var members = GetMembers(entityType, nameResolver);
            foreach (var member in members)
            {
                var columnName = nameResolver.GetColumnName(member);
                if (String.IsNullOrWhiteSpace(columnName))
                {
                    continue;
                }
                if (member is PropertyInfo property)
                {
                    var column = GetColumnDefinition(property.PropertyType, columnName);
                    if (column == null)
                    {
                        continue;
                    }
                    var body = Expression.Convert(Expression.Property(Expression.Convert(entity, entityType), property), typeof(object));
                    var lambda = Expression.Lambda<Func<IColumnContext, object, object>>(body, context, entity);
                    var getter = lambda.Compile();
                    dynamicMapper.CustomMapping(column).WithWriter(getter);
                }
                else if (member is FieldInfo field)
                {
                    var column = GetColumnDefinition(field.FieldType, columnName);
                    if (column == null)
                    {
                        continue;
                    }
                    var body = Expression.Convert(Expression.Field(Expression.Convert(entity, entityType), field), typeof(object));
                    var lambda = Expression.Lambda<Func<IColumnContext, object, object>>(body, context, entity);
                    var getter = lambda.Compile();
                    dynamicMapper.CustomMapping(column).WithWriter(getter);
                }
            }
            
            return typedMapper.GetWriter(writer, optionsCopy);
        }

        private static IEnumerable<MemberInfo> GetMembers(Type entityType, IAutoMapResolver resolver)
        {
            var bindingFlags = BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public;
            var members = entityType.GetTypeInfo().GetMembers(bindingFlags)
                .Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                .Select((m, i) => (member: m, positions: (user: resolver.GetPosition(m), builtin: i)))
                .Where(x => x.positions.user != -1)
                .OrderBy(x => x.positions)
                .Select(x => x.member)
                .ToArray();
            return members;
        }

        private static IColumnDefinition GetColumnDefinition(Type type, string columnName)
        {
            if (columnLookup.TryGetValue(type, out var factory))
            {
                return factory(columnName);
            }
            return null;
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
        IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset?>> accessor);

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
        ITimeSpanPropertyMapping Property(Expression<Func<TEntity, TimeSpan>> accessor);

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ITimeSpanPropertyMapping Property(Expression<Func<TEntity, TimeSpan?>> accessor);

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
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor) where TEnum : Enum;

        /// <summary>
        /// Associates the property with the type mapper and returns an object for configuration.
        /// </summary>
        /// <typeparam name="TEnum">The enumerated type of the property.</typeparam>
        /// <param name="accessor">An expression that returns the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor) where TEnum : struct, Enum;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored();

        /// <summary>
        /// Specifies the next column will be mapped using custom functions.
        /// </summary>
        /// <param name="column">The custom column definition for parsing and formatting the column.</param>
        /// <returns>An object to configure the custom mapping.</returns>
        ICustomMapping<TEntity> CustomMapping(IColumnDefinition column);

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
        ISeparatedValueTypedReader<TEntity> GetReader(TextReader reader, SeparatedValueOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        void Write(TextWriter writer, IEnumerable<TEntity> entities, SeparatedValueOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        Task WriteAsync(TextWriter writer, IEnumerable<TEntity> entities, SeparatedValueOptions options = null);

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
        IDateTimeOffsetPropertyMapping DateTimeOffsetProperty(string memberName);

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
        /// <param name="memberName">The name of the property to map.</param>
        /// <returns>An object to configure the property mapping.</returns>
        ITimeSpanPropertyMapping TimeSpanProperty(string memberName);

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
        IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(string memberName) where TEnum : struct, Enum;

        /// <summary>
        /// Specifies that the next column is ignored and returns an object for configuration.
        /// </summary>
        /// <returns>An object to configure the mapping.</returns>
        IIgnoredMapping Ignored();

        /// <summary>
        /// Specifies the next column will be mapped using custom functions.
        /// </summary>
        /// <param name="column">The custom column definition for parsing and formatting the column.</param>
        /// <returns>An object to configure the custom mapping.</returns>
        ICustomMapping CustomMapping(IColumnDefinition column);

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
        ISeparatedValueTypedReader<object> GetReader(TextReader reader, SeparatedValueOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        void Write(TextWriter writer, IEnumerable<object> entities, SeparatedValueOptions options = null);

        /// <summary>
        /// Writes the given entities to the given stream.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="entities">The entities to write to the stream.</param>
        /// <param name="options">The options used to format the output.</param>
        Task WriteAsync(TextWriter writer, IEnumerable<object> entities, SeparatedValueOptions options = null);

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
        IMapperSource<TEntity>
    {
        private readonly MemberLookup lookup = new MemberLookup();
        private bool isOptimized = true;

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
            if (factory != null)
            {
                lookup.SetFactory(factory);
            }
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool>> accessor)
        {
            var member = GetMember(accessor);
            return GetBooleanMapping(member, false);
        }

        public IBooleanPropertyMapping Property(Expression<Func<TEntity, bool?>> accessor)
        {
            var member = GetMember(accessor);
            return GetBooleanMapping(member, true);
        }

        private IBooleanPropertyMapping GetBooleanMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new BooleanColumn(member.Name) { IsNullable = isNullable };
                return new BooleanPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IByteArrayPropertyMapping Property(Expression<Func<TEntity, byte[]>> accessor)
        {
            var member = GetMember(accessor);
            return GetByteArrayMapping(member);
        }

        private IByteArrayPropertyMapping GetByteArrayMapping(IMemberAccessor member)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                ByteArrayColumn column = new ByteArrayColumn(member.Name);
                return new ByteArrayPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte>> accessor)
        {
            var member = GetMember(accessor);
            return GetByteMapping(member, false);
        }

        public IBytePropertyMapping Property(Expression<Func<TEntity, byte?>> accessor)
        {
            var member = GetMember(accessor);
            return GetByteMapping(member, true);
        }

        private IBytePropertyMapping GetByteMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                ByteColumn column = new ByteColumn(member.Name) { IsNullable = isNullable };
                return new BytePropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte>> accessor)
        {
            var member = GetMember(accessor);
            return GetSByteMapping(member, false);
        }

        public ISBytePropertyMapping Property(Expression<Func<TEntity, sbyte?>> accessor)
        {
            var member = GetMember(accessor);
            return GetSByteMapping(member, true);
        }

        private ISBytePropertyMapping GetSByteMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                SByteColumn column = new SByteColumn(member.Name) { IsNullable = isNullable };
                return new SBytePropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public ICharArrayPropertyMapping Property(Expression<Func<TEntity, char[]>> accessor)
        {
            var member = GetMember(accessor);
            return GetCharArrayMapping(member);
        }

        private ICharArrayPropertyMapping GetCharArrayMapping(IMemberAccessor member)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                CharArrayColumn column = new CharArrayColumn(member.Name);
                return new CharArrayPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char>> accessor)
        {
            var member = GetMember(accessor);
            return GetCharMapping(member, false);
        }

        public ICharPropertyMapping Property(Expression<Func<TEntity, char?>> accessor)
        {
            var member = GetMember(accessor);
            return GetCharMapping(member, true);
        }

        private ICharPropertyMapping GetCharMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                CharColumn column = new CharColumn(member.Name) { IsNullable = isNullable };
                return new CharPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime>> accessor)
        {
            var member = GetMember(accessor);
            return GetDateTimeMapping(member, false);
        }

        public IDateTimePropertyMapping Property(Expression<Func<TEntity, DateTime?>> accessor)
        {
            var member = GetMember(accessor);
            return GetDateTimeMapping(member, true);
        }

        private IDateTimePropertyMapping GetDateTimeMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DateTimeColumn column = new DateTimeColumn(member.Name) { IsNullable = isNullable };
                return new DateTimePropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset>> accessor)
        {
            var member = GetMember(accessor);
            return GetDateTimeOffsetMapping(member, false);
        }

        public IDateTimeOffsetPropertyMapping Property(Expression<Func<TEntity, DateTimeOffset?>> accessor)
        {
            var member = GetMember(accessor);
            return GetDateTimeOffsetMapping(member, true);
        }

        private IDateTimeOffsetPropertyMapping GetDateTimeOffsetMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new DateTimeOffsetColumn(member.Name) { IsNullable = isNullable };
                return new DateTimeOffsetPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal>> accessor)
        {
            var member = GetMember(accessor);
            return GetDecimalMapping(member, false);
        }

        public IDecimalPropertyMapping Property(Expression<Func<TEntity, decimal?>> accessor)
        {
            var member = GetMember(accessor);
            return GetDecimalMapping(member, true);
        }

        private IDecimalPropertyMapping GetDecimalMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DecimalColumn column = new DecimalColumn(member.Name) { IsNullable = isNullable };
                return new DecimalPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double>> accessor)
        {
            var member = GetMember(accessor);
            return GetDoubleMapping(member, false);
        }

        public IDoublePropertyMapping Property(Expression<Func<TEntity, double?>> accessor)
        {
            var member = GetMember(accessor);
            return GetDoubleMapping(member, true);
        }

        private IDoublePropertyMapping GetDoubleMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                DoubleColumn column = new DoubleColumn(member.Name) { IsNullable = isNullable };
                return new DoublePropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid>> accessor)
        {
            var member = GetMember(accessor);
            return GetGuidMapping(member, false);
        }

        public IGuidPropertyMapping Property(Expression<Func<TEntity, Guid?>> accessor)
        {
            var member = GetMember(accessor);
            return GetGuidMapping(member, true);
        }

        private IGuidPropertyMapping GetGuidMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                GuidColumn column = new GuidColumn(member.Name) { IsNullable = isNullable };
                return new GuidPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short>> accessor)
        {
            var member = GetMember(accessor);
            return GetInt16Mapping(member, false);
        }

        public IInt16PropertyMapping Property(Expression<Func<TEntity, short?>> accessor)
        {
            var member = GetMember(accessor);
            return GetInt16Mapping(member, true);
        }

        private IInt16PropertyMapping GetInt16Mapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int16Column column = new Int16Column(member.Name) { IsNullable = isNullable };
                return new Int16PropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public ITimeSpanPropertyMapping Property(Expression<Func<TEntity, TimeSpan>> accessor)
        {
            var member = GetMember(accessor);
            return GetTimeSpanMapping(member, false);
        }

        public ITimeSpanPropertyMapping Property(Expression<Func<TEntity, TimeSpan?>> accessor)
        {
            var member = GetMember(accessor);
            return GetTimeSpanMapping(member, true);
        }

        private ITimeSpanPropertyMapping GetTimeSpanMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new TimeSpanColumn(member.Name) { IsNullable = isNullable };
                return new TimeSpanPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort>> accessor)
        {
            var member = GetMember(accessor);
            return GetUInt16Mapping(member, false);
        }

        public IUInt16PropertyMapping Property(Expression<Func<TEntity, ushort?>> accessor)
        {
            var member = GetMember(accessor);
            return GetUInt16Mapping(member, true);
        }

        private IUInt16PropertyMapping GetUInt16Mapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt16Column column = new UInt16Column(member.Name) { IsNullable = isNullable };
                return new UInt16PropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int>> accessor)
        {
            var member = GetMember(accessor);
            return GetInt32Mapping(member, false);
        }

        public IInt32PropertyMapping Property(Expression<Func<TEntity, int?>> accessor)
        {
            var member = GetMember(accessor);
            return GetInt32Mapping(member, true);
        }

        private IInt32PropertyMapping GetInt32Mapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int32Column column = new Int32Column(member.Name) { IsNullable = isNullable };
                return new Int32PropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint>> accessor)
        {
            var member = GetMember(accessor);
            return GetUInt32Mapping(member, false);
        }

        public IUInt32PropertyMapping Property(Expression<Func<TEntity, uint?>> accessor)
        {
            var member = GetMember(accessor);
            return GetUInt32Mapping(member, true);
        }

        private IUInt32PropertyMapping GetUInt32Mapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt32Column column = new UInt32Column(member.Name) { IsNullable = isNullable };
                return new UInt32PropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long>> accessor)
        {
            var member = GetMember(accessor);
            return GetInt64Mapping(member, false);
        }

        public IInt64PropertyMapping Property(Expression<Func<TEntity, long?>> accessor)
        {
            var member = GetMember(accessor);
            return GetInt64Mapping(member, true);
        }

        private IInt64PropertyMapping GetInt64Mapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                Int64Column column = new Int64Column(member.Name) { IsNullable = isNullable };
                return new Int64PropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong>> accessor)
        {
            var member = GetMember(accessor);
            return GetUInt64Mapping(member, false);
        }

        public IUInt64PropertyMapping Property(Expression<Func<TEntity, ulong?>> accessor)
        {
            var member = GetMember(accessor);
            return GetUInt64Mapping(member, true);
        }

        private IUInt64PropertyMapping GetUInt64Mapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                UInt64Column column = new UInt64Column(member.Name) { IsNullable = isNullable };
                return new UInt64PropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float>> accessor)
        {
            var member = GetMember(accessor);
            return GetSingleMapping(member, false);
        }

        public ISinglePropertyMapping Property(Expression<Func<TEntity, float?>> accessor)
        {
            var member = GetMember(accessor);
            return GetSingleMapping(member, true);
        }

        private ISinglePropertyMapping GetSingleMapping(IMemberAccessor member, bool isNullable)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                SingleColumn column = new SingleColumn(member.Name) { IsNullable = isNullable };
                return new SinglePropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string>> accessor)
        {
            var member = GetMember(accessor);
            return GetStringMapping(member);
        }

        private IStringPropertyMapping GetStringMapping(IMemberAccessor member)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                StringColumn column = new StringColumn(member.Name);
                return new StringPropertyMapping(column, member, fileIndex, workIndex);
            });
        }

        public ISeparatedValueComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, ISeparatedValueTypeMapper<TProp> mapper)
        {
            var member = GetMember(accessor);
            return GetComplexMapping(member, mapper);
        }

        private ISeparatedValueComplexPropertyMapping GetComplexMapping<TProp>(IMemberAccessor member, ISeparatedValueTypeMapper<TProp> mapper)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) => new SeparatedValueComplexPropertyMapping<TProp>(mapper, member, fileIndex, workIndex));
        }

        public IFixedLengthComplexPropertyMapping ComplexProperty<TProp>(Expression<Func<TEntity, TProp>> accessor, IFixedLengthTypeMapper<TProp> mapper)
        {
            var member = GetMember(accessor);
            return GetComplexMapping(member, mapper);
        }

        private IFixedLengthComplexPropertyMapping GetComplexMapping<TProp>(IMemberAccessor member, IFixedLengthTypeMapper<TProp> mapper)
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) => new FixedLengthComplexPropertyMapping<TProp>(mapper, member, fileIndex, workIndex));
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum>> accessor)
            where TEnum : Enum
        {
            var member = GetMember(accessor);
            return GetEnumMapping<TEnum>(member, false);
        }

        public IEnumPropertyMapping<TEnum> EnumProperty<TEnum>(Expression<Func<TEntity, TEnum?>> accessor)
            where TEnum : struct, Enum
        {
            var member = GetMember(accessor);
            return GetEnumMapping<TEnum>(member, true);
        }

        private IEnumPropertyMapping<TEnum> GetEnumMapping<TEnum>(IMemberAccessor member, bool isNullable)
            where TEnum : Enum
        {
            return lookup.GetOrAddMember(member, (fileIndex, workIndex) =>
            {
                var column = new EnumColumn<TEnum>(member.Name) { IsNullable = isNullable };
                return new EnumPropertyMapping<TEnum>(column, member, fileIndex, workIndex);
            });
        }

        public IIgnoredMapping Ignored()
        {
            return lookup.AddIgnored();
        }

        public ICustomMapping<TEntity> CustomMapping(IColumnDefinition column)
        {
            var mapping = lookup.GetOrAddCustomMapping(column.ColumnName, (fileIndex, workIndex) => new CustomMapping<TEntity>(column, fileIndex, workIndex));
            return mapping;
        }

        private static IMemberAccessor GetMember<TProp>(Expression<Func<TEntity, TProp>> accessor)
        {
            return MemberAccessorBuilder.GetMember(accessor);
        }

        public IEnumerable<TEntity> Read(TextReader reader, SeparatedValueOptions options = null)
        {
            SeparatedValueSchema schema = GetSchemaInternal();
            var separatedValueReader = new SeparatedValueReader(reader, schema, options);
            return Read(separatedValueReader);
        }

        private IEnumerable<TEntity> Read(SeparatedValueReader reader)
        {
            var typedReader = GetTypedReader(reader);
            while (typedReader.Read())
            {
                yield return typedReader.Current;
            }
        }

        public ISeparatedValueTypedReader<TEntity> GetReader(TextReader reader, SeparatedValueOptions options = null)
        {
            var schema = GetSchemaInternal();
            var separatedValueReader = new SeparatedValueReader(reader, schema, options);
            return GetTypedReader(separatedValueReader);
        }

        internal ISeparatedValueTypedReader<TEntity> GetReader(SeparatedValueReader reader)
        {
            return GetTypedReader(reader);
        }

        private SeparatedValueTypedReader<TEntity> GetTypedReader(SeparatedValueReader reader)
        {
            var mapper = new Mapper<TEntity>(lookup, GetCodeGenerator());
            var typedReader = new SeparatedValueTypedReader<TEntity>(reader, mapper);
            return typedReader;
        }

        public void Write(TextWriter writer, IEnumerable<TEntity> entities, SeparatedValueOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var schema = GetSchemaInternal();
            var separatedValueWriter = new SeparatedValueWriter(writer, schema, options);
            Write(separatedValueWriter, entities);
        }

        private void Write(IWriterWithMetadata writer, IEnumerable<TEntity> entities)
        {
            var typedWriter = GetTypedWriter(writer);
            typedWriter.WriteAll(entities);
        }

        public async Task WriteAsync(TextWriter writer, IEnumerable<TEntity> entities, SeparatedValueOptions options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var schema = GetSchemaInternal();
            var separatedValueWriter = new SeparatedValueWriter(writer, schema, options);
            await WriteAsync(separatedValueWriter, entities).ConfigureAwait(false);
        }

        private async Task WriteAsync(IWriterWithMetadata writer, IEnumerable<TEntity> entities)
        {
            var typedWriter = GetTypedWriter(writer);
            await typedWriter.WriteAllAsync(entities).ConfigureAwait(false);
        }

        public ITypedWriter<TEntity> GetWriter(TextWriter writer, SeparatedValueOptions options = null)
        {
            var schema = GetSchemaInternal();
            var separatedValueWriter = new SeparatedValueWriter(writer, schema, options);
            return GetTypedWriter(separatedValueWriter);
        }

        private TypedWriter<TEntity> GetTypedWriter(IWriterWithMetadata writer)
        {
            var mapper = new Mapper<TEntity>(lookup, GetCodeGenerator());
            return new TypedWriter<TEntity>(writer, mapper);
        }

        public SeparatedValueSchema GetSchema()
        {
            return GetSchemaInternal();
        }

        private SeparatedValueSchema GetSchemaInternal()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            var mappings = lookup.GetMappings();
            foreach (IMemberMapping mapping in mappings)
            {
                IColumnDefinition column = mapping.ColumnDefinition;
                schema.AddColumn(column);
            }
            return schema;
        }

        SeparatedValueSchema IDynamicSeparatedValueTypeConfiguration.GetSchema()
        {
            return GetSchemaInternal();
        }

        IBooleanPropertyMapping IDynamicSeparatedValueTypeConfiguration.BooleanProperty(string memberName)
        {
            var member = GetMember<bool?>(memberName);
            return GetBooleanMapping(member, IsNullable(member));
        }

        IByteArrayPropertyMapping IDynamicSeparatedValueTypeConfiguration.ByteArrayProperty(string memberName)
        {
            var member = GetMember<byte[]>(memberName);
            return GetByteArrayMapping(member);
        }

        IBytePropertyMapping IDynamicSeparatedValueTypeConfiguration.ByteProperty(string memberName)
        {
            var member = GetMember<byte?>(memberName);
            return GetByteMapping(member, IsNullable(member));
        }

        ISBytePropertyMapping IDynamicSeparatedValueTypeConfiguration.SByteProperty(string memberName)
        {
            var member = GetMember<sbyte?>(memberName);
            return GetSByteMapping(member, IsNullable(member));
        }

        ICharArrayPropertyMapping IDynamicSeparatedValueTypeConfiguration.CharArrayProperty(string memberName)
        {
            var member = GetMember<char[]>(memberName);
            return GetCharArrayMapping(member);
        }

        ICharPropertyMapping IDynamicSeparatedValueTypeConfiguration.CharProperty(string memberName)
        {
            var member = GetMember<char?>(memberName);
            return GetCharMapping(member, IsNullable(member));
        }

        IDateTimePropertyMapping IDynamicSeparatedValueTypeConfiguration.DateTimeProperty(string memberName)
        {
            var member = GetMember<DateTime?>(memberName);
            return GetDateTimeMapping(member, IsNullable(member));
        }

        IDateTimeOffsetPropertyMapping IDynamicSeparatedValueTypeConfiguration.DateTimeOffsetProperty(string memberName)
        {
            var member = GetMember<DateTimeOffset?>(memberName);
            return GetDateTimeOffsetMapping(member, IsNullable(member));
        }

        IDecimalPropertyMapping IDynamicSeparatedValueTypeConfiguration.DecimalProperty(string memberName)
        {
            var member = GetMember<decimal?>(memberName);
            return GetDecimalMapping(member, IsNullable(member));
        }

        IDoublePropertyMapping IDynamicSeparatedValueTypeConfiguration.DoubleProperty(string memberName)
        {
            var member = GetMember<double?>(memberName);
            return GetDoubleMapping(member, IsNullable(member));
        }

        IGuidPropertyMapping IDynamicSeparatedValueTypeConfiguration.GuidProperty(string memberName)
        {
            var member = GetMember<Guid?>(memberName);
            return GetGuidMapping(member, IsNullable(member));
        }

        IInt16PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int16Property(string memberName)
        {
            var member = GetMember<short?>(memberName);
            return GetInt16Mapping(member, IsNullable(member));
        }

        IUInt16PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt16Property(string memberName)
        {
            var member = GetMember<ushort?>(memberName);
            return GetUInt16Mapping(member, IsNullable(member));
        }

        IInt32PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int32Property(string memberName)
        {
            var member = GetMember<int?>(memberName);
            return GetInt32Mapping(member, IsNullable(member));
        }

        IUInt32PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt32Property(string memberName)
        {
            var member = GetMember<uint?>(memberName);
            return GetUInt32Mapping(member, IsNullable(member));
        }

        IInt64PropertyMapping IDynamicSeparatedValueTypeConfiguration.Int64Property(string memberName)
        {
            var member = GetMember<long?>(memberName);
            return GetInt64Mapping(member, IsNullable(member));
        }

        IUInt64PropertyMapping IDynamicSeparatedValueTypeConfiguration.UInt64Property(string memberName)
        {
            var member = GetMember<ulong?>(memberName);
            return GetUInt64Mapping(member, IsNullable(member));
        }

        ISinglePropertyMapping IDynamicSeparatedValueTypeConfiguration.SingleProperty(string memberName)
        {
            var member = GetMember<float?>(memberName);
            return GetSingleMapping(member, IsNullable(member));
        }

        IStringPropertyMapping IDynamicSeparatedValueTypeConfiguration.StringProperty(string memberName)
        {
            var member = GetMember<string>(memberName);
            return GetStringMapping(member);
        }

        ITimeSpanPropertyMapping IDynamicSeparatedValueTypeConfiguration.TimeSpanProperty(string memberName)
        {
            var member = GetMember<TimeSpan>(memberName);
            return GetTimeSpanMapping(member, IsNullable(member));
        }

        ISeparatedValueComplexPropertyMapping IDynamicSeparatedValueTypeConfiguration.ComplexProperty<TProp>(string memberName, ISeparatedValueTypeMapper<TProp> mapper)
        {
            var member = GetMember<string>(memberName);
            return GetComplexMapping(member, mapper);
        }

        IFixedLengthComplexPropertyMapping IDynamicSeparatedValueTypeConfiguration.ComplexProperty<TProp>(string memberName, IFixedLengthTypeMapper<TProp> mapper)
        {
            var member = GetMember<string>(memberName);
            return GetComplexMapping(member, mapper);
        }

        IEnumPropertyMapping<TEnum> IDynamicSeparatedValueTypeConfiguration.EnumProperty<TEnum>(string memberName)
        {
            var member = GetMember<TEnum?>(memberName);
            return GetEnumMapping<TEnum>(member, IsNullable(member));
        }



        IIgnoredMapping IDynamicSeparatedValueTypeConfiguration.Ignored()
        {
            return Ignored();
        }

        ICustomMapping IDynamicSeparatedValueTypeConfiguration.CustomMapping(IColumnDefinition column)
        {
            var mapping = lookup.GetOrAddCustomMapping(column.ColumnName, (fileIndex, workIndex) => new CustomMapping<TEntity>(column, fileIndex, workIndex));
            return mapping;
        }

        private static IMemberAccessor GetMember<TProp>(string memberName)
        {
            return MemberAccessorBuilder.GetMember<TEntity>(typeof(TProp), memberName);
        }

        private static bool IsNullable(IMemberAccessor accessor)
        {
            if (!accessor.Type.GetTypeInfo().IsValueType)
            {
                return true;
            }
            return Nullable.GetUnderlyingType(accessor.Type) != null;
        }

        IEnumerable<object> IDynamicSeparatedValueTypeMapper.Read(TextReader reader, SeparatedValueOptions options)
        {
            return (IEnumerable<object>)Read(reader, options);
        }

        ISeparatedValueTypedReader<object> IDynamicSeparatedValueTypeMapper.GetReader(TextReader reader, SeparatedValueOptions options)
        {
            return (ISeparatedValueTypedReader<object>)GetReader(reader, options);
        }

        void IDynamicSeparatedValueTypeMapper.Write(TextWriter writer, IEnumerable<object> entities, SeparatedValueOptions options)
        {
            var converted = entities.Cast<TEntity>();
            Write(writer, converted, options);
        }

        async Task IDynamicSeparatedValueTypeMapper.WriteAsync(TextWriter writer, IEnumerable<object> entities, SeparatedValueOptions options)
        {
            var converted = entities.Cast<TEntity>();
            await WriteAsync(writer, converted, options).ConfigureAwait(false);
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

        public void UseFactory<TOther>(Func<TOther> factory)
        {
            lookup.SetFactory(factory);
        }

        void IDynamicSeparatedValueTypeConfiguration.UseFactory(Type entityType, Func<object> factory)
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
