using System;

namespace FlatFiles
{
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
}
