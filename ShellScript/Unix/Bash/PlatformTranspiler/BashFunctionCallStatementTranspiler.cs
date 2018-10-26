using System;
using System.IO;
using System.Text;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashFunctionCallStatementTranspiler : BashEvaluationStatementTranspiler
    {
        public override Type StatementType => typeof(FunctionCallStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            if (!(statement is FunctionCallStatement functionCallStatement)) throw new InvalidOperationException();

            if (scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
            {
                return funcInfo.DataType != DataTypes.Void;
            }
            
            return false;
        }
        
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            if (!(statement is FunctionCallStatement functionCallStatement)) throw new InvalidOperationException();

            EvaluationStatement evalStt = functionCallStatement;
            var result = GetExpression(context, scope, metaWriter, nonInlinePartWriter, null, evalStt);

            writer.Write(result.Expression);
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is FunctionCallStatement functionCallStatement)) throw new InvalidOperationException();

            var blockUsageContext = new BlockStatement(new [] {statement}, statement.Info);
            
            var resultVar = context.GetLastFunctionCallStorageVariable(metaWriter);

            EvaluationStatement evalStt = functionCallStatement;
            var result = GetExpression(context, scope, metaWriter, writer, blockUsageContext, evalStt);

            if (result.IsEmptyResult)
            {
                return;
            }
            
            if (result.DataType == DataTypes.Void && !(result.Template is FunctionCallStatement))
            {
                writer.WriteLine(result.Expression);
            }
            else
            {
                writer.WriteLine($"{resultVar}={result.Expression}");
            }
        }
    }
}