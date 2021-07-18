using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of the data being written.
    /// </summary>
    public sealed class SeparatedValueSchemaInjector
    {
        private static readonly SchemaMatcher nonMatcher = new(null, values => false);
        private readonly List<SchemaMatcher> matchers = new();
        private SchemaMatcher defaultMatcher = nonMatcher;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueSchemaInjector.
        /// </summary>
        public SeparatedValueSchemaInjector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public ISeparatedValueSchemaInjectorWhenBuilder When(Func<object?[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new SeparatedValueSchemaInjectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="schema">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public void WithDefault(SeparatedValueSchema? schema)
        {
            if (schema == null)
            {
                defaultMatcher = nonMatcher;
            }
            else
            {
                defaultMatcher = new SchemaMatcher(schema, values => true);
            }
        }

        private void Add(SeparatedValueSchema schema, Func<object?[], bool> predicate)
        {
            var matcher = new SchemaMatcher(schema, predicate);
            matchers.Add(matcher);
        }

        internal SeparatedValueSchema? GetSchema(object?[] values)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Predicate(values))
                {
                    return matcher.Schema;
                }
            }
            if (defaultMatcher.Predicate(values))
            {
                return defaultMatcher.Schema;
            }
            throw new FlatFileException(Resources.MissingMatcher);
        }

        private sealed class SchemaMatcher
        {
            public SchemaMatcher(SeparatedValueSchema? schema, Func<object?[], bool> predicate)
            {
                Schema = schema;
                Predicate = predicate;
            }

            public SeparatedValueSchema? Schema { get; }

            public Func<object?[], bool> Predicate { get; }
        }

        private sealed class SeparatedValueSchemaInjectorWhenBuilder : ISeparatedValueSchemaInjectorWhenBuilder
        {
            private readonly SeparatedValueSchemaInjector selector;
            private readonly Func<object?[], bool> predicate;

            public SeparatedValueSchemaInjectorWhenBuilder(SeparatedValueSchemaInjector selector, Func<object?[], bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public void Use(SeparatedValueSchema schema)
            {
                if (schema == null)
                {
                    throw new ArgumentNullException(nameof(schema));
                }
                selector.Add(schema, predicate);
            }
        }
    }
}
