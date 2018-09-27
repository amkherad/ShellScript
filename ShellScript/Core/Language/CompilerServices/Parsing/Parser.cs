using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.PreProcessors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public partial class Parser
    {
        public const string ConstantKeyword = "const";
        
        public static HashSet<string> Keywords = new HashSet<string>
        {
            "if",
            "else",
            "switch",
            "case",
            "default",
            "while",
            "do",
            "switch",
            "case",
            "function",
            "int",
            "long",
            "double",
            "float",
            "void",
            "decimal",
            "null",
            "nil",
            "numeric",
            "string",
            "object",
            "class",
            "delegate",
            "const",
            "array",
            "int[]",
            "long[]",
            "double[]",
            "float[]",
            "var",
            "variant[]",
            "numeric[]",
            "decimal[]",
            "for",
            "foreach",
            "loop",
            "return",
            "async",
            "await",
            "throw",
            "in",
            "notin",
            "like",
            "notlike",
            "call",
            "echo",
        };

        public Context Context { get; }
        private Lexer _lexer;
        private PreProcessorParser _preProcessorParser;

        public Parser(Context context)
        {
            Context = context;
            _lexer = new Lexer();
            _preProcessorParser = new PreProcessorParser(this, context);
        }
        
        public Parser(Context context, Lexer lexer, PreProcessorParser preProcessorParser)
        {
            Context = context;
            _lexer = lexer;
            _preProcessorParser = preProcessorParser;
        }


        public IEnumerable<IStatement> Parse(TextReader reader, ParserInfo info)
        {
            var tokens = _lexer.Tokenize(reader);
            
            using (var tokensEnumerator = _preProcessorParser.CreateParserProxy(tokens.GetEnumerator(), info))
            {
                IStatement statement;
                while ((statement = ReadStatement(tokensEnumerator, info)) != null)
                {
                    yield return statement;
                }
            }
        }

        public StatementInfo CreateStatementInfo(ParserInfo info, Token token)
        {
            return new StatementInfo(info.FilePath, token.LineNumber, token.ColumnStart);
        }


        public DataTypes TokenTypeToDataType(Token token, DataTypes dataType = DataTypes.String)
        {
            switch (token.Type)
            {
                case TokenType.DataType:
                {
                    switch (token.Value.ToLower())
                    {
                        case "int":
                            return DataTypes.Decimal;
                        case "bool":
                        case "boolean":
                            return DataTypes.Boolean;
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
                        //case "variant":
                        //case "var":
                        //    return DataTypes.Variant;

                        case "void":
                            return DataTypes.Void;
                        
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
                        //case "variant[]":
                        //    return DataTypes.Variant | DataTypes.Array;
                        
                        case "null":
                            return dataType;
                    }

                    break;
                }
                case TokenType.Null:
                    return dataType;
            }

            return dataType;
        }

        protected IStatement ReadStatement(IPeekingEnumerator<Token> enumerator, ParserInfo info)
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
                        return ReadVariableOrFunctionDefinition(token, enumerator, info);
                    
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
                    //case TokenType.Function:
                    //    return ReadFunction(token, enumerator, info);
                    case TokenType.Return:
                        return ReadReturn(token, enumerator, info);

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
                    case TokenType.PreprocessorIf:
                    case TokenType.PreprocessorElse:
                    case TokenType.PreprocessorElseIf:
                    case TokenType.PreprocessorEndIf:
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
                    case TokenType.Case:
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
                        throw UnexpectedSyntax(token, info);
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
            return new IllegalSyntaxException(token?.LineNumber ?? 0, token?.ColumnStart ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="IllegalSyntaxException">IllegalSyntaxException</returns>
        protected ParserException InvalidIdentifierName(string identifierName, Token token, ParserInfo info)
        {
            return new IllegalSyntaxException(
                $"Invalid identifier name '{identifierName}'",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(Token token, ParserInfo info)
        {
            return new ParserSyntaxException(
                $"Unexpected token of type {token.Type}({token.Value}) found",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(Token token, ParserInfo info, Exception innerException)
        {
            return new ParserSyntaxException(
                $"Unexpected token '{token.Value}' found",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, info,
                innerException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException EndOfFile(Token token, ParserInfo info)
        {
            return new ParserSyntaxException(
                "Unexpected end of file reached",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, info);
        }
    }
}