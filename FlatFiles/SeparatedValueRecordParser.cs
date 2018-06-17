using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordParser
    {
        private readonly List<string> _tokens = new List<string>();
        private readonly StringBuilder _token = new StringBuilder();
        private readonly RetryReader _reader;
        private readonly ISeparatorMatcher _separatorMatcher;
        private readonly ISeparatorMatcher _recordSeparatorMatcher;
        private readonly ISeparatorMatcher _postfixMatcher;
        private readonly int _separatorLength;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            _reader = reader;
            Options = options.Clone();
            _separatorMatcher = SeparatorMatcher.GetMatcher(reader, options.Separator);
            _recordSeparatorMatcher = SeparatorMatcher.GetMatcher(reader, options.RecordSeparator);
            if (options.RecordSeparator != null && options.RecordSeparator.StartsWith(options.Separator))
            {
                string postfix = options.RecordSeparator.Substring(options.Separator.Length);
                _postfixMatcher = SeparatorMatcher.GetMatcher(reader, postfix);
            }
            _separatorLength = Math.Max(Options.RecordSeparator?.Length ?? 2, Options.Separator.Length);
        }

        internal SeparatedValueOptions Options { get; }

        public bool IsEndOfStream()
        {
            if (_reader.ShouldLoadBuffer(1))
            {
                _reader.LoadBuffer();
            }
            return _reader.IsEndOfStream();
        }

        public async ValueTask<bool> IsEndOfStreamAsync()
        {
            if (_reader.ShouldLoadBuffer(1))
            {
                await _reader.LoadBufferAsync().ConfigureAwait(false);
            }
            return _reader.IsEndOfStream();
        }

        private void AddToken()
        {
            string value = _token.ToString();
            _tokens.Add(value);
            _token.Clear();
        }

        public string[] ReadRecord()
        {
            TokenType tokenType = GetNextToken();
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = GetNextToken();
            }
            string[] results = _tokens.ToArray();
            _tokens.Clear();
            return results;
        }

        public async Task<string[]> ReadRecordAsync()
        {
            TokenType tokenType = await GetNextTokenAsync().ConfigureAwait(false);
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = await GetNextTokenAsync().ConfigureAwait(false);
            }
            string[] results = _tokens.ToArray();
            _tokens.Clear();
            return results;
        }

        private TokenType GetNextToken()
        {
            if (!Options.PreserveWhiteSpace)
            {
                TokenType tokenType = SkipWhiteSpace();
                if (tokenType != TokenType.Normal)
                {
                    AddToken();
                    return tokenType;
                }
            }
            if (_reader.ShouldLoadBuffer(1))
            {
                _reader.LoadBuffer();
            }
            if (_reader.IsMatch1(Options.Quote))
            {
                return GetQuotedToken();
            }

            return GetUnquotedToken();
        }

        private async ValueTask<TokenType> GetNextTokenAsync()
        {
            if (!Options.PreserveWhiteSpace)
            {
                TokenType tokenType = await SkipWhiteSpaceAsync().ConfigureAwait(false);
                if (tokenType != TokenType.Normal)
                {
                    AddToken();
                    return tokenType;
                }
            }
            if (_reader.ShouldLoadBuffer(1))
            {
                await _reader.LoadBufferAsync().ConfigureAwait(false);
            }
            if (_reader.IsMatch1(Options.Quote))
            {
                return await GetQuotedTokenAsync().ConfigureAwait(false);
            }

            return await GetUnquotedTokenAsync().ConfigureAwait(false);
        }

        private TokenType GetUnquotedToken()
        {
            if (_reader.ShouldLoadBuffer(_separatorLength))
            {
                _reader.LoadBuffer();
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                _reader.Read();
                _token.Append(_reader.Current);
                if (_reader.ShouldLoadBuffer(_separatorLength))
                {
                    _reader.LoadBuffer();
                }
                tokenType = GetSeparator();
            }
            TrimTokenEnd();
            AddToken();
            return tokenType;
        }

        private async ValueTask<TokenType> GetUnquotedTokenAsync()
        {
            if (_reader.ShouldLoadBuffer(_separatorLength))
            {
                await _reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                _reader.Read();
                _token.Append(_reader.Current);
                if (_reader.ShouldLoadBuffer(_separatorLength))
                {
                    await _reader.LoadBufferAsync().ConfigureAwait(false);
                }
                tokenType = GetSeparator();
            }
            TrimTokenEnd();
            AddToken();
            return tokenType;
        }

        private void TrimTokenEnd()
        {
            if (Options.PreserveWhiteSpace)
            {
                return;
            }
            if (char.IsWhiteSpace(_token[_token.Length - 1]))
            {
                int trailingSize = 1;
                while (char.IsWhiteSpace(_token[_token.Length - trailingSize - 1]))
                {
                    ++trailingSize;
                }
                _token.Length -= trailingSize;
            }
        }

        private TokenType GetQuotedToken()
        {
            if (_reader.ShouldLoadBuffer(1))
            {
                _reader.LoadBuffer();
            }
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal && _reader.Read())
            {
                if (_reader.Current != Options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    _token.Append(_reader.Current);
                }
                else
                {
                    if (_reader.ShouldLoadBuffer(1))
                    {
                        _reader.LoadBuffer();
                    }
                    if (_reader.IsMatch1(Options.Quote))
                    {
                        // Escaped quote (two quotes in a row)
                        _token.Append(_reader.Current);
                    }
                    else
                    {
                        tokenType = Options.PreserveWhiteSpace ? AppendWhiteSpace() : SkipWhiteSpace();
                        // If we find anything other than a separator, it's a syntax error.
                        if (tokenType == TokenType.Normal)
                        {
                            break;
                        }

                        AddToken();
                        return tokenType;
                    }
                }
                if (_reader.ShouldLoadBuffer(1))
                {
                    _reader.LoadBuffer();
                }
            }
            throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
        }

        private async ValueTask<TokenType> GetQuotedTokenAsync()
        {
            if (_reader.ShouldLoadBuffer(1))
            {
                await _reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal && _reader.Read())
            {
                if (_reader.Current != Options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    _token.Append(_reader.Current);
                }
                else
                {
                    if (_reader.ShouldLoadBuffer(1))
                    {
                        await _reader.LoadBufferAsync().ConfigureAwait(false);
                    }
                    if (_reader.IsMatch1(Options.Quote))
                    {
                        // Escaped quote (two quotes in a row)
                        _token.Append(_reader.Current);
                    }
                    else
                    {
                        if (Options.PreserveWhiteSpace)
                        {
                            // We've encountered a stand-alone quote.
                            // We go looking for a separator, keeping any leading whitespace.
                            tokenType = await AppendWhiteSpaceAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            // We've encountered a stand-alone quote.
                            // We go looking for a separator, skipping any leading whitespace.
                            tokenType = await SkipWhiteSpaceAsync().ConfigureAwait(false);
                        }
                        // If we find anything other than a separator, it's a syntax error.
                        if (tokenType == TokenType.Normal)
                        {
                            break;
                        }

                        AddToken();
                        return tokenType;
                    }
                }
                if (_reader.ShouldLoadBuffer(1))
                {
                    await _reader.LoadBufferAsync().ConfigureAwait(false);
                }
            }
            throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
        }

        private TokenType SkipWhiteSpace()
        {
            if (_reader.ShouldLoadBuffer(_separatorLength))
            {
                _reader.LoadBuffer();
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (_reader.ShouldLoadBuffer(_separatorLength + 1))
                {
                    _reader.LoadBuffer();
                }
                if (!_reader.IsWhitespace())
                {
                    break;
                }
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private async ValueTask<TokenType> SkipWhiteSpaceAsync()
        {
            if (_reader.ShouldLoadBuffer(_separatorLength))
            {
                await _reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (_reader.ShouldLoadBuffer(_separatorLength + 1))
                {
                    await _reader.LoadBufferAsync().ConfigureAwait(false);
                }
                if (!_reader.IsWhitespace())
                {
                    break;
                }
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private TokenType AppendWhiteSpace()
        {
            if (_reader.ShouldLoadBuffer(_separatorLength))
            {
                _reader.LoadBuffer();
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (_reader.ShouldLoadBuffer(_separatorLength + 1))
                {
                    _reader.LoadBuffer();
                }
                if (!_reader.IsWhitespace())
                {
                    break;
                }
                _token.Append(_reader.Current);
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private async ValueTask<TokenType> AppendWhiteSpaceAsync()
        {
            if (_reader.ShouldLoadBuffer(_separatorLength))
            {
                await _reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (_reader.ShouldLoadBuffer(_separatorLength + 1))
                {
                    await _reader.LoadBufferAsync().ConfigureAwait(false);
                }
                if (!_reader.IsWhitespace())
                {
                    break;
                }
                _token.Append(_reader.Current);
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private TokenType GetSeparator()
        {
            if (_reader.IsEndOfStream())
            {
                return TokenType.EndOfStream;
            }

            if (_separatorMatcher.IsMatch())
            {
                // This code handles the case where the separator is a substring of the record separator.
                // We check to see if the remaining characters make up the record separator.
                if (_postfixMatcher != null && _postfixMatcher.IsMatch())
                {
                    return TokenType.EndOfRecord;
                }

                return TokenType.EndOfToken;
            }

            if (_postfixMatcher == null && _recordSeparatorMatcher.IsMatch())
            {
                // If the separator is a substring of the record separator and we didn't find it,
                // we won't find the record separator either.
                return TokenType.EndOfRecord;
            }

            return TokenType.Normal;
        }

        private enum TokenType
        {
            Normal,
            EndOfStream,
            EndOfRecord,
            EndOfToken
        }
    }
}
