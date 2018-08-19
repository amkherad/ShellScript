using System;
using System.Collections.Generic;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public partial class Parser
    {
        public BlockStatement ReadBlockStatement(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            var statements = new List<IStatement>();

            IStatement statement;
            while ((statement = ReadStatement(enumerator, info)) != null)
            {
                statements.Add(statement);
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(enumerator.Current, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseBrace)
                throw UnexpectedSyntax(token, info);

            return new BlockStatement(statements.ToArray(), info);
        }

        public ConstantValueStatement ReadConstantValue(Token token, PeekingEnumerator<Token> enumerator,
            ParserInfo info, DataTypes dataTypeHint)
        {
            if (token.Type != TokenType.Number ||
                token.Type != TokenType.StringValue1 ||
                token.Type != TokenType.StringValue2 ||
                token.Type != TokenType.Null)
            {
                throw UnexpectedSyntax(token, info);
            }

            return new ConstantValueStatement(dataTypeHint, token.Value, info);
        }

        public FunctionParameterDefinitionStatement ReadParameterDefinition(Token token,
            PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.DataType)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //variable name.
                throw EndOfFile(token, info);

            var dataType = TokenTypeToDataType(token);

            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, info);

            if (enumerator.TryPeek(out var peek) && peek.Type == TokenType.Assignment)
            {
                enumerator.MoveNext();

                if (!enumerator.MoveNext()) //default value.
                    throw EndOfFile(token, info);

                var defaultValue = ReadConstantValue(enumerator.Current, enumerator, info, dataType);

                return new FunctionParameterDefinitionStatement(dataType, definitionName.Value, defaultValue, info);
            }

            return new FunctionParameterDefinitionStatement(dataType, definitionName.Value, null, info);
        }

        public FunctionParameterDefinitionStatement[] ReadParameterDefinitions(Token token,
            PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);

            var result = new List<FunctionParameterDefinitionStatement>();

            if (!enumerator.MoveNext())
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.DataType)
                throw UnexpectedSyntax(token, info);

            FunctionParameterDefinitionStatement statement;
            while ((statement = ReadParameterDefinition(token, enumerator, info)) != null)
            {
                result.Add(statement);

                token = enumerator.Current;

                if (enumerator.TryPeek(out var peek))
                {
                    if (peek.Type != TokenType.Comma)
                    {
                        break;
                    }
                }
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, info);

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// function XXX[()] {
        ///     []
        /// }
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <exception cref="???"></exception>
        public IStatement ReadFunction(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.Function)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //function name.
                throw EndOfFile(token, info);

            var functionName = enumerator.Current;
            if (functionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //parenthesis or braces
                throw EndOfFile(token, info);

            FunctionParameterDefinitionStatement[] parameters = null;
            BlockStatement block = null;

            token = enumerator.Current;
            if (token.Type == TokenType.OpenParenthesis)
            {
                parameters = ReadParameterDefinitions(token, enumerator, info);

                if (!enumerator.MoveNext())
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type != TokenType.OpenBrace)
                    throw UnexpectedSyntax(token, info);

                block = ReadBlockStatement(token, enumerator, info);
            }
            else if (token.Type == TokenType.OpenBrace)
            {
                block = ReadBlockStatement(token, enumerator, info);
            }
            else
            {
                throw UnexpectedSyntax(token, info);
            }

            return new FunctionStatement(functionName.Value, parameters, block, info);
        }

        public EvaluationStatement ReadEvaluationStatement(Token token, PeekingEnumerator<Token> enumerator,
            ParserInfo info)
        {
            return null;
        }

        public IfElseStatement ReadIf(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.If)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                throw EndOfFile(token, info);

            token = enumerator.Current;
            var condition = ReadEvaluationStatement(token, enumerator, info);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, info);

            //TODO: missing support of single if statement.
            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            var block = ReadBlockStatement(token, enumerator, info);

            if (!enumerator.TryPeek(out var peek) || peek.Type != TokenType.Else) //else or else if
            {
                return new IfElseStatement(new ConditionalBlockStatement(condition, block, info), info);
            }

            var elseIfBlocks = new List<ConditionalBlockStatement>();
            
            for(;;)
            {
                enumerator.MoveNext(); //get the else.

                token = enumerator.Current;
                if (token.Type != TokenType.Else)
                    throw UnexpectedSyntax(token, info);

                if (!enumerator.MoveNext()) //get the else.
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type == TokenType.If) //else if
                {
                    if (!enumerator.MoveNext()) //open parenthesis
                        throw EndOfFile(token, info);
                    
                    if (token.Type != TokenType.OpenParenthesis)
                        throw UnexpectedSyntax(token, info);

                    if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                        throw EndOfFile(token, info);

                    var elifCondition = ReadEvaluationStatement(token, enumerator, info);
                    
                    if (!enumerator.MoveNext()) //close parenthesis
                        throw EndOfFile(token, info);
                    
                    if (token.Type != TokenType.CloseParenthesis)
                        throw UnexpectedSyntax(token, info);
                    
                    if (!enumerator.MoveNext()) //open brace
                        throw EndOfFile(token, info);
                    
                    if (token.Type != TokenType.OpenBrace)
                        throw UnexpectedSyntax(token, info);

                    var elifBlock = ReadBlockStatement(token, enumerator, info);
                    
                    elseIfBlocks.Add(new ConditionalBlockStatement(elifCondition, elifBlock, info));
                }
                else if (token.Type == TokenType.OpenBrace) //else
                {
                    var elseBlock = ReadBlockStatement(token, enumerator, info);

                    return new IfElseStatement(new ConditionalBlockStatement(condition, block, info), elseIfBlocks.ToArray(), elseBlock, info);
                }
                
                if (!enumerator.TryPeek(out peek) || peek.Type != TokenType.Else) //else or else if
                {
                    return new IfElseStatement(new ConditionalBlockStatement(condition, block, info), elseIfBlocks.ToArray(), info);
                }
            }
        }

        public WhileStatement ReadWhile(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.While)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                throw EndOfFile(token, info);

            var condition = ReadEvaluationStatement(token, enumerator, info);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, info);
            
            if (!enumerator.MoveNext()) //open brace or semicolon
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type == TokenType.SequenceTerminator)
            {
                return new WhileStatement(condition, null, info);
            }
            else if (token.Type == TokenType.OpenBrace)
            {
                var statements = ReadBlockStatement(token, enumerator, info);

                return new WhileStatement(condition, statements, info);
            }
            else
            {
                throw UnexpectedSyntax(token, info);
            }
        }

        public DoWhileStatement ReadDoWhile(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.Do)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                throw EndOfFile(token, info);

            var statements = ReadBlockStatement(token, enumerator, info);
            
            if (!enumerator.MoveNext()) //while
                throw EndOfFile(token, info);
            
            token = enumerator.Current;
            if (token.Type != TokenType.While)
                throw UnexpectedSyntax(token, info);
            
            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, info);
            
            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);
            
            if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                throw EndOfFile(token, info);
            
            var condition = ReadEvaluationStatement(token, enumerator, info);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, info);

            if (info.SemicolonRequired)
            {
                if (!enumerator.MoveNext()) //semicolon
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, info);
            }

            return new DoWhileStatement(condition, statements, info);
        }
    }
}