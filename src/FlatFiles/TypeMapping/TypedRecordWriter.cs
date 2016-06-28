using System;
using System.Collections.Generic;

namespace FlatFiles.TypeMapping
{
    internal class TypedRecordWriter<TEntity>
    {
        private readonly Func<TEntity, object[]> getter;

        public TypedRecordWriter(List<IPropertyMapping> mappings)
        {
            this.getter = CodeGenerator.GetWriter<TEntity>(mappings);
        }

        public object[] Write(TEntity entity)
        {
            object[] values = getter(entity);
            return values;
        }
    }
}
