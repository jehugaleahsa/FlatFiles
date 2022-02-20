namespace FlatFiles
{
    internal interface IRecordSeparatorMatcher
    {
        int Size { get; }

        bool IsMatch();

        string Trim(string value);
    }
}
