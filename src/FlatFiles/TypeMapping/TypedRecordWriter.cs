using System;
using System.Collections.Generic;

namespace FlatFiles.TypeMapping
{
    internal class TypedRecordWriter<TEntity>
    {
        private readonly Func<TEntity, object[]> getter;

        public TypedRecordWriter(ICodeGenerator codeGenerator, List<IMemberMapping> mappings)
        {
            this.getter = codeGenerator.GetWriter<TEntity>(mappings);
        }

        public object[] Write(TEntity entity)
        {
            object[] values = getter(entity);
            return values;
        }
    }
}
