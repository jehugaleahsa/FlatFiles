namespace FlatFiles.TypeMapping
{
    internal interface ITypeMatcherContext
    {
        int LogicalCount { get; }

        void Serialize(IRecordContext context, object? value, object?[] values);
    }
}
