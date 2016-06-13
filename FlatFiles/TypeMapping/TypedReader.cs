using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    ///  Represents a reader that will generate entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being read.</typeparam>
    public interface ITypedReader<TEntity>
    {
        /// <summary>
        /// Gets the schema being used by the parser to parse record values.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        ISchema GetSchema();

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was read; otherwise, false if the end of file was reached.</returns>
        bool Read();

        /// <summary>
        /// Skips the next record from the file.
        /// </summary>
        /// <returns>True if the next record was skipped; otherwise, false if the end of the file was reached.</returns>
        bool Skip();

        /// <summary>
        /// Gets the last read entity.
        /// </summary>
        TEntity Current { get; }
    }

    internal sealed class TypedReader<TEntity> : ITypedReader<TEntity>
    {
        private readonly IReader reader;
        private readonly IRecordReader<TEntity> deserializer;
        private TEntity current;

        public TypedReader(IReader reader, IRecordReader<TEntity> deserializer)
        {
            this.reader = reader;
            this.deserializer = deserializer;
        }

        public ISchema GetSchema()
        {
            return reader.GetSchema();
        }

        public bool Read()
        {
            if (!reader.Read())
            {
                return false;
            }
            object[] values = reader.GetValues();
            current = deserializer.Read(values);
            return true;
        }

        public bool Skip()
        {
            return reader.Skip();
        }

        public TEntity Current
        {
            get { return current; }
        }
    }
}
