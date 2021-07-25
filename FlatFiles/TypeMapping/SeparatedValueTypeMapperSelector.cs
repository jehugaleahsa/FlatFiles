using System;
using System.Collections.Generic;
using System.IO;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents a class that can dynamically map types based on the shap of the record.
    /// </summary>
    public sealed class SeparatedValueTypeMapperSelector
    {
        private readonly List<TypeMapperMatcher> matchers = new();
        private IDynamicSeparatedValueTypeMapper? defaultMapper;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueTypeMapperSelector.
        /// </summary>
        public SeparatedValueTypeMapperSelector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public ISeparatedValueTypeMapperSelectorWhenBuilder When(Func<string[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new SeparatedValueTypeMapperSelectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default type mapper to use.</param>
        public void WithDefault<TEntity>(ISeparatedValueTypeMapper<TEntity>? typeMapper)
        {
            defaultMapper = (IDynamicSeparatedValueTypeMapper?)typeMapper;
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default schema to use.</param>
        public void WithDefault(IDynamicSeparatedValueTypeMapper? typeMapper)
        {
            defaultMapper = typeMapper;
        }

        /// <summary>
        /// Gets a typed reader for reading the objects from the file.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="options">The separate value options to use.</param>
        /// <returns>The typed reader.</returns>
        public ISeparatedValueTypedReader<object> GetReader(TextReader reader, SeparatedValueOptions? options = null)
        {
            var selector = new SeparatedValueSchemaSelector();
            var valueReader = new SeparatedValueReader(reader, selector, options);
            var multiReader = new MultiplexingSeparatedValueTypedReader(valueReader);
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

        private Func<Func<IRecordContext, object?[], object?>> GetReader(IDynamicSeparatedValueTypeMapper defaultMapper)
        {
            var source = (IMapperSource)defaultMapper;
            var reader = source.GetMapper();
            return () => reader.GetReader();
        }

        internal void Add(IDynamicSeparatedValueTypeMapper typeMapper, Func<string[], bool> predicate)
        {
            matchers.Add(new TypeMapperMatcher(typeMapper, predicate));
        }

        private sealed class TypeMapperMatcher
        {
            public TypeMapperMatcher(IDynamicSeparatedValueTypeMapper typeMapper, Func<string[], bool> predicate)
            {
                TypeMapper = typeMapper;
                Predicate = predicate;
            }

            public IDynamicSeparatedValueTypeMapper TypeMapper { get; }

            public Func<string[], bool> Predicate { get; }
        }

        private sealed class SeparatedValueTypeMapperSelectorWhenBuilder : ISeparatedValueTypeMapperSelectorWhenBuilder
        {
            private readonly SeparatedValueTypeMapperSelector selector;
            private readonly Func<string[], bool> predicate;

            public SeparatedValueTypeMapperSelectorWhenBuilder(SeparatedValueTypeMapperSelector selector, Func<string[], bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public void Use<TEntity>(ISeparatedValueTypeMapper<TEntity> typeMapper)
            {
                var dynamicMapper = (IDynamicSeparatedValueTypeMapper)typeMapper;
                Use(dynamicMapper);
            }

            public void Use(IDynamicSeparatedValueTypeMapper typeMapper)
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
