using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public partial class Parser
    {
        private Lexer _lexer;

        public Parser()
        {
            _lexer = new Lexer();
        }
        
        public Parser(Lexer lexer)
        {
            _lexer = lexer;
        }


        public IEnumerable<IStatement> Parse(TextReader reader, ParserInfo info)
        {
            var tokens = _lexer.Tokenize(reader);
            
            using (var tokensEnumerator = new PeekingEnumerator<Token>(tokens.GetEnumerator()))
            {
                IStatement statement;
                while ((statement = ReadStatement(tokensEnumerator, info)) != null)
                {
                    yield return statement;
                }
            }
        }


        public DataTypes TokenTypeToDataType(Token token, DataTypes dataType = DataTypes.Variant)
        {
            switch (token.Type)
            {
                case TokenType.DataType:
                {
                    switch (token.Value)
                    {
                        case "null":
                            return dataType;
                        case "int":
                            return DataTypes.Decimal;
                        case "decimal":
                            return DataTypes.Decimal;
                        case "number":
                            return DataTypes.Numeric;
                        case "float":
                            return DataTypes.Float;
                        case "double":
                            return DataTypes.Float;
                        case "object":
                            return DataTypes.Class;
                        case "variant":
                        case "var":
                            return DataTypes.Variant;

                        case "int[]":
                            return DataTypes.Decimal | DataTypes.Array;
                        case "decimal[]":
                            return DataTypes.Decimal | DataTypes.Array;
                        case "number[]":
                            return DataTypes.Numeric | DataTypes.Array;
                        case "float[]":
                            return DataTypes.Float | DataTypes.Array;
                        case "double[]":
                            return DataTypes.Float | DataTypes.Array;
                        case "object[]":
                            return DataTypes.Class | DataTypes.Array;
                        case "variant[]":
                            return DataTypes.Variant | DataTypes.Array;
                    }

                    break;
                }
                case TokenType.Null:
                    return dataType;
            }

            return dataType;
        }

        protected IStatement ReadStatement(PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            Token token;

            while (enumerator.MoveNext())
            {
                token = enumerator.Current;

                switch (token.Type)
                {
                    case TokenType.Echo:
                        return ReadEcho(token, enumerator, info);
                    case TokenType.IdentifierName:
                        return ReadAssignmentOrFunctionCall(token, enumerator, info);
                    case TokenType.DataType:
                        return ReadVariableDefinition(token, enumerator, false, info);
                    
                    case TokenType.If:
                        return ReadIf(token, enumerator, info);
                    case TokenType.For:
                        return ReadFor(token, enumerator, info);
                    case TokenType.ForEach:
                        return ReadForEach(token, enumerator, info);
                    case TokenType.While:
                        return ReadWhile(token, enumerator, info);
                    case TokenType.Do:
                        return ReadDoWhile(token, enumerator, info);
                    case TokenType.Loop:
                        return ReadLoop(token, enumerator, info);
                    
                    //case TokenType.Class:
                    //    return ReadClass(token, enumerator, info);
                    case TokenType.Function:
                        return ReadFunction(token, enumerator, info);

                    case TokenType.OpenBrace:
                        return ReadBlockStatement(token, enumerator, info);
                    
                    case TokenType.OpenParenthesis:
                        break;
                    case TokenType.Throw:
                        break;
                    case TokenType.Async:
                        break;
                    case TokenType.Await:
                        break;
                    case TokenType.Call:
                        break;
                    
                    
                    
                    case TokenType.SequenceTerminator:
                    case TokenType.SequenceTerminatorNewLine:
                        continue;
                    
                    case TokenType.Comment:
                    case TokenType.MultiLineCommentOpen:
                    case TokenType.MultiLineCommentClose:
                        throw UnexpectedSyntax(token, info);


                    case TokenType.Minus:
                    case TokenType.Plus:
                    case TokenType.Else:
                    case TokenType.AndLogical:
                    case TokenType.And:
                    case TokenType.OrLogical:
                    case TokenType.Or:
                    case TokenType.Equals:
                    case TokenType.NotEquals:
                    case TokenType.Asterisk:
                    case TokenType.Assignment:
                    case TokenType.CloseParenthesis:
                    case TokenType.CloseBrace:
                    case TokenType.OpenBracket:
                    case TokenType.CloseBracket:
                    case TokenType.Division:
                    case TokenType.BackSlash:
                    case TokenType.Dot:
                    case TokenType.Comma:
                    case TokenType.In:
                    case TokenType.NotIn:
                    case TokenType.Like:
                    case TokenType.NotLike:
                    case TokenType.Number:
                    case TokenType.StringValue1:
                    case TokenType.StringValue2:
                        throw UnexpectedSyntax(token, info);
                    
                    case TokenType.NotDefined:
                        throw IllegalSyntax(token, info);
                    case TokenType.Invalid:
                        throw IllegalSyntax(token, info);
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="IllegalSyntaxException">IllegalSyntaxException</returns>
        protected ParserException IllegalSyntax(Token token, ParserInfo info)
        {
            return new IllegalSyntaxException(token?.LineNumber ?? 0, token?.ColumnOffset ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(Token token, ParserInfo info)
        {
            return new ParserSyntaxException(token?.LineNumber ?? 0, token?.ColumnOffset ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException EndOfFile(Token token, ParserInfo info)
        {
            return new ParserSyntaxException(token?.LineNumber ?? 0, token?.ColumnOffset ?? 0, info);
        }
    }
}