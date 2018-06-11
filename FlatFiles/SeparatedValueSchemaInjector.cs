using System;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface ISeparatedValueSchemaInjectorWhenBuilder
    {
        /// <summary>
        /// Specifies which schema to use when the predicate is matched.
        /// </summary>
        /// <param name="schema">The schema to use.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        void Use(SeparatedValueSchema schema);
    }

    /// <summary>
    /// Represents a class that can dynamically provide the schema based on the shape of the data being written.
    /// </summary>
    public class SeparatedValueSchemaInjector
    {
        private readonly static SchemaMatcher nonMatcher = new SchemaMatcher() { Predicate = (values) => false };
        private readonly List<SchemaMatcher> matchers;
        private SchemaMatcher defaultMatcher;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueSchemaInjector.
        /// </summary>
        public SeparatedValueSchemaInjector()
        {
            this.matchers = new List<SchemaMatcher>();
            this.defaultMatcher = nonMatcher;
        }

        /// <summary>
        /// Indicates that the given schema should be used when the predicate returns true.
        /// </summary>
        /// <param name="predicate">Indicates whether the schema should be used for a record.</param>
        /// <returns>An object for specifying which schema to use when the predicate matches.</returns>
        /// <exception cref="System.ArgumentNullException">The predicate is null.</exception>
        /// <remarks>Previously registered schemas will be used if their predicates match.</remarks>
        public ISeparatedValueSchemaInjectorWhenBuilder When(Func<object[], bool> predicate)
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
        public void WithDefault(SeparatedValueSchema schema)
        {
            defaultMatcher = schema == null ? nonMatcher : new SchemaMatcher() { Predicate = (values) => true, Schema = schema };
        }

        private SchemaMatcher Add(SeparatedValueSchema schema, Func<object[], bool> predicate)
        {
            var matcher = new SchemaMatcher()
            {
                Schema = schema,
                Predicate = predicate
            };
            matchers.Add(matcher);
            return matcher;
        }

        internal SeparatedValueSchema GetSchema(object[] values)
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

        private class SchemaMatcher
        {
            public SeparatedValueSchema Schema { get; set; }

            public Func<object[], bool> Predicate { get; set; }
        }

        private class SeparatedValueSchemaInjectorWhenBuilder : ISeparatedValueSchemaInjectorWhenBuilder
        {
            private readonly SeparatedValueSchemaInjector selector;
            private readonly Func<object[], bool> predicate;

            public SeparatedValueSchemaInjectorWhenBuilder(SeparatedValueSchemaInjector selector, Func<object[], bool> predicate)
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
