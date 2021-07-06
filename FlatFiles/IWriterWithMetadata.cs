namespace FlatFiles
{
    internal interface IWriterWithMetadata : IWriter
    {
        IRecordContext GetMetadata();
    }
}
