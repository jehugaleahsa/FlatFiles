using System;
using System.Collections.Generic;
using System.IO;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents a class that can dynamically map types based on the shape of the record.
    /// </summary>
    public sealed class FixedLengthTypeMapperInjector : ITypeMapperInjector
    {
        private readonly List<TypeMapperMatcher> matchers = new();
        private TypeMapperMatcher? defaultMatcher;

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IFixedLengthTypeMapperInjectorWhenBuilder<TEntity> When<TEntity>(Func<TEntity, bool>? predicate = null)
        {
            return new FixedLengthTypeMapperInjectorWhenBuilder<TEntity>(this, predicate);
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IFixedLengthTypeMapperInjectorWhenBuilder When(Func<object, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new FixedLengthTypeMapperInjectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default type mapper to use.</param>
        public void WithDefault<TEntity>(IFixedLengthTypeMapper<TEntity>? typeMapper)
        {
            WithDefault((IDynamicFixedLengthTypeMapper?)typeMapper);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default schema to use.</param>
        public void WithDefault(IDynamicFixedLengthTypeMapper? typeMapper)
        {
            defaultMatcher = typeMapper == null ? null : new TypeMapperMatcher(typeMapper, o => true);
        }

        /// <summary>
        /// Gets a typed writer for writing the objects to the file.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <param name="options">The separate value options to use.</param>
        /// <returns>The typed writer.</returns>
        public ITypedWriter<object> GetWriter(TextWriter writer, FixedLengthOptions? options = null)
        {
            var injector = new FixedLengthSchemaInjector();
            var valueWriter = new FixedLengthWriter(writer, injector, options);
            var multiWriter = new MultiplexingTypedWriter(valueWriter, this);
            foreach (var matcher in matchers)
            {
                var schema = matcher.Reset();
                injector.When(values => matcher.IsMatch).Use(schema);
            }
            if (defaultMatcher != null)
            {
                var schema = defaultMatcher.Reset();
                injector.WithDefault(schema);
            }
            return multiWriter;
        }

        internal void Add(IDynamicFixedLengthTypeMapper typeMapper, Func<object, bool> predicate)
        {
            var matcher = new TypeMapperMatcher(typeMapper, predicate);
            matchers.Add(matcher);
        }

        ITypeMatcherContext ITypeMapperInjector.SetMatcher(object entity)
        {
            ITypeMatcherContext? context = null;
            foreach (var matcher in matchers)
            {
                if (context == null && matcher.Predicate(entity))
                {
                    matcher.IsMatch = true;
                    matcher.Initialize();
                    context = matcher;
                }
                else
                {
                    matcher.IsMatch = false;
                }
            }
            if (context == null)
            {
                if (defaultMatcher == null)
                {
                    throw new FlatFileException(Resources.MissingMatcher);
                }
                defaultMatcher.Initialize();
                context = defaultMatcher;
            }
            return context;
        }

        private sealed class TypeMapperMatcher : ITypeMatcherContext
        {
            public TypeMapperMatcher(IDynamicFixedLengthTypeMapper typeMapper, Func<object, bool> predicate)
            {
                TypeMapper = typeMapper;
                Predicate = predicate;
            }

            public IDynamicFixedLengthTypeMapper TypeMapper { get; }

            public Func<object, bool> Predicate { get; }

            public bool IsMatch { get; set; }

            public int LogicalCount { get; set; }

            public Action<IRecordContext, object?, object?[]>? Serializer { get; set; }

            void ITypeMatcherContext.Serialize(IRecordContext context, object? value, object?[] values)
            {
                Serializer!(context, value, values);
            }

            public void Initialize()
            {
                if (Serializer == null)
                {
                    var source = (IMapperSource)TypeMapper;
                    var mapper = source.GetMapper();
                    LogicalCount = mapper.LogicalCount;
                    Serializer = mapper.GetWriter();
                }
            }

            public FixedLengthSchema Reset()
            {
                LogicalCount = 0;
                Serializer = null;
                var schema = TypeMapper.GetSchema();
                return schema;
            }
        }

        private sealed class FixedLengthTypeMapperInjectorWhenBuilder : IFixedLengthTypeMapperInjectorWhenBuilder
        {
            private readonly FixedLengthTypeMapperInjector selector;
            private readonly Func<object, bool> predicate;

            public FixedLengthTypeMapperInjectorWhenBuilder(FixedLengthTypeMapperInjector selector, Func<object, bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public void Use(IDynamicFixedLengthTypeMapper typeMapper)
            {
                if (typeMapper == null)
                {
                    throw new ArgumentNullException(nameof(typeMapper));
                }
                selector.Add(typeMapper, predicate);
            }
        }

        private sealed class FixedLengthTypeMapperInjectorWhenBuilder<TEntity> : IFixedLengthTypeMapperInjectorWhenBuilder<TEntity>
        {
            private static readonly Func<object, bool> typeCheck = o => o is TEntity;
            private readonly FixedLengthTypeMapperInjector selector;
            private readonly Func<object, bool> predicate;

            public FixedLengthTypeMapperInjectorWhenBuilder(FixedLengthTypeMapperInjector selector, Func<TEntity, bool>? predicate)
            {
                this.selector = selector;
                this.predicate = predicate == null ? typeCheck : o => o is TEntity entity && predicate(entity);
            }

            public void Use(IFixedLengthTypeMapper<TEntity> typeMapper)
            {
                if (typeMapper == null)
                {
                    throw new ArgumentNullException(nameof(typeMapper));
                }
                var dynamicMapper = (IDynamicFixedLengthTypeMapper)typeMapper;
                selector.Add(dynamicMapper, predicate);
            }
        }
    }
}
