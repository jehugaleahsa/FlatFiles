﻿using System;

namespace FlatFiles
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface IDelimitedSchemaInjectorWhenBuilder
    {
        /// <summary>
        /// Specifies which schema to use when the predicate is matched.
        /// </summary>
        /// <param name="schema">The schema to use.</param>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        void Use(DelimitedSchema schema);
    }
}
