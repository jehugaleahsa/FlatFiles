using System;

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
        /// <exception cref="ArgumentNullException">Schema is null.</exception>
        IFixedLengthSchemaSelectorUseBuilder Use(FixedLengthSchema schema);
    }
}
