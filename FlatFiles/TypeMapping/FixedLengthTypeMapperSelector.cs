using System;
using System.Collections.Generic;
using System.IO;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents a class that can dynamically map types based on the shap of the record.
    /// </summary>
    public sealed class FixedLengthTypeMapperSelector
    {
        private readonly List<TypeMapperMatcher> matchers = new();
        private IDynamicFixedLengthTypeMapper? defaultMapper;

        /// <summary>
        /// Initializes a new instance of a FixedLengthTypeMapperSelector.
        /// </summary>
        public FixedLengthTypeMapperSelector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IFixedLengthTypeMapperSelectorWhenBuilder When(Func<string, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new FixedLengthTypeMapperSelectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default type mapper to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public void WithDefault<TEntity>(IFixedLengthTypeMapper<TEntity>? typeMapper)
        {
            defaultMapper = (IDynamicFixedLengthTypeMapper?)typeMapper;
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public void WithDefault(IDynamicFixedLengthTypeMapper? typeMapper)
        {
            defaultMapper = typeMapper;
        }

        /// <summary>
        /// Gets a typed reader for reading the objects from the file.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="options">The separate value options to use.</param>
        /// <returns>The typed reader.</returns>
        public IFixedLengthTypedReader<object?> GetReader(TextReader reader, FixedLengthOptions? options = null)
        {
            var selector = new FixedLengthSchemaSelector();
            var valueReader = new FixedLengthReader(reader, selector, options);
            var multiReader = new MultiplexingFixedLengthTypedReader(valueReader);
            foreach (var matcher in matchers)
            {
                var typedReader = new Lazy<Func<IRecordContext, object?[], object?>>(GetReader(matcher.TypeMapper));
                selector.When(matcher.Predicate).Use(matcher.TypeMapper.GetSchema()).OnMatch(() => multiReader.Deserializer = typedReader.Value);
            }
            if (defaultMapper != null)
            {
                var typeReader = new Lazy<Func<IRecordContext, object?[], object?>>(GetReader (defaultMapper));
                selector.WithDefault(defaultMapper.GetSchema()).OnMatch(() => multiReader.Deserializer = typeReader.Value);
            }
            return multiReader;
        }

        private Func<Func<IRecordContext, object?[], object?>> GetReader(IDynamicFixedLengthTypeMapper defaultMapper)
        {
            var source = (IMapperSource)defaultMapper;
            var reader = source.GetMapper();
            return () => reader.GetReader();
        }

        internal void Add(IDynamicFixedLengthTypeMapper typeMapper, Func<string, bool> predicate)
        {
            matchers.Add(new TypeMapperMatcher(typeMapper, predicate));
        }

        private sealed class TypeMapperMatcher
        {
            public TypeMapperMatcher(IDynamicFixedLengthTypeMapper typeMapper, Func<string, bool> predicate)
            {
                TypeMapper = typeMapper;
                Predicate = predicate;
            }

            public IDynamicFixedLengthTypeMapper TypeMapper { get; }

            public Func<string, bool> Predicate { get; }
        }

        private sealed class FixedLengthTypeMapperSelectorWhenBuilder : IFixedLengthTypeMapperSelectorWhenBuilder
        {
            private readonly FixedLengthTypeMapperSelector selector;
            private readonly Func<string, bool> predicate;

            public FixedLengthTypeMapperSelectorWhenBuilder(FixedLengthTypeMapperSelector selector, Func<string, bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public void Use<TEntity>(IFixedLengthTypeMapper<TEntity> typeMapper)
            {
                var dynamicMapper = (IDynamicFixedLengthTypeMapper)typeMapper;
                Use(dynamicMapper);
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
    }
}
