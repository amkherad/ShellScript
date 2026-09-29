using System;
using System.IO;
using ShellScript.Core;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Core.Array;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashVariableDefinitionStatementTranspiler : VariableDefinitionStatementTranspilerBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteVariableDefinition(Context context, Scope scope, TextWriter writer, string name,
            string expression)
        {
            if (scope.IsInsideMethod)
                writer.WriteLine($"local {name}={expression}");
            else
                writer.WriteLine($"{name}={expression}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WriteLastStatusCodeStoreVariableDefinition(Context context, Scope scope, TextWriter writer,
            string nameHint)
        {
            var name = scope.NewHelperVariable(TypeDescriptor.Integer, nameHint);

            if (scope.IsInsideMethod)
                writer.WriteLine($"local {name}=$?");
            else
                writer.WriteLine($"{name}=$?");

            return name;
        }


        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer,
            TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            if (varDefStt.IsConstant)
            {
                if (!varDefStt.HasDefaultValue)
                {
                    throw new InvalidOperationException();
                }

                var result =
                    EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, varDefStt.DefaultValue);

                if (!(result is ConstantValueStatement constantValueStatement))
                {
                    throw new InvalidOperationException(ErrorMessages.ConstantValueRequired);
                }

                if (!StatementHelpers.IsAssignableFrom(context, scope, varDefStt.TypeDescriptor,
                    constantValueStatement.TypeDescriptor))
                {
                    throw new TypeMismatchCompilerException(constantValueStatement.TypeDescriptor,
                        varDefStt.TypeDescriptor, varDefStt.Info);
                }

                scope.ReserveNewConstant(varDefStt.TypeDescriptor, varDefStt.Name, constantValueStatement.Value);
            }
            else
            {
                var typeDescriptor = varDefStt.TypeDescriptor;
                if (typeDescriptor.DataType == DataTypes.Lookup && typeDescriptor.Lookup != null &&
                    scope.TryGetUserClass(typeDescriptor.Lookup.Value.Name, out _))
                {
                    typeDescriptor = ObjectModelHelpers.UserClass(typeDescriptor.Lookup.Value.Name);
                }

                var skipDefinition = false;
                if (varDefStt.HasDefaultValue && varDefStt.DefaultValue is ObjectCreationStatement objectCreation)
                {
                    if (!ObjectModelHelpers.IsUserClassType(typeDescriptor))
                    {
                        throw new InvalidOperationException();
                    }

                    BashObjectModel.WriteDeclareInstance(context, scope, writer, varDefStt.Name);
                    BashObjectModel.WriteConstructorInvocation(context, scope, writer, metaWriter,
                        objectCreation.ClassName, varDefStt.Name, objectCreation.Arguments, varDefStt.Info, varDefStt);
                    scope.ReserveNewVariable(typeDescriptor, varDefStt.Name);
                    scope.IncrementStatements();
                    return;
                }

                if (varDefStt.HasDefaultValue)
                {
                    var def = varDefStt.DefaultValue;
                    var transpiler = context.GetEvaluationTranspilerForStatement(def);
                    if (transpiler == null)
                    {
                        throw new InvalidOperationException();
                    }

                    //it pins non-inlinable values to a helper variable:
                    //int x = 34 * myFunc();
                    //becomes:
                    //myFuncResult=myFunc()
                    //x=$((34 * myFuncResult))
                    if (typeDescriptor.IsArray())
                    {
                        var p = new ExpressionBuilderParams(context, scope, metaWriter, writer,
                            new BlockStatement(null, varDefStt.Info));

                        scope.ReserveNewVariable(typeDescriptor, varDefStt.Name);
                        skipDefinition = true;

                        var targetVar = new VariableAccessStatement(varDefStt.Name, varDefStt.Info);
                        
                        var call = transpiler.CallApiFunction<ApiArray.Copy>(p, new[] {targetVar, def}, varDefStt,
                            varDefStt.Info);

                        if (!call.IsEmptyResult)
                        {
                            WriteVariableDefinition(context, scope, writer, varDefStt.Name, call.Expression);
                        }
                    }
                    else
                    {
                        var result = transpiler.GetExpression(context, scope, metaWriter, writer, null, def);
                        var expression = result.Expression;
                        if (def is ConstantValueStatement stringConstant && stringConstant.IsString() &&
                            StringHelpers.DeQuote(stringConstant.Value).IndexOf('\\', StringComparison.Ordinal) >= 0)
                        {
                            expression = BashTranspilerHelpers.ToBashStringLiteral(stringConstant.Value);
                        }

                        WriteVariableDefinition(context, scope, writer, varDefStt.Name, expression);
                    }
                }
                else
                {
                    if (ObjectModelHelpers.IsUserClassType(typeDescriptor))
                    {
                        BashObjectModel.WriteDeclareInstance(context, scope, writer, varDefStt.Name);
                        skipDefinition = true;
                    }
                    else
                    {
                        WriteVariableDefinition(context, scope, writer, varDefStt.Name,
                            context.Platform.GetDefaultValue(typeDescriptor.DataType));
                    }
                }

                if (!skipDefinition)
                {
                    scope.ReserveNewVariable(typeDescriptor, varDefStt.Name);
                }
                else if (ObjectModelHelpers.IsUserClassType(typeDescriptor))
                {
                    scope.ReserveNewVariable(typeDescriptor, varDefStt.Name);
                }

                scope.IncrementStatements();
            }
        }
    }
}