using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShellScript.Core.Language.Compiler.Lexing
{
    public class Lexer
    {
        public const char EscapeCharacter = '\\';
        public const string EscapeCharacterStr = "\\";

        private static readonly Dictionary<TokenType, string> TokenPatterns = new Dictionary<TokenType, string>
        {
            {TokenType.StringValue1, "^'[^']*'"},
            {TokenType.StringValue2, "^\"[^\"]*\""},

            {TokenType.OpenParenthesis, "^\\("},
            {TokenType.CloseParenthesis, "^\\)"},

            {TokenType.OpenBrace, "^\\{"},
            {TokenType.CloseBrace, "^\\}"},

            {TokenType.OpenBracket, "^\\["},
            {TokenType.CloseBracket, "^\\]"},

            {TokenType.And, "^&"},
            {TokenType.AndLogical, "^&&"},
            {TokenType.Or, "^\\|"},
            {TokenType.OrLogical, "^\\|\\|"},

            {TokenType.Dot, "^\\."},
            {TokenType.Comma, "^,"},
            {TokenType.Assignment, "^="},
            {TokenType.Equals, "^=="},
            {TokenType.NotEquals, "^!="},

            {TokenType.Minus, "^\\-"},
            {TokenType.Plus, "^\\+"},
            {TokenType.Asterisk, "^\\*"},
            {TokenType.Division, "^/"},

            {TokenType.BackSlash, "^\\\\"},

            {TokenType.If, "^if"},
            {TokenType.Else, "^else"},
            
            {TokenType.PreprocessorIf, "^#if"},
            {TokenType.PreprocessorElseIf, "^#elseif"},
            {TokenType.PreprocessorElse, "^#else"},
            {TokenType.PreprocessorEndIf, "^#endif"},
            
            {TokenType.For, "^for"},
            {TokenType.ForEach, "^foreach"},
            {TokenType.Do, "^do"},
            {TokenType.While, "^while"},
            {TokenType.Loop, "^loop"},

            {TokenType.Throw, "^throw"},
            {TokenType.Comment, "(^//)"},
            {TokenType.MultiLineCommentOpen, "(^/\\*)"},
            {TokenType.MultiLineCommentClose, "(^\\*/)"},

            {TokenType.Async, "^async"},
            {TokenType.Await, "^await"},

            {TokenType.In, "^in"},
            {TokenType.NotIn, "^notin"},

            {TokenType.Like, "^like"},
            {TokenType.NotLike, "^notlike"},

            {TokenType.Call, "^call"},

            {TokenType.DataType, "^((const)|(var)|(int)|(long)|(double)|(float)|(object)|(variant)|(number))"},

            {TokenType.Echo, "^echo"},

            {TokenType.Number, "^\\d+"},

            {TokenType.SequenceTerminator, "^;"},
        };

        private static readonly Dictionary<TokenType, Regex> TokenRegexes = new Dictionary<TokenType, Regex>(
            TokenPatterns.ToDictionary(
                keySelector => keySelector.Key,
                elementSelector => new Regex(elementSelector.Value, RegexOptions.Compiled)
            )
        );

        private static readonly Regex ValidIdentifierName = new Regex(@"^\w+", RegexOptions.Compiled);

        /// <summary>
        /// This method tokenize the input stream.
        /// </summary>
        /// <remarks>
        /// This method yield returns the tokens.
        /// </remarks>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<Token> Tokenize(TextReader reader)
        {
            var line = string.Empty;
            var lineNumber = 0;

            string cline; //current line
            while ((cline = reader.ReadLine()) != null)
            {
                if (cline[cline.Length - 1] == EscapeCharacter)
                {
                    line += cline.Substring(0, cline.Length - 1); //remove the line escape character.
                    lineNumber++;
                    continue;
                }

                line += ' ' + cline;

                var lineLength = line.Length;

                foreach (var token in TokenizeLine(line, lineNumber))
                {
                    yield return token;
                }

                yield return new Token(
                    Environment.NewLine,
                    TokenType.SequenceTerminatorNewLine,
                    lineLength,
                    lineLength);

                lineNumber = 0;
            }
        }

        /// <summary>
        /// Tokenizes a single line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public Token[] TokenizeLine(string line, int lineNumber)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            var columnNumber = 0;
            var tokens = new List<Token>(10);

            while (!string.IsNullOrWhiteSpace(line))
            {
                if (TryFindMatch(line, out var matchType, out var matchString))
                {
                    tokens.Add(new Token(matchString, matchType, columnNumber, lineNumber));
                    line = line.Substring(matchString.Length);
                    columnNumber += matchString.Length;
                }
                else
                {
                    line = line.Substring(1);
                    columnNumber++;
                }
            }

            return tokens.ToArray();
        }

        public bool TryFindMatch(string text, out TokenType matchType, out string matchString)
        {
            Match match;
            foreach (var token in TokenRegexes)
            {
                match = token.Value.Match(text);
                if (match.Success)
                {
                    matchType = token.Key;
                    matchString = match.Value;
                    return true;
                }
            }

            match = ValidIdentifierName.Match(text);
            if (match.Success)
            {
                matchType = TokenType.IdentifierName;
                matchString = match.Value;
                return true;
            }

            matchType = TokenType.NotDefined;
            matchString = default;
            return false;
        }
    }
}