namespace FlatFiles
{
    internal interface ISeparatorMatcher
    {
        int Size { get; }

        bool IsMatch();

        string Trim(string value);
    }
}
