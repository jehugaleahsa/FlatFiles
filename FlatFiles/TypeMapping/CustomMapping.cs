using System;

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
        ICustomMapping WithReader(Action<object, object> reader);

        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithReader(Action<IColumnContext, object, object> reader);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Action<object, object[]> writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Action<IColumnContext, object, object[]> writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Func<object, object> writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping WithWriter(Func<IColumnContext, object, object> writer);
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
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithReader(Action<TEntity, object> reader);

        /// <summary>
        /// Specifies the delegate used to store a value in an entity. 
        /// </summary>
        /// <param name="reader">A delegate that can populate the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithReader(Action<IColumnContext, TEntity, object> reader);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter(Action<TEntity, object[]> writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter(Action<IColumnContext, TEntity, object[]> writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter<TProp>(Func<TEntity, TProp> writer);

        /// <summary>
        /// Specifies the delegate used to store an entity value in the output.
        /// </summary>
        /// <param name="writer">A delegate that can extract a value from the entity.</param>
        /// <returns>The mapper for further customizations.</returns>
        ICustomMapping<TEntity> WithWriter<TProp>(Func<IColumnContext, TEntity, TProp> writer);
    }

    internal sealed class CustomMapping<TEntity> : ICustomMapping<TEntity>, ICustomMapping, IMemberMapping
    {
        public CustomMapping(IColumnDefinition column, int physicalIndex, int logicalIndex)
        {
            ColumnDefinition = column;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public IMemberAccessor Member => null;

        public Action<IColumnContext, object, object> Reader { get; private set; }

        public Action<IColumnContext, object, object[]> Writer { get; private set; }

        public IColumnDefinition ColumnDefinition { get; }

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }

        public ICustomMapping<TEntity> WithReader(Action<TEntity, object> reader)
        {
            Reader = reader == null ? (Action<IColumnContext, object, object>)null : (ctx, e, v) => reader((TEntity)e, v);
            return this;
        }

        public ICustomMapping<TEntity> WithReader(Action<IColumnContext, TEntity, object> reader)
        {
            Reader = reader == null ? (Action<IColumnContext, object, object>)null : (ctx, e, v) => reader(ctx, (TEntity)e, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithReader(Action<object, object> reader)
        {
            Reader = reader == null ? (Action<IColumnContext, object, object>)null : (ctx, e, v) => reader(e, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithReader(Action<IColumnContext, object, object> reader)
        {
            Reader = reader;
            return this;
        }

        public ICustomMapping<TEntity> WithWriter(Action<TEntity, object[]> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) => writer((TEntity)e, v);
            return this;
        }

        public ICustomMapping<TEntity> WithWriter(Action<IColumnContext, TEntity, object[]> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) => writer(ctx, (TEntity)e, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Action<object, object[]> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) => writer(e, v);
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Action<IColumnContext, object, object[]> writer)
        {
            Writer = writer;
            return this;
        }

        public ICustomMapping<TEntity> WithWriter<TProp>(Func<TEntity, TProp> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) =>
            {
                v[ctx.LogicalIndex] = writer((TEntity)e);
            };
            return this;
        }

        public ICustomMapping<TEntity> WithWriter<TProp>(Func<IColumnContext, TEntity, TProp> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) =>
            {
                v[ctx.LogicalIndex] = writer(ctx, (TEntity)e);
            };
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Func<object, object> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) =>
            {
                v[ctx.LogicalIndex] = writer(e);
            };
            return this;
        }

        ICustomMapping ICustomMapping.WithWriter(Func<IColumnContext, object, object> writer)
        {
            Writer = writer == null ? (Action<IColumnContext, object, object[]>)null : (ctx, e, v) =>
            {
                v[ctx.LogicalIndex] = writer(ctx, e);
            };
            return this;
        }
    }
}
