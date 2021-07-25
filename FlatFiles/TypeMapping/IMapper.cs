using System;

namespace FlatFiles.TypeMapping
{
    internal interface IMapper
    {
        IMemberAccessor? Member { get; }

        int LogicalCount { get; }

        Func<IRecordContext, object?[], object?> GetReader();

        Action<IRecordContext, object?, object?[]> GetWriter();
    }

    internal interface IMapper<TEntity> : IMapper
    {
        new Func<IRecordContext, object?[], TEntity> GetReader();

        new Action<IRecordContext, TEntity, object?[]> GetWriter();
    }
}
