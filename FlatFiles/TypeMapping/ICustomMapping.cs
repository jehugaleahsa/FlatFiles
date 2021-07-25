using System;
using System.Linq.Expressions;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents a mapping that allows for specifying a custom reader and writer delegate.
    /// </summary>
    public interface ICustomMapping
    {
        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithReader(Action<object?, object?>? reader);

        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithReader(Action<IColumnContext?, object?, object?>? reader);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Action<object?, object?[]>? writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Action<IColumnContext?, object?, object?[]>? writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Func<object?, object?>? writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Func<IColumnContext?, object?, object?>? writer);
    }

    /// <summary>
    /// Represents a mapping that allows for specifying a custom reader and writer delegate.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being mapped.</typeparam>
    public interface ICustomMapping<TEntity>
    {
        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <typeparam name="TProp">The type of the property being read.</typeparam>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithReader<TProp>(Expression<Func<TEntity, TProp>>? reader);

        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithReader(Action<TEntity, object?>? reader);

        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithReader(Action<IColumnContext?, TEntity, object?>? reader);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter(Action<TEntity, object?[]>? writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter(Action<IColumnContext?, TEntity, object?[]>? writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <typeparam name="TProp">The type of the property being read.</typeparam>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter<TProp>(Func<TEntity, TProp>? writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <typeparam name="TProp">The type of the property being read.</typeparam>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter<TProp>(Func<IColumnContext?, TEntity, TProp>? writer);
    }
}
