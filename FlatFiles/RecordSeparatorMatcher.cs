namespace FlatFiles
{
    internal static class RecordSeparatorMatcher
    {
        public static IRecordSeparatorMatcher GetMatcher(RetryReader reader, string? separator)
        {
            if (separator == null)
            {
                return new DefaultRecordSeparatorMatcher(reader);
            }
            return separator.Length switch
            {
                1 => new OneCharacterRecordSeparatorMatcher(reader, separator[0]),
                2 => new TwoCharacterRecordSeparatorMatcher(reader, separator[0], separator[1]),
                _ => new StringRecordSeparatorMatcher(reader, separator),
            };
        }
    }
}
