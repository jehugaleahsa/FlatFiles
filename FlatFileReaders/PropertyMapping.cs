using System;
using FlatFileReaders.Properties;
using System.Reflection;

namespace FlatFileReaders
{
    /// <summary>
    /// Allows a property to be mapped to a column.
    /// </summary>
    /// <typeparam name="TType">The type whose properties are being mapped.</typeparam>
    /// <typeparam name="TProp">The type of the property being mapped.</typeparam>
    internal sealed class PropertyMapping<TType, TProp> : IPropertyMapping<TType, TProp>
    {
        private readonly Mapper<TType> builder;
        private readonly PropertyInfo property;

        /// <summary>
        /// Initializes a new instance of a PropertyMapping.
        /// </summary>
        /// <param name="builder">The schema builder being configured.</param>
        /// <param name="property">The property being mapped.</param>
        public PropertyMapping(Mapper<TType> builder, PropertyInfo property)
        {
            this.builder = builder;
            this.property = property;
        }

        /// <summary>
        /// Maps the property to the column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>A class that allows a different converter to be specified.</returns>
        public Mapper<TType> To(string columnName)
        {
            if (String.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException(Resources.BlankColumnName, "columnName");
            }
            CustomColumnConfiguration configuration = builder.FindConfiguration(property.Name);
            if (configuration == null)
            {
                configuration = new CustomColumnConfiguration();
                configuration.Property = property;
                builder.AddConfiguration(configuration);
            }
            configuration.ColumnName = columnName;
            return builder;
        }
    }
}
