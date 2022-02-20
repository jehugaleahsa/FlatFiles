using System;
using System.Collections.Generic;

namespace FlatFiles
{
    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of a read record.
    /// </summary>
    public sealed class DelimitedSchemaSelector
    {
        private static readonly SchemaMatcher nonMatcher = new(null, values => false);
        private readonly List<SchemaMatcher> matchers = new();
        private SchemaMatcher defaultMatcher = nonMatcher;

        /// <summary>
        /// Initializes a new instance of a DelimitedSchemaSelector.
        /// </summary>
        public DelimitedSchemaSelector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IDelimitedSchemaSelectorWhenBuilder When(Func<string[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new DelimitedSchemaSelectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="schema">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public IDelimitedSchemaSelectorUseBuilder WithDefault(DelimitedSchema? schema)
        {
            if (schema == null)
            {
                defaultMatcher = nonMatcher;
            }
            else
            {
                defaultMatcher = new SchemaMatcher(schema, values => true);
            }
            return new DelimitedSchemaSelectorUseBuilder(defaultMatcher);
        }

        private SchemaMatcher Add(DelimitedSchema schema, Func<string[], bool> predicate)
        {
            var matcher = new SchemaMatcher(schema, predicate);
            matchers.Add(matcher);
            return matcher;
        }

        internal DelimitedSchema? GetSchema(string[] values)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Predicate(values))
                {
                    matcher.Action?.Invoke();
                    return matcher.Schema;
                }
            }
            if (defaultMatcher.Predicate(values))
            {
                defaultMatcher.Action?.Invoke();
                return defaultMatcher.Schema;
            }
            return null;
        }

        private sealed class SchemaMatcher
        {
            public SchemaMatcher(DelimitedSchema? schema, Func<string[], bool> predicate)
            {
                Schema = schema;
                Predicate = predicate;
            }

            public DelimitedSchema? Schema { get; }

            public Func<string[], bool> Predicate { get; }

            public Action? Action { get; set; }
        }

        private sealed class DelimitedSchemaSelectorWhenBuilder : IDelimitedSchemaSelectorWhenBuilder
        {
            private readonly DelimitedSchemaSelector selector;
            private readonly Func<string[], bool> predicate;

            public DelimitedSchemaSelectorWhenBuilder(DelimitedSchemaSelector selector, Func<string[], bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public IDelimitedSchemaSelectorUseBuilder Use(DelimitedSchema schema)
            {
                if (schema == null)
                {
                    throw new ArgumentNullException(nameof(schema));
                }
                var matcher = selector.Add(schema, predicate);
                return new DelimitedSchemaSelectorUseBuilder(matcher);
            }
        }

        private sealed class DelimitedSchemaSelectorUseBuilder : IDelimitedSchemaSelectorUseBuilder
        {
            private readonly SchemaMatcher matcher;

            public DelimitedSchemaSelectorUseBuilder(SchemaMatcher matcher)
            {
                this.matcher = matcher;
            }

            public void OnMatch(Action action)
            {
                matcher.Action = action;
            }
        }
    }
}
