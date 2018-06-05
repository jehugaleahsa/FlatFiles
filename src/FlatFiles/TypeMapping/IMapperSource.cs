using System;

namespace FlatFiles.TypeMapping
{
    internal interface IMapperSource
    {
        IMapper GetMapper();
    }

    internal interface IMapperSource<TEntity>
    {
        IMapper<TEntity> GetMapper();
    }
}
