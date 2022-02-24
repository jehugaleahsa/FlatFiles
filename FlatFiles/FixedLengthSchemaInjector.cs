using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of the data being written.
    /// </summary>
    public sealed class FixedLengthSchemaInjector
    {
        private readonly List<SchemaMatcher> matchers = new();
        private SchemaMatcher? defaultMatcher;

        /// <summary>
        /// Initializes a new instance of a FixedLengthSchemaInjector.
        /// </summary>
        public FixedLengthSchemaInjector()
        {
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public IFixedLengthSchemaInjectorWhenBuilder When(Func<object?[], bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new FixedLengthSchemaInjectorWhenBuilder(this, predicate);
        }

        /// <summary>
        /// Provides the schema to use by default when no other matches are found.
        /// </summary>
        /// <param name="schema">The default schema to use.</param>
        /// <returns>The current selector to allow for further customization.</returns>
        public void WithDefault(FixedLengthSchema schema)
        {
            if (schema == null)
            {
                defaultMatcher = null;
            }
            else
            {
                defaultMatcher = new SchemaMatcher(schema, values => true);
            }
        }

        private void Add(FixedLengthSchema schema, Func<object?[], bool> predicate)
        {
            var matcher = new SchemaMatcher(schema, predicate);
            matchers.Add(matcher);
        }

        internal FixedLengthSchema GetSchema(object?[] values)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Predicate(values))
                {
                    return matcher.Schema;
                }
            }
            if (defaultMatcher != null && defaultMatcher.Predicate(values))
            {
                return defaultMatcher.Schema;
            }
            throw new FlatFileException(Resources.MissingMatcher);
        }

        private sealed class SchemaMatcher
        {
            public SchemaMatcher(FixedLengthSchema schema, Func<object?[], bool> predicate)
            {
                Schema = schema;
                Predicate = predicate;
            }

            public FixedLengthSchema Schema { get; }

            public Func<object?[], bool> Predicate { get; }
        }

        private sealed class FixedLengthSchemaInjectorWhenBuilder : IFixedLengthSchemaInjectorWhenBuilder
        {
            private readonly FixedLengthSchemaInjector injector;
            private readonly Func<object?[], bool> predicate;

            public FixedLengthSchemaInjectorWhenBuilder(FixedLengthSchemaInjector injector, Func<object?[], bool> predicate)
            {
                this.injector = injector;
                this.predicate = predicate;
            }

            public void Use(FixedLengthSchema schema)
            {
                if (schema == null)
                {
                    throw new ArgumentNullException(nameof(schema));
                }
                injector.Add(schema, predicate);
            }
        }
    }
}
