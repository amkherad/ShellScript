using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashFunctionCallStatementTranspiler : BashEvaluationStatementTranspiler
    {
        public override Type StatementType => typeof(FunctionCallStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return true;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter,
            IStatement statement)
        {
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is FunctionCallStatement functionCallStatement)) throw new InvalidOperationException();

            if (!scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
            {
                throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                    functionCallStatement.Info);
            }
            
            CheckParameters(context, scope, functionCallStatement, funcInfo);

            var resultVar = context.GetLastFunctionCallStorageVariable(metaWriter);

            if (funcInfo.InlinedStatement != null && context.Flags.UseInlining)
            {
                var inlined = FunctionInfo.UnWrapInlinedStatement(context, scope, funcInfo);
                var transpiler = context.GetTranspilerForStatement(inlined);
                transpiler.WriteBlock(context, scope, writer, metaWriter, inlined);
            }
            else
            {
                using (var inlineWriter = new StringWriter())
                {
                    foreach (var stt in functionCallStatement.Parameters)
                    {
                        inlineWriter.Write(' ');

                        var transpiler = context.GetEvaluationTranspilerForStatement(stt);
                        transpiler.WriteInline(context, scope, inlineWriter, metaWriter, writer, stt);
                    }

                    writer.Write($"{resultVar}=`{funcInfo.Name}");
                    writer.Write(inlineWriter.ToString());
                    writer.WriteLine('`');
                }
            }
        }

        public static void CheckParameters(Context context, Scope scope, FunctionCallStatement functionCallStatement,
            FunctionInfo funcInfo)
        {
            if (funcInfo.IsParams)
                return;

            var funcInfoParameters = funcInfo.Parameters;
            var funcCallParameters = functionCallStatement.Parameters;

            var funcInfoParametersCount = funcInfoParameters?.Length ?? 0;
            var funcCallParametersCount = funcCallParameters?.Length ?? 0;

            if (funcInfoParametersCount != funcCallParametersCount)
            {
                throw new MethodParameterMismatchCompilerException(functionCallStatement.FunctionName,
                    functionCallStatement.Info);
            }

            if (funcCallParametersCount == 0)
            {
                return;
            }

            for (var i = 0; i < funcInfo.Parameters.Length; i++)
            {
                var passed = functionCallStatement.Parameters[i];
                var schema = funcInfo.Parameters[i];

                var dataType = passed.GetDataType(context, scope);

                if (dataType != schema.DataType)
                {
                    if (schema.DataType == DataTypes.Numeric || schema.DataType == DataTypes.Float &&
                        dataType == DataTypes.Decimal)
                    {
                        continue;
                    }

                    throw new MethodParameterMismatchCompilerException(functionCallStatement.FunctionName,
                        functionCallStatement.Info);
                }
            }
        }
    }
}