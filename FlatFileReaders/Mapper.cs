using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Builds a schema that can be used to populate an object by
    /// extracting values from a file.
    /// </summary>
    /// <typeparam name="TType">The type to create the schema for.</typeparam>
    public sealed class Mapper<TType>
    {
        private Func<TType> factory;
        private readonly List<CustomColumnConfiguration> columns;

        /// <summary>
        /// Initializes a new instance of a SchemaBuilder.
        /// </summary>
        public Mapper()
        {
            this.factory = () => Activator.CreateInstance<TType>();
            this.columns = new List<CustomColumnConfiguration>();
        }

        /// <summary>
        /// Provide a factory method for creating instances of the desired type.
        /// </summary>
        /// <param name="factory">A function that creates the desired type.</param>
        /// <returns>The updated mapper.</returns>
        public Mapper<TType> WithFactory(Func<TType> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            this.factory = factory;
            return this;
        }

        /// <summary>
        /// Define a mapping for the property returned by the selector.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="propertySelector">A function that returns the property to map.</param>
        /// <returns>A class to specify which column to map to.</returns>
        public IPropertyMapping<TType, TProp> Map<TProp>(Expression<Func<TType, TProp>> propertySelector)
        {
            if (propertySelector == null)
            {
                throw new ArgumentNullException("propertySelector");
            }
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException(Resources.BadPropertySelector, "propertySelector");
            }
            PropertyInfo property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException(Resources.BadPropertySelector, "propertySelector");
            }
            return new PropertyMapping<TType, TProp>(this, property);
        }

        /// <summary>
        /// Maps the property returned by the selector to the column with the given name.
        /// </summary>
        /// <param name="configuration">The next configuration to add to the schema.</param>
        /// <returns>The schema builder.</returns>
        internal void AddConfiguration(CustomColumnConfiguration configuration)
        {
            columns.Add(configuration);
        }

        /// <summary>
        /// Finds the mapping configuration for a property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property to find the mapping for.</param>
        /// <returns>The mapping configuration -or- null if none is found.</returns>
        internal CustomColumnConfiguration FindConfiguration(string propertyName)
        {
            return columns.Find(configuration => configuration.Property.Name == propertyName);
        }

        /// <summary>
        /// Extracts the objects from the parser using the mapping information.
        /// </summary>
        /// <param name="reader">The reader to extract the data from.</param>
        /// <returns>The extracted objects.</returns>
        public IEnumerable<TType> Extract(FlatFileReader reader)
        {
            List<TType> results = new List<TType>();
            while (reader.Read())
            {
                TType instance = createInstance(reader);
                results.Add(instance);
            }
            return results;
        }

        private TType createInstance(FlatFileReader reader)
        {
            TType instance = factory();
            for (int index = 0; index != columns.Count; ++index)
            {
                CustomColumnConfiguration configuration = columns[index];
                object value = reader.GetValue(index);
                configuration.Property.SetValue(instance, value, null);
            }
            return instance;
        }
    }
}
