namespace FlatFiles
{
    internal sealed class OneCharacterRecordSeparatorMatcher : IRecordSeparatorMatcher
    {
        private readonly RetryReader reader;
        private readonly char first;

        public OneCharacterRecordSeparatorMatcher(RetryReader reader, char first)
        {
            this.reader = reader;
            this.first = first;
        }

        public int Size => 1;

        public bool IsMatch()
        {
            return reader.IsMatch1(first);
        }

        public string Trim(string value)
        {
            int length = value.Length;
            if (length >= 1 && value[length - 1] == first)
            {
                return value.Substring(0, length - 1);
            }
            return value;
        }
    }
}
