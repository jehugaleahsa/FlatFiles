namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    public interface IDelimitedTypeMapperInjectorWhenBuilder
    {
        /// <summary>
        /// Specifies which type mapper to use when the predicate is matched.
        /// </summary>
        /// <param name="mapper">The type mapper to use.</param>
        /// <returns>The type mapper selector.</returns>
        void Use(IDynamicDelimitedTypeMapper mapper);
    }

    /// <summary>
    /// Allows specifying which schema to use when a predicate is matched.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity mapped by the mapper.</typeparam>
    public interface IDelimitedTypeMapperInjectorWhenBuilder<TEntity>
    {
        /// <summary>
        /// Specifies which type mapper to use when the predicate is matched.
        /// </summary>
        /// <param name="mapper">The type mapper to use.</param>
        /// <returns>The type mapper selector.</returns>
        void Use(IDelimitedTypeMapper<TEntity> mapper);
    }
}
