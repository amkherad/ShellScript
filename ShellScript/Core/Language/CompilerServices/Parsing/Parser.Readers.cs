using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.XPath;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;

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
        public BlockStatement ReadBlockStatement(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            var statements = new List<IStatement>();

            var continueRead = true;
            while (continueRead)
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

            return new BlockStatement(statements.ToArray(), CreateStatementInfo(info, token));
        }


        private EvaluationStatement _createEvaluation(LinkedList<IStatement> statements, Token token, ParserInfo info)
        {
            if (statements.Count == 0)
            {
                return new NopStatement(CreateStatementInfo(info, token));
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

                //this block checks if there's a addition or subtraction operator in the first place of statements or
                //- the statement before op is another operator.
                //digit sign (negative/positive operator)
                var prev = node.Previous;
                if (prev == null || prev.Value is IOperator)
                {
                    if (op is SubtractionOperator)
                    {
                        var valNode = node.Next;
                        var val = valNode?.Value as EvaluationStatement;
                        if (val == null) //next may be operator or null !!!
                        {
                            throw new InvalidOperationException();
                        }

                        var negation = ArithmeticEvaluationStatement.CreateNegate(
                            new NegativeNumberOperator(op.Info), val, op.Info);

                        val.ParentStatement = negation;
                        node.Value = negation;
                        statements.Remove(valNode);
                        continue;
                    }

                    if (op is AdditionOperator)
                    {
                        statements.Remove(node);
                        continue;
                    }

                    //throw new InvalidOperationException();
                }

                //digit sign (negative/positive operator)
                var next = node.Next;
                if (next != null && next.Value is IOperator)
                {
                    var nOp = next.Value;
                    var nNode = next.Next;
                    var nOperand = nNode?.Value as EvaluationStatement;

                    if (nOperand == null)
                    {
                        throw new InvalidOperationException();
                    }

                    if (nOp is SubtractionOperator)
                    {
                        var negation = ArithmeticEvaluationStatement.CreateNegate(
                            new NegativeNumberOperator(nOp.Info), nOperand, nOp.Info);

                        nOperand.ParentStatement = negation;
                        next.Value = negation;

                        statements.Remove(nNode);
                    }
                    else if (nOp is AdditionOperator)
                    {
                        statements.Remove(nOp);
                    }
                    else //Invalid expression.
                    {
                        throw new InvalidOperationException();
                    }
                }

                switch (op)
                {
                    case BitwiseOperator bitwiseOperator:
                    {
                        if (op is BitwiseNotOperator bitwiseNotOperator)
                        {
                            var operandNode = node.Next;
                            var operand = operandNode?.Value as EvaluationStatement;
                            if (operand == null)
                            {
                                throw UnexpectedSyntax(token, info);
                            }

                            var stt = BitwiseEvaluationStatement.CreateNot(bitwiseNotOperator, operand,
                                CreateStatementInfo(info, token));

                            operand.ParentStatement = stt;
                            node.Value = stt;

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

                            var stt = new BitwiseEvaluationStatement(left, bitwiseOperator, right,
                                CreateStatementInfo(info, token));

                            left.ParentStatement = stt;
                            right.ParentStatement = stt;
                            node.Value = stt;

                            statements.Remove(leftNode);
                            statements.Remove(rightNode);
                        }

                        break;
                    }
                    case LogicalOperator logicalOperator:
                    {
                        if (op is NotOperator notOperator)
                        {
                            var operandNode = node.Next;
                            var operand = operandNode?.Value as EvaluationStatement;
                            if (operand == null)
                            {
                                throw UnexpectedSyntax(token, info);
                            }

                            var stt = LogicalEvaluationStatement.CreateNot(notOperator, operand,
                                CreateStatementInfo(info, token));

                            operand.ParentStatement = stt;
                            node.Value = stt;

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

                            var stt = new LogicalEvaluationStatement(left, logicalOperator, right,
                                CreateStatementInfo(info, token));

                            left.ParentStatement = stt;
                            right.ParentStatement = stt;
                            node.Value = stt;

                            statements.Remove(leftNode);
                            statements.Remove(rightNode);
                        }

                        break;
                    }
                    case ArithmeticOperator arithmeticOperator:
                    {
                        if (op is IncrementOperator || op is DecrementOperator)
                        {
                            var operandNode = node.Previous;
                            var operand = operandNode?.Value as VariableAccessStatement;

                            EvaluationStatement statement;
                            if (operand == null)
                            {
                                operandNode = node.Next;
                                operand = operandNode?.Value as VariableAccessStatement;

                                if (operand == null)
                                {
                                    throw UnexpectedSyntax(token, info);
                                }

                                statement = ArithmeticEvaluationStatement.CreatePrefix(arithmeticOperator, operand,
                                    CreateStatementInfo(info, token));
                            }
                            else
                            {
                                statement = ArithmeticEvaluationStatement.CreatePostfix(arithmeticOperator, operand,
                                    CreateStatementInfo(info, token));
                            }

                            operand.ParentStatement = statement;
                            node.Value = statement;

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

                            var stt = new ArithmeticEvaluationStatement(left, arithmeticOperator, right,
                                CreateStatementInfo(info, token));
                            node.Value = stt;

                            left.ParentStatement = stt;
                            right.ParentStatement = stt;

                            statements.Remove(leftNode);
                            statements.Remove(rightNode);
                        }

                        break;
                    }
                    case AssignmentOperator assignmentOperator:
                    {
                        var leftNode = node.Previous;
                        var left = leftNode?.Value as EvaluationStatement;
                        var rightNode = node.Next;
                        var right = rightNode?.Value as EvaluationStatement;

                        if (left == null || right == null)
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        var stt = new AssignmentStatement(left, right, CreateStatementInfo(info, token));
                        node.Value = stt;

                        left.ParentStatement = stt;
                        right.ParentStatement = stt;

                        statements.Remove(leftNode);
                        statements.Remove(rightNode);
                        
                        break;
                    }
                    default:
                    {
                        throw UnexpectedSyntax(token, info); //WTF!
                    }
                }
            }

            if (statements.Count == 0)
            {
                return null;
            }

            if (statements.First.Value is EvaluationStatement evaluationStatement)
            {
                return evaluationStatement;
            }

            throw UnexpectedSyntax(token, info);
        }

        /// <summary>
        /// Reads an evaluation statement (i.e. "x = 3 * 4")
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public EvaluationStatement ReadEvaluationStatement(Token token, IPeekingEnumerator<Token> enumerator,
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
                        statements.AddLast(new ConstantValueStatement(DataTypes.String, token.Value,
                            CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Number:
                    {
                        if (long.TryParse(token.Value, out var decimalResult))
                        {
                            statements.AddLast(new ConstantValueStatement(DataTypes.Decimal,
                                decimalResult.ToString(NumberFormatInfo.InvariantInfo),
                                CreateStatementInfo(info, token)));
                        }
                        else if (double.TryParse(token.Value, out var floatResult))
                        {
                            statements.AddLast(new ConstantValueStatement(DataTypes.Float,
                                floatResult.ToString(NumberFormatInfo.InvariantInfo),
                                CreateStatementInfo(info, token)));
                        }
                        else
                        {
                            throw UnexpectedSyntax(token, info);
                        }

                        break;
                    }

                    case TokenType.True:
                    {
                        statements.AddLast(new ConstantValueStatement(DataTypes.Boolean,
                            "True",
                            CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.False:
                    {
                        statements.AddLast(new ConstantValueStatement(DataTypes.Boolean,
                            "False",
                            CreateStatementInfo(info, token)));
                        break;
                    }

                    case TokenType.Null:
                    {
                        statements.AddLast(new ConstantValueStatement(DataTypes.String, token.Value,
                            CreateStatementInfo(info, token)));
                        break;
                    }

                    case TokenType.Plus:
                    {
                        statements.AddLast(new AdditionOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Minus:
                    {
                        statements.AddLast(new SubtractionOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Asterisk:
                    {
                        statements.AddLast(new MultiplicationOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Division:
                    {
                        statements.AddLast(new DivisionOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Increment:
                    {
                        statements.AddLast(new IncrementOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Decrement:
                    {
                        statements.AddLast(new DecrementOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.BackSlash:
                    {
                        statements.AddLast(new ModulusOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Reminder:
                    {
                        statements.AddLast(new ReminderOperator(CreateStatementInfo(info, token)));
                        break;
                    }

                    case TokenType.Not:
                    {
                        statements.AddLast(new NotOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.BitwiseNot:
                    {
                        statements.AddLast(new BitwiseNotOperator(CreateStatementInfo(info, token)));
                        break;
                    }

                    case TokenType.Equals:
                    {
                        statements.AddLast(new EqualOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.NotEquals:
                    {
                        statements.AddLast(new NotEqualOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.GreaterEqual:
                    {
                        statements.AddLast(new GreaterEqualOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Greater:
                    {
                        statements.AddLast(new GreaterOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.LessEqual:
                    {
                        statements.AddLast(new LessEqualOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Less:
                    {
                        statements.AddLast(new LessOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.And:
                    {
                        statements.AddLast(new BitwiseAndOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.AndLogical:
                    {
                        statements.AddLast(new LogicalAndOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Or:
                    {
                        statements.AddLast(new BitwiseOrOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.OrLogical:
                    {
                        statements.AddLast(new LogicalOrOperator(CreateStatementInfo(info, token)));
                        break;
                    }
                    case TokenType.Assignment:
                    {
                        statements.AddLast(new AssignmentOperator(CreateStatementInfo(info, token)));
                        break;
                    }

                    case TokenType.CloseParenthesis:
                    case TokenType.SequenceTerminator:
                    case TokenType.OpenBrace:
                    case TokenType.SequenceTerminatorNewLine:
                    case TokenType.Comma:
                    case TokenType.Colon: //used in pre-processors
                    {
                        try
                        {
                            var result = _createEvaluation(statements, token, info);

                            return result;
                        }
                        catch (Exception ex)
                        {
                            throw UnexpectedSyntax(token, info, ex);
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
                        case TokenType.Colon: //used in pre-processors
                        {
                            try
                            {
                                var result = _createEvaluation(statements, token, info);

                                return result;
                            }
                            catch (Exception ex)
                            {
                                throw UnexpectedSyntax(token, info, ex);
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
                else
                {
                    try
                    {
                        var result = _createEvaluation(statements, token, info);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw UnexpectedSyntax(token, info, ex);
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
        public ConstantValueStatement ReadConstantValue(Token token, IPeekingEnumerator<Token> enumerator,
            ParserInfo info, DataTypes dataTypeHint)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    dataTypeHint = DataTypes.Numeric;
                    break;
                case TokenType.StringValue1:
                case TokenType.StringValue2:
                    dataTypeHint = DataTypes.String;
                    break;
                case TokenType.Null:
                    break;
                case TokenType.True:
                case TokenType.False:
                    dataTypeHint = DataTypes.Boolean;
                    break;
                default:
                    throw UnexpectedSyntax(token, info);
            }

            return new ConstantValueStatement(dataTypeHint, token.Value, CreateStatementInfo(info, token));
        }

        public EvaluationStatement ReadAssignmentOrFunctionCall(Token token, IPeekingEnumerator<Token> enumerator,
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
                        var statement = ReadEvaluationStatement(token, enumerator, info);

                        var sttInfo = CreateStatementInfo(info, token);
                        var leftSide = new VariableAccessStatement(firstToken.Value, sttInfo);
                        var result = new AssignmentStatement(leftSide, statement,
                            sttInfo);

                        leftSide.ParentStatement = result;
                        statement.ParentStatement = result;

                        return result;
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

        public EvaluationStatement ReadVariableOrFunctionCall(Token token, IPeekingEnumerator<Token> enumerator,
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
                        className = functionName;
                        functionName = token;

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
                                        enumerator.MoveNext();

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

                    var funcParams = parameters.Where(p => !(p is NopStatement)).ToArray();
                    var result = new FunctionCallStatement(
                        className?.Value,
                        functionName.Value,
                        DataTypes.Void,
                        funcParams,
                        CreateStatementInfo(info, token)
                    );

                    foreach (var param in funcParams)
                    {
                        param.ParentStatement = result;
                    }

                    return result;
                }
            }

            return new VariableAccessStatement(token.Value, CreateStatementInfo(info, token));
        }


        public IStatement ReadVariableDefinitionAndAssignment(Token token,
            IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.DataType)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //variable name or constant type.
                throw EndOfFile(token, info);

            var isConstant = false;
            if (token.Value == ConstantKeyword)
            {
                token = enumerator.Current;

                if (token.Type != TokenType.DataType)
                    throw UnexpectedSyntax(token, info);

                isConstant = true;

                if (!enumerator.MoveNext()) //variable name.
                    throw EndOfFile(token, info);
            }

            var dataType = TokenTypeToDataType(token);

            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, info);
            if (Keywords.Contains(definitionName.Value)) //impossible check.
                throw InvalidIdentifierName(definitionName.Value, token, info);

            if (!enumerator.MoveNext()) //assignment
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.Assignment)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //value
                throw EndOfFile(token, info);

            EvaluationStatement value;

            token = enumerator.Current;
            if (token.Type == TokenType.IdentifierName)
            {
                value = new VariableAccessStatement(token.Value, CreateStatementInfo(info, token));
            }
            else if (token.Type == TokenType.Number)
            {
                value = new ConstantValueStatement(DataTypes.Numeric, token.Value,
                    CreateStatementInfo(info, token));
            }
            else if (token.Type == TokenType.StringValue1 || token.Type == TokenType.StringValue2)
            {
                value = new ConstantValueStatement(DataTypes.String, token.Value, CreateStatementInfo(info, token));
            }
            else if (token.Type == TokenType.Null)
            {
                value = new ConstantValueStatement(DataTypes.String, token.Value, CreateStatementInfo(info, token));
            }
            else
            {
                throw UnexpectedSyntax(token, info);
            }

            var result = new VariableDefinitionStatement(dataType, definitionName.Value, isConstant, value,
                CreateStatementInfo(info, token));

            value.ParentStatement = result;

            return result;
        }


        public FunctionParameterDefinitionStatement ReadParameterDefinition(Token token,
            IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.DataType)
                throw UnexpectedSyntax(token, info);

            var dataType = TokenTypeToDataType(token);

            if (!enumerator.MoveNext()) //variable name.
                throw EndOfFile(token, info);

            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, info);

            if (enumerator.TryPeek(out var peek) && peek.Type == TokenType.Assignment)
            {
                enumerator.MoveNext();

                if (!enumerator.MoveNext()) //default value.
                    throw EndOfFile(token, info);

                var defaultValue = ReadConstantValue(enumerator.Current, enumerator, info, dataType);

                var result = new FunctionParameterDefinitionStatement(dataType, definitionName.Value, defaultValue,
                    CreateStatementInfo(info, token));

                defaultValue.ParentStatement = result;

                return result;
            }

            return new FunctionParameterDefinitionStatement(dataType, definitionName.Value, null,
                CreateStatementInfo(info, token));
        }

        public FunctionParameterDefinitionStatement[] ReadParameterDefinitions(Token token,
            IPeekingEnumerator<Token> enumerator, ParserInfo info)
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
        public IStatement ReadVariableOrFunctionDefinition(Token token, IPeekingEnumerator<Token> enumerator,
            ParserInfo info)
        {
            var constToken = token;

            if (token.Type != TokenType.DataType)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //variable name or function name or constant type.
                throw EndOfFile(token, info);

            var isConstant = false;
            if (token.Value == ConstantKeyword)
            {
                constToken = token;
                token = enumerator.Current;

                if (token.Type != TokenType.DataType)
                    throw UnexpectedSyntax(token, info);

                isConstant = true;

                if (!enumerator.MoveNext()) //constant name.
                    throw EndOfFile(token, info);
            }

            var dataType = TokenTypeToDataType(token);

            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, info);
            if (Keywords.Contains(definitionName.Value)) //impossible check.
                throw InvalidIdentifierName(definitionName.Value, token, info);

            Token peek;

            if (enumerator.TryPeek(out peek))
            {
                if (peek.Type == TokenType.Assignment)
                {
                    if (dataType == DataTypes.Void)
                    {
                        throw UnexpectedSyntax(token, info);
                    }

                    enumerator.MoveNext(); //assignment

                    if (!enumerator.MoveNext()) //value
                        throw EndOfFile(token, info);


                    token = enumerator.Current;

                    var value = ReadEvaluationStatement(token, enumerator, info);

                    var result = new VariableDefinitionStatement(dataType, definitionName.Value, isConstant, value,
                        CreateStatementInfo(info, token));

                    value.ParentStatement = result;

                    return result;
                }

                if (peek.Type == TokenType.OpenParenthesis)
                {
                    if (isConstant)
                    {
                        throw UnexpectedSyntax(constToken, info);
                    }

                    enumerator.MoveNext(); //open parenthesis

                    FunctionParameterDefinitionStatement[] parameters = null;
                    BlockStatement block = null;

                    token = enumerator.Current;
                    //if (token.Type == TokenType.OpenParenthesis)
                    {
                        if (enumerator.TryPeek(out peek))
                        {
                            if (peek.Type == TokenType.CloseParenthesis)
                            {
                                enumerator.MoveNext(); //read close parenthesis.
                            }
                            else
                            {
                                parameters = ReadParameterDefinitions(token, enumerator, info);
                            }
                        }
                        else
                        {
                            throw EndOfFile(token, info);
                        }

                        if (!enumerator.MoveNext())
                            throw EndOfFile(token, info);

                        token = enumerator.Current;
                        if (token.Type != TokenType.OpenBrace)
                            throw UnexpectedSyntax(token, info);

                        block = ReadBlockStatement(token, enumerator, info);
                    }
                    //else if (token.Type == TokenType.OpenBrace)
                    //{
                    //    block = ReadBlockStatement(token, enumerator, info);
                    //}
                    //else
                    //{
                    //    throw UnexpectedSyntax(token, info);
                    //}

                    var result = new FunctionStatement(dataType, definitionName.Value, parameters, block,
                        CreateStatementInfo(info, token));

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            if (param.DefaultValue != null)
                            {
                                param.DefaultValue.ParentStatement = result;
                            }
                        }
                    }

                    return result;
                }
            }

            if (dataType == DataTypes.Void)
            {
                throw UnexpectedSyntax(token, info);
            }

            if (isConstant)
            {
                throw UnexpectedSyntax(peek, info);
            }

            return new VariableDefinitionStatement(dataType, definitionName.Value, false, null,
                CreateStatementInfo(info, token));
        }

        public IfElseStatement ReadIf(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
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
            if (token.Type == TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, info);

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
                var sttInfo = CreateStatementInfo(info, token);

                var conditionalBlock = new ConditionalBlockStatement(condition, block, sttInfo);
                condition.ParentStatement = conditionalBlock;

                return new IfElseStatement(conditionalBlock, sttInfo);
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

                    token = enumerator.Current;
                    if (token.Type != TokenType.OpenParenthesis)
                        throw UnexpectedSyntax(token, info);

                    if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                        throw EndOfFile(token, info);

                    token = enumerator.Current;
                    if (token.Type == TokenType.CloseParenthesis)
                        throw UnexpectedSyntax(token, info);

                    var elifCondition = ReadEvaluationStatement(token, enumerator, info);

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

                    var elifBlock = ReadBlockStatement(token, enumerator, info);

                    var conditionalBlock = new ConditionalBlockStatement(elifCondition, elifBlock,
                        CreateStatementInfo(info, token));
                    elifCondition.ParentStatement = conditionalBlock;

                    elseIfBlocks.Add(conditionalBlock);
                }
                else if (token.Type == TokenType.OpenBrace) //else
                {
                    var elseBlock = ReadBlockStatement(token, enumerator, info);

                    var sttInfo = CreateStatementInfo(info, token);
                    return new IfElseStatement(new ConditionalBlockStatement(condition, block, sttInfo),
                        elseIfBlocks.ToArray(), elseBlock, sttInfo);
                }

                if (!enumerator.TryPeek(out peek) || peek.Type != TokenType.Else) //else or else if
                {
                    var sttInfo = CreateStatementInfo(info, token);

                    var conditionalBlock = new ConditionalBlockStatement(condition, block, sttInfo);
                    condition.ParentStatement = conditionalBlock;

                    return new IfElseStatement(conditionalBlock, elseIfBlocks.ToArray(), sttInfo);
                }
            }
        }

        public SwitchCaseStatement ReadSwitchCase(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            return null;
        }

        public WhileStatement ReadWhile(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
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

            WhileStatement result;

            token = enumerator.Current;
            if (token.Type == TokenType.SequenceTerminator)
            {
                result = new WhileStatement(condition, null, CreateStatementInfo(info, token));
            }
            else if (token.Type == TokenType.OpenBrace)
            {
                var statements = ReadBlockStatement(token, enumerator, info);

                result = new WhileStatement(condition, statements, CreateStatementInfo(info, token));
            }
            else
            {
                throw UnexpectedSyntax(token, info);
            }

            condition.ParentStatement = result;
            return result;
        }

        public DoWhileStatement ReadDoWhile(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
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

            var result = new DoWhileStatement(condition, statements, CreateStatementInfo(info, token));
            condition.ParentStatement = result;

            return result;
        }

        public WhileStatement ReadLoop(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            return ReadWhile(token, enumerator, info);
        }

        //TODO: support multiple post loop evaluations
        public ForStatement ReadFor(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            throw new NotImplementedException();
            if (token.Type != TokenType.For)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //next token (datatype, variable name, or semicolon)
                throw EndOfFile(token, info);

            IStatement[] preLoopAssignments = null;
            EvaluationStatement condition = null;
            IStatement[] postLoopEvaluations = null;

            token = enumerator.Current;
            if (token.Type == TokenType.DataType)
            {
                //preLoopAssignments = ReadVariableDefinitionAndAssignment(token, enumerator, info);

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

                token = enumerator.Current;
                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, info);
            }

            if (!enumerator.MoveNext()) //post evaluations
                throw EndOfFile(token, info);

            if (token.Type != TokenType.CloseParenthesis)
            {
                //postLoopEvaluations = ReadEvaluationStatement(token, enumerator, info);

                if (!enumerator.MoveNext()) //close parenthesis
                    throw EndOfFile(token, info);

                token = enumerator.Current;
                if (token.Type != TokenType.CloseParenthesis)
                    throw UnexpectedSyntax(token, info);
            }

            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, info);

            IStatement block = ReadBlockStatement(token, enumerator, info);

            var result = new ForStatement(preLoopAssignments, condition, postLoopEvaluations, block,
                CreateStatementInfo(info, token));

            if (condition != null)
            {
                condition.ParentStatement = result;
            }

            return result;
        }

        public ForEachStatement ReadForEach(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
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

            IStatement variable;
            EvaluationStatement variableEval = null;
            if (dataType == null)
            {
                variableEval = new VariableAccessStatement(token.Value, CreateStatementInfo(info, token));
                variable = variableEval;
            }
            else
            {
                variable = new VariableDefinitionStatement(dataType.Value, token.Value, false, null,
                    CreateStatementInfo(info, token));
            }

            if (!enumerator.MoveNext()) //in
                throw EndOfFile(token, info);

            token = enumerator.Current;
            if (token.Type != TokenType.In)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext()) //id
                throw EndOfFile(token, info);

            token = enumerator.Current;

            var iterator = ReadEvaluationStatement(token, enumerator, info);

            //var iterator = new VariableAccessStatement(token.Value, CreateStatementInfo(info, token));

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

            var result = new ForEachStatement(variable, iterator, block, CreateStatementInfo(info, token));

            iterator.ParentStatement = result;
            if (variableEval != null)
            {
                variableEval.ParentStatement = result;
            }

            return result;
        }

        public IStatement ReadClass(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            return null;
        }


        public IStatement ReadEcho(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.Echo)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, info);

            token = enumerator.Current;

            var statement = ReadEvaluationStatement(token, enumerator, info);

            var result = new EchoStatement(new[] {statement}, CreateStatementInfo(info, token));

            statement.ParentStatement = result;

            return result;
        }

        public ReturnStatement ReadReturn(Token token, IPeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            if (token.Type != TokenType.Return)
                throw UnexpectedSyntax(token, info);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, info);

            token = enumerator.Current;

            if (token.Type == TokenType.SequenceTerminator ||
                token.Type == TokenType.SequenceTerminatorNewLine ||
                token.Type == TokenType.CloseBrace)
            {
                return new ReturnStatement(null, CreateStatementInfo(info, token));
            }

            var statement = ReadEvaluationStatement(token, enumerator, info);

            var result = new ReturnStatement(statement, CreateStatementInfo(info, token));

            statement.ParentStatement = result;

            return result;
        }
    }
}