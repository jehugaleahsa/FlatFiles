using System;

namespace FlatFiles.TypeMapping
{
    internal interface ICodeGenerator
    {
        Func<TEntity> GetFactory<TEntity>();

        Action<IRecordContext, TEntity, object?[]> GetReader<TEntity>(IMemberMapping[] mappings);

        Action<IRecordContext, TEntity, object?[]> GetWriter<TEntity>(IMemberMapping[] mappings);
    }
}
