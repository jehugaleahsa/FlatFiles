using System;

namespace FlatFiles.TypeMapping
{
    internal class UntypedWriter<TEntity> : ITypedWriter<object>
    {
        private readonly ITypedWriter<TEntity> writer;

        public UntypedWriter(ITypedWriter<TEntity> writer)
        {
            this.writer = writer;
        }

        public ISchema GetSchema()
        {
            return writer.GetSchema();
        }

        public void Write(object entity)
        {
            writer.Write((TEntity)entity);
        }
    }
}
