using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface ISeparatedValueSchemaSelectorWhenBuilder
    {
        /// <summary>
        /// Specifies which schema to use when the predicate is matched.
        /// </summary>
        /// <param name="schema">The schema to use.</param>
        /// <returns>The builder for further configuration.</returns>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        ISeparatedValueSchemaSelectorUseBuilder Use(SeparatedValueSchema schema);
    }

    /// <summary>
    /// Allows specifying additional actions to take when a predicate is matched.
    /// </summary>
    public interface ISeparatedValueSchemaSelectorUseBuilder
    {
        /// <summary>
        /// Register a method to fire whenever a match is made.
        /// </summary>
        /// <param name="action">The action to take.</param>
        void OnMatch(Action action);
    }

    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of a read record.
    /// </summary>
    public class SeparatedValueSchemaSelector
    {
        private readonly static SchemaMatcher nonMatcher = new SchemaMatcher { Predicate = values => false };
        private readonly List<SchemaMatcher> matchers = new List<SchemaMatcher>();
        private SchemaMatcher defaultMatcher = nonMatcher;

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="System.ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public ISeparatedValueSchemaSelectorWhenBuilder When(Func<string[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new SeparatedValueSchemaSelectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="schema">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public ISeparatedValueSchemaSelectorUseBuilder WithDefault(SeparatedValueSchema schema)
        {
            defaultMatcher = schema == null ? nonMatcher : new SchemaMatcher { Predicate = values => true, Schema = schema };
            return new SeparatedValueSchemaSelectorUseBuilder(defaultMatcher);
        }

        private SchemaMatcher Add(SeparatedValueSchema schema, Func<string[], bool> predicate)
        {
            var matcher = new SchemaMatcher
            {
                Schema = schema,
                Predicate = predicate
            };
            matchers.Add(matcher);
            return matcher;
        }

        internal SeparatedValueSchema GetSchema(string[] values)
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
            throw new FlatFileException(Resources.MissingMatcher);
        }

        private class SchemaMatcher
        {
            public SeparatedValueSchema Schema { get; set; }

            public Func<string[], bool> Predicate { get; set; }

            public Action Action { get; set; }
        }

        private class SeparatedValueSchemaSelectorWhenBuilder : ISeparatedValueSchemaSelectorWhenBuilder
        {
            private readonly SeparatedValueSchemaSelector selector;
            private readonly Func<string[], bool> predicate;

            public SeparatedValueSchemaSelectorWhenBuilder(SeparatedValueSchemaSelector selector, Func<string[], bool> predicate)
            {
                this.selector = selector;
                this.predicate = predicate;
            }

            public ISeparatedValueSchemaSelectorUseBuilder Use(SeparatedValueSchema schema)
            {
                if (schema == null)
                {
                    throw new ArgumentNullException(nameof(schema));
                }
                var matcher = selector.Add(schema, predicate);
                return new SeparatedValueSchemaSelectorUseBuilder(matcher);
            }
        }

        private class SeparatedValueSchemaSelectorUseBuilder : ISeparatedValueSchemaSelectorUseBuilder
        {
            private readonly SchemaMatcher matcher;

            public SeparatedValueSchemaSelectorUseBuilder(SchemaMatcher matcher)
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
