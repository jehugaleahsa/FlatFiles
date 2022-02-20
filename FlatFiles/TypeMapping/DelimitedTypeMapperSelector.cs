using System;
using System.Collections.Generic;
using System.IO;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents a class that can dynamically map types based on the shap of the record.
    /// </summary>
    public sealed class DelimitedTypeMapperSelector
    {
        private readonly List<TypeMapperMatcher> matchers = new();
        private IDynamicDelimitedTypeMapper? defaultMapper;

        /// <summary>
        /// Initializes a new instance of a DelimitedTypeMapperSelector.
        /// </summary>
        public DelimitedTypeMapperSelector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IDelimitedTypeMapperSelectorWhenBuilder When(Func<string[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new DelimitedTypeMapperSelectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default type mapper to use.</param>
        public void WithDefault<TEntity>(IDelimitedTypeMapper<TEntity>? typeMapper)
        {
            defaultMapper = (IDynamicDelimitedTypeMapper?)typeMapper;
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default schema to use.</param>
        public void WithDefault(IDynamicDelimitedTypeMapper? typeMapper)
        {
            defaultMapper = typeMapper;
        }

        /// <summary>
        /// Gets a typed reader for reading the objects from the file.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="options">The separate value options to use.</param>
        /// <returns>The typed reader.</returns>
        public IDelimitedTypedReader<object> GetReader(TextReader reader, DelimitedOptions? options = null)
        {
            var selector = new DelimitedSchemaSelector();
            var valueReader = new DelimitedReader(reader, selector, options);
            var multiReader = new MultiplexingDelimitedTypedReader(valueReader);
            foreach (var matcher in matchers)
            {
                var typedReader = new Lazy<Func<IRecordContext, object?[], object?>>(GetReader(matcher.TypeMapper));
                selector.When(matcher.Predicate).Use(matcher.TypeMapper.GetSchema()).OnMatch(() => multiReader.Deserializer = typedReader.Value);
            }
            if (defaultMapper != null)
            {
                var typeReader = new Lazy<Func<IRecordContext, object?[], object?>>(GetReader(defaultMapper));
                selector.WithDefault(defaultMapper.GetSchema()).OnMatch(() => multiReader.Deserializer = typeReader.Value);
            }
            return multiReader;
        }

        private Func<Func<IRecordContext, object?[], object?>> GetReader(IDynamicDelimitedTypeMapper defaultMapper)
        {
            var source = (IMapperSource)defaultMapper;
            var reader = source.GetMapper();
            return () => reader.GetReader();
        }

        internal void Add(IDynamicDelimitedTypeMapper typeMapper, Func<string[], bool> predicate)
        {
            matchers.Add(new TypeMapperMatcher(typeMapper, predicate));
        }

        private sealed class TypeMapperMatcher
        {
            public TypeMapperMatcher(IDynamicDelimitedTypeMapper typeMapper, Func<string[], bool> predicate)
            {
                TypeMapper = typeMapper;
                Predicate = predicate;
            }

            public IDynamicDelimitedTypeMapper TypeMapper { get; }

            public Func<string[], bool> Predicate { get; }
        }

        private sealed class DelimitedTypeMapperSelectorWhenBuilder : IDelimitedTypeMapperSelectorWhenBuilder
        {
            private readonly DelimitedTypeMapperSelector selector;
            private readonly Func<string[], bool> predicate;

            public DelimitedTypeMapperSelectorWhenBuilder(DelimitedTypeMapperSelector selector, Func<string[], bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public void Use<TEntity>(IDelimitedTypeMapper<TEntity> typeMapper)
            {
                var dynamicMapper = (IDynamicDelimitedTypeMapper)typeMapper;
                Use(dynamicMapper);
            }

            public void Use(IDynamicDelimitedTypeMapper typeMapper)
            {
                if (typeMapper == null)
                {
                    throw new ArgumentNullException(nameof(typeMapper));
                }
                selector.Add(typeMapper, predicate);
            }
        }
    }
}
