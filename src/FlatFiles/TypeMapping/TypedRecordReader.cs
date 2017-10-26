using System;
using System.Collections.Generic;

namespace FlatFiles.TypeMapping
{
    internal class TypedRecordReader<TEntity>
    {
        private readonly Func<TEntity> factory;
        private readonly Action<TEntity, object[]> setter;

        public TypedRecordReader(
            Func<TEntity> factory,
            ICodeGenerator codeGenerator,
            List<IPropertyMapping> mappings)
        {
            this.factory = factory;
            this.setter = codeGenerator.GetReader<TEntity>(mappings);
        }

        public TEntity Read(object[] values)
        {
            TEntity entity = factory();
            setter(entity, values);
            return entity;
        }
    }
}
