using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    internal class UntypedWriter<TEntity> : ITypedWriter<object>
    {
        private readonly ITypedWriter<TEntity> _writer;

        public UntypedWriter(ITypedWriter<TEntity> writer)
        {
            _writer = writer;
        }

        public ISchema GetSchema()
        {
            return _writer.GetSchema();
        }

        public void Write(object entity)
        {
            _writer.Write((TEntity)entity);
        }

        public async Task WriteAsync(object entity)
        {
            await _writer.WriteAsync((TEntity)entity).ConfigureAwait(false);
        }
    }
}
