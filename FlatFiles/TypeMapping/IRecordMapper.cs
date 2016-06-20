using System;

namespace FlatFiles.TypeMapping
{
    internal interface IRecordMapper<TEntity>
    {
        TypedRecordReader<TEntity> GetReader();

        TypedRecordWriter<TEntity> GetWriter();
    }
}
