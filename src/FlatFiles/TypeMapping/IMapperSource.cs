using System;

namespace FlatFiles.TypeMapping
{
    internal interface IMapperSource<TEntity>
    {
        IMapper<TEntity> GetMapper();
    }
}
