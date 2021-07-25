using System;
using System.Collections.Generic;

namespace FlatFiles
{
    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of a read record.
    /// </summary>
    public sealed class FixedLengthSchemaSelector
    {
        private readonly List<SchemaMatcher> matchers = new();
        private SchemaMatcher? defaultMatcher;

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IFixedLengthSchemaSelectorWhenBuilder When(Func<string, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new FixedLengthSchemaSelectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="schema">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public IFixedLengthSchemaSelectorUseBuilder WithDefault(FixedLengthSchema schema)
        {
            if (schema == null)
            {
                defaultMatcher = null;
            }
            else
            {
                defaultMatcher = new SchemaMatcher(schema, values => true);
            }
            return new FixedLengthSchemaSelectorUseBuilder(defaultMatcher);
        }

        private SchemaMatcher Add(FixedLengthSchema schema, Func<string, bool> predicate)
        {
            var matcher = new SchemaMatcher(schema, predicate);
            matchers.Add(matcher);
            return matcher;
        }

        internal FixedLengthSchema? GetSchema(string record)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Predicate(record))
                {
                    matcher.Action?.Invoke();
                    return matcher.Schema;
                }
            }
            if (defaultMatcher != null && defaultMatcher.Predicate(record))
            {
                defaultMatcher.Action?.Invoke();
                return defaultMatcher.Schema;
            }
            return null;
        }

        private sealed class SchemaMatcher
        {
            public SchemaMatcher(FixedLengthSchema schema, Func<string, bool> predicate)
            {
                Schema = schema;
                Predicate = predicate;
            }

            public FixedLengthSchema Schema { get; }

            public Func<string, bool> Predicate { get; }

            public Action? Action { get; set; }
        }

        private sealed class FixedLengthSchemaSelectorWhenBuilder : IFixedLengthSchemaSelectorWhenBuilder
        {
            private readonly FixedLengthSchemaSelector selector;
            private readonly Func<string, bool> predicate;

            public FixedLengthSchemaSelectorWhenBuilder(FixedLengthSchemaSelector selector, Func<string, bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public IFixedLengthSchemaSelectorUseBuilder Use(FixedLengthSchema schema)
            {
                if (schema == null)
                {
                    throw new ArgumentNullException(nameof(schema));
                }
                var matcher = selector.Add(schema, predicate);
                return new FixedLengthSchemaSelectorUseBuilder(matcher);
            }
        }

        private sealed class FixedLengthSchemaSelectorUseBuilder : IFixedLengthSchemaSelectorUseBuilder
        {
            private readonly SchemaMatcher? matcher;

            public FixedLengthSchemaSelectorUseBuilder(SchemaMatcher? matcher)
            {
                this.matcher = matcher;
            }

            public void OnMatch(Action? action)
            {
                if (matcher != null)
                {
                    matcher.Action = action;
                }
            }
        }
    }
}
