using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FlatFiles.Properties;

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
            var mapper = Activator.CreateInstance(mapperType)!;
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
            var mapper = Activator.CreateInstance(mapperType, factory)!;
            return (IDynamicFixedLengthTypeMapper)mapper;
        }
    }

    internal sealed class FixedLengthTypeMapper<TEntity> 
        : IFixedLengthTypeMapper<TEntity>, 
        IDynamicFixedLengthTypeMapper,
        IMapperSource<TEntity>
    {
        private readonly MemberLookup lookup = new();
        private readonly Dictionary<IMemberMapping, Window> windowLookup = new();
        private bool isOptimized = true;

        public FixedLengthTypeMapper()
            : this(null)
        {
        }

        public FixedLengthTypeMapper(Func<object> factory)
            : this(() => (TEntity)factory())
        {
        }

        public FixedLengthTypeMapper(Func<TEntity>? factory)
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new BooleanColumn(member.Name) { IsNullable = isNullable };
                return new BooleanPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new ByteArrayColumn(member.Name);
                return new ByteArrayPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new ByteColumn(member.Name) { IsNullable = isNullable };
                return new BytePropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new SByteColumn(member.Name) { IsNullable = isNullable };
                return new SBytePropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicaIndex, logicalIndex) =>
            {
                var column = new CharArrayColumn(member.Name);
                return new CharArrayPropertyMapping(column, member, physicaIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new CharColumn(member.Name) { IsNullable = isNullable };
                return new CharPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new DateTimeColumn(member.Name) { IsNullable = isNullable };
                return new DateTimePropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new DateTimeOffsetColumn(member.Name) { IsNullable = isNullable };
                return new DateTimeOffsetPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new DecimalColumn(member.Name) { IsNullable = isNullable };
                return new DecimalPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new DoubleColumn(member.Name) { IsNullable = isNullable };
                return new DoublePropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new GuidColumn(member.Name) { IsNullable = isNullable };
                return new GuidPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new Int16Column(member.Name) { IsNullable = isNullable };
                return new Int16PropertyMapping(column, member, physicalIndex, logicalIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public ITimeSpanPropertyMapping Property(Expression<Func<TEntity, TimeSpan>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetTimeSpanMapping(member, window, false);
        }

        public ITimeSpanPropertyMapping Property(Expression<Func<TEntity, TimeSpan?>> accessor, Window window)
        {
            var member = GetMember(accessor);
            return GetTimeSpanMapping(member, window, true);
        }

        private ITimeSpanPropertyMapping GetTimeSpanMapping(IMemberAccessor member, Window window, bool isNullable)
        {
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new TimeSpanColumn(member.Name) { IsNullable = isNullable };
                return new TimeSpanPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new UInt16Column(member.Name) { IsNullable = isNullable };
                return new UInt16PropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new Int32Column(member.Name) { IsNullable = isNullable };
                return new Int32PropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new UInt32Column(member.Name) { IsNullable = isNullable };
                return new UInt32PropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new Int64Column(member.Name) { IsNullable = isNullable };
                return new Int64PropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new UInt64Column(member.Name) { IsNullable = isNullable };
                return new UInt64PropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new SingleColumn(member.Name) { IsNullable = isNullable };
                return new SinglePropertyMapping(column, member, physicalIndex, logicalIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        public IStringPropertyMapping Property(Expression<Func<TEntity, string?>> accesor, Window window)
        {
            var member = GetMember(accesor);
            return GetStringMapping(member, window);
        }

        private IStringPropertyMapping GetStringMapping(IMemberAccessor member, Window window)
        {
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new StringColumn(member.Name);
                return new StringPropertyMapping(column, member, physicalIndex, logicalIndex);
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                return new SeparatedValueComplexPropertyMapping<TProp>(mapper, member, physicalIndex, logicalIndex);
            });
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                return new FixedLengthComplexPropertyMapping<TProp>(mapper, member, physicalIndex, logicalIndex);
            });
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
            var mapping = lookup.GetOrAddMember(member, (physicalIndex, logicalIndex) =>
            {
                var column = new EnumColumn<TEnum>(member.Name) { IsNullable = isNullable };
                return new EnumPropertyMapping<TEnum>(column, member, physicalIndex, logicalIndex);
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
            var columnName = column.ColumnName;
            if (String.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException(Resources.BlankColumnName, nameof(column));
            }
            var mapping = lookup.GetOrAddCustomMapping(columnName, (physicalIndex, logicalIndex) =>
            {
                return new CustomMapping<TEntity>(column, physicalIndex, logicalIndex);
            });
            windowLookup[mapping] = window;
            return mapping;
        }

        private static IMemberAccessor GetMember<TProp>(Expression<Func<TEntity, TProp>> accessor)
        {
            return MemberAccessorBuilder.GetMember(accessor);
        }

        public IEnumerable<TEntity> Read(TextReader reader, FixedLengthOptions? options = null)
        {
            var typedReader = GetReader(reader, options);
            return typedReader.ReadAll();
        }

#if !NET451 && !NETSTANDARD1_6 && !NETSTANDARD2_0
        public IAsyncEnumerable<TEntity> ReadAsync(TextReader reader, FixedLengthOptions? options = null)
        {
            var typedReader = GetReader(reader, options);
            return typedReader.ReadAllAsync();
        }
#endif

        public IFixedLengthTypedReader<TEntity> GetReader(TextReader reader, FixedLengthOptions? options = null)
        {
            var schema = GetSchemaInternal();
            var fixedLengthReader = new FixedLengthReader(reader, schema, options);
            return GetTypedReader(fixedLengthReader);
        }

        private FixedLengthTypedReader<TEntity> GetTypedReader(FixedLengthReader reader)
        {
            var mapper = new Mapper<TEntity>(lookup, GetCodeGenerator());
            return new FixedLengthTypedReader<TEntity>(reader, mapper);
        }

        public void Write(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions? options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var typedWriter = GetWriter(writer, options);
            typedWriter.WriteAll(entities);
        }

        public Task WriteAsync(TextWriter writer, IEnumerable<TEntity> entities, FixedLengthOptions? options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var typedWriter = GetWriter(writer, options);
            return typedWriter.WriteAllAsync(entities);
        }

#if !NET451 && !NETSTANDARD1_6 && !NETSTANDARD2_0
        public Task WriteAsync(TextWriter writer, IAsyncEnumerable<TEntity> entities, FixedLengthOptions? options = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            var typedWriter = GetWriter(writer, options);
            return typedWriter.WriteAllAsync(entities);
        }
#endif

        public ITypedWriter<TEntity> GetWriter(TextWriter writer, FixedLengthOptions? options = null)
        {
            var schema = GetSchemaInternal();
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
            return GetSchemaInternal();
        }

        private FixedLengthSchema GetSchemaInternal()
        {
            var schema = new FixedLengthSchema();
            var mappings = lookup.GetMappings();
            foreach (IMemberMapping mapping in mappings)
            {
                var column = mapping.ColumnDefinition;
                var window = windowLookup[mapping];
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

        ITimeSpanPropertyMapping IDynamicFixedLengthTypeConfiguration.TimeSpanProperty(string memberName, Window window)
        {
            var member = GetMember<TimeSpan>(memberName);
            return GetTimeSpanMapping(member, window, IsNullable(member));
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
            return (ICustomMapping) CustomMapping(column, window);
        }

        private static IMemberAccessor GetMember<TProp>(string memberName)
        {
            var member = MemberAccessorBuilder.GetMember<TEntity, TProp>(memberName);
            if (member == null)
            {
                throw new ArgumentException(Resources.BadPropertySelector, nameof(member));
            }
            return member;
        }

        private static bool IsNullable(IMemberAccessor accessor)
        {
            if (!accessor.Type.GetTypeInfo().IsValueType)
            {
                return true;
            }
            return Nullable.GetUnderlyingType(accessor.Type) != null;
        }

        IEnumerable<object> IDynamicFixedLengthTypeMapper.Read(TextReader reader, FixedLengthOptions? options)
        {
            IDynamicFixedLengthTypeMapper untypedMapper = this;
            var untypedReader = untypedMapper.GetReader(reader, options);
            return untypedReader.ReadAll();
        }

#if !NET451 && !NETSTANDARD1_6 && !NETSTANDARD2_0
        IAsyncEnumerable<object> IDynamicFixedLengthTypeMapper.ReadAsync(TextReader reader, FixedLengthOptions? options)
        {
            IDynamicFixedLengthTypeMapper untypedMapper = this;
            var untypedReader = untypedMapper.GetReader(reader, options);
            return untypedReader.ReadAllAsync();
        }
#endif

        IFixedLengthTypedReader<object> IDynamicFixedLengthTypeMapper.GetReader(TextReader reader, FixedLengthOptions? options)
        {
            return (IFixedLengthTypedReader<object>)GetReader(reader, options);
        }

        void IDynamicFixedLengthTypeMapper.Write(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions? options)
        {
            IDynamicFixedLengthTypeMapper untypedMapper = this;
            var untypedWriter = untypedMapper.GetWriter(writer, options);
            untypedWriter.WriteAll(entities);
        }

        Task IDynamicFixedLengthTypeMapper.WriteAsync(TextWriter writer, IEnumerable<object> entities, FixedLengthOptions? options)
        {
            IDynamicFixedLengthTypeMapper untypedMapper = this;
            var untypedWriter = untypedMapper.GetWriter(writer, options);
            return untypedWriter.WriteAllAsync(entities);
        }

#if !NET451 && !NETSTANDARD1_6 && !NETSTANDARD2_0
        Task IDynamicFixedLengthTypeMapper.WriteAsync(TextWriter writer, IAsyncEnumerable<object> entities, FixedLengthOptions? options)
        {
            IDynamicFixedLengthTypeMapper untypedMapper = this;
            var untypedWriter = untypedMapper.GetWriter(writer, options);
            return untypedWriter.WriteAllAsync(entities);
        }
#endif

        ITypedWriter<object> IDynamicFixedLengthTypeMapper.GetWriter(TextWriter writer, FixedLengthOptions? options)
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
                ? new EmitCodeGenerator() 
                : new ReflectionCodeGenerator();
        }
    }
}
