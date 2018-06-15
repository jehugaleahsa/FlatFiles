using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface IFixedLengthSchemaSelectorWhenBuilder
    {
        /// <summary>
        /// Specifies which schema to use when the predicate is matched.
        /// </summary>
        /// <param name="schema">The schema to use.</param>
        /// <returns>The builder for further configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Schema is null.</exception>
        IFixedLengthSchemaSelectorUseBuilder Use(FixedLengthSchema schema);
    }

    /// <summary>
    /// Allows specifying additional actions to take when a predicate is matched.
    /// </summary>
    public interface IFixedLengthSchemaSelectorUseBuilder
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
    public class FixedLengthSchemaSelector
    {
        private readonly static SchemaMatcher nonMatcher = new SchemaMatcher() { Predicate = (values) => false };
        private readonly List<SchemaMatcher> matchers = new List<SchemaMatcher>();
        private SchemaMatcher defaultMatcher = nonMatcher;

        /// <summary>
        /// Initializes a new instance of a FixedLengthSchemaSelector.
        /// </summary>
        public FixedLengthSchemaSelector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="System.ArgumentNullException">The predicate is null.</exception>
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
            defaultMatcher = schema == null ? nonMatcher : new SchemaMatcher() { Predicate = (values) => true, Schema = schema };
            return new FixedLengthSchemaSelectorUseBuilder(defaultMatcher);
        }

        private SchemaMatcher Add(FixedLengthSchema schema, Func<string, bool> predicate)
        {
            var matcher = new SchemaMatcher()
            {
                Schema = schema,
                Predicate = predicate
            };
            matchers.Add(matcher);
            return matcher;
        }

        internal FixedLengthSchema GetSchema(string record)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Predicate(record))
                {
                    matcher.Action?.Invoke();
                    return matcher.Schema;
                }
            }
            if (defaultMatcher.Predicate(record))
            {
                defaultMatcher.Action?.Invoke();
                return defaultMatcher.Schema;
            }
            throw new FlatFileException(Resources.MissingMatcher);
        }

        private class SchemaMatcher
        {
            public FixedLengthSchema Schema { get; set; }

            public Func<string, bool> Predicate { get; set; }

            public Action Action { get; set; }
        }

        private class FixedLengthSchemaSelectorWhenBuilder : IFixedLengthSchemaSelectorWhenBuilder
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

        private class FixedLengthSchemaSelectorUseBuilder : IFixedLengthSchemaSelectorUseBuilder
        {
            private readonly SchemaMatcher matcher;

            public FixedLengthSchemaSelectorUseBuilder(SchemaMatcher matcher)
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
