using System;
using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public partial class Parser
    {
        /// <summary>
        /// Reads a block (i.e. everything between "{" and "}")
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <exception cref="IllegalSyntaxException"></exception>
        /// <exception cref="ParserSyntaxException"></exception>
        /// <exception cref="ParserException"></exception>
        public BlockStatement ReadBlockStatement(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            var statements = new List<IStatement>();

            var continueRead = true;
            while(continueRead)
            {
                if (!enumerator.TryPeek(out var peek))
                {
                    break;
                }
                switch (peek.Type)
                {
                    case TokenType.SequenceTerminator:
                    //case TokenType.SequenceTerminatorNewLine:
                        enumerator.MoveNext();
                        continue;
                    case TokenType.CloseBrace:
                        continueRead = false;
                        break;
                    default:
                    {
                        var statement = ReadStatement(enumerator, info);
                        statements.Add(statement);
                        break;
                    }
                }
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(enumerator.Current, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseBrace)
                throw UnexpectedSyntax(token, info);

            return new BlockStatement(statements.ToArray());
        }

        public EvaluationStatement ReadAssignmentOrFunctionCall(Token token, PeekingEnumerator<Token> enumerator,
            ParserInfo info)
        {
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, info);

            var firstToken = token;

            if (enumerator.TryPeek(out var peek))
            {
                switch (peek.Type)
                {
                    case TokenType.Assignment:
                    {
                        enumerator.MoveNext(); //read the assignment operator

                        if (!enumerator.MoveNext()) //read the next token for ReadEvaluationStatement
                            throw EndOfFile(token, info);

                        token = enumerator.Current;
                        var result = ReadEvaluationStatement(token, enumerator, info);

                        return new AssignmentStatement(new VariableAccessStatement(firstToken.Value), result);
                    }
                    case TokenType.OpenParenthesis:
                    case TokenType.Dot:
                    {
                        return ReadVariableOrFunctionCall(token, enumerator, info);
                    }
                    default:
                        throw UnexpectedSyntax(peek, info);
                }
            }

            throw UnexpectedSyntax(token, info);
        }

        public EvaluationStatement ReadVariableOrFunctionCall(Token token, PeekingEnumerator<Token> enumerator,
            ParserInfo info)
        {
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, info);

            var functionName = token;
            Token className = null;

            if (enumerator.TryPeek(out var peek))
            {
                var isMemberFunction = peek.Type == TokenType.Dot;
                if (peek.Type == TokenType.OpenParenthesis || isMemberFunction)
                {
                    enumerator.MoveNext(); //read the open parenthesis or dot
                    var parameters = new List<EvaluationStatement>();

                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, info);

                    token = enumerator.Current;

                    if (isMemberFunction)
                    {
                        className = token;

                        if (!enumerator.MoveNext())
                            throw EndOfFile(token, info);

                        token = enumerator.Current;
                        if (token.Type != TokenType.OpenParenthesis)
                            throw UnexpectedSyntax(token, info);

                        if (!enumerator.MoveNext()) //read the next token for ReadEvaluationStatement
                            throw EndOfFile(token, info);

                        token = enumerator.Current;
                    }

                    if (token.Type != TokenType.CloseParenthesis)
                    {
                        var continueRead = true;
                        for (; continueRead;)
                        {
                            var statement = ReadEvaluationStatement(token, enumerator, info);
                            parameters.Add(statement);

                            if (enumerator.TryPeek(out var nextPeek))
                            {
                                switch (nextPeek.Type)
                                {
                                    case TokenType.Comma:
                                    {
                                        if (statement is NopStatement)
                                            throw UnexpectedSyntax(token, info);

                                        enumerator.MoveNext(); //skip the comma

                                        if (!enumerator.MoveNext())
                                            throw EndOfFile(token, info);
                                        token = enumerator.Current;

                                        break;
                                    }
                                    case TokenType.CloseParenthesis:
                                    {
                                        continueRead = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    return new FunctionCallStatement(
                        className?.Value,
                        functionName.Value,
                        parameters.Where(p => !(p is NopStatement)).ToArray());
                }
            }

            return new VariableAccessStatement(token.Value);
        }


        private EvaluationStatement _createEvaluation(LinkedList<IStatement> statements, Token token, ParserInfo info)
        {
            if (statements.Count < 1)
            {
                return new NopStatement();
            }

            while (statements.Count > 1)
            {
                var op = statements.OfType<IOperator>().OrderByDescending(s => s.Order).FirstOrDefault();

                if (op == null)
                {
                    throw UnexpectedSyntax(token, info);
                }

                var node = statements.Find(op);
                if (node == null)
                    throw UnexpectedSyntax(token, info);

                if (op is BitwiseOperator bitwiseOperator)
                {
                    if (op is BitwiseNotOperator bitwiseNotOperator)
                    {
                        var operandNode = node.Next;
                        var operand = operandNode?.Value as EvaluationStatement;
                        if (operand == null)
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        node.Value = BitwiseEvaluationStatement.CreateNot(bitwiseNotOperator, operand);
                        statements.Remove(operandNode);
                    }
                    else
                    {
                        var leftNode = node.Previous;
                        var left = leftNode?.Value as EvaluationStatement;
                        var rightNode = node.Next;
                        var right = rightNode?.Value as EvaluationStatement;

                        if (left == null || right == null)
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        node.Value = new BitwiseEvaluationStatement(left, bitwiseOperator, right);
                        statements.Remove(leftNode);
                        statements.Remove(rightNode);
                    }
                }
                else if (op is LogicalOperator logicalOperator)
                {
                    if (op is NotOperator notOperator)
                    {
                        var operandNode = node.Next;
                        var operand = operandNode?.Value as EvaluationStatement;
                        if (operand == null)
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        node.Value = LogicalEvaluationStatement.CreateNot(notOperator, operand);
                        statements.Remove(operandNode);
                    }
                    else
                    {
                        var leftNode = node.Previous;
                        var left = leftNode?.Value as EvaluationStatement;
                        var rightNode = node.Next;
                        var right = rightNode?.Value as EvaluationStatement;

                        if (left == null || right == null)
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        node.Value = new LogicalEvaluationStatement(left, logicalOperator, right);
                        statements.Remove(leftNode);
                        statements.Remove(rightNode);
                    }
                }
                else if (op is ArithmeticOperator arithmeticOperator)
                {
                    if (op is IncrementOperator incOperator)
                    {
                        var operandNode = node.Previous;
                        var operand = operandNode?.Value as VariableAccessStatement;
                        if (operand == null)
                        {
                            operandNode = node.Next;
                            operand = operandNode?.Value as VariableAccessStatement;

                            if (operand == null)
                            {
                                throw UnexpectedSyntax(token, info);
                            }

                            node.Value = ArithmeticEvaluationStatement.CreatePrefixIncrement(incOperator, operand);
                        }
                        else
                        {
                            node.Value = ArithmeticEvaluationStatement.CreatePostfixIncrement(incOperator, operand);
                        }

                        statements.Remove(operandNode);
                    }
                    else if (op is DecrementOperator decOperator)
                    {
                        var operandNode = node.Previous;
                        var operand = operandNode?.Value as VariableAccessStatement;
                        if (operand == null)
                        {
                            operandNode = node.Next;
                            operand = operandNode?.Value as VariableAccessStatement;

                            if (operand == null)
                            {
                                throw UnexpectedSyntax(token, info);
                            }

                            node.Value = ArithmeticEvaluationStatement.CreatePrefixDecrement(decOperator, operand);
                        }
                        else
                        {
                            node.Value = ArithmeticEvaluationStatement.CreatePostfixDecrement(decOperator, operand);
                        }

                        statements.Remove(operandNode);
                    }
                    else
                    {
                        var leftNode = node.Previous;
                        var left = leftNode?.Value as EvaluationStatement;
                        var rightNode = node.Next;
                        var right = rightNode?.Value as EvaluationStatement;

                        if (left == null || right == null)
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        node.Value = new ArithmeticEvaluationStatement(left, arithmeticOperator, right);
                        statements.Remove(leftNode);
                        statements.Remove(rightNode);
                    }
                }
                else
                {
                    throw UnexpectedSyntax(token, info);//WTF!
                }
            }

            return (EvaluationStatement) statements.First.Value;
        }

        /// <summary>
        /// Reads an evaluation statement (i.e. "x = 3 * 4")
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public EvaluationStatement ReadEvaluationStatement(Token token, PeekingEnumerator<Token> enumerator,
            ParserInfo info)
        {
            var statements = new LinkedList<IStatement>();

            for (;;)
            {
                switch (token.Type)
                {
                    case TokenType.OpenParenthesis:
                    {
                        if (!enumerator.MoveNext()) //open parenthesis
                            throw EndOfFile(token, info);

                        token = enumerator.Current;
                        statements.AddLast(ReadEvaluationStatement(token, enumerator, info));

                        if (!enumerator.MoveNext()) //close parenthesis
                            throw EndOfFile(token, info);

                        token = enumerator.Current;
                        if (token.Type != TokenType.CloseParenthesis)
                            throw UnexpectedSyntax(token, info);

                        break;
                    }
                    case TokenType.IdentifierName:
                    {
                        statements.AddLast(ReadVariableOrFunctionCall(token, enumerator, info));
                        break;
                    }
                    case TokenType.StringValue1:
                    case TokenType.StringValue2:
                    {
                        statements.AddLast(new ConstantValueStatement(DataTypes.String, token.Value));
                        break;
                    }
                    case TokenType.Number:
                    {
                        statements.AddLast(new ConstantValueStatement(DataTypes.Numeric, token.Value));
                        break;
                    }
                    case TokenType.Null:
                    {
                        statements.AddLast(new ConstantValueStatement(DataTypes.Variant, token.Value));
                        break;
                    }

                    case TokenType.Plus:
                    {
                        statements.AddLast(new AdditionOperator());
                        break;
                    }
                    case TokenType.Minus:
                    {
                        statements.AddLast(new SubtractionOperator());
                        break;
                    }
                    case TokenType.Asterisk:
                    {
                        statements.AddLast(new MultiplicationOperator());
                        break;
                    }
                    case TokenType.Division:
                    {
                        statements.AddLast(new DivisionOperator());
                        break;
                    }
                    case TokenType.Increment:
                    {
                        statements.AddLast(new IncrementOperator());
                        break;
                    }
                    case TokenType.Decrement:
                    {
                        statements.AddLast(new DecrementOperator());
                        break;
                    }
                    case TokenType.BackSlash:
                    {
                        statements.AddLast(new ModulusOperator());
                        break;
                    }
                    case TokenType.Reminder:
                    {
                        statements.AddLast(new ReminderOperator());
                        break;
                    }
                    case TokenType.Not:
                    {
                        statements.AddLast(new ReminderOperator());
                        break;
                    }

                    case TokenType.Equals:
                    {
                        statements.AddLast(new EqualOperator());
                        break;
                    }
                    case TokenType.NotEquals:
                    {
                        statements.AddLast(new NotEqualOperator());
                        break;
                    }
                    case TokenType.GreaterEqual:
                    {
                        statements.AddLast(new GreaterEqualOperator());
                        break;
                    }
                    case TokenType.Greater:
                    {
                        statements.AddLast(new GreaterOperator());
                        break;
                    }
                    case TokenType.LessEqual:
                    {
                        statements.AddLast(new LessEqualOperator());
                        break;
                    }
                    case TokenType.Less:
                    {
                        statements.AddLast(new LessOperator());
                        break;
                    }

                    case TokenType.CloseParenthesis:
                    case TokenType.SequenceTerminator:
                    case TokenType.OpenBrace:
                    case TokenType.SequenceTerminatorNewLine:
                    case TokenType.Comma:
                    {
                        try
                        {
                            var result = _createEvaluation(statements, token, info);

                            return result;
                        }
                        catch (Exception ex)
                        {
                            throw UnexpectedSyntax(token, info);
                        }
                    }
                    default:
                        throw UnexpectedSyntax(token, info);
                }

                if (enumerator.TryPeek(out var peek))
                {
                    switch (peek.Type)
                    {
                        case TokenType.CloseParenthesis:
                        case TokenType.SequenceTerminator:
                        case TokenType.OpenBrace:
                        case TokenType.Comma:
                        {
                            try
                            {
                                var result = _createEvaluation(statements, token, info);

                                return result;
                            }
                            catch (Exception ex)
                            {
                                throw UnexpectedSyntax(token, info);
                            }
                        }
                        default:
                        {
                            if (!enumerator.MoveNext())
                                throw EndOfFile(token, info);

                            token = enumerator.Current;

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads a constant VALUE (i.e. 3 or "test" or null)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="info"></param>
        /// <param name="dataTypeHint"></param>
        /// <returns></returns>
        /// <exception cref="???"></exception>
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

            return new ConstantValueStatement(dataTypeHint, token.Value);
        }


        public VariableDefinitionStatement ReadVariableDefinition(Token token,
            PeekingEnumerator<Token> enumerator, bool forceAssignment, ParserInfo info)
        {
            if (token.Type != TokenType.DataType)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //variable name.
                throw EndOfFile(token, info);

            var dataType = TokenTypeToDataType(token);

            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, info);

            if (forceAssignment)
            {
                if (!enumerator.MoveNext()) //assignment
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type != TokenType.Assignment)
                    throw UnexpectedSyntax(token, info);

                if (!enumerator.MoveNext()) //value
                    throw EndOfFile(token, info);

                IStatement value;

                token = enumerator.Current;
                if (token.Type == TokenType.IdentifierName)
                {
                    value = new VariableAccessStatement(token.Value);
                }
                else if (token.Type == TokenType.Number)
                {
                    value = new ConstantValueStatement(DataTypes.Numeric, token.Value);
                }
                else if (token.Type == TokenType.StringValue1 || token.Type == TokenType.StringValue2)
                {
                    value = new ConstantValueStatement(DataTypes.String, token.Value);
                }
                else if (token.Type == TokenType.Null)
                {
                    value = new ConstantValueStatement(DataTypes.Variant, token.Value);
                }
                else
                {
                    throw UnexpectedSyntax(token, info);
                }

                return new VariableDefinitionStatement(dataType, definitionName.Value, value);
            }

            if (enumerator.TryPeek(out var peek) && peek.Type == TokenType.Assignment)
            {
                enumerator.MoveNext();

                if (!enumerator.MoveNext()) //default value.
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                var defaultValue = ReadEvaluationStatement(token, enumerator, info);

                return new VariableDefinitionStatement(dataType, definitionName.Value, defaultValue);
            }

            return new VariableDefinitionStatement(dataType, definitionName.Value, null);
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

                return new FunctionParameterDefinitionStatement(dataType, definitionName.Value, defaultValue);
            }

            return new FunctionParameterDefinitionStatement(dataType, definitionName.Value, null);
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

            return new FunctionStatement(functionName.Value, parameters, block);
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
                return new IfElseStatement(new ConditionalBlockStatement(condition, block));
            }

            var elseIfBlocks = new List<ConditionalBlockStatement>();

            for (;;)
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

                    elseIfBlocks.Add(new ConditionalBlockStatement(elifCondition, elifBlock));
                }
                else if (token.Type == TokenType.OpenBrace) //else
                {
                    var elseBlock = ReadBlockStatement(token, enumerator, info);

                    return new IfElseStatement(new ConditionalBlockStatement(condition, block),
                        elseIfBlocks.ToArray(), elseBlock);
                }

                if (!enumerator.TryPeek(out peek) || peek.Type != TokenType.Else) //else or else if
                {
                    return new IfElseStatement(new ConditionalBlockStatement(condition, block),
                        elseIfBlocks.ToArray());
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
                return new WhileStatement(condition, null);
            }
            else if (token.Type == TokenType.OpenBrace)
            {
                var statements = ReadBlockStatement(token, enumerator, info);

                return new WhileStatement(condition, statements);
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

            return new DoWhileStatement(condition, statements);
        }

        public WhileStatement ReadLoop(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            return ReadWhile(token, enumerator, info);
        }

        public ForStatement ReadFor(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.For)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //next token (datatype, variable name, or semicolon)
                throw EndOfFile(token, info);

            IStatement preLoopAssignments = null;
            IStatement condition = null;
            IStatement postLoopEvaluations = null;

            token = enumerator.Current;
            if (token.Type == TokenType.DataType)
            {
                preLoopAssignments = ReadVariableDefinition(token, enumerator, true, info);

                if (!enumerator.MoveNext()) //semicolon
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, info);
            }
            else if (token.Type != TokenType.SequenceTerminator)
            {
                throw UnexpectedSyntax(token, info);
            }

            if (!enumerator.MoveNext()) //condition
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.SequenceTerminator)
            {
                condition = ReadEvaluationStatement(token, enumerator, info);

                if (!enumerator.MoveNext()) //semicolon
                    throw EndOfFile(token, info);

                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, info);
            }

            if (!enumerator.MoveNext()) //post evaluations
                throw EndOfFile(token, info);

            if (token.Type != TokenType.CloseParenthesis)
            {
                postLoopEvaluations = ReadEvaluationStatement(token, enumerator, info);

                if (!enumerator.MoveNext()) //close parenthesis
                    throw EndOfFile(token, info);

                if (token.Type != TokenType.CloseParenthesis)
                    throw UnexpectedSyntax(token, info);
            }

            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, info);

            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            IStatement block = ReadBlockStatement(token, enumerator, info);

            return new ForStatement(preLoopAssignments, condition, postLoopEvaluations, block);
        }

        public ForEachStatement ReadForEach(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.ForEach)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //dataType or identifier name
                throw EndOfFile(token, info);

            DataTypes? dataType = null;

            token = enumerator.Current;
            if (token.Type == TokenType.DataType)
            {
                dataType = TokenTypeToDataType(token);

                if (!enumerator.MoveNext()) //identifier
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type != TokenType.IdentifierName)
                    throw UnexpectedSyntax(token, info);
            }
            else if (token.Type != TokenType.IdentifierName)
            {
                throw UnexpectedSyntax(token, info);
            }

            IStatement variable = dataType == null
                ? (IStatement) new VariableAccessStatement(token.Value)
                : new VariableDefinitionStatement(dataType.Value, token.Value, null);

            if (!enumerator.MoveNext()) //in
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.In)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //id
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, info);

            var iterator = new VariableAccessStatement(token.Value);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            var block = ReadBlockStatement(token, enumerator, info);

            return new ForEachStatement(variable, iterator, block);
        }

        public IStatement ReadClass(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            return null;
        }


        public IStatement ReadEcho(Token token, PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.Echo)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext())
                throw UnexpectedSyntax(token, info);

            token = enumerator.Current;

            var statement = ReadEvaluationStatement(token, enumerator, info);

            return new SdkFunctionCallStatement("echo", new [] {statement});
        }
    }
}