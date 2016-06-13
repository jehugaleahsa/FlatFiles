using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents an configurator that can build a schema.
    /// </summary>
    public interface ISchemaBuilder
    {
        /// <summary>
        /// Gets the schema defined by the current configuration.
        /// </summary>
        /// <returns>The schema.</returns>
        ISchema GetSchema();
    }

    internal interface IRecordReader
    {
        object Read(object[] values);
    }

    internal interface IRecordReader<TEntity> : IRecordReader
    {
        new TEntity Read(object[] values);
    }

    internal interface IRecordWriter
    {
        object[] Write(object entity);
    }

    internal interface IRecordWriter<TEntity> : IRecordWriter
    {
        object[] Write(TEntity entity);
    }

    internal interface IRecordMapper
    {
        IRecordReader GetReader();

        IRecordWriter GetWriter();
    }
}
