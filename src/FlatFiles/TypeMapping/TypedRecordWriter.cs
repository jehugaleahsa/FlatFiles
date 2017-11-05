using System;
using System.Linq;

namespace FlatFiles.TypeMapping
{
    internal class TypedRecordWriter<TEntity>
    {
        private readonly Action<TEntity, object[]> getter;

        public TypedRecordWriter(ICodeGenerator codeGenerator, IMemberMapping[] mappings)
        {
            this.getter = codeGenerator.GetWriter<TEntity>(mappings);
            this.MemberCount = mappings.Where(x => x.Member != null).Count();
        }

        public int MemberCount { get; private set; }

        public void Write(TEntity entity, object[] values)
        {
            getter(entity, values);
        }
    }
}
