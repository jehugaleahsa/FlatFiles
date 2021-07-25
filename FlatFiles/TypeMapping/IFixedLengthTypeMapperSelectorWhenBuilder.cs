using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface IFixedLengthTypeMapperSelectorWhenBuilder
    {
        /// <summary>
        /// Specifies which type mapper to use when the predicate is matched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity mapped by the mapper.</typeparam>
        /// <param name="mapper">The type mapper to use.</param>
        /// <exception cref="ArgumentNullException">The mapper is null.</exception>
        /// <returns>The type mapper selector.</returns>
        void Use<TEntity>(IFixedLengthTypeMapper<TEntity> mapper);

        /// <summary>
        /// Specifies which type mapper to use when the predicate is matched.
        /// </summary>
        /// <param name="mapper">The type mapper to use.</param>
        /// <exception cref="ArgumentNullException">The mapper is null.</exception>
        /// <returns>The type mapper selector.</returns>
        void Use(IDynamicFixedLengthTypeMapper mapper);
    }
}
