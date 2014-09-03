using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents an configurator that can build a schema.
    /// </summary>
    public interface ISchemaBuilder
    {
        /// <summary>
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        ISchema GetSchema();
    }
}
