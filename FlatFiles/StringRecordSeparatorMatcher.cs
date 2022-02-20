namespace FlatFiles
{
    internal sealed class StringRecordSeparatorMatcher : IRecordSeparatorMatcher
    {
        private readonly RetryReader reader;
        private readonly string separator;

        public StringRecordSeparatorMatcher(RetryReader reader, string separator)
        {
            this.reader = reader;
            this.separator = separator;
        }

        public int Size => separator.Length;

        public bool IsMatch()
        {
            return reader.IsMatch(separator);
        }

        public string Trim(string value)
        {
            if (value.EndsWith(separator))
            {
                return value.Substring(0, value.Length - separator.Length);
            }
            return value;
        }
    }
}
