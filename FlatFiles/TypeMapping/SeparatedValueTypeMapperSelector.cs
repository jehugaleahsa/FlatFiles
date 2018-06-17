using System;
using System.Collections.Generic;
using System.IO;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface ISeparatedValueTypeMapperSelectorWhenBuilder
    {
        /// <summary>
        /// Specifies which type mapper to use when the predicate is matched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity mapped by the mapper.</typeparam>
        /// <param name="mapper">The type mapper to use.</param>
        /// <returns>The type mapper selector.</returns>
        void Use<TEntity>(ISeparatedValueTypeMapper<TEntity> mapper);

        /// <summary>
        /// Specifies which type mapper to use when the predicate is matched.
        /// </summary>
        /// <param name="mapper">The type mapper to use.</param>
        /// <returns>The type mapper selector.</returns>
        void Use(IDynamicSeparatedValueTypeMapper mapper);
    }

    /// <summary>
    /// Represents a class that can dynamically map types based on the shap of the record.
    /// </summary>
    public class SeparatedValueTypeMapperSelector
    {
        private readonly List<TypeMapperMatcher> _matchers = new List<TypeMapperMatcher>();
        private IDynamicSeparatedValueTypeMapper _defaultMapper;

        internal Func<object[], object> Reader { get; set; }

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
        public void WithDefault<TEntity>(ISeparatedValueTypeMapper<TEntity> typeMapper)
        {
            _defaultMapper = (IDynamicSeparatedValueTypeMapper)typeMapper;
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="typeMapper">The default schema to use.</param>
        public void WithDefault(IDynamicSeparatedValueTypeMapper typeMapper)
        {
            _defaultMapper = typeMapper;
        }

        /// <summary>
        /// Gets a typed reader for reading the objects from the file.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="options">The separate value options to use.</param>
        /// <returns>The typed reader.</returns>
        public ISeparatedValueTypedReader<object> GetReader(TextReader reader, SeparatedValueOptions options = null)
        {
            var selector = new SeparatedValueSchemaSelector();
            var valueReader = new SeparatedValueReader(reader, selector, options);
            var multiReader = new MultiplexingSeparatedValueTypedReader(valueReader, this);
            foreach (var matcher in _matchers)
            {
                var typedReader = new Lazy<Func<object[], object>>(GetReader(matcher.TypeMapper));
                selector.When(matcher.Predicate).Use(matcher.TypeMapper.GetSchema()).OnMatch(() => Reader = typedReader.Value);
            }
            if (_defaultMapper != null)
            {
                var typeReader = new Lazy<Func<object[], object>>(GetReader(_defaultMapper));
                selector.WithDefault(_defaultMapper.GetSchema()).OnMatch(() => Reader = typeReader.Value);
            }
            return multiReader;
        }

        private Func<Func<object[], object>> GetReader(IDynamicSeparatedValueTypeMapper defaultMapper)
        {
            var source = (IMapperSource)defaultMapper;
            var reader = source.GetMapper();
            return () => reader.GetReader();
        }

        internal void Add(IDynamicSeparatedValueTypeMapper typeMapper, Func<string[], bool> predicate)
        {
            _matchers.Add(new TypeMapperMatcher
            {
                TypeMapper = typeMapper,
                Predicate = predicate
            });
        }

        private class TypeMapperMatcher
        {
            public IDynamicSeparatedValueTypeMapper TypeMapper { get; set; }

            public Func<string[], bool> Predicate { get; set; }
        }

        private class SeparatedValueTypeMapperSelectorWhenBuilder : ISeparatedValueTypeMapperSelectorWhenBuilder
        {
            private readonly SeparatedValueTypeMapperSelector _selector;
            private readonly Func<string[], bool> _predicate;

            public SeparatedValueTypeMapperSelectorWhenBuilder(SeparatedValueTypeMapperSelector selector, Func<string[], bool> predicate)
            {
                _selector = selector;
                _predicate = predicate;
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
                _selector.Add(typeMapper, _predicate);
            }
        }
    }
}
