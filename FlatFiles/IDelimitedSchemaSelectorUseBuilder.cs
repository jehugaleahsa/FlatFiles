using System;

namespace FlatFiles
{
    /// <summary>
    /// Allows specifying additional actions to take when a predicate is matched.
    /// </summary>
    public interface IDelimitedSchemaSelectorUseBuilder
    {
        /// <summary>
        /// Register a method to fire whenever a match is made.
        /// </summary>
        /// <param name="action">The action to take.</param>
        /// <exception cref="ArgumentNullException">The action is null.</exception>
        void OnMatch(Action action);
    }
}
