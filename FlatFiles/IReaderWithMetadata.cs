namespace FlatFiles
{
    internal interface IReaderWithMetadata : IReader
    {
        IRecordContext GetMetadata();
    }
}
